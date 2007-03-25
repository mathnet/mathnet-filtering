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
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Packages.ObjectModel;

namespace MathNet.Symbolics.Packages.Standard.Properties
{
    /// <summary>
    /// Property restricting a signal to be an integer > 0, that is one of 1,2,3,...
    /// </summary>
    public sealed class PositiveIntegerSetProperty : PropertyBase
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("PositiveIntegerSet", "Std");
        private PositiveIntegerSetProperty() { }

        #region Singleton
        private static PositiveIntegerSetProperty _instance;
        public static PositiveIntegerSetProperty Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new PositiveIntegerSetProperty();
                return _instance;
            }
        }
        #endregion

        public static MathIdentifier TypeIdentifier
        {
            get { return _customTypeId; }
        }
        public override MathIdentifier TypeId
        {
            get { return _customTypeId; }
        }

        public override bool StillValidAfterEvent(Signal signal)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(signal.Value == null)
                return false;
            return IntegerValue.CanConvertLosslessFrom(signal.Value) && IntegerValue.ConvertFrom(signal.Value).Value >= 0;
        }
        public override bool StillValidAfterDrive(Signal signal)
        {
            return false;
        }
        public override bool StillValidAfterUndrive(Signal signal)
        {
            return true;
        }

        public override bool Equals(IProperty other)
        {
            return other is PositiveIntegerSetProperty;
        }

        #region Serialization
        //protected override void InnerSerialize(XmlWriter writer)
        //{
        //}
        //private static PositiveIntegerSetProperty InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    return Instance;
        //}
        private static PositiveIntegerSetProperty Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return Instance;
        }
        #endregion
    }
}
