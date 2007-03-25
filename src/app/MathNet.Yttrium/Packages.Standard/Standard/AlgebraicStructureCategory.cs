#region Math.NET Yttrium (GPL) by Christoph Ruegg
// Math.NET Yttrium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2007, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Xml;
using System.Collections.Generic;

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.Packages.Standard
{
    public class AlgebraicStructureCategory : CategoryBase
    {
        private readonly IEntity _additiveEntity;
        private readonly IEntity _multiplicativeEntity; // = null;
        private EAlgebraicStructure _structure;

        public AlgebraicStructureCategory(EAlgebraicStructure structure, IEntity additive, IEntity multiplicative)
            : base("AlgebraicStructure" + "_" + additive.EntityId.ToString() + "_" + multiplicative.EntityId.ToString() + "_" + structure.ToString().Replace(',', '-'), "Std")
        {
            _structure = structure;
            _additiveEntity = additive;
            _multiplicativeEntity = multiplicative;
        }
        public AlgebraicStructureCategory(EAlgebraicStructure structure, IEntity additive)
            : base("AlgebraicStructure" + "_" + additive.EntityId.ToString() + "__" + structure.ToString().Replace(',', '-'), "Std")
        {
            _structure = structure;
            _additiveEntity = additive;
        }

        
        public EAlgebraicStructure Structure
        {
            get { return _structure; }
        }

        public IEntity AdditiveEntity
        {
            get { return _additiveEntity; }
        }

        public IEntity MultiplicativeEntity
        {
            get { return _multiplicativeEntity; }
        }

        public override CategoryMembershipLevel IsMember(Signal signal, bool ignoreCache)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            CategoryMembershipLevel membership;
            if(ignoreCache)
                membership = CategoryMembershipLevel.Unknown;
            else
                if(TryGetCachedMembership(signal, out membership))
                    return membership;

            if(signal.BehavesAsSourceSignal)
                return CategoryMembershipLevel.Unknown;

            Port port = signal.DrivenByPort;
            return IsMember(port);
        }

        public override CategoryMembershipLevel IsMember(Port port)
        {
            if(port == null)
                throw new ArgumentNullException("port");

            CategoryMembershipLevel childMembership;
            IEntity entity = port.Entity;

            if(port.BusCount != 0)
                return CategoryMembershipLevel.NotMember;

            childMembership = EvaluateJointChildMembership(port.InputSignals);
            if(childMembership == CategoryMembershipLevel.NotMember)
                return CategoryMembershipLevel.NotMember;

            if(entity.EqualsById(_additiveEntity))
            {
                if((_structure & EAlgebraicStructure.AdditiveClose) == EAlgebraicStructure.AdditiveClose && childMembership == CategoryMembershipLevel.Member)
                    return CategoryMembershipLevel.Member;
                else //CategoryMembershipLevel.Unknown
                    return CategoryMembershipLevel.Unknown;
            }
            else if(entity.EqualsById(_multiplicativeEntity))
            {
                if((_structure & EAlgebraicStructure.MultiplicativeClose) == EAlgebraicStructure.MultiplicativeClose && childMembership == CategoryMembershipLevel.Member)
                    return CategoryMembershipLevel.Member;
                else //CategoryMembershipLevel.Unknown
                    return CategoryMembershipLevel.Unknown;
            }
            else
                return CategoryMembershipLevel.NotMember;
        }

        public override bool Equals(CategoryBase other)
        {
            if(!base.Equals(other))
                return false;
            AlgebraicStructureCategory asc = other as AlgebraicStructureCategory;
            if(asc == null)
                return false;
            return _structure.Equals(asc._structure) && _additiveEntity.Equals(asc._additiveEntity) && _multiplicativeEntity.Equals(asc._multiplicativeEntity);
        }

        #region Serialization
        //protected override void InnerSerialize(XmlWriter writer)
        //{
        //    writer.WriteElementString("Structure", _structure.ToString());
        //    writer.WriteElementString("AdditiveEntity", _additiveEntity.EntityId.ToString());
        //    if(_multiplicativeEntity != null)
        //        writer.WriteElementString("MultiplicativeEntity", _multiplicativeEntity.EntityId.ToString());
        //}
        //protected static AlgebraicStructureCategory InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    if(context == null)
        //        throw new ArgumentNullException("context");

        //    EAlgebraicStructure structure = (EAlgebraicStructure)Enum.Parse(typeof(EAlgebraicStructure), reader.ReadElementString("Structure"));
        //    IEntity ae = context.Library.LookupEntity(MathIdentifier.Parse(reader.ReadElementString("AdditiveEntity")));
        //    if(reader.IsStartElement("MultiplicativeEntity"))
        //    {
        //        IEntity me = context.Library.LookupEntity(MathIdentifier.Parse(reader.ReadElementString("MultiplicativeEntity")));
        //        return new AlgebraicStructureCategory(structure, ae, me);
        //    }
        //    return new AlgebraicStructureCategory(structure, ae);
        //}
        public override void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            writer.WriteElementString("Structure", _structure.ToString());
            writer.WriteElementString("AdditiveEntity", _additiveEntity.EntityId.ToString());
            if(_multiplicativeEntity != null)
                writer.WriteElementString("MultiplicativeEntity", _multiplicativeEntity.EntityId.ToString());
        }
        private static AlgebraicStructureCategory Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            EAlgebraicStructure structure = (EAlgebraicStructure)Enum.Parse(typeof(EAlgebraicStructure), reader.ReadElementString("Structure"));
            IEntity ae = Service<ILibrary>.Instance.LookupEntity(MathIdentifier.Parse(reader.ReadElementString("AdditiveEntity")));
            if(reader.IsStartElement("MultiplicativeEntity"))
            {
                IEntity me = Service<ILibrary>.Instance.LookupEntity(MathIdentifier.Parse(reader.ReadElementString("MultiplicativeEntity")));
                return new AlgebraicStructureCategory(structure, ae, me);
            }
            return new AlgebraicStructureCategory(structure, ae);
        }
        #endregion
    }
}
