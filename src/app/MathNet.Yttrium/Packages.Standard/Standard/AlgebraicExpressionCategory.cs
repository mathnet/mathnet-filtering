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

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Packages.ObjectModel;
using System.Collections.Generic;

namespace MathNet.Symbolics.Packages.Standard
{
    /// <summary>
    /// Category of all algebraic expressions.
    /// </summary>
    /// <remarks>
    /// An algebraic expression is constructed using integers, symbols,
    /// function forms and the algebraic operators (+,-,*,/,^,!).
    /// </remarks>
    public class AlgebraicExpressionCategory : CategoryBase
    {
        protected AlgebraicExpressionCategory()
            : base("AlgebraicExpression", "Std")
        {
        }

        public override CategoryMembershipLevel IsMember(Signal signal, bool ignoreCache)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(signal.IsCyclic)
                return CategoryMembershipLevel.NotMember;

            CategoryMembershipLevel membership;
            if(ignoreCache)
                membership = CategoryMembershipLevel.Unknown;
            else
                if(TryGetCachedMembership(signal, out membership))
                    return membership;

            if(signal.BehavesAsSourceSignal) //Symbol (maybe we should apply harder constraints here..)
                return CategoryMembershipLevel.Member;

            Port port = signal.DrivenByPort;
            if(port.IsCompletelyConnected)
                return IsMember(port);
            else
                return CategoryMembershipLevel.NotMember;
        }

        public override CategoryMembershipLevel IsMember(Port port)
        {
            for(int i = 0; i < port.OutputSignalCount; i++)
                if(port.OutputSignals[i].IsCyclic)
                    return CategoryMembershipLevel.NotMember;

            CategoryMembershipLevel childMembership;
            IEntity entity = port.Entity;

            if(port.BusCount != 0)
                return CategoryMembershipLevel.NotMember;

            childMembership = EvaluateJointChildMembership(port.InputSignals);
            if(childMembership == CategoryMembershipLevel.NotMember)
                return CategoryMembershipLevel.NotMember;

            MathIdentifier id = entity.EntityId;

            if(id.Equals("Add", "Std") || id.Equals("Subtract", "Std") || id.Equals("Multiply", "Std")
                || id.Equals("Divide", "Std") || id.Equals("Power", "Std") || id.Equals("Factorial", "Std"))
            {
                if(childMembership == CategoryMembershipLevel.Member)
                    return CategoryMembershipLevel.Member;
                else //CategoryMembershipLevel.Unknown
                    return CategoryMembershipLevel.Unknown;
            }
            else
                return CategoryMembershipLevel.NotMember;
        }

        #region Singleton
        private static AlgebraicExpressionCategory _instance;
        public static AlgebraicExpressionCategory Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new AlgebraicExpressionCategory();
                return _instance;
            }
        }
        #endregion

        public static bool IsAlgebraicExpressionMember(Port port)
        {
            return (Instance.IsMember(port) == CategoryMembershipLevel.Member);
        }
        public static bool IsNotAlgebraicExpressionMember(Port port)
        {
            return (Instance.IsMember(port) == CategoryMembershipLevel.NotMember);
        }

        #region Serialization
        //protected override void InnerSerialize(XmlWriter writer)
        //{
        //}
        //protected static AlgebraicExpressionCategory InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    return Instance;
        //}
        private static AlgebraicExpressionCategory Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return Instance;
        }
        #endregion
    }
}
