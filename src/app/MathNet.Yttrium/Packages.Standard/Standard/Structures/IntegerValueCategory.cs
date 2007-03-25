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
using System.Collections.Generic;
using System.Text;
using System.Xml;


using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Packages.ObjectModel;

namespace MathNet.Symbolics.Packages.Standard.Structures
{
    /// <summary>
    /// The category Z: ordered set of all signed integers
    /// </summary>
    public class IntegerValueCategory : CategoryBase
    {
        protected IntegerValueCategory()
            : base("IntegerValueExpression", "Std") { }

        public override CategoryMembershipLevel IsMember(Signal signal, bool ignoreCache)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(signal.Value == null)
                return CategoryMembershipLevel.Unknown;
            if(signal.Value is IntegerValue)
                return CategoryMembershipLevel.Member;
            return CategoryMembershipLevel.NotMember;
        }

        public override CategoryMembershipLevel IsMember(Port port)
        {
            return EvaluateJointChildMembership(port.InputSignals);
        }

        #region Singleton
        private static IntegerValueCategory _instance;
        public static IntegerValueCategory Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new IntegerValueCategory();
                return _instance;
            }
        }
        #endregion

        public static bool IsIntegerValueMember(Port port)
        {
            return (Instance.IsMember(port) == CategoryMembershipLevel.Member);
        }
        public static bool IsNotIntegerValueMember(Port port)
        {
            return (Instance.IsMember(port) == CategoryMembershipLevel.NotMember);
        }

        #region Serialization
        //protected override void InnerSerialize(XmlWriter writer)
        //{
        //}
        //public static IntegerValueCategory InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    return Instance;
        //}
        private static IntegerValueCategory Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return Instance;
        }
        #endregion
    }
}
