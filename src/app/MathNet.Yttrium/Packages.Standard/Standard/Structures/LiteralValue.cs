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
using System.Collections.Generic;

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Conversion;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Repository;

namespace MathNet.Symbolics.Packages.Standard.Structures
{
    /// <summary>string literal</summary>
    public class LiteralValue : ValueStructureBase, IEquatable<LiteralValue>, IComparable<LiteralValue>
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("Literal", "Std");
        private readonly string _dataValue;

        #region Conversion & Conversion Routing
        public static void Register(ILibrary library)
        {
            library.AddCustomDataType<LiteralValue>();
            //library.AddCustomDataType(new CustomDataRef(typeof(LiteralValue), ValueConverter<LiteralValue>.Router));
            ValueConverter<LiteralValue>.AddConverterFrom<IntegerValue>(true, ConvertFrom);
        }
        public static LiteralValue ConvertFrom(ICustomData value)
        {
            return (LiteralValue)ValueConverter<LiteralValue>.ConvertFrom(value);
        }
        public static LiteralValue ConvertFrom(IntegerValue value) { return new LiteralValue(value.ToString()); }
        public static explicit operator LiteralValue(IntegerValue value) { return new LiteralValue(value.ToString()); }
        public static bool CanConvertLosslessFrom(ICustomData value)
        {
            return ValueConverter<LiteralValue>.Router.CanConvertLosslessFrom(value);
        }        
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

        public static MathIdentifier TypeIdentifier
        {
            get { return _customTypeId; }
        }
        public override MathIdentifier TypeId
        {
            get { return _customTypeId; }
        }

        #region Direct Function Implementations
        public LiteralValue Concatenate(LiteralValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new LiteralValue(_dataValue + op._dataValue);
        }
        #endregion

        #region Constants
        public static Signal ParseConstant(string value)
        {
            if(value == null)
                throw new ArgumentNullException("value");

            return Constant(value.Trim());
        }
        public static Signal Constant(string value)
        {
            Signal s = Binder.CreateSignal(new LiteralValue(value));
            s.Label = s.Value.ToString() + "_Constant";
            s.AddConstraint(Properties.ConstantSignalProperty.Instance);
            return s;
        }

        public static Signal ConstantEmpty
        {
            get
            {
                MathIdentifier id = new MathIdentifier("LiteralValueConstantEmpty", "Std");
                Signal ret;
                if(Service<ISignalCache>.Instance.TryGetValue(id, out ret))
                    return ret;
                else
                {
                    ret = Constant(string.Empty);
                    Service<ISignalCache>.Instance.Add(id, ret);
                    return ret;
                }
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

        public override bool Equals(IValueStructure other)
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
        //protected override void InnerSerialize(System.Xml.XmlWriter writer)
        //{
        //    writer.WriteString(_dataValue);
        //}
        //private static LiteralValue InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    return new LiteralValue(reader.ReadString());
        //}
        public override void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            writer.WriteString(_dataValue);
        }
        private static LiteralValue Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return new LiteralValue(reader.ReadString());
        }
        #endregion
    }
}
