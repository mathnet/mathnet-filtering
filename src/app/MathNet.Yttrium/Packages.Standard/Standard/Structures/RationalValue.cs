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
using System.Xml;

using MathNet.Numerics;
using MathNet.Symbolics.Packages.Standard.Trigonometry;
using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Conversion;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Repository;
using MathNet.Symbolics.Formatter;

namespace MathNet.Symbolics.Packages.Standard.Structures
{
    /// <summary>signed rational (fraction)</summary>
    public class RationalValue : ValueStructureBase, IEquatable<RationalValue>, IComparable<RationalValue>, IAlgebraicField<RationalValue>, IFormattableLeaf
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("Rational", "Std");
        private readonly long _numeratorValue; // = 0;
        private readonly long _denominatorValue = 1;

        #region Conversion & Conversion Routing
        public static void Register(ILibrary library)
        {
            library.AddCustomDataType<RationalValue>();
            //library.AddCustomDataType(new CustomDataRef(typeof(RationalValue), ValueConverter<RationalValue>.Router));
            ValueConverter<RationalValue>.AddConverterFrom<IntegerValue>(true, ConvertFromInteger);
            ValueConverter<RationalValue>.AddConverterTo(ValueConverter<IntegerValue>.Router, false,
                delegate(object value)
                {
                    RationalValue rv = (RationalValue)value;
                    return new IntegerValue(rv._numeratorValue / rv._denominatorValue);
                });
        }
        public static RationalValue ConvertFrom(ICustomData value)
        {
            return (RationalValue)ValueConverter<RationalValue>.ConvertFrom(value);
        }
        public static RationalValue ConvertFromInteger(IntegerValue value) { return new RationalValue(value); }
        public static explicit operator RationalValue(IntegerValue value) { return new RationalValue(value); }
        public static bool CanConvertLosslessFrom(ICustomData value)
        {
            return ValueConverter<RationalValue>.Router.CanConvertLosslessFrom(value);
        }        
        #endregion

        #region Constructors
        public RationalValue()
        {
            //this.dataValue = 0;
            //this.denominatorValue = 1;
        }
        public RationalValue(long numerator, long denominator)
        {
            if(denominator < 0)
            {
                denominator = -denominator;
                numerator = -numerator;
            }

            long gcd = Fn.Gcd(numerator, denominator);

            _numeratorValue = numerator / gcd;
            _denominatorValue = denominator / gcd;
        }
        public RationalValue(IntegerValue value)
        {
            if(value == null)
                throw new ArgumentNullException("value");

            _numeratorValue = value.Value;
            _denominatorValue = 1;
        }
        #endregion

        #region Basic Operation Processes
        internal class AddProcess : GenericFunctionProcess
        {
            public AddProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public AddProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action(bool isInit, Signal origin)
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(RationalValue.AdditiveIdentity);
                else
                {
                    RationalValue sum = ConvertFrom(Inputs[0].Value);
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        RationalValue integer = ConvertFrom(Inputs[i].Value);
                        sum = sum.Add(integer);
                    }
                    PublishToOutputs(sum);
                }
            }
        }
        internal class SubtractProcess : GenericFunctionProcess
        {
            public SubtractProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public SubtractProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action(bool isInit, Signal origin)
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(RationalValue.AdditiveIdentity);
                else
                {
                    RationalValue sum = ConvertFrom(Inputs[0].Value);
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        RationalValue integer = ConvertFrom(Inputs[i].Value);
                        sum = sum.Subtract(integer);
                    }
                    PublishToOutputs(sum);
                }
            }
        }
        internal class MultiplyProcess : GenericFunctionProcess
        {
            public MultiplyProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public MultiplyProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action(bool isInit, Signal origin)
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(RationalValue.MultiplicativeIdentity);
                else
                {
                    RationalValue product = ConvertFrom(Inputs[0].Value);
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        RationalValue integer = ConvertFrom(Inputs[i].Value);
                        product = product.Multiply(integer);
                    }
                    PublishToOutputs(product);
                }
            }
        }
        internal class DivideProcess : GenericFunctionProcess
        {
            public DivideProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public DivideProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action(bool isInit, Signal origin)
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(RationalValue.MultiplicativeIdentity);
                else
                {
                    RationalValue product = ConvertFrom(Inputs[0].Value);
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        RationalValue integer = ConvertFrom(Inputs[i].Value);
                        product = product.Divide(integer);
                    }
                    PublishToOutputs(product);
                }
            }
        }
        #endregion

        public IntegerValue Numerator
        {
            get { return new IntegerValue(_numeratorValue); }
        }
        public IntegerValue Denominator
        {
            get { return new IntegerValue(_denominatorValue); }
        }
        public long NumeratorValue
        {
            get { return _numeratorValue; }
        }
        public long DenominatorValue
        {
            get { return _denominatorValue; }
        }

        public bool IsUnitFraction
        {
            get { return _numeratorValue == 1; }
        }

        public bool IsInteger
        {
            get { return _denominatorValue == 1; }
        }

        public static MathIdentifier TypeIdentifier
        {
            get { return _customTypeId; }
        }
        public override MathIdentifier TypeId
        {
            get { return _customTypeId; }
        }

        #region Direct Operator Implementations
        public static bool operator <(RationalValue left, RationalValue right)
        {
            return left.ToDouble() < right.ToDouble();
        }
        public static bool operator >(RationalValue left, RationalValue right)
        {
            return left.ToDouble() > right.ToDouble();
        }
        #endregion

        #region Direct Function Implementations
        #region Arithmetic Function
        public RationalValue Add(RationalValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RationalValue(
                _numeratorValue * op._denominatorValue + _denominatorValue * op._numeratorValue,
                _denominatorValue * op._denominatorValue);
        }
        public RationalValue Subtract(RationalValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RationalValue(
                _numeratorValue * op._denominatorValue - _denominatorValue * op._numeratorValue,
                _denominatorValue * op._denominatorValue);
        }
        public RationalValue Negate()
        {
            return new RationalValue(
                -_numeratorValue,
                _denominatorValue);
        }
        public RationalValue Multiply(RationalValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RationalValue(
                _numeratorValue * op._numeratorValue,
                _denominatorValue * op._denominatorValue);
        }
        public RationalValue Multiply(IntegerValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RationalValue(
                _numeratorValue * op.Value,
                _denominatorValue);
        }
        public RationalValue Divide(RationalValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RationalValue(
                _numeratorValue * op._denominatorValue,
                _denominatorValue * op._numeratorValue);
        }
        public RationalValue Divide(IntegerValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RationalValue(
                _numeratorValue,
                _denominatorValue * op.Value);
        }
        public RationalValue Invert()
        {
            return new RationalValue(
                _denominatorValue,
                _numeratorValue);
        }
        public RationalValue Absolute()
        {
            return (_numeratorValue < 0) ? new RationalValue(-_numeratorValue, _denominatorValue) : this;
        }
        public RationalValue Power(IntegerValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RationalValue((long)Math.Round(Math.Pow(_numeratorValue, op.Value)), (long)Math.Round(Math.Pow(_denominatorValue, op.Value)));
        }
        public RealValue Power(RationalValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RealValue(Math.Pow(ToDouble(), op.ToDouble()));
        }
        public RationalValue Scale(RationalValue scalar)
        {
            return Multiply(scalar);
        }
        #endregion
        #region Trigonometric Functions
        /// <summary>The principal argument (in radians) of the complex number x+I*y</summary>
        /// <param name="nominator">y</param>
        /// <param name="denominator">x</param>
        public RealValue ReverseTangent()
        {
            return new RealValue(Trig.InverseTangentFromRational(_numeratorValue, _denominatorValue));
        }
        #endregion
        #endregion

        #region Constants
        public static Signal Constant(long numerator, long denominator)
        {
            Signal s = Binder.CreateSignal(new RationalValue(numerator, denominator));
            s.Label = s.Value.ToString() + "_Constant";
            s.EnableFlag(StdAspect.ConstantFlag);
            //s.AddConstraint(Properties.ConstantSignalProperty.Instance);
            return s;
        }

        public static Signal ConstantHalf
        {
            get
            {
                MathIdentifier id = new MathIdentifier("RationalValueConstantHalf", "Std");
                Signal ret;
                if(Service<ISignalCache>.Instance.TryGetValue(id, out ret))
                    return ret;
                else
                {
                    ret = Constant(1, 2);
                    Service<ISignalCache>.Instance.Add(id, ret);
                    return ret;
                }
            }
        }

        public static RationalValue Zero
        {
            get { return new RationalValue(0, 1); } //TODO: Think about Singleton like design...
        }

        public static RationalValue One
        {
            get { return new RationalValue(1, 1); } //TODO: Think about Singleton like design...
        }

        public static RationalValue AdditiveIdentity
        {
            get { return new RationalValue(0, 1); } //TODO: Think about Singleton like design...
        }

        public static RationalValue MultiplicativeIdentity
        {
            get { return new RationalValue(1, 1); } //TODO: Think about Singleton like design...
        }

        public static RationalValue Half
        {
            get { return new RationalValue(1, 2); } //TODO: Think about Singleton like design...
        }
        #endregion

        public double ToDouble()
        {
            return ((double)_numeratorValue) / ((double)_denominatorValue);
        }

        public bool Equals(IntegerValue other)
        {
            return other != null && _numeratorValue == other.Value && _denominatorValue == 1;
        }
        public override bool Equals(IValueStructure other)
        {
            IntegerValue integerValue = other as IntegerValue;
            if(integerValue != null)
                return Equals(integerValue);

            RationalValue rationalValue = other as RationalValue;
            if(rationalValue != null)
                return Equals(rationalValue);

            RealValue realValue = other as RealValue;
            if(realValue != null)
                return realValue.Equals(this);

            return other == this;
        }
        public bool Equals(RationalValue other)
        {
            return other != null && _numeratorValue.Equals(other._numeratorValue) && _denominatorValue.Equals(other._denominatorValue);
        }
        public int CompareTo(RationalValue other)
        {
            return ToDouble().CompareTo(other.ToDouble());
        }
        public override int GetHashCode()
        {
            return _numeratorValue.GetHashCode() ^ _denominatorValue.GetHashCode();
        }

        #region IAlgebraicMultiplicativeIdentityElement Members
        public bool IsMultiplicativeIdentity
        {
            get { return _numeratorValue == 1 && _denominatorValue == 1; }
        }
        IValueStructure IAlgebraicMultiplicativeIdentityElement.MultiplicativeIdentity
        {
            get { return MultiplicativeIdentity; }
        }
        #endregion
        #region IAlgebraicAdditiveIdentityElement Members
        public bool IsAdditiveIdentity
        {
            get { return _numeratorValue == 0 && !double.IsInfinity(_denominatorValue); }
        }
        IValueStructure IAlgebraicAdditiveIdentityElement.AdditiveIdentity
        {
            get { return AdditiveIdentity; }
        }
        #endregion

        #region Serialization
        //protected override void InnerSerialize(System.Xml.XmlWriter writer)
        //{
        //    writer.WriteElementString("Numerator", _numeratorValue.ToString(Config.InternalNumberFormat));
        //    writer.WriteElementString("Denominator", _denominatorValue.ToString(Config.InternalNumberFormat));
        //}
        //private static RationalValue InnerDeserialize(IContext context, System.Xml.XmlReader reader)
        //{
        //    return new RationalValue(long.Parse(reader.ReadElementString("Numerator"), Config.InternalNumberFormat),
        //        long.Parse(reader.ReadElementString("Denominator"), Config.InternalNumberFormat));
        //}
        public override void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            writer.WriteElementString("Numerator", _numeratorValue.ToString(Config.InternalNumberFormat));
            writer.WriteElementString("Denominator", _denominatorValue.ToString(Config.InternalNumberFormat));
        }
        private static RationalValue Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return new RationalValue(long.Parse(reader.ReadElementString("Numerator"), Config.InternalNumberFormat),
                long.Parse(reader.ReadElementString("Denominator"), Config.InternalNumberFormat));
        }
        #endregion

        #region Formatting
        private string FormatImpl(out int precedence)
        {
            IFormatProvider format = System.Globalization.NumberFormatInfo.InvariantInfo;
            if(IsInteger)
            {
                precedence = -1;
                return _numeratorValue.ToString(format);
            }
            else
            {
                precedence = 50;
                return _numeratorValue.ToString(format) + "/" + _denominatorValue.ToString(format);
            }
        }
        public override string ToString()
        {
            int precedence;
            return FormatBase(FormatImpl(out precedence), FormattingOptions.Default);
        }
        string IFormattableLeaf.Format(FormattingOptions options, out int precedence)
        {
            return FormatBase(FormatImpl(out precedence), options);
        }
        #endregion
    }
}
