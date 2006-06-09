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

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Templates;

namespace MathNet.Symbolics.StdPackage.Structures
{
    //public sealed class MatrixValue<T> : ValueStructure, IEquatable<MatrixValue<T>>, IComparable<MatrixValue<T>>
    //    where T : ValueStructure, IEquatable<T>, IComparable<T>
    //{
    //    private readonly T[,] dataValue;

    //    #region Conversion
    //    public static explicit operator ComplexValue(IntegerValue value)
    //    {
    //        return new ComplexValue(value);
    //    }
    //    public static explicit operator ComplexValue(RationalValue value)
    //    {
    //        return new ComplexValue(value);
    //    }
    //    public static explicit operator ComplexValue(RealValue value)
    //    {
    //        return new ComplexValue(value);
    //    }
    //    public static ComplexValue ConvertFrom(ValueStructure value)
    //    {
    //        ComplexValue cov = value as ComplexValue;
    //        if(cov != null)
    //            return cov;

    //        RealValue rev = value as RealValue;
    //        if(rev != null)
    //            return new ComplexValue(rev);

    //        RationalValue rav = value as RationalValue;
    //        if(rav != null)
    //            return new ComplexValue(rav);

    //        IntegerValue inv = value as IntegerValue;
    //        if(inv != null)
    //            return new ComplexValue(inv);

    //        throw new MathNet.Symbolics.Backend.Exceptions.StructureNotSupportedException(value);
    //    }
    //    public static bool IsConvertibleFrom(ValueStructure value)
    //    {
    //        return value != null && (value is ComplexValue || value is RealValue || value is RationalValue || value is IntegerValue);
    //    }
    //    #endregion

    //    public MatrixValue()
    //    {
    //    }
    //    public MatrixValue(T[,] value)
    //    {
    //        dataValue = value;
    //    }
    //    public MatrixValue(IntegerValue realValue)
    //    {
    //        dataValue.Real = realValue.ToDouble();
    //    }
    //    public MatrixValue(RationalValue realValue)
    //    {
    //        dataValue.Real = realValue.ToDouble();
    //    }
    //    public MatrixValue(RealValue realValue)
    //    {
    //        dataValue.Real = realValue.Value;
    //    }
    //    public MatrixValue(RealValue realValue, RealValue imagValue)
    //    {
    //        dataValue = Complex.FromRealImaginary(realValue.Value, imagValue.Value);
    //    }

    //    public Complex Value
    //    {
    //        get { return dataValue; }
    //    }
    //    public double RealValue
    //    {
    //        get { return dataValue.Real; }
    //    }
    //    public double ImaginaryValue
    //    {
    //        get { return dataValue.Imag; }
    //    }
    //    public double Modulus
    //    {
    //        get { return dataValue.Modulus; }
    //    }
    //    public double ModulusSquared
    //    {
    //        get { return dataValue.ModulusSquared; }
    //    }
    //    public double Argument
    //    {
    //        get { return dataValue.Argument; }
    //    }
    //    public bool IsZero
    //    {
    //        get { return dataValue.IsZero; }
    //    }
    //    public bool IsReal
    //    {
    //        get { return dataValue.IsReal; }
    //    }

    //    #region Direct Function Implementations
    //    #region Arithmetic Function
    //    public static ComplexValue Add(ComplexValue summand1, ComplexValue summand2) { return summand1.Add(summand2); }
    //    public ComplexValue Add(ComplexValue summand)
    //    {
    //        return new ComplexValue(dataValue + summand.dataValue);
    //    }
    //    public static ComplexValue Subtract(ComplexValue minuend, ComplexValue subtrahend) { return minuend.Subtract(subtrahend); }
    //    public ComplexValue Subtract(ComplexValue subtrahend)
    //    {
    //        return new ComplexValue(dataValue - subtrahend.dataValue);
    //    }
    //    public static ComplexValue Negate(ComplexValue subtrahend) { return subtrahend.Negate(); }
    //    public ComplexValue Negate()
    //    {
    //        return new ComplexValue(-dataValue);
    //    }
    //    public static ComplexValue Conjugate(ComplexValue complex) { return complex.Conjugate(); }
    //    public ComplexValue Conjugate()
    //    {
    //        return new ComplexValue(dataValue.Conjugate);
    //    }
    //    public static ComplexValue Multiply(ComplexValue multiplicand, ComplexValue multiplier) { return multiplicand.Multiply(multiplier); }
    //    public ComplexValue Multiply(ComplexValue multiplier)
    //    {
    //        return new ComplexValue(dataValue * multiplier.dataValue);
    //    }
    //    public static ComplexValue Divide(ComplexValue dividend, ComplexValue divisor) { return dividend.Divide(divisor); }
    //    public ComplexValue Divide(ComplexValue divisor)
    //    {
    //        //if(op.IsZero)
    //        //    return ComplexInfinitySymbol.Symbol;
    //        return new ComplexValue(dataValue / divisor.dataValue);
    //    }
    //    public static ComplexValue Invert(ComplexValue divisor) { return divisor.Invert(); }
    //    public ComplexValue Invert()
    //    {
    //        //if(IsZero)
    //        //    return ComplexInfinitySymbol.Symbol;
    //        double mod2 = dataValue.ModulusSquared;
    //        return new ComplexValue(dataValue.Real / mod2, -dataValue.Imag / mod2);
    //    }
    //    public static RealValue Absolute(ComplexValue complex) { return complex.Absolute(); }
    //    public RealValue Absolute()
    //    {
    //        return new RealValue(dataValue.Modulus);
    //    }
    //    #endregion
    //    #region Exponential Functions
    //    public static ComplexValue Exponential(ComplexValue exponent) { return exponent.Exponential(); }
    //    public ComplexValue Exponential()
    //    {
    //        return new ComplexValue(dataValue.Exp());
    //    }
    //    public static ComplexValue NaturalLogarithm(ComplexValue antilogarithm) { return antilogarithm.NaturalLogarithm(); }
    //    public ComplexValue NaturalLogarithm()
    //    {
    //        return new ComplexValue(dataValue.Ln());
    //    }
    //    public static ComplexValue Power(ComplexValue radix, ComplexValue exponent) { return radix.Power(exponent); }
    //    public ComplexValue Power(ComplexValue exponent)
    //    {
    //        return new ComplexValue(dataValue.Pow(exponent.Value));
    //    }
    //    public static ComplexValue Root(ComplexValue radicand, ComplexValue rootexponent) { return radicand.Root(rootexponent); }
    //    public ComplexValue Root(ComplexValue rootexponent)
    //    {
    //        return new ComplexValue(dataValue.Root(rootexponent.Value));
    //    }
    //    public static ComplexValue Square(ComplexValue radix) { return radix.Square(); }
    //    public ComplexValue Square()
    //    {
    //        return new ComplexValue(dataValue.Square());
    //    }
    //    public static ComplexValue SquareRoot(ComplexValue radicand) { return radicand.SquareRoot(); }
    //    public ComplexValue SquareRoot()
    //    {
    //        return new ComplexValue(dataValue.Sqrt());
    //    }
    //    #endregion
    //    #region Trigonometric Functions
    //    /// <summary>Trigonometric Sine (Sinus) of an angle in radians</summary>
    //    public ComplexValue Sine()
    //    {
    //        return new ComplexValue(dataValue.Sin());
    //    }
    //    /// <summary>Trigonometric Cosine (Cosinus) of an angle in radians</summary>
    //    public ComplexValue Cosine()
    //    {
    //        return new ComplexValue(dataValue.Cos());
    //    }
    //    /// <summary>Trigonometric Tangent (Tangens) of an angle in radians</summary>
    //    public ComplexValue Tangent()
    //    {
    //        return new ComplexValue(dataValue.Tan());
    //    }
    //    /// <summary>Trigonometric Cotangent (Cotangens) of an angle in radians</summary>
    //    public ComplexValue Cotangent()
    //    {
    //        return new ComplexValue(dataValue.Cot());
    //    }
    //    /// <summary>Trigonometric Secant (Sekans) of an angle in radians</summary>
    //    public ComplexValue Secant()
    //    {
    //        return new ComplexValue(dataValue.Sec());
    //    }
    //    /// <summary>Trigonometric Cosecant (Cosekans) of an angle in radians</summary>
    //    public ComplexValue Cosecant()
    //    {
    //        return new ComplexValue(dataValue.Csc());
    //    }
    //    #endregion
    //    #region Trigonometric Inverse Functions
    //    /// <summary>Trigonometric Arcus Sine (Arkussinus) in radians</summary>
    //    public ComplexValue InverseSine()
    //    {
    //        return new ComplexValue(dataValue.Asin());
    //    }
    //    /// <summary>Trigonometric Arcus Cosine (Arkuscosinus) in radians</summary>
    //    public ComplexValue InverseCosine()
    //    {
    //        return new ComplexValue(dataValue.Acos());
    //    }
    //    /// <summary>Trigonometric Arcus Tangent (Arkustangens) in radians</summary>
    //    public ComplexValue InverseTangent()
    //    {
    //        return new ComplexValue(dataValue.Atan());
    //    }
    //    /// <summary>Trigonometric Arcus Cotangent (Arkuscotangens) in radians</summary>
    //    public ComplexValue InverseCotangent()
    //    {
    //        return new ComplexValue(dataValue.Acot());
    //    }
    //    /// <summary>Trigonometric Arcus Secant (Arkussekans) in radians</summary>
    //    public ComplexValue InverseSecant()
    //    {
    //        return new ComplexValue(dataValue.Asec());
    //    }
    //    /// <summary>Trigonometric Arcus Cosecant (Arkuscosekans) in radians</summary>
    //    public ComplexValue InverseCosecant()
    //    {
    //        return new ComplexValue(dataValue.Acsc());
    //    }
    //    #endregion
    //    #region Hyperbolic Functions
    //    /// <summary>Trigonometric Hyperbolic Sine (Sinus hyperbolicus)</summary>
    //    public ComplexValue HyperbolicSine()
    //    {
    //        return new ComplexValue(dataValue.Sinh());
    //    }
    //    /// <summary>Trigonometric Hyperbolic Cosine (Cosinus hyperbolicus)</summary>
    //    public ComplexValue HyperbolicCosine()
    //    {
    //        return new ComplexValue(dataValue.Cosh());
    //    }
    //    /// <summary>Trigonometric Hyperbolic Tangent (Tangens hyperbolicus)</summary>
    //    public ComplexValue HyperbolicTangent()
    //    {
    //        return new ComplexValue(dataValue.Tanh());
    //    }
    //    /// <summary>Trigonometric Hyperbolic Cotangent (Cotangens hyperbolicus)</summary>
    //    public ComplexValue HyperbolicCotangent()
    //    {
    //        return new ComplexValue(dataValue.Coth());
    //    }
    //    /// <summary>Trigonometric Hyperbolic Secant (Sekans hyperbolicus)</summary>
    //    public ComplexValue HyperbolicSecant()
    //    {
    //        return new ComplexValue(dataValue.Sech());
    //    }
    //    /// <summary>Trigonometric Hyperbolic Cosecant (Cosekans hyperbolicus)</summary>
    //    public ComplexValue HyperbolicCosecant()
    //    {
    //        return new ComplexValue(dataValue.Csch());
    //    }
    //    #endregion
    //    #region Hyperbolic Area Functions
    //    /// <summary>Trigonometric Hyperbolic Area Sine (Areasinus hyperbolicus)</summary>
    //    public ComplexValue InverseHyperbolicSine()
    //    {
    //        return new ComplexValue(dataValue.Asinh());
    //    }
    //    /// <summary>Trigonometric Hyperbolic Area Cosine (Areacosinus hyperbolicus)</summary>
    //    public ComplexValue InverseHyperbolicCosine()
    //    {
    //        return new ComplexValue(dataValue.Acosh());
    //    }
    //    /// <summary>Trigonometric Hyperbolic Area Tangent (Areatangens hyperbolicus)</summary>
    //    public ComplexValue InverseHyperbolicTangent()
    //    {
    //        return new ComplexValue(dataValue.Atanh());
    //    }
    //    /// <summary>Trigonometric Hyperbolic Area Cotangent (Areacotangens hyperbolicus)</summary>
    //    public ComplexValue InverseHyperbolicCotangent()
    //    {
    //        return new ComplexValue(dataValue.Acoth());
    //    }
    //    /// <summary>Trigonometric Hyperbolic Area Secant (Areasekans hyperbolicus)</summary>
    //    public ComplexValue InverseHyperbolicSecant()
    //    {
    //        return new ComplexValue(dataValue.Asech());
    //    }
    //    /// <summary>Trigonometric Hyperbolic Area Cosecant (Areacosekans hyperbolicus)</summary>
    //    public ComplexValue InverseHyperbolicCosecant()
    //    {
    //        return new ComplexValue(dataValue.Acsch());
    //    }
    //    #endregion
    //    #endregion

    //    #region Constants
    //    public static Signal Constant(Context context, Complex value)
    //    {
    //        return Constant(context, value, value.ToString());
    //    }
    //    public static Signal Constant(Context context, Complex value, string name)
    //    {
    //        Signal s = new Signal(context, new ComplexValue(value));
    //        s.Label = name + "_Constant";
    //        s.Properties.AddProperty(Properties.ConstantSignalProperty.Instance);
    //        return s;
    //    }

    //    public static ComplexValue I
    //    {
    //        get { return new ComplexValue(Complex.I); } //TODO: Think about Singleton like design...
    //    }

    //    public static ComplexValue Zero
    //    {
    //        get { return new ComplexValue(Complex.Zero); } //TODO: Think about Singleton like design...
    //    }

    //    public static ComplexValue One
    //    {
    //        get { return new ComplexValue(Complex.One); } //TODO: Think about Singleton like design...
    //    }

    //    public static ComplexValue AdditiveIdentity
    //    {
    //        get { return new ComplexValue(Complex.Zero); } //TODO: Think about Singleton like design...
    //    }

    //    public static ComplexValue MultiplicativeIdentity
    //    {
    //        get { return new ComplexValue(Complex.One); } //TODO: Think about Singleton like design...
    //    }
    //    #endregion

    //    public override string ToString()
    //    {
    //        return dataValue.ToString();
    //    }

    //    public bool Equals(RealValue other)
    //    {
    //        return dataValue.IsReal && dataValue.Real == other.Value;
    //    }
    //    public bool Equals(RationalValue other)
    //    {
    //        return dataValue.IsReal && dataValue.Real == other.ToDouble();
    //    }
    //    public bool Equals(IntegerValue other)
    //    {
    //        return dataValue.IsReal && dataValue.Real == other.ToDouble();
    //    }
    //    public override bool Equals(ValueStructure other)
    //    {
    //        IntegerValue integerValue = other as IntegerValue;
    //        if(integerValue != null)
    //            return Equals(integerValue);

    //        RationalValue rationalValue = other as RationalValue;
    //        if(rationalValue != null)
    //            return Equals(rationalValue);

    //        RealValue realValue = other as RealValue;
    //        if(realValue != null)
    //            return Equals(realValue);

    //        ComplexValue complexValue = other as ComplexValue;
    //        if(complexValue != null)
    //            return Equals(complexValue);

    //        return other == this;
    //    }
    //    public bool Equals(ComplexValue other)
    //    {
    //        return dataValue.Equals(other.dataValue);
    //    }
    //    public int CompareTo(ComplexValue other)
    //    {
    //        return dataValue.CompareTo(other.dataValue);
    //    }
    //    public override int GetHashCode()
    //    {
    //        return dataValue.GetHashCode();
    //    }

    //    #region IAlgebraicRingWithUnity Members
    //    public bool IsMultiplicativeIdentity
    //    {
    //        get { return dataValue.IsOne; }
    //    }
    //    ValueStructure IAlgebraicRingWithUnity.MultiplicativeIdentity
    //    {
    //        get { return MultiplicativeIdentity; }
    //    }
    //    #endregion
    //    #region IAlgebraicMonoid Members
    //    public bool IsAdditiveIdentity
    //    {
    //        get { return dataValue.IsZero; }
    //    }
    //    ValueStructure IAlgebraicMonoid.AdditiveIdentity
    //    {
    //        get { return AdditiveIdentity; }
    //    }
    //    #endregion
    //}
}
