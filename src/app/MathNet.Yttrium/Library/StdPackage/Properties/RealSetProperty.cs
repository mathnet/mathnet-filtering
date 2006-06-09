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
using System.Xml;
using System.Text;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Properties;
using MathNet.Symbolics.StdPackage.Structures;

namespace MathNet.Symbolics.StdPackage.Properties
{
    public sealed class RealSetProperty : Property
    {
        private static readonly MathIdentifier _propertyId = new MathIdentifier("RealSet", "Std");
        private RealSetProperty() { }

        #region Singleton
        private static RealSetProperty _instance;
        public static RealSetProperty Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new RealSetProperty();
                return _instance;
            }
        }
        #endregion

        public static MathIdentifier PropertyIdentifier
        {
            get { return _propertyId; }
        }
        public override MathIdentifier PropertyId
        {
            get { return _propertyId; }
        }

        public override bool StillValidAfterEvent(Signal signal)
        {
            if(signal.Value == null)
                return false;
            return RealValue.Converter.CanConvertLosslessFrom(signal.Value);
        }
        public override bool StillValidAfterDrive(Signal signal)
        {
            return false;
        }
        public override bool StillValidAfterUndrive(Signal signal)
        {
            return true;
        }

        public override bool Equals(Property other)
        {
            return other is RealSetProperty;
        }

        #region Serialization
        protected override void InnerSerialize(XmlWriter writer)
        {
        }
        private static RealSetProperty InnerDeserialize(Context context, XmlReader reader)
        {
            return Instance;
        }
        #endregion
    }
}
