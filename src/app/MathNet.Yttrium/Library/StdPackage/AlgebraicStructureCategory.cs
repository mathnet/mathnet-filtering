#region Copyright 2001-2006 Christoph Daniel Rüegg [GPL]
//Math.NET Symbolics: Yttrium, part of Math.NET
//Copyright (c) 2001-2006, Christoph Daniel Rueegg, http://cdrnet.net/.
//All rights reserved.
//This Math.NET package is available under the terms of the GPL.

//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Xml;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Properties;

namespace MathNet.Symbolics.StdPackage
{
    public class AlgebraicStructureCategory : Category
    {
        private readonly Entity _additiveEntity;
        private readonly Entity _multiplicativeEntity; // = null;
        private EAlgebraicStructure _structure;

        public AlgebraicStructureCategory(EAlgebraicStructure structure, Entity additive, Entity multiplicative)
            : base("AlgebraicStructure" + "_" + additive.EntityId.ToString() + "_" + multiplicative.EntityId.ToString() + "_" + structure.ToString().Replace(',', '-'), "Std")
        {
            _structure = structure;
            _additiveEntity = additive;
            _multiplicativeEntity = multiplicative;
        }
        public AlgebraicStructureCategory(EAlgebraicStructure structure, Entity additive)
            : base("AlgebraicStructure" + "_" + additive.EntityId.ToString() + "__" + structure.ToString().Replace(',', '-'), "Std")
        {
            _structure = structure;
            _additiveEntity = additive;
        }

        
        public EAlgebraicStructure Structure
        {
            get { return _structure; }
        }

        public Entity AdditiveEntity
        {
            get { return _additiveEntity; }
        }

        public Entity MultiplicativeEntity
        {
            get { return _multiplicativeEntity; }
        }

        public override ECategoryMembership IsMember(Signal signal, bool ignoreCache)
        {
            ECategoryMembership membership;
            if(ignoreCache)
                membership = ECategoryMembership.Unknown;
            else
                if(TryGetCachedMembership(signal, out membership))
                    return membership;

            if(signal.BehavesAsSourceSignal)
                return ECategoryMembership.Unknown;

            Port port = signal.DrivenByPort;
            return IsMember(port);
        }

        public override ECategoryMembership IsMember(Port port)
        {
            ECategoryMembership childMembership;
            Entity entity = port.Entity;

            if(port.BusCount != 0)
                return ECategoryMembership.NotMember;

            childMembership = EvaluateJointChildMembership(port.InputSignals);
            if(childMembership == ECategoryMembership.NotMember)
                return ECategoryMembership.NotMember;

            if(entity.EqualsById(_additiveEntity))
            {
                if((_structure & EAlgebraicStructure.AdditiveClose) == EAlgebraicStructure.AdditiveClose && childMembership == ECategoryMembership.Member)
                    return ECategoryMembership.Member;
                else //ECategoryMembership.Unknown
                    return ECategoryMembership.Unknown;
            }
            else if(entity.EqualsById(_multiplicativeEntity))
            {
                if((_structure & EAlgebraicStructure.MultiplicativeClose) == EAlgebraicStructure.MultiplicativeClose && childMembership == ECategoryMembership.Member)
                    return ECategoryMembership.Member;
                else //ECategoryMembership.Unknown
                    return ECategoryMembership.Unknown;
            }
            else
                return ECategoryMembership.NotMember;
        }

        public override bool Equals(Category other)
        {
            if(!base.Equals(other))
                return false;
            AlgebraicStructureCategory asc = other as AlgebraicStructureCategory;
            if(asc == null)
                return false;
            return _structure.Equals(asc._structure) && _additiveEntity.Equals(asc._additiveEntity) && _multiplicativeEntity.Equals(asc._multiplicativeEntity);
        }

        #region Serialization
        protected override void InnerSerialize(XmlWriter writer)
        {
            writer.WriteElementString("Structure", _structure.ToString());
            writer.WriteElementString("AdditiveEntity", _additiveEntity.EntityId.ToString());
            if(_multiplicativeEntity != null)
                writer.WriteElementString("MultiplicativeEntity", _multiplicativeEntity.EntityId.ToString());
        }
        protected static AlgebraicStructureCategory InnerDeserialize(Context context, XmlReader reader)
        {
            EAlgebraicStructure structure = (EAlgebraicStructure)Enum.Parse(typeof(EAlgebraicStructure), reader.ReadElementString("Structure"));
            Entity ae = context.Library.LookupEntity(MathIdentifier.Parse(reader.ReadElementString("AdditiveEntity")));
            if(reader.IsStartElement("MultiplicativeEntity"))
            {
                Entity me = context.Library.LookupEntity(MathIdentifier.Parse(reader.ReadElementString("MultiplicativeEntity")));
                return new AlgebraicStructureCategory(structure, ae, me);
            }
            return new AlgebraicStructureCategory(structure, ae);
        }
        #endregion
    }
}
