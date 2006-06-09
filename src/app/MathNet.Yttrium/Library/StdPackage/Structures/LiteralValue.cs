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
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.ValueConversion;

namespace MathNet.Symbolics.StdPackage.Structures
{
    /// <summary>string literal</summary>
    public class LiteralValue : ValueStructure, IEquatable<LiteralValue>, IComparable<LiteralValue>
    {
        private static readonly MathIdentifier _structureId = new MathIdentifier("Literal", "Std");
        private readonly string _dataValue;

        #region Conversion & Conversion Routing
        private static readonly ConversionRouter _router = new ConversionRouter(_structureId);
        public static ConversionRouter Converter
        {
            get { return _router; }
        }
        public static void PublishConversionRouteNeighbors(StructureTable table)
        {
            if(table == null) throw new ArgumentNullException("table");
            table.AddStructure(_structureId, _router, typeof(LiteralValue));
            //router.AddSourceNeighbour(RealValue.Router, true, LocalConvertFrom);
        }
        private static ValueStructure LocalConvertFrom(ValueStructure value)
        {
            //RealValue inv = value as RealValue;
            //if(inv != null) return new LiteralValue(inv.Value.ToString(Context.NumberFormat));
            return (LiteralValue)value;
        }
        public static LiteralValue ConvertFrom(ValueStructure value)
        {
            return (LiteralValue)_router.ConvertFrom(value);
        }
        //public static LiteralValue ConvertFrom(RealValue value) { return new LiteralValue(value); }
        //public static explicit operator LiteralValue(RealValue value) { return new LiteralValue(value); }
        #endregion

        #region Constructors
        public LiteralValue()
        {
            _dataValue = string.Empty;
        }
        public LiteralValue(string value)
        {
            _dataValue = value;
        }
        #endregion

        public string Value
        {
            get { return _dataValue; }
        }

        public static MathIdentifier StructureIdentifier
        {
            get { return _structureId; }
        }
        public override MathIdentifier StructureId
        {
            get { return _structureId; }
        }

        #region Direct Function Implementations
        public LiteralValue Concatenate(LiteralValue op)
        {
            return new LiteralValue(_dataValue + op._dataValue);
        }
        #endregion

        #region Constants
        public static Signal ParseConstant(Context context, string value)
        {
            return Constant(context, value.Trim());
        }
        public static Signal Constant(Context context, string value)
        {
            Signal s = new Signal(context, new LiteralValue(value));
            s.Label = s.Value.ToString() + "_Constant";
            s.Properties.AddProperty(Properties.ConstantSignalProperty.Instance);
            return s;
        }

        public static Signal ConstantEmpty(Context context)
        {
            MathIdentifier id = new MathIdentifier("LiteralValueConstantEmpty", "Std");
            Signal ret;
            if(context.SingletonSignals.TryGetValue(id, out ret))
                return ret;
            else
            {
                ret = Constant(context, string.Empty);
                context.SingletonSignals.Add(id, ret);
                return ret;
            }
        }

        public static LiteralValue Empty
        {
            get { return new LiteralValue(string.Empty); } //TODO: Think about Singleton like design...
        }
        #endregion

        public override string ToString()
        {
            return base.ToString() + "(" + _dataValue + ")";
        }

        public override bool Equals(ValueStructure other)
        {
            LiteralValue literalValue = other as LiteralValue;
            if(literalValue != null)
                return Equals(literalValue);

            return other == this;
        }
        public bool Equals(LiteralValue other)
        {
            return other != null && _dataValue.Equals(other._dataValue);
        }
        public int CompareTo(LiteralValue other)
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
            writer.WriteString(_dataValue);
        }
        private static LiteralValue InnerDeserialize(Context context, System.Xml.XmlReader reader)
        {
            return new LiteralValue(reader.ReadString());
        }
        #endregion
    }
}
