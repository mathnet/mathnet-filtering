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

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Packages.Standard.Trigonometry;
using MathNet.Numerics.LinearAlgebra;

using MathNet.Symbolics.Packages.ObjectModel;

namespace MathNet.Symbolics.Packages.Standard.Structures
{
    ///// <summary>signed real numbers</summary>
    //public class RealMatrixValue : AbstractValueStructure, IEquatable<RealMatrixValue> //, IComparable<RealMatrixValue>
    //{
    //    private static readonly MathIdentifier _customTypeId = new MathIdentifier("RealMatrix", "Std");
    //    private readonly Matrix _dataValue;

    //    #region Conversion
    //    public static explicit operator RealMatrixValue(RealValue value)
    //    {
    //        return new RealMatrixValue(value.Value);
    //    }
    //    public static explicit operator RealMatrixValue(IntegerValue value)
    //    {
    //        return new RealMatrixValue(value.ToDouble());
    //    }
    //    public static explicit operator RealMatrixValue(RationalValue value)
    //    {
    //        return new RealMatrixValue(value.ToDouble());
    //    }
    //    public static RealMatrixValue ConvertFrom(ValueStructure value)
    //    {
    //        RealMatrixValue remv = value as RealMatrixValue;
    //        if(remv != null)
    //            return remv;

    //        RealValue rev = value as RealValue;
    //        if(rev != null)
    //            return new RealMatrixValue(rev.Value);

    //        RationalValue rav = value as RationalValue;
    //        if(rav != null)
    //            return new RealMatrixValue(rav.ToDouble());

    //        IntegerValue inv = value as IntegerValue;
    //        if(inv != null)
    //            return new RealMatrixValue(inv.ToDouble());

    //        throw new MathNet.Symbolics.Backend.Exceptions.StructureNotSupportedException(value);
    //    }
    //    public static bool IsConvertibleFrom(ValueStructure value)
    //    {
    //        return value != null && (value is RealMatrixValue || value is RealValue || value is RationalValue || value is IntegerValue);
    //    }
    //    #endregion

    //    public RealMatrixValue()
    //    {
    //        _dataValue = new Matrix(0, 0);
    //    }
    //    public RealMatrixValue(Matrix value)
    //    {
    //        _dataValue = value;
    //    }
    //    public RealMatrixValue(double[,] value)
    //    {
    //        _dataValue = new Matrix(value);
    //    }
    //    public RealMatrixValue(double[] value, int numberOfRows)
    //    {
    //        _dataValue = new Matrix(value, numberOfRows);
    //    }
    //    public RealMatrixValue(int numberOfRows, int numberOfColumns, double value)
    //    {
    //        _dataValue = new Matrix(numberOfRows, numberOfColumns, value);
    //    }
    //    public RealMatrixValue(double value)
    //    {
    //        _dataValue = new Matrix(1, 1, value);
    //    }
    //    public RealMatrixValue(RealValue value)
    //    {
    //        _dataValue = new Matrix(1, 1, value.Value);
    //    }

    //    public Matrix Value
    //    {
    //        get { return _dataValue; }
    //    }

    //    public static MathIdentifier TypeIdentifier
    //    {
    //        get { return _customTypeId; }
    //    }
    //    public override MathIdentifier StructureId
    //    {
    //        get { return _customTypeId; }
    //    }

    //    #region Direct Function Implementations
    //    #region Arithmetic Function
    //    public RealMatrixValue Add(RealMatrixValue op)
    //    {
    //        return new RealMatrixValue(_dataValue + op._dataValue);
    //    }
    //    public RealMatrixValue Subtract(RealMatrixValue op)
    //    {
    //        return new RealMatrixValue(_dataValue - op._dataValue);
    //    }
    //    public RealMatrixValue Negate()
    //    {
    //        Matrix m = _dataValue.Clone();
    //        m.UnaryMinus();
    //        return new RealMatrixValue(m);
    //    }
    //    public RealMatrixValue Multiply(RealValue op)
    //    {
    //        return new RealMatrixValue(op.Value * _dataValue);
    //    }
    //    public RealMatrixValue Multiply(RealMatrixValue op)
    //    {
    //        return new RealMatrixValue(_dataValue * op._dataValue);
    //    }
    //    public RealMatrixValue Invert()
    //    {
    //        return new RealMatrixValue(_dataValue.Inverse());
    //    }
    //    public RealValue Absolute()
    //    {
    //        return new RealValue(_dataValue.Norm2());
    //    }
    //    #endregion
    //    #region Norm, Rank, Condition
    //    public RealValue Norm1()
    //    {
    //        return new RealValue(_dataValue.Norm1());
    //    }
    //    public RealValue Norm2()
    //    {
    //        return new RealValue(_dataValue.Norm2());
    //    }
    //    public RealValue NormF()
    //    {
    //        return new RealValue(_dataValue.NormF());
    //    }
    //    public RealValue NormInf()
    //    {
    //        return new RealValue(_dataValue.NormInf());
    //    }
    //    public RealValue Condition()
    //    {
    //        return new RealValue(_dataValue.Condition());
    //    }
    //    public IntegerValue Rank()
    //    {
    //        return new IntegerValue(_dataValue.Rank());
    //    }
    //    public RealValue Determinant()
    //    {
    //        return new RealValue(_dataValue.Determinant());
    //    }
    //    #endregion
    //    #region Exponential Functions
    //    //public RealValue Exponential()
    //    //{
    //    //    return new RealValue(Math.Exp(dataValue));
    //    //}
    //    //public RealValue NaturalLogarithm()
    //    //{
    //    //    return new RealValue(Math.Log(dataValue));
    //    //}
    //    //public RealValue Power(RealValue op)
    //    //{
    //    //    return new RealValue(Math.Pow(dataValue, op.dataValue));
    //    //}
    //    //public RealValue Root(RealValue op)
    //    //{
    //    //    return new RealValue(Math.Pow(dataValue, 1 / op.Value));
    //    //}
    //    //public RealValue Square()
    //    //{
    //    //    return new RealValue(dataValue * dataValue);
    //    //}
    //    //public RealValue SquareRoot()
    //    //{
    //    //    return new RealValue(Math.Sqrt(dataValue));
    //    //}
    //    #endregion
    //    #region Trigonometric Functions
    //    ///// <summary>Trigonometric Sine (Sinus) of an angle in radians</summary>
    //    //public RealValue Sine()
    //    //{
    //    //    return new RealValue(Trig.Sine(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Cosine (Cosinus) of an angle in radians</summary>
    //    //public RealValue Cosine()
    //    //{
    //    //    return new RealValue(Trig.Cosine(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Tangent (Tangens) of an angle in radians</summary>
    //    //public RealValue Tangent()
    //    //{
    //    //    return new RealValue(Trig.Tangent(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Cotangent (Cotangens) of an angle in radians</summary>
    //    //public RealValue Cotangent()
    //    //{
    //    //    return new RealValue(Trig.Cotangent(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Secant (Sekans) of an angle in radians</summary>
    //    //public RealValue Secant()
    //    //{
    //    //    return new RealValue(Trig.Secant(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Cosecant (Cosekans) of an angle in radians</summary>
    //    //public RealValue Cosecant()
    //    //{
    //    //    return new RealValue(Trig.Cosecant(dataValue));
    //    //}
    //    #endregion
    //    #region Trigonometric Inverse Functions
    //    ///// <summary>Trigonometric Arcus Sine (Arkussinus) in radians</summary>
    //    //public RealValue InverseSine()
    //    //{
    //    //    return new RealValue(Trig.InverseSine(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Arcus Cosine (Arkuscosinus) in radians</summary>
    //    //public RealValue InverseCosine()
    //    //{
    //    //    return new RealValue(Trig.InverseCosine(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Arcus Tangent (Arkustangens) in radians</summary>
    //    //public RealValue InverseTangent()
    //    //{
    //    //    return new RealValue(Trig.InverseTangent(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Arcus Cotangent (Arkuscotangens) in radians</summary>
    //    //public RealValue InverseCotangent()
    //    //{
    //    //    return new RealValue(Trig.InverseCotangent(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Arcus Secant (Arkussekans) in radians</summary>
    //    //public RealValue InverseSecant()
    //    //{
    //    //    return new RealValue(Trig.InverseSecant(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Arcus Cosecant (Arkuscosekans) in radians</summary>
    //    //public RealValue InverseCosecant()
    //    //{
    //    //    return new RealValue(Trig.InverseCosecant(dataValue));
    //    //}
    //    #endregion
    //    #region Hyperbolic Functions
    //    ///// <summary>Trigonometric Hyperbolic Sine (Sinus hyperbolicus)</summary>
    //    //public RealValue HyperbolicSine()
    //    //{
    //    //    return new RealValue(Trig.HyperbolicSine(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Hyperbolic Cosine (Cosinus hyperbolicus)</summary>
    //    //public RealValue HyperbolicCosine()
    //    //{
    //    //    return new RealValue(Trig.HyperbolicCosine(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Hyperbolic Tangent (Tangens hyperbolicus)</summary>
    //    //public RealValue HyperbolicTangent()
    //    //{
    //    //    return new RealValue(Trig.HyperbolicTangent(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Hyperbolic Cotangent (Cotangens hyperbolicus)</summary>
    //    //public RealValue HyperbolicCotangent()
    //    //{
    //    //    return new RealValue(Trig.HyperbolicCotangent(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Hyperbolic Secant (Sekans hyperbolicus)</summary>
    //    //public RealValue HyperbolicSecant()
    //    //{
    //    //    return new RealValue(Trig.HyperbolicSecant(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Hyperbolic Cosecant (Cosekans hyperbolicus)</summary>
    //    //public RealValue HyperbolicCosecant()
    //    //{
    //    //    return new RealValue(Trig.HyperbolicCosecant(dataValue));
    //    //}
    //    #endregion
    //    #region Hyperbolic Area Functions
    //    ///// <summary>Trigonometric Hyperbolic Area Sine (Areasinus hyperbolicus)</summary>
    //    //public RealValue InverseHyperbolicSine()
    //    //{
    //    //    return new RealValue(Trig.InverseHyperbolicSine(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Hyperbolic Area Cosine (Areacosinus hyperbolicus)</summary>
    //    //public RealValue InverseHyperbolicCosine()
    //    //{
    //    //    return new RealValue(Trig.InverseHyperbolicCosine(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Hyperbolic Area Tangent (Areatangens hyperbolicus)</summary>
    //    //public RealValue InverseHyperbolicTangent()
    //    //{
    //    //    return new RealValue(Trig.InverseHyperbolicTangent(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Hyperbolic Area Cotangent (Areacotangens hyperbolicus)</summary>
    //    //public RealValue InverseHyperbolicCotangent()
    //    //{
    //    //    return new RealValue(Trig.InverseHyperbolicCotangent(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Hyperbolic Area Secant (Areasekans hyperbolicus)</summary>
    //    //public RealValue InverseHyperbolicSecant()
    //    //{
    //    //    return new RealValue(Trig.InverseHyperbolicSecant(dataValue));
    //    //}
    //    ///// <summary>Trigonometric Hyperbolic Area Cosecant (Areacosekans hyperbolicus)</summary>
    //    //public RealValue InverseHyperbolicCosecant()
    //    //{
    //    //    return new RealValue(Trig.InverseHyperbolicCosecant(dataValue));
    //    //}
    //    #endregion
    //    #endregion

    //    #region Constants
    //    //public static Signal ParseConstant(IContext context, string value)
    //    //{
    //    //    return Constant(context, double.Parse(value, context.NumberFormat), value.Trim());
    //    //}
    //    public static Signal Constant(IContext context, Matrix value, string name)
    //    {
    //        Signal s = new Signal(context, new RealMatrixValue(value));
    //        s.Label = name + "_Constant";
    //        s.AddConstraint(Properties.ConstantSignalProperty.Instance);
    //        return s;
    //    }

    //    public static RealMatrixValue Empty
    //    {
    //        get { return new RealMatrixValue(0, 0, 0d); } //TODO: Think about Singleton like design...
    //    }

    //    public static RealMatrixValue MultiplicativeIdentity(int dimension)
    //    {
    //        return new RealMatrixValue(Matrix.Identity(dimension, dimension));
    //    }

    //    public static RealMatrixValue AdditiveIdentityIdentity(int dimension)
    //    {
    //        return new RealMatrixValue(new Matrix(dimension, dimension, 0d));
    //    }
    //    #endregion

    //    public override string ToString()
    //    {
    //        return base.ToString() + "(" + _dataValue.ToString() + ")";
    //    }

    //    public bool Equals(RealValue other)
    //    {
    //        return _dataValue.RowCount == 1 && _dataValue.ColumnCount == 1 && _dataValue[0,0] == other.Value;
    //    }
    //    public bool Equals(RationalValue other)
    //    {
    //        return _dataValue.RowCount == 1 && _dataValue.ColumnCount == 1 && _dataValue[0, 0] == other.ToDouble();
    //    }
    //    public bool Equals(IntegerValue other)
    //    {
    //        return _dataValue.RowCount == 1 && _dataValue.ColumnCount == 1 && _dataValue[0, 0] == other.ToDouble();
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

    //        RealMatrixValue realMatrixValue = other as RealMatrixValue;
    //        if(realMatrixValue != null)
    //            return Equals(realMatrixValue);

    //        return other == this;
    //    }
    //    public bool Equals(RealMatrixValue other)
    //    {
    //        return _dataValue.Equals(other._dataValue);
    //    }
    //    //public int CompareTo(RealMatrixValue other)
    //    //{
    //    //    return dataValue.CompareTo(other.dataValue);
    //    //}
    //    public override int GetHashCode()
    //    {
    //        return _dataValue.GetHashCode();
    //    }

    //    #region Serialization TODO
    //    public override ExtensionTypeXml Serialize(IContext context)
    //    {
    //        throw new NotImplementedException();

    //        //ExtensionTypeXml xml = new ExtensionTypeXml();
    //        //Type t = typeof(RealMatrixValue);
    //        //xml.QualifiedName = t.AssemblyQualifiedName;
    //        //XmlDocument doc = new XmlDocument();
    //        //XmlElement el = doc.CreateElement("Value");
    //        //el.InnerText = dataValue.ToString();
    //        //xml.Any = new XmlElement[] { el };
    //        //return xml;
    //    }
    //    public static new RealMatrixValue Deserialize(ExtensionTypeXml xml, IContext context)
    //    {
    //        throw new NotImplementedException();

    //        //return new RealMatrixValue(Matrix.Parse(xml.Any[0].InnerText, context.NumberFormat));
    //    }
    //    #endregion
    //}
}
