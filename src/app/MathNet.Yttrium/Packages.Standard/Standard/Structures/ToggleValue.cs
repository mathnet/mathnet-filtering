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

using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Conversion;
using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.Packages.Standard.Structures
{
    public sealed class ToggleValue : ValueStructureBase, IEquatable<ToggleValue>, IComparable<ToggleValue>
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("Toggle", "Std");
        private readonly bool _dataValue; // = false;
        private static ToggleValue _toggleA, _toggleB;

        #region Conversion & Conversion Routing
        public static void Register(ILibrary library)
        {
            library.AddCustomDataType<ToggleValue>();
            //library.AddCustomDataType(new CustomDataRef(typeof(ToggleValue), ValueConverter<ToggleValue>.Router));
            ValueConverter<ToggleValue>.AddConverterFrom(ValueConverter<IntegerValue>.Router, false,
                delegate(ICustomData value) { return ((IntegerValue)value).Value == 0 ? _toggleB : _toggleA; });
            ValueConverter<ToggleValue>.AddConverterTo(ValueConverter<IntegerValue>.Router, true,
                delegate(ICustomData value) { return ((ToggleValue)value)._dataValue ? IntegerValue.Zero : IntegerValue.One; });
        }
        
        public static ToggleValue ConvertFrom(IValueStructure value)
        {
            return (ToggleValue)ValueConverter<ToggleValue>.ConvertFrom(value);
        }
        public static bool CanConvertLosslessFrom(IValueStructure value)
        {
            return ValueConverter<ToggleValue>.Router.CanConvertLosslessFrom(value);
        }        
        #endregion

        public static ToggleValue InitialToggle
        {
            get
            {
                if(_toggleA == null)
                {
                    _toggleA = new ToggleValue(false);
                    _toggleB = new ToggleValue(true);
                }
                return _toggleA;
            }
        }

        public static MathIdentifier TypeIdentifier
        {
            get { return _customTypeId; }
        }
        public override MathIdentifier TypeId
        {
            get { return _customTypeId; }
        }

        private ToggleValue(bool value)
        {
            _dataValue = value;
        }

        public ToggleValue Toggle()
        {
            return _dataValue ? _toggleA : _toggleB;
        }

        public override string ToString()
        {
            return base.ToString() + (_dataValue ? "(B)" : "(A)");
        }

        public override bool Equals(IValueStructure other)
        {
            ToggleValue toggleValue = other as ToggleValue;
            if(toggleValue != null)
                return Equals(toggleValue);

            return other == this;
        }
        public bool Equals(ToggleValue other)
        {
            return other != null && _dataValue.Equals(other._dataValue);
        }
        public int CompareTo(ToggleValue other)
        {
            return _dataValue.CompareTo(other._dataValue);
        }
        public override int GetHashCode()
        {
            return _dataValue.GetHashCode();
        }

        #region Serialization
        //protected override void InnerSerialize(System.Xml.XmlWriter writer)
        //{
        //    if(_dataValue)
        //        writer.WriteString("B");
        //    else
        //        writer.WriteString("A");
        //}
        //private static ToggleValue InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    if(reader.ReadString().Equals("B"))
        //        return InitialToggle.Toggle();
        //    else
        //        return InitialToggle;
        //}
        public override void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            if(_dataValue)
                writer.WriteString("B");
            else
                writer.WriteString("A");
        }
        private static ToggleValue Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            if(reader.ReadString().Equals("B"))
                return InitialToggle.Toggle();
            else
                return InitialToggle;
        }
        #endregion
    }
}
