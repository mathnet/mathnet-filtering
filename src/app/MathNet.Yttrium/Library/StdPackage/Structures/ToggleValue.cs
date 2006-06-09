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
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.ValueConversion;

namespace MathNet.Symbolics.StdPackage.Structures
{
    public sealed class ToggleValue : ValueStructure, IEquatable<ToggleValue>, IComparable<ToggleValue>
    {
        private static readonly MathIdentifier _structureId = new MathIdentifier("Toggle", "Std");
        private readonly bool _dataValue; // = false;
        private static ToggleValue _toggleA, _toggleB;

        #region Conversion & Conversion Routing
        private static readonly ConversionRouter _router = new ConversionRouter(_structureId);
        public static ConversionRouter Converter
        {
            get { return _router; }
        }
        public static void PublishConversionRouteNeighbors(StructureTable table)
        {
            if(table == null) throw new ArgumentNullException("table");
            table.AddStructure(_structureId, _router, typeof(ToggleValue));
            _router.AddSourceNeighbor(IntegerValue.Converter, false, LocalConvertFrom);
        }
        private static ValueStructure LocalConvertFrom(ValueStructure value)
        {
            IntegerValue inv = value as IntegerValue;
            if(inv != null) return inv.Value == 0 ? _toggleB : _toggleA;
            return (ToggleValue)value;
        }
        public static ToggleValue ConvertFrom(ValueStructure value)
        {
            return (ToggleValue)_router.ConvertFrom(value);
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

        public static MathIdentifier StructureIdentifier
        {
            get { return _structureId; }
        }
        public override MathIdentifier StructureId
        {
            get { return _structureId; }
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
            return base.ToString() + (_dataValue ? "(A)" : "(B)");
        }

        public override bool Equals(ValueStructure other)
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
        protected override void InnerSerialize(System.Xml.XmlWriter writer)
        {
            if(_dataValue)
                writer.WriteString("B");
            else
                writer.WriteString("A");
        }
        private static ToggleValue InnerDeserialize(Context context, System.Xml.XmlReader reader)
        {
            if(reader.ReadString().Equals("B"))
                return InitialToggle.Toggle();
            else
                return InitialToggle;
        }
        #endregion
    }
}
