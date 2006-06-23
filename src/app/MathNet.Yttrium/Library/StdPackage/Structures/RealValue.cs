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
    /// <summary>signed real numbers</summary>
    public class RealValue : ValueStructure, IEquatable<RealValue>, IComparable<RealValue>, IAlgebraicField<RealValue>
    {
        private static readonly MathIdentifier _structureId = new MathIdentifier("Real", "Std");
        private readonly double _dataValue; // = 0;

        #region Conversion & Conversion Routing
        private static readonly ConversionRouter _router = new ConversionRouter(_structureId);
        public static ConversionRouter Converter
        {
            get { return _router; }
        }
        public static void PublishConversionRouteNeighbors(StructureTable table)
        {
            if(table == null) throw new ArgumentNullException("table");
            table.AddStructure(_structureId, _router, typeof(RealValue));
            _router.AddSourceNeighbor(IntegerValue.Converter, true, LocalConvertFrom);
            _router.AddSourceNeighbor(RationalValue.Converter, true, LocalConvertFrom);
            IntegerValue.Converter.AddSourceNeighbor(_router, false, LocalConvertToStdInteger);
        }
        private static ValueStructure LocalConvertFrom(ValueStructure value)
        {
            IntegerValue inv = value as IntegerValue;
            if(inv != null) return new RealValue(inv);
            RationalValue rav = value as RationalValue;
            if(rav != null) return new RealValue(rav);
            return (RealValue)value;
        }
        private static ValueStructure LocalConvertToStdInteger(ValueStructure value)
        {
            return new IntegerValue((long)Math.Round(((RealValue)value).Value,0));
        }
        public static RealValue ConvertFrom(ValueStructure value)
        {
            return (RealValue)_router.ConvertFrom(value);
        }
        public static RealValue ConvertFrom(IntegerValue value) { return new RealValue(value); }
        public static explicit operator RealValue(IntegerValue value) { return new RealValue(value); }
        public static RealValue ConvertFrom(RationalValue value) { return new RealValue(value); }
        public static explicit operator RealValue(RationalValue value) { return new RealValue(value); }
        #endregion

        #region Constructors
        public RealValue()
        {
            //this.dataValue = 0;
        }
        public RealValue(double value)
        {
            _dataValue = value;
        }
        public RealValue(IntegerValue value)
        {
            _dataValue = (double)value.Value;
        }
        public RealValue(RationalValue value)
        {
            _dataValue = (double)value.NumeratorValue / (double)value.DenominatorValue;
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
                    PublishToOutputs(RealValue.AdditiveIdentity);
                else
                {
                    RealValue sum = ConvertFrom(Inputs[0].Value);
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        RealValue integer = ConvertFrom(Inputs[i].Value);
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
                    PublishToOutputs(RealValue.AdditiveIdentity);
                else
                {
                    RealValue sum = ConvertFrom(Inputs[0].Value);
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        RealValue integer = ConvertFrom(Inputs[i].Value);
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
                    PublishToOutputs(RealValue.MultiplicativeIdentity);
                else
                {
                    RealValue product = ConvertFrom(Inputs[0].Value);
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        RealValue integer = ConvertFrom(Inputs[i].Value);
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
                    PublishToOutputs(RealValue.MultiplicativeIdentity);
                else
                {
                    RealValue product = ConvertFrom(Inputs[0].Value);
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        RealValue integer = ConvertFrom(Inputs[i].Value);
                        product = product.Divide(integer);
                    }
                    PublishToOutputs(product);
                }
            }
        }
        #endregion

        public double Value
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
        #region Arithmetic Function
        public RealValue Add(RealValue op)
        {
            return new RealValue(_dataValue + op._dataValue);
        }
        public RealValue Subtract(RealValue op)
        {
            return new RealValue(_dataValue - op._dataValue);
        }
        public RealValue Negate()
        {
            return new RealValue(-_dataValue);
        }
        public RealValue Multiply(RealValue op)
        {
            return new RealValue(_dataValue * op._dataValue);
        }
        public RealValue Divide(RealValue op)
        {
            return new RealValue(_dataValue / op.Value);
        }
        public RealValue Invert()
        {
            return new RealValue(1d / _dataValue);
        }
        public RealValue Absolute()
        {
            return (_dataValue < 0) ? new RealValue(-_dataValue) : this;
        }
        public RealValue Scale(RealValue scalar)
        {
            return Multiply(scalar);
        }
        #endregion
        #region Exponential Functions
        public RealValue Exponential()
        {
            return new RealValue(Math.Exp(_dataValue));
        }
        public RealValue NaturalLogarithm()
        {
            return new RealValue(Math.Log(_dataValue));
        }
        public RealValue Power(RealValue op)
        {
            return new RealValue(Math.Pow(_dataValue, op._dataValue));
        }
        public RealValue Root(RealValue op)
        {
            return new RealValue(Math.Pow(_dataValue, 1 / op.Value));
        }
        public RealValue Square()
        {
            return new RealValue(_dataValue * _dataValue);
        }
        public RealValue SquareRoot()
        {
            return new RealValue(Math.Sqrt(_dataValue));
        }
        #endregion
        #region Trigonometric Functions
        /// <summary>Trigonometric Sine (Sinus) of an angle in radians</summary>
        public RealValue Sine()
        {
            return new RealValue(Trig.Sine(_dataValue));
        }
        /// <summary>Trigonometric Cosine (Cosinus) of an angle in radians</summary>
        public RealValue Cosine()
        {
            return new RealValue(Trig.Cosine(_dataValue));
        }
        /// <summary>Trigonometric Tangent (Tangens) of an angle in radians</summary>
        public RealValue Tangent()
        {
            return new RealValue(Trig.Tangent(_dataValue));
        }
        /// <summary>Trigonometric Cotangent (Cotangens) of an angle in radians</summary>
        public RealValue Cotangent()
        {
            return new RealValue(Trig.Cotangent(_dataValue));
        }
        /// <summary>Trigonometric Secant (Sekans) of an angle in radians</summary>
        public RealValue Secant()
        {
            return new RealValue(Trig.Secant(_dataValue));
        }
        /// <summary>Trigonometric Cosecant (Cosekans) of an angle in radians</summary>
        public RealValue Cosecant()
        {
            return new RealValue(Trig.Cosecant(_dataValue));
        }
        #endregion
        #region Trigonometric Inverse Functions
        /// <summary>Trigonometric Arcus Sine (Arkussinus) in radians</summary>
        public RealValue InverseSine()
        {
            return new RealValue(Trig.InverseSine(_dataValue));
        }
        /// <summary>Trigonometric Arcus Cosine (Arkuscosinus) in radians</summary>
        public RealValue InverseCosine()
        {
            return new RealValue(Trig.InverseCosine(_dataValue));
        }
        /// <summary>Trigonometric Arcus Tangent (Arkustangens) in radians</summary>
        public RealValue InverseTangent()
        {
            return new RealValue(Trig.InverseTangent(_dataValue));
        }
        /// <summary>Trigonometric Arcus Cotangent (Arkuscotangens) in radians</summary>
        public RealValue InverseCotangent()
        {
            return new RealValue(Trig.InverseCotangent(_dataValue));
        }
        /// <summary>Trigonometric Arcus Secant (Arkussekans) in radians</summary>
        public RealValue InverseSecant()
        {
            return new RealValue(Trig.InverseSecant(_dataValue));
        }
        /// <summary>Trigonometric Arcus Cosecant (Arkuscosekans) in radians</summary>
        public RealValue InverseCosecant()
        {
            return new RealValue(Trig.InverseCosecant(_dataValue));
        }
        #endregion
        #region Hyperbolic Functions
        /// <summary>Trigonometric Hyperbolic Sine (Sinus hyperbolicus)</summary>
        public RealValue HyperbolicSine()
        {
            return new RealValue(Trig.HyperbolicSine(_dataValue));
        }
        /// <summary>Trigonometric Hyperbolic Cosine (Cosinus hyperbolicus)</summary>
        public RealValue HyperbolicCosine()
        {
            return new RealValue(Trig.HyperbolicCosine(_dataValue));
        }
        /// <summary>Trigonometric Hyperbolic Tangent (Tangens hyperbolicus)</summary>
        public RealValue HyperbolicTangent()
        {
            return new RealValue(Trig.HyperbolicTangent(_dataValue));
        }
        /// <summary>Trigonometric Hyperbolic Cotangent (Cotangens hyperbolicus)</summary>
        public RealValue HyperbolicCotangent()
        {
            return new RealValue(Trig.HyperbolicCotangent(_dataValue));
        }
        /// <summary>Trigonometric Hyperbolic Secant (Sekans hyperbolicus)</summary>
        public RealValue HyperbolicSecant()
        {
            return new RealValue(Trig.HyperbolicSecant(_dataValue));
        }
        /// <summary>Trigonometric Hyperbolic Cosecant (Cosekans hyperbolicus)</summary>
        public RealValue HyperbolicCosecant()
        {
            return new RealValue(Trig.HyperbolicCosecant(_dataValue));
        }
        #endregion
        #region Hyperbolic Area Functions
        /// <summary>Trigonometric Hyperbolic Area Sine (Areasinus hyperbolicus)</summary>
        public RealValue InverseHyperbolicSine()
        {
            return new RealValue(Trig.InverseHyperbolicSine(_dataValue));
        }
        /// <summary>Trigonometric Hyperbolic Area Cosine (Areacosinus hyperbolicus)</summary>
        public RealValue InverseHyperbolicCosine()
        {
            return new RealValue(Trig.InverseHyperbolicCosine(_dataValue));
        }
        /// <summary>Trigonometric Hyperbolic Area Tangent (Areatangens hyperbolicus)</summary>
        public RealValue InverseHyperbolicTangent()
        {
            return new RealValue(Trig.InverseHyperbolicTangent(_dataValue));
        }
        /// <summary>Trigonometric Hyperbolic Area Cotangent (Areacotangens hyperbolicus)</summary>
        public RealValue InverseHyperbolicCotangent()
        {
            return new RealValue(Trig.InverseHyperbolicCotangent(_dataValue));
        }
        /// <summary>Trigonometric Hyperbolic Area Secant (Areasekans hyperbolicus)</summary>
        public RealValue InverseHyperbolicSecant()
        {
            return new RealValue(Trig.InverseHyperbolicSecant(_dataValue));
        }
        /// <summary>Trigonometric Hyperbolic Area Cosecant (Areacosekans hyperbolicus)</summary>
        public RealValue InverseHyperbolicCosecant()
        {
            return new RealValue(Trig.InverseHyperbolicCosecant(_dataValue));
        }
        #endregion
        #endregion

        #region Constants
        public static Signal ParseConstant(Context context, string value)
        {
            return Constant(context, double.Parse(value, Context.NumberFormat), value.Trim());
        }
        public static Signal Constant(Context context, double value)
        {
            return Constant(context, value, value.ToString(Context.NumberFormat));
        }
        public static Signal Constant(Context context, double value, string name)
        {
            Signal s = new Signal(context, new RealValue(value));
            s.Label = name + "_Constant";
            s.AddConstraint(Properties.ConstantSignalProperty.Instance);
            return s;
        }

        public static Signal ConstantPI(Context context)
        {
            MathIdentifier id = new MathIdentifier("RealValueConstantPI", "Std");
            Signal ret;
            if(context.SingletonSignals.TryGetValue(id, out ret))
                return ret;
            else
            {
                ret = Constant(context, Math.PI, "PI");
                context.SingletonSignals.Add(id, ret);
                return ret;
            }
        }

        public static Signal ConstantE(Context context)
        {
            MathIdentifier id = new MathIdentifier("RealValueConstantE", "Std");
            Signal ret;
            if(context.SingletonSignals.TryGetValue(id, out ret))
                return ret;
            else
            {
                ret = Constant(context, Math.E, "E");
                context.SingletonSignals.Add(id, ret);
                return ret;
            }
        }

        public static RealValue PI
        {
            get { return new RealValue(Math.PI); } //TODO: Think about Singleton like design...
        }

        public static RealValue E
        {
            get { return new RealValue(Math.E); } //TODO: Think about Singleton like design...
        }

        public static RealValue Zero
        {
            get { return new RealValue(0); } //TODO: Think about Singleton like design...
        }

        public static RealValue One
        {
            get { return new RealValue(1); } //TODO: Think about Singleton like design...
        }

        public static RealValue AdditiveIdentity
        {
            get { return new RealValue(0); } //TODO: Think about Singleton like design...
        }

        public static RealValue MultiplicativeIdentity
        {
            get { return new RealValue(1); } //TODO: Think about Singleton like design...
        }
        #endregion

        public override string ToString()
        {
            return base.ToString() + "(" + _dataValue.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ")";
        }

        public bool Equals(RationalValue other)
        {
            return other != null && _dataValue == other.ToDouble();
        }
        public bool Equals(IntegerValue other)
        {
            return other != null && _dataValue == (double)other.Value;
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
                return Equals(realValue);

            return other == this;
        }
        public bool Equals(RealValue other)
        {
            return other != null && _dataValue.Equals(other._dataValue);
        }
        public int CompareTo(RealValue other)
        {
            return _dataValue.CompareTo(other._dataValue);
        }
        public override int GetHashCode()
        {
            return _dataValue.GetHashCode();
        }

        #region IAlgebraicRingWithUnity Members
        public bool IsMultiplicativeIdentity
        {
            get { return _dataValue == 1d; }
        }
        ValueStructure IAlgebraicRingWithUnity.MultiplicativeIdentity
        {
            get { return MultiplicativeIdentity; }
        }
        #endregion
        #region IAlgebraicMonoid Members
        public bool IsAdditiveIdentity
        {
            get { return _dataValue == 0d; }
        }
        ValueStructure IAlgebraicMonoid.AdditiveIdentity
        {
            get { return AdditiveIdentity; }
        }
        #endregion

        #region Serialization
        protected override void InnerSerialize(System.Xml.XmlWriter writer)
        {
            writer.WriteString(_dataValue.ToString(Context.NumberFormat));
        }
        private static RealValue InnerDeserialize(Context context, System.Xml.XmlReader reader)
        {
            return new RealValue(double.Parse(reader.ReadString(), Context.NumberFormat));
        }
        #endregion
    }
}
