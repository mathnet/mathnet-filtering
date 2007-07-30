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

namespace MathNet.Symbolics.Packages.Standard.Structures
{
    /// <summary>signed real numbers</summary>
    public class RealValue : ValueStructureBase, IEquatable<RealValue>, IComparable<RealValue>, IAlgebraicField<RealValue>
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("Real", "Std");
        private readonly double _dataValue; // = 0;

        #region Conversion & Conversion Routing
        public static void Register(ILibrary library)
        {
            library.AddCustomDataType<RealValue>();
            library.AddArbitraryType(typeof(double));
            //library.AddCustomDataType(new CustomDataRef(typeof(RealValue), ValueConverter<RealValue>.Router));
            ValueConverter<RealValue>.AddConverterFrom<IntegerValue>(true, ConvertFrom);
            ValueConverter<RealValue>.AddConverterFrom<RationalValue>(true, ConvertFrom);
            ValueConverter<RealValue>.AddConverterTo(ValueConverter<IntegerValue>.Router, false,
                delegate(object value) { return new IntegerValue((long)Math.Round(((RealValue)value).Value, 0)); });
            ValueConverter<IntegerValue>.AddConverterFrom(ValueConverter<double>.Router, true,
                delegate(object value) { return new RealValue((double)value); });
            ValueConverter<IntegerValue>.AddConverterTo(ValueConverter<double>.Router, true,
                delegate(object value) { return ((RealValue)value).Value; });
        }
        public static RealValue ConvertFrom(ICustomData value)
        {
            return (RealValue)ValueConverter<RealValue>.ConvertFrom(value);
        }
        public static RealValue ConvertFrom(IntegerValue value) { return new RealValue(value); }
        public static explicit operator RealValue(IntegerValue value) { return new RealValue(value); }
        public static RealValue ConvertFrom(RationalValue value) { return new RealValue(value); }
        public static explicit operator RealValue(RationalValue value) { return new RealValue(value); }
        public static bool CanConvertLosslessFrom(ICustomData value)
        {
            return ValueConverter<RealValue>.Router.CanConvertLosslessFrom(value);
        }
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
            if(value == null)
                throw new ArgumentNullException("value");

            _dataValue = (double)value.Value;
        }
        public RealValue(RationalValue value)
        {
            if(value == null)
                throw new ArgumentNullException("value");

            _dataValue = (double)value.NumeratorValue / (double)value.DenominatorValue;
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
        internal class SubtractProcess : GenericFunctionProcess
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
        internal class MultiplyProcess : GenericFunctionProcess
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
        internal class DivideProcess : GenericFunctionProcess
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

        public static MathIdentifier TypeIdentifier
        {
            get { return _customTypeId; }
        }
        public override MathIdentifier TypeId
        {
            get { return _customTypeId; }
        }

        #region Direct Function Implementations
        #region Arithmetic Function
        public RealValue Add(RealValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RealValue(_dataValue + op._dataValue);
        }
        public RealValue Subtract(RealValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RealValue(_dataValue - op._dataValue);
        }
        public RealValue Negate()
        {
            return new RealValue(-_dataValue);
        }
        public RealValue Multiply(RealValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RealValue(_dataValue * op._dataValue);
        }
        public RealValue Divide(RealValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RealValue(_dataValue / op._dataValue);
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
            if(op == null)
                throw new ArgumentNullException("op");

            return new RealValue(Math.Pow(_dataValue, op._dataValue));
        }
        public RealValue Root(RealValue op)
        {
            if(op == null)
                throw new ArgumentNullException("op");

            return new RealValue(Math.Pow(_dataValue, 1 / op._dataValue));
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
        public static Signal ParseConstant(string value)
        {
            if(value == null)
                throw new ArgumentNullException("value");

            return Constant(double.Parse(value, Config.InternalNumberFormat), value.Trim());
        }
        public static Signal Constant(double value)
        {
            return Constant(value, value.ToString(Config.InternalNumberFormat));
        }
        public static Signal Constant(double value, string name)
        {
            Signal s = Binder.CreateSignal(new RealValue(value));
            s.Label = name + "_Constant";
            s.EnableFlag(StdAspect.ConstantFlag);
            //s.AddConstraint(Properties.ConstantSignalProperty.Instance);
            return s;
        }

        public static Signal ConstantPI
        {
            get
            {
                MathIdentifier id = new MathIdentifier("RealValueConstantPI", "Std");
                Signal ret;
                if(Service<ISignalCache>.Instance.TryGetValue(id, out ret))
                    return ret;
                else
                {
                    ret = Constant(Math.PI, "PI");
                    Service<ISignalCache>.Instance.Add(id, ret);
                    return ret;
                }
            }
        }

        public static Signal ConstantE
        {
            get
            {
                MathIdentifier id = new MathIdentifier("RealValueConstantE", "Std");
                Signal ret;
                if(Service<ISignalCache>.Instance.TryGetValue(id, out ret))
                    return ret;
                else
                {
                    ret = Constant(Math.E, "E");
                    Service<ISignalCache>.Instance.Add(id, ret);
                    return ret;
                }
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

        #region IAlgebraicMultiplicativeIdentityElement Members
        public bool IsMultiplicativeIdentity
        {
            get { return _dataValue == 1d; }
        }
        IValueStructure IAlgebraicMultiplicativeIdentityElement.MultiplicativeIdentity
        {
            get { return MultiplicativeIdentity; }
        }
        #endregion
        #region IAlgebraicAdditiveIdentityElement Members
        public bool IsAdditiveIdentity
        {
            get { return _dataValue == 0d; }
        }
        IValueStructure IAlgebraicAdditiveIdentityElement.AdditiveIdentity
        {
            get { return AdditiveIdentity; }
        }
        #endregion

        #region Serialization
        //protected override void InnerSerialize(System.Xml.XmlWriter writer)
        //{
        //    writer.WriteString(_dataValue.ToString(Config.InternalNumberFormat));
        //}
        //private static RealValue InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    return new RealValue(double.Parse(reader.ReadString(), Config.InternalNumberFormat));
        //}
        public override void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            writer.WriteString(_dataValue.ToString(Config.InternalNumberFormat));
        }
        private static RealValue Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return new RealValue(double.Parse(reader.ReadString(), Config.InternalNumberFormat));
        }
        #endregion
    }
}
