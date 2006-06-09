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
using System.Collections.Generic;
using System.Text;
using System.Xml;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Properties;

namespace MathNet.Symbolics.StdPackage.Structures
{
    /// <summary>
    /// The category R: set of all signed real numbers.
    /// </summary>
    public class RealValueCategory : Category
    {
        protected RealValueCategory()
            : base("RealValueExpression", "Std") { }

        public override ECategoryMembership IsMember(Signal signal, bool ignoreCache)
        {
            if(signal.Value == null)
                return ECategoryMembership.Unknown;
            if(signal.Value is RealValue || signal.Value is RationalValue || signal.Value is IntegerValue)
                return ECategoryMembership.Member;
            return ECategoryMembership.NotMember;
        }

        public override ECategoryMembership IsMember(Port port)
        {
            return EvaluateJointChildMembership(port.InputSignals);
        }

        #region Singleton
        private static RealValueCategory _instance;
        public static RealValueCategory Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new RealValueCategory();
                return _instance;
            }
        }
        #endregion

        public static bool IsRealValueMember(Port port)
        {
            return (Instance.IsMember(port) == ECategoryMembership.Member);
        }
        public static bool IsNotRealValueMember(Port port)
        {
            return (Instance.IsMember(port) == ECategoryMembership.NotMember);
        }

        #region Serialization
        protected override void InnerSerialize(XmlWriter writer)
        {
        }
        protected static RealValueCategory InnerDeserialize(Context context, XmlReader reader)
        {
            return Instance;
        }
        #endregion
    }
}
