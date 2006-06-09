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
    /// <summary>
    /// Category of all algebraic expressions.
    /// </summary>
    /// <remarks>
    /// An algebraic expression is constructed using integers, symbols,
    /// function forms and the algebraic operators (+,-,*,/,^,!).
    /// </remarks>
    public class AlgebraicExpressionCategory : Category
    {
        protected AlgebraicExpressionCategory()
            : base("AlgebraicExpression", "Std")
        {
        }

        public override ECategoryMembership IsMember(Signal signal, bool ignoreCache)
        {
            if(signal.IsCyclic)
                return ECategoryMembership.NotMember;

            ECategoryMembership membership;
            if(ignoreCache)
                membership = ECategoryMembership.Unknown;
            else
                if(TryGetCachedMembership(signal, out membership))
                    return membership;

            if(signal.BehavesAsSourceSignal) //Symbol (maybe we should apply harder constraints here..)
                return ECategoryMembership.Member;

            Port port = signal.DrivenByPort;
            if(port.IsCompletelyConnected)
                return IsMember(port);
            else
                return ECategoryMembership.NotMember;
        }

        public override ECategoryMembership IsMember(Port port)
        {
            for(int i = 0; i < port.OutputSignalCount; i++)
                if(port.OutputSignals[i].IsCyclic)
                    return ECategoryMembership.NotMember;

            ECategoryMembership childMembership;
            Entity entity = port.Entity;

            if(port.BusCount != 0)
                return ECategoryMembership.NotMember;

            childMembership = EvaluateJointChildMembership(port.InputSignals);
            if(childMembership == ECategoryMembership.NotMember)
                return ECategoryMembership.NotMember;

            MathIdentifier id = entity.EntityId;

            if(id.Equals("Add", "Std") || id.Equals("Subtract", "Std") || id.Equals("Multiply", "Std")
                || id.Equals("Divide", "Std") || id.Equals("Power", "Std") || id.Equals("Factorial", "Std"))
            {
                if(childMembership == ECategoryMembership.Member)
                    return ECategoryMembership.Member;
                else //ECategoryMembership.Unknown
                    return ECategoryMembership.Unknown;
            }
            else
                return ECategoryMembership.NotMember;
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
            return (Instance.IsMember(port) == ECategoryMembership.Member);
        }
        public static bool IsNotAlgebraicExpressionMember(Port port)
        {
            return (Instance.IsMember(port) == ECategoryMembership.NotMember);
        }

        #region Serialization
        protected override void InnerSerialize(XmlWriter writer)
        {
        }
        protected static AlgebraicExpressionCategory InnerDeserialize(Context context, XmlReader reader)
        {
            return Instance;
        }
        #endregion
    }
}
