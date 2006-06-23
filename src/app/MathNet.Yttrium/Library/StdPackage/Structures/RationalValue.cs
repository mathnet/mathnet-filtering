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
using MathNet.Symbolics.StdPackage.Trigonometry;
using MathNet.Numerics;

namespace MathNet.Symbolics.StdPackage.Structures
{
    /// <summary>signed rational (fraction)</summary>
    public class RationalValue : ValueStructure, IEquatable<RationalValue>, IComparable<RationalValue>, IAlgebraicField<RationalValue>
    {
        private static readonly MathIdentifier _structureId = new MathIdentifier("Rational", "Std");
        private readonly long _numeratorValue; // = 0;
        private readonly long _denominatorValue = 1;

        #region Conversion & Conversion Routing
        private static readonly ConversionRouter _router = new ConversionRouter(_structureId);
        public static ConversionRouter Converter
        {
            get { return _router; }
        }
        public static void PublishConversionRouteNeighbors(StructureTable table)
        {
            if(table == null) throw new ArgumentNullException("table");
            table.AddStructure(_structureId, _router, typeof(RationalValue));
            _router.AddSourceNeighbor(IntegerValue.Converter, true, LocalConvertFrom);
            IntegerValue.Converter.AddSourceNeighbor(_router, false, LocalConvertToStdInteger);
        }
        private static ValueStructure LocalConvertFrom(ValueStructure value)
        {
            IntegerValue inv = value as IntegerValue;
            if(inv != null) return new RationalValue(inv);
            return (RationalValue)value;
        }
        private static ValueStructure LocalConvertToStdInteger(ValueStructure value)
        {
            RationalValue rv = (RationalValue)value;
            return new IntegerValue(rv._numeratorValue / rv._denominatorValue);
        }
        public static RationalValue ConvertFrom(ValueStructure value)
        {
            return (RationalValue)_router.ConvertFrom(value);
        }
        public static RationalValue ConvertFrom(IntegerValue value) { return new RationalValue(value); }
        public static explicit operator RationalValue(IntegerValue value) { return new RationalValue(value); }
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

            long gcd = Gcd(numerator, denominator);

            _numeratorValue = numerator / gcd;
            _denominatorValue = denominator / gcd;
        }
        public RationalValue(IntegerValue value)
        {
            _numeratorValue = value.Value;
            _denominatorValue = 1;
        }
        #endregion

        #region Basic Operation Processes
        public class AddProcess : GenericFunctionProcess
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
        public class SubtractProcess : GenericFunctionProcess
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
        public class MultiplyProcess : GenericFunctionProcess
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
        public class DivideProcess : GenericFunctionProcess
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

        private static long Gcd(long a, long b)
        {
            long rest;

            if(a > b)
            {
                rest = b;
                b = Math.Abs(a);
                a = Math.Abs(rest);
            }
            else
            {
                a = Math.Abs(a);
                b = Math.Abs(b);
            }

            if(a == 0)
                return b;
            else if(b == 0)
                return a;

            while((rest = b % a) != 0)
            {
                b = a;
                a = rest;
            }
            return a;
        }

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

        public static MathIdentifier StructureIdentifier
        {
            get { return _structureId; }
        }
        public override MathIdentifier StructureId
        {
            get { return _structureId; }
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
            return new RationalValue(
                _numeratorValue * op._denominatorValue + _denominatorValue * op._numeratorValue,
                _denominatorValue * op._denominatorValue);
        }
        public RationalValue Subtract(RationalValue op)
        {
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
            return new RationalValue(
                _numeratorValue * op._numeratorValue,
                _denominatorValue * op._denominatorValue);
        }
        public RationalValue Multiply(IntegerValue op)
        {
            return new RationalValue(
                _numeratorValue * op.Value,
                _denominatorValue);
        }
        public RationalValue Divide(RationalValue op)
        {
            return new RationalValue(
                _numeratorValue * op._denominatorValue,
                _denominatorValue * op._numeratorValue);
        }
        public RationalValue Divide(IntegerValue op)
        {
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
            return new RationalValue((long)Math.Round(Math.Pow(_numeratorValue, op.Value)), (long)Math.Round(Math.Pow(_denominatorValue, op.Value)));
        }
        public RealValue Power(RationalValue op)
        {
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
        public static Signal Constant(Context context, long numerator, long denominator)
        {
            Signal s = new Signal(context, new RationalValue(numerator, denominator));
            s.Label = s.Value.ToString() + "_Constant";
            s.AddConstraint(Properties.ConstantSignalProperty.Instance);
            return s;
        }

        public static Signal ConstantHalf(Context context)
        {
            MathIdentifier id = new MathIdentifier("RationalValueConstantHalf", "Std");
            Signal ret;
            if(context.SingletonSignals.TryGetValue(id, out ret))
                return ret;
            else
            {
                ret = Constant(context, 1, 2);
                context.SingletonSignals.Add(id, ret);
                return ret;
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

        public override string ToString()
        {
            IFormatProvider format = System.Globalization.NumberFormatInfo.InvariantInfo;
            return base.ToString() + "(" + _numeratorValue.ToString(format) + "/" + _denominatorValue.ToString(format) + ")";
        }

        public bool Equals(IntegerValue other)
        {
            return other != null && _numeratorValue == other.Value && _denominatorValue == 1;
        }
        public override bool Equals(ValueStructure other)
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

        #region IAlgebraicRingWithUnity Members
        public bool IsMultiplicativeIdentity
        {
            get { return _numeratorValue == 1 && _denominatorValue == 1; }
        }
        ValueStructure IAlgebraicRingWithUnity.MultiplicativeIdentity
        {
            get { return MultiplicativeIdentity; }
        }
        #endregion
        #region IAlgebraicMonoid Members
        public bool IsAdditiveIdentity
        {
            get { return _numeratorValue == 0 && !double.IsInfinity(_denominatorValue); }
        }
        ValueStructure IAlgebraicMonoid.AdditiveIdentity
        {
            get { return AdditiveIdentity; }
        }
        #endregion

        #region Serialization
        protected override void InnerSerialize(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Numerator", _numeratorValue.ToString(Context.NumberFormat));
            writer.WriteElementString("Denominator", _denominatorValue.ToString(Context.NumberFormat));
        }
        private static RationalValue InnerDeserialize(Context context, System.Xml.XmlReader reader)
        {
            return new RationalValue(long.Parse(reader.ReadElementString("Numerator"), Context.NumberFormat),
                long.Parse(reader.ReadElementString("Denominator"), Context.NumberFormat));
        }
        #endregion
    }
}
