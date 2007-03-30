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

using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Packages.Standard;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Conversion;
using System.Xml;

namespace MathNet.Symbolics.Packages.PetriNet
{
    public class TokenValue : ValueStructureBase, IEquatable<TokenValue>, IComparable<TokenValue>, IAlgebraicAbelianGroup<TokenValue>
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("Token", "PetriNet");
        private readonly long _dataValue; // = 0;

        #region Conversion & Conversion Routing
        public static void Register(ILibrary library)
        {
            library.AddCustomDataType<TokenValue>();
            ValueConverter<TokenValue>.AddConverterFrom<IntegerValue>(true, ConvertFrom);
            ValueConverter<TokenValue>.AddConverterTo(ValueConverter<IntegerValue>.Router, true,
                delegate(object value)
                {
                    return new IntegerValue(((TokenValue)value).Value);
                });
        }
        public static TokenValue ConvertFrom(ICustomData value)
        {
            return (TokenValue)ValueConverter<TokenValue>.ConvertFrom(value);
        }
        public static TokenValue ConvertFrom(IntegerValue value) { return new TokenValue(value.Value); }
        public static explicit operator TokenValue(IntegerValue value) { return new TokenValue(value.Value); }
        public static bool CanConvertLosslessFrom(ICustomData value)
        {
            return ValueConverter<TokenValue>.Router.CanConvertLosslessFrom(value);
        }
        #endregion

        #region Constructors
        public TokenValue()
        {
            //this.dataValue = 0;
        }
        public TokenValue(long value)
        {
            _dataValue = value;
        }
        #endregion

        public long Value
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
        public TokenValue Add(TokenValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new TokenValue(_dataValue + op._dataValue);
        }
        public TokenValue Subtract(TokenValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new TokenValue(_dataValue - op._dataValue);
        }
        public TokenValue Negate()
        {
            return new TokenValue(-_dataValue);
        }
        public TokenValue Increment()
        {
            return new TokenValue(_dataValue + 1);
        }
        public TokenValue Decrement()
        {
            return new TokenValue(_dataValue - 1);
        }
        public TokenValue Absolute()
        {
            return (_dataValue < 0) ? new TokenValue(-_dataValue) : this;
        }
        public TokenValue Max(TokenValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return (_dataValue >= op._dataValue) ? this : op;
        }
        public TokenValue Min(TokenValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return (_dataValue <= op._dataValue) ? this : op;
        }
        #endregion

        #region Constants
        public static TokenValue Zero
        {
            get { return new TokenValue(0); } //TODO: Think about Singleton like design...
        }

        public static TokenValue One
        {
            get { return new TokenValue(1); } //TODO: Think about Singleton like design...
        }

        public static TokenValue AdditiveIdentity
        {
            get { return new TokenValue(0); } //TODO: Think about Singleton like design...
        }
        #endregion

        public override string ToString()
        {
            return base.ToString() + "(" + _dataValue.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ")";
        }

        public bool Equals(TokenValue other)
        {
            return other != null && _dataValue.Equals(other._dataValue);
        }
        public override bool Equals(IValueStructure other)
        {
            TokenValue integerValue = other as TokenValue;
            if(integerValue != null)
                return Equals(integerValue);

            return false;
        }
        
        public int CompareTo(TokenValue other)
        {
            return _dataValue.CompareTo(other._dataValue);
        }
        public override int GetHashCode()
        {
            return _dataValue.GetHashCode();
        }

        #region IAlgebraicMonoid Members
        public bool IsAdditiveIdentity
        {
            get { return _dataValue == 0; }
        }
        IValueStructure IAlgebraicMonoid.AdditiveIdentity
        {
            get { return AdditiveIdentity; }
        }
        #endregion

        #region Serialization
        public override void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            writer.WriteString(_dataValue.ToString(Config.InternalNumberFormat));
        }
        private static TokenValue Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return new TokenValue(long.Parse(reader.ReadString(), Config.InternalNumberFormat));
        }
        #endregion

    }
}
