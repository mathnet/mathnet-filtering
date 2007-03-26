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

namespace MathNet.Symbolics.Packages.Standard.Structures
{
    /// <summary>complex numbers with signed real components</summary>
    public class ComplexValue : ValueStructureBase, IEquatable<ComplexValue>, IComparable<ComplexValue>, IAlgebraicField<ComplexValue>
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("Complex", "Std");
        private readonly Complex _dataValue;

        #region Conversion & Conversion Routing
        public static void Register(ILibrary library)
        {
            library.AddCustomDataType<ComplexValue>();
            //library.AddCustomDataType(new CustomDataRef(typeof(ComplexValue), ValueConverter<ComplexValue>.Router));
            ValueConverter<ComplexValue>.AddConverterFrom<IntegerValue>(true, ConvertFrom);
            ValueConverter<ComplexValue>.AddConverterFrom<RationalValue>(true, ConvertFrom);
            ValueConverter<ComplexValue>.AddConverterFrom<RealValue>(true, ConvertFrom);
            ValueConverter<ComplexValue>.AddConverterTo(ValueConverter<RealValue>.Router, false,
                delegate(ICustomData value) { return new RealValue(((ComplexValue)value).RealValue); });
        }
        public static ComplexValue ConvertFrom(ICustomData value)
        {
            return (ComplexValue)ValueConverter<ComplexValue>.ConvertFrom(value);
        }
        public static ComplexValue ConvertFrom(IntegerValue value) { return new ComplexValue(value); }
        public static explicit operator ComplexValue(IntegerValue value) { return new ComplexValue(value); }
        public static ComplexValue ConvertFrom(RationalValue value) { return new ComplexValue(value); }
        public static explicit operator ComplexValue(RationalValue value) { return new ComplexValue(value); }
        public static ComplexValue ConvertFrom(RealValue value) { return new ComplexValue(value); }
        public static explicit operator ComplexValue(RealValue value) { return new ComplexValue(value); }
        public static bool CanConvertLosslessFrom(ICustomData value)
        {
            return ValueConverter<ComplexValue>.Router.CanConvertLosslessFrom(value);
        }        
        #endregion

        #region Constructors
        public ComplexValue()
        {
        }
        public ComplexValue(Complex value)
        {
            _dataValue = value;
        }
        public ComplexValue(double realValue)
        {
            _dataValue.Real = realValue;
        }
        public ComplexValue(double realValue, double imagValue)
        {
            _dataValue = Complex.FromRealImaginary(realValue, imagValue);
        }
        public ComplexValue(IntegerValue realValue)
        {
            _dataValue.Real = realValue.ToDouble();
        }
        public ComplexValue(RationalValue realValue)
        {
            _dataValue.Real = realValue.ToDouble();
        }
        public ComplexValue(RealValue realValue)
        {
            _dataValue.Real = realValue.Value;
        }
        public ComplexValue(RealValue realValue, RealValue imagValue)
        {
            _dataValue = Complex.FromRealImaginary(realValue.Value, imagValue.Value);
        }
        #endregion

        public Complex Value
        {
            get { return _dataValue; }
        }
        public double RealValue
        {
            get { return _dataValue.Real; }
        }
        public double ImaginaryValue
        {
            get { return _dataValue.Imag; }
        }
        public double Modulus
        {
            get { return _dataValue.Modulus; }
        }
        public double ModulusSquared
        {
            get { return _dataValue.ModulusSquared; }
        }
        public double Argument
        {
            get { return _dataValue.Argument; }
        }
        public bool IsZero
        {
            get { return _dataValue.IsZero; }
        }
        public bool IsReal
        {
            get { return _dataValue.IsReal; }
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
        public static ComplexValue Add(ComplexValue summand1, ComplexValue summand2) { return summand1.Add(summand2); }
        public ComplexValue Add(ComplexValue summand)
        {
            return new ComplexValue(_dataValue + summand._dataValue);
        }
        public static ComplexValue Subtract(ComplexValue minuend, ComplexValue subtrahend) { return minuend.Subtract(subtrahend); }
        public ComplexValue Subtract(ComplexValue subtrahend)
        {
            return new ComplexValue(_dataValue - subtrahend._dataValue);
        }
        public static ComplexValue Negate(ComplexValue subtrahend) { return subtrahend.Negate(); }
        public ComplexValue Negate()
        {
            return new ComplexValue(-_dataValue);
        }
        public static ComplexValue Conjugate(ComplexValue complex) { return complex.Conjugate(); }
        public ComplexValue Conjugate()
        {
            return new ComplexValue(_dataValue.Conjugate);
        }
        public static ComplexValue Multiply(ComplexValue multiplicand, ComplexValue multiplier) { return multiplicand.Multiply(multiplier); }
        public ComplexValue Multiply(ComplexValue multiplier)
        {
            return new ComplexValue(_dataValue * multiplier._dataValue);
        }
        public static ComplexValue Divide(ComplexValue dividend, ComplexValue divisor) { return dividend.Divide(divisor); }
        public ComplexValue Divide(ComplexValue divisor)
        {
            //if(op.IsZero)
            //    return ComplexInfinitySymbol.Symbol;
            return new ComplexValue(_dataValue / divisor._dataValue);
        }
        public static ComplexValue Invert(ComplexValue divisor) { return divisor.Invert(); }
        public ComplexValue Invert()
        {
            //if(IsZero)
            //    return ComplexInfinitySymbol.Symbol;
            double mod2 = _dataValue.ModulusSquared;
            return new ComplexValue(_dataValue.Real / mod2, -_dataValue.Imag / mod2);
        }
        public static RealValue Absolute(ComplexValue complex) { return complex.Absolute(); }
        public RealValue Absolute()
        {
            return new RealValue(_dataValue.Modulus);
        }
        public ComplexValue Scale(ComplexValue scalar)
        {
            return Multiply(scalar);
        }
        #endregion
        #region Exponential Functions
        public static ComplexValue Exponential(ComplexValue exponent) { return exponent.Exponential(); }
        public ComplexValue Exponential()
        {
            return new ComplexValue(_dataValue.Exponential());
        }
        public static ComplexValue NaturalLogarithm(ComplexValue antilogarithm) { return antilogarithm.NaturalLogarithm(); }
        public ComplexValue NaturalLogarithm()
        {
            return new ComplexValue(_dataValue.NaturalLogarithm());
        }
        public static ComplexValue Power(ComplexValue radix, ComplexValue exponent) { return radix.Power(exponent); }
        public ComplexValue Power(ComplexValue exponent)
        {
            return new ComplexValue(_dataValue.Power(exponent.Value));
        }
        public static ComplexValue Root(ComplexValue radicand, ComplexValue rootexponent) { return radicand.Root(rootexponent); }
        public ComplexValue Root(ComplexValue rootexponent)
        {
            return new ComplexValue(_dataValue.Root(rootexponent.Value));
        }
        public static ComplexValue Square(ComplexValue radix) { return radix.Square(); }
        public ComplexValue Square()
        {
            return new ComplexValue(_dataValue.Square());
        }
        public static ComplexValue SquareRoot(ComplexValue radicand) { return radicand.SquareRoot(); }
        public ComplexValue SquareRoot()
        {
            return new ComplexValue(_dataValue.SquareRoot());
        }
        #endregion
        #region Trigonometric Functions
        /// <summary>Trigonometric Sine (Sinus) of an angle in radians</summary>
        public ComplexValue Sine()
        {
            return new ComplexValue(_dataValue.Sine());
        }
        /// <summary>Trigonometric Cosine (Cosinus) of an angle in radians</summary>
        public ComplexValue Cosine()
        {
            return new ComplexValue(_dataValue.Cosine());
        }
        /// <summary>Trigonometric Tangent (Tangens) of an angle in radians</summary>
        public ComplexValue Tangent()
        {
            return new ComplexValue(_dataValue.Tangent());
        }
        /// <summary>Trigonometric Cotangent (Cotangens) of an angle in radians</summary>
        public ComplexValue Cotangent()
        {
            return new ComplexValue(_dataValue.Cotangent());
        }
        /// <summary>Trigonometric Secant (Sekans) of an angle in radians</summary>
        public ComplexValue Secant()
        {
            return new ComplexValue(_dataValue.Secant());
        }
        /// <summary>Trigonometric Cosecant (Cosekans) of an angle in radians</summary>
        public ComplexValue Cosecant()
        {
            return new ComplexValue(_dataValue.Cosecant());
        }
        #endregion
        #region Trigonometric Inverse Functions
        /// <summary>Trigonometric Arcus Sine (Arkussinus) in radians</summary>
        public ComplexValue InverseSine()
        {
            return new ComplexValue(_dataValue.InverseSine());
        }
        /// <summary>Trigonometric Arcus Cosine (Arkuscosinus) in radians</summary>
        public ComplexValue InverseCosine()
        {
            return new ComplexValue(_dataValue.InverseCosine());
        }
        /// <summary>Trigonometric Arcus Tangent (Arkustangens) in radians</summary>
        public ComplexValue InverseTangent()
        {
            return new ComplexValue(_dataValue.InverseTangent());
        }
        /// <summary>Trigonometric Arcus Cotangent (Arkuscotangens) in radians</summary>
        public ComplexValue InverseCotangent()
        {
            return new ComplexValue(_dataValue.InverseCotangent());
        }
        /// <summary>Trigonometric Arcus Secant (Arkussekans) in radians</summary>
        public ComplexValue InverseSecant()
        {
            return new ComplexValue(_dataValue.InverseSecant());
        }
        /// <summary>Trigonometric Arcus Cosecant (Arkuscosekans) in radians</summary>
        public ComplexValue InverseCosecant()
        {
            return new ComplexValue(_dataValue.InverseCosecant());
        }
        #endregion
        #region Hyperbolic Functions
        /// <summary>Trigonometric Hyperbolic Sine (Sinus hyperbolicus)</summary>
        public ComplexValue HyperbolicSine()
        {
            return new ComplexValue(_dataValue.HyperbolicSine());
        }
        /// <summary>Trigonometric Hyperbolic Cosine (Cosinus hyperbolicus)</summary>
        public ComplexValue HyperbolicCosine()
        {
            return new ComplexValue(_dataValue.HyperbolicCosine());
        }
        /// <summary>Trigonometric Hyperbolic Tangent (Tangens hyperbolicus)</summary>
        public ComplexValue HyperbolicTangent()
        {
            return new ComplexValue(_dataValue.HyperbolicTangent());
        }
        /// <summary>Trigonometric Hyperbolic Cotangent (Cotangens hyperbolicus)</summary>
        public ComplexValue HyperbolicCotangent()
        {
            return new ComplexValue(_dataValue.HyperbolicCotangent());
        }
        /// <summary>Trigonometric Hyperbolic Secant (Sekans hyperbolicus)</summary>
        public ComplexValue HyperbolicSecant()
        {
            return new ComplexValue(_dataValue.HyperbolicSecant());
        }
        /// <summary>Trigonometric Hyperbolic Cosecant (Cosekans hyperbolicus)</summary>
        public ComplexValue HyperbolicCosecant()
        {
            return new ComplexValue(_dataValue.HyperbolicCosecant());
        }
        #endregion
        #region Hyperbolic Area Functions
        /// <summary>Trigonometric Hyperbolic Area Sine (Areasinus hyperbolicus)</summary>
        public ComplexValue InverseHyperbolicSine()
        {
            return new ComplexValue(_dataValue.InverseHyperbolicSine());
        }
        /// <summary>Trigonometric Hyperbolic Area Cosine (Areacosinus hyperbolicus)</summary>
        public ComplexValue InverseHyperbolicCosine()
        {
            return new ComplexValue(_dataValue.InverseHyperbolicCosine());
        }
        /// <summary>Trigonometric Hyperbolic Area Tangent (Areatangens hyperbolicus)</summary>
        public ComplexValue InverseHyperbolicTangent()
        {
            return new ComplexValue(_dataValue.InverseHyperbolicTangent());
        }
        /// <summary>Trigonometric Hyperbolic Area Cotangent (Areacotangens hyperbolicus)</summary>
        public ComplexValue InverseHyperbolicCotangent()
        {
            return new ComplexValue(_dataValue.InverseHyperbolicCotangent());
        }
        /// <summary>Trigonometric Hyperbolic Area Secant (Areasekans hyperbolicus)</summary>
        public ComplexValue InverseHyperbolicSecant()
        {
            return new ComplexValue(_dataValue.InverseHyperbolicSecant());
        }
        /// <summary>Trigonometric Hyperbolic Area Cosecant (Areacosekans hyperbolicus)</summary>
        public ComplexValue InverseHyperbolicCosecant()
        {
            return new ComplexValue(_dataValue.InverseHyperbolicCosecant());
        }
        #endregion
        #endregion

        #region Constants
        public static Signal Constant(Complex value)
        {
            return Constant(value, value.ToString());
        }
        public static Signal Constant(Complex value, string name)
        {
            Signal s = Binder.CreateSignal(new ComplexValue(value));
            s.Label = name + "_Constant";
            s.AddConstraint(Properties.ConstantSignalProperty.Instance);
            return s;
        }

        public static ComplexValue I
        {
            get { return new ComplexValue(Complex.I); } //TODO: Think about Singleton like design...
        }

        public static ComplexValue Zero
        {
            get { return new ComplexValue(Complex.Zero); } //TODO: Think about Singleton like design...
        }

        public static ComplexValue One
        {
            get { return new ComplexValue(Complex.One); } //TODO: Think about Singleton like design...
        }

        public static ComplexValue AdditiveIdentity
        {
            get { return new ComplexValue(Complex.Zero); } //TODO: Think about Singleton like design...
        }

        public static ComplexValue MultiplicativeIdentity
        {
            get { return new ComplexValue(Complex.One); } //TODO: Think about Singleton like design...
        }
        #endregion

        public override string ToString()
        {
            return base.ToString() + "(" + _dataValue.ToString() + ")";
        }

        public bool Equals(RealValue other)
        {
            return other != null && _dataValue.IsReal && _dataValue.Real == other.Value;
        }
        public bool Equals(RationalValue other)
        {
            return other != null && _dataValue.IsReal && _dataValue.Real == other.ToDouble();
        }
        public bool Equals(IntegerValue other)
        {
            return other != null && _dataValue.IsReal && _dataValue.Real == other.ToDouble();
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

            ComplexValue complexValue = other as ComplexValue;
            if(complexValue != null)
                return Equals(complexValue);

            return other == this;
        }
        public bool Equals(ComplexValue other)
        {
            return other != null && _dataValue.Equals(other._dataValue);
        }
        public int CompareTo(ComplexValue other)
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
            get { return _dataValue.IsOne; }
        }
        IValueStructure IAlgebraicRingWithUnity.MultiplicativeIdentity
        {
            get { return MultiplicativeIdentity; }
        }
        #endregion
        #region IAlgebraicMonoid Members
        public bool IsAdditiveIdentity
        {
            get { return _dataValue.IsZero; }
        }
        IValueStructure IAlgebraicMonoid.AdditiveIdentity
        {
            get { return AdditiveIdentity; }
        }
        #endregion

        #region Serialization
        //protected override void InnerSerialize(System.Xml.XmlWriter writer)
        //{
        //    writer.WriteElementString("Real", _dataValue.Real.ToString(Config.InternalNumberFormat));
        //    writer.WriteElementString("Imag", _dataValue.Imag.ToString(Config.InternalNumberFormat));
        //}
        //private static ComplexValue InnerDeserialize(IContext context, XmlReader reader)
        //{
        //    return new ComplexValue(double.Parse(reader.ReadElementString("Real"), Config.InternalNumberFormat),
        //        double.Parse(reader.ReadElementString("Imag"), Config.InternalNumberFormat));
        //}
        public override void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
            writer.WriteElementString("Real", _dataValue.Real.ToString(Config.InternalNumberFormat));
            writer.WriteElementString("Imag", _dataValue.Imag.ToString(Config.InternalNumberFormat));
        }
        private static ComplexValue Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return new ComplexValue(double.Parse(reader.ReadElementString("Real"), Config.InternalNumberFormat),
                double.Parse(reader.ReadElementString("Imag"), Config.InternalNumberFormat));
        }
        #endregion
    }
}
