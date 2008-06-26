#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2004-2008, Christoph Rüegg, http://christoph.ruegg.name
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published 
// by the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Text;
using System.Collections.Generic;
using MathNet.Numerics.Properties;
using MathNet.Numerics.Distributions;

namespace MathNet.Numerics.LinearAlgebra
{

    /// <summary>
    /// Complex Vector.
    /// </summary>
    [Serializable]
    public class ComplexVector :
        IVector<Complex>,
        IList<Complex>,
        ICloneable
    {

        private Complex[] _data;
        private int _length;

        /// <summary>
        /// Gets dimensionality of the vector.
        /// </summary>
        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Gets or sets the element indexed by <c>i</c>
        /// in the <c>Vector</c>.
        /// </summary>
        /// <param name="i">Dimension index.</param>
        public Complex this[int i]
        {
            get { return _data[i]; }
            set { _data[i] = value; }
        }

        #region Constructors and static constructive methods

        /// <summary>
        /// Constructs an n-dimensional vector of zeros.
        /// </summary>
        /// <param name="n">Dimensionality of vector.</param>
        public
        ComplexVector(
            int n
            )
        {
            _length = n;
            _data = new Complex[_length];
        }

        /// <summary>
        /// Constructs an n-dimensional unit vector for i'th coordinate.
        /// </summary>
        /// <param name="n">Dimensionality of vector.</param>
        /// <param name="i">Coordinate index.</param>
        public
        ComplexVector(
            int n,
            int i
            )
        {
            _length = n;
            _data = new Complex[_length];
            _data[i] = 1.0;
        }

        /// <summary>
        /// Constructs an n-dimensional constant vector.
        /// </summary>
        /// <param name="n">Dimensionality of vector.</param>
        /// <param name="value">Fill the vector with this scalar value.</param>
        public
        ComplexVector(
            int n,
            Complex value
            )
        {
            _length = n;
            _data = new Complex[_length];
            for(int i = 0; i < _length; i++)
            {
                _data[i] = value;
            }
        }

        /// <summary>
        /// Constructs a vector from a 1-D array, directly using
        /// the provided array as internal data structure.
        /// </summary>
        /// <param name="components">One-dimensional array of doubles.</param>
        /// <seealso cref="Create(Complex[])"/>
        public
        ComplexVector(
            Complex[] components
            )
        {
            _length = components.Length;
            _data = components;
        }

        /// <summary>
        /// Constructs a vector from a copy of a 1-D array.
        /// </summary>
        /// <param name="components">One-dimensional array of doubles.</param>
        public static
        ComplexVector
        Create(
            Complex[] components
            )
        {
            if(null == components)
            {
                throw new ArgumentNullException("components");
            }

            Complex[] newData = new Complex[components.Length];
            components.CopyTo(newData, 0);

            return new ComplexVector(newData);
        }

        /// <summary>
        /// Constructs a complex vector from a real and an imaginary vector.
        /// </summary>
        /// <param name="realComponents">One-dimensional array of doubles representing the real part of the vector.</param>
        /// <param name="imagComponents">One-dimensional array of doubles representing the imaginary part of the vector.</param>
        public static
        ComplexVector
        Create(
            IList<double> realComponents,
            IList<double> imagComponents
            )
        {
            if(null == realComponents)
            {
                throw new ArgumentNullException("realComponents");
            }

            if(null == imagComponents)
            {
                throw new ArgumentNullException("imagComponents");
            }

            if(realComponents.Count != imagComponents.Count)
            {
                throw new ArgumentException(Properties.Resources.ArgumentVectorsSameLengths);
            }

            Complex[] newData = new Complex[realComponents.Count];
            for(int i = 0; i < newData.Length; i++)
            {
                newData[i] = new Complex(realComponents[i], imagComponents[i]);
            }

            return new ComplexVector(newData);
        }

        // TODO: Add Random Vector Generations Methods here

        /// <summary>
        /// Generates an n-dimensional vector filled with 1.
        /// </summary>
        /// <param name="n">Dimensionality of vector.</param>
        public static
        ComplexVector
        Ones(
            int n
            )
        {
            return new ComplexVector(n, 1.0);
        }

        /// <summary>
        /// Generates an n-dimensional vector filled with 0.
        /// </summary>
        /// <param name="n">Dimensionality of vector.</param>
        public static
        ComplexVector
        Zeros(
            int n
            )
        {
            return new ComplexVector(n);
        }

        /// <summary>
        /// Generates an n-dimensional unit vector for i'th coordinate.
        /// </summary>
        /// <param name="n">Dimensionality of vector.</param>
        /// <param name="i">Coordinate index.</param>
        public static
        ComplexVector
        BasisVector(
            int n,
            int i
            )
        {
            return new ComplexVector(n, i);
        }

        #endregion

        #region Conversion Operators and conversion to other types

        /// <summary>
        /// Returns a reference to the internel data structure.
        /// </summary>
        public static implicit
        operator Complex[](
            ComplexVector v
            )
        {
            return v._data;
        }

        /// <summary>
        /// Returns a vector bound directly to a reference of the provided array.
        /// </summary>
        public static implicit
        operator ComplexVector(
            Complex[] v
            )
        {
            return new ComplexVector(v);
        }

        /// <summary>
        /// Copies the internal data structure to an array.
        /// </summary>
        public
        Complex[]
        CopyToArray()
        {
            Complex[] newData = new Complex[_length];
            _data.CopyTo(newData, 0);
            return newData;
        }

        /// <summary>
        /// Create a matrix based on this vector in column form (one single column).
        /// </summary>
        public
        ComplexMatrix
        ToColumnMatrix()
        {
            Complex[][] m = ComplexMatrix.CreateMatrixData(_length, 1);
            for(int i = 0; i < _data.Length; i++)
            {
                m[i][0] = _data[i];
            }

            return new ComplexMatrix(m);
        }

        /// <summary>
        /// Create a matrix based on this vector in row form (one single row).
        /// </summary>
        public
        ComplexMatrix
        ToRowMatrix()
        {
            Complex[][] m = ComplexMatrix.CreateMatrixData(1, _length);
            Complex[] mRow = m[0];
            for(int i = 0; i < _data.Length; i++)
            {
                mRow[i] = _data[i];
            }

            return new ComplexMatrix(m);
        }

        #endregion

        #region Elementary operations

        /// <summary>
        /// Add another complex vector to this vector.
        /// </summary>
        /// <param name="b">The other complex vector.</param>
        /// <returns>
        /// Vector ret[i] = this[i] + b[i]
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded + operator.
        /// </remarks>
        /// <seealso cref="AddInplace(IVector&lt;Complex&gt;)"/>
        /// <seealso cref="operator + (ComplexVector, ComplexVector)"/>
        public
        ComplexVector
        Add(
            IVector<Complex> b
            )
        {
            CheckMatchingVectorDimensions(this, b);

            Complex[] v = new Complex[_length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = _data[i] + b[i];
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// Add another real vector to this vector.
        /// </summary>
        /// <param name="b">The other real vector.</param>
        /// <returns>
        /// Vector ret[i] = this[i] + b[i]
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded + operator.
        /// </remarks>
        /// <seealso cref="AddInplace(IVector&lt;double&gt;)"/>
        /// <seealso cref="operator + (ComplexVector, Vector)"/>
        public
        ComplexVector
        Add(
            IVector<double> b
            )
        {
            CheckMatchingVectorDimensions(this, b);

            Complex[] v = new Complex[_length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = _data[i] + b[i];
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// Add a complex scalar to all elements of this vector.
        /// </summary>
        /// <param name="b">The complex scalar.</param>
        /// <returns>
        /// Vector ret[i] = this[i] + b
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded + operator.
        /// </remarks>
        /// <seealso cref="AddInplace(Complex)"/>
        /// <seealso cref="operator + (ComplexVector, Complex)"/>
        public
        ComplexVector
        Add(
            Complex b
            )
        {
            Complex[] v = new Complex[_length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = _data[i] + b;
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// In place addition of a complex vector to this vector.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="Add(IVector&lt;Complex&gt;)"/>
        /// <seealso cref="operator + (ComplexVector, ComplexVector)"/>
        public
        void
        AddInplace(
            IVector<Complex> b
            )
        {
            CheckMatchingVectorDimensions(this, b);

            for(int i = 0; i < _length; i++)
            {
                _data[i] += b[i];
            }
        }

        /// <summary>
        /// In place addition of a real vector to this vector.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="Add(IVector&lt;double&gt;)"/>
        /// <seealso cref="operator + (ComplexVector, Vector)"/>
        public
        void
        AddInplace(
            IVector<double> b
            )
        {
            CheckMatchingVectorDimensions(this, b);

            for(int i = 0; i < _length; i++)
            {
                _data[i] += b[i];
            }
        }

        /// <summary>
        /// In place addition of a complex scalar to all elements of this vector.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="Add(Complex)"/>
        /// <seealso cref="operator + (ComplexVector, Complex)"/>
        public
        void
        AddInplace(
            Complex b
            )
        {
            for(int i = 0; i < _length; i++)
            {
                _data[i] += b;
            }
        }

        /// <summary>
        /// Subtract a complex vector from this vector.
        /// </summary>
        /// <param name="b">The other complex vector.</param>
        /// <returns>
        /// Vector ret[i] = this[i] - b[i]
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded - operator.
        /// </remarks>
        /// <seealso cref="SubtractInplace(IVector&lt;Complex&gt;)"/>
        /// <seealso cref="operator - (ComplexVector, ComplexVector)"/>
        public
        ComplexVector
        Subtract(
            IVector<Complex> b
            )
        {
            CheckMatchingVectorDimensions(this, b);

            Complex[] v = new Complex[_length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = _data[i] - b[i];
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// Subtract a real vector from this vector.
        /// </summary>
        /// <param name="b">The other real vector.</param>
        /// <returns>
        /// Vector ret[i] = this[i] - b[i]
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded - operator.
        /// </remarks>
        /// <seealso cref="SubtractInplace(IVector&lt;double&gt;)"/>
        /// <seealso cref="operator - (ComplexVector, Vector)"/>
        public
        ComplexVector
        Subtract(
            IVector<double> b
            )
        {
            CheckMatchingVectorDimensions(this, b);

            Complex[] v = new Complex[_length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = _data[i] - b[i];
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// Subtract a complex scalar from all elements of this vector.
        /// </summary>
        /// <param name="b">The complex scalar.</param>
        /// <returns>
        /// Vector ret[i] = this[i] - b
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded - operator.
        /// </remarks>
        /// <seealso cref="SubtractInplace(Complex)"/>
        /// <seealso cref="operator - (ComplexVector, Complex)"/>
        public
        ComplexVector
        Subtract(
            Complex b
            )
        {
            Complex[] v = new Complex[_length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = _data[i] - b;
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// In place subtraction of a complex vector from this vector.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="Subtract(IVector&lt;Complex&gt;)"/>
        /// <seealso cref="operator - (ComplexVector, ComplexVector)"/>
        public
        void
        SubtractInplace(
            IVector<Complex> b
            )
        {
            CheckMatchingVectorDimensions(this, b);

            for(int i = 0; i < _length; i++)
            {
                _data[i] -= b[i];
            }
        }

        /// <summary>
        /// In place subtraction of a real vector from this vector.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="Subtract(IVector&lt;double&gt;)"/>
        /// <seealso cref="operator - (ComplexVector, Vector)"/>
        public
        void
        SubtractInplace(
            IVector<double> b
            )
        {
            CheckMatchingVectorDimensions(this, b);

            for(int i = 0; i < _length; i++)
            {
                _data[i] -= b[i];
            }
        }

        /// <summary>
        /// In place subtraction of a complex scalar from all elements of this vector.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="Subtract(Complex)"/>
        /// <seealso cref="operator - (ComplexVector, Complex)"/>
        public
        void
        SubtractInplace(
            Complex b
            )
        {
            for(int i = 0; i < _length; i++)
            {
                _data[i] -= b;
            }
        }

        /// <summary>
        /// Negate this vector.
        /// </summary>
        /// <returns>
        /// Vector ret[i] = -this[i]
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded - operator.
        /// </remarks>
        /// <seealso cref="NegateInplace"/>
        /// <seealso cref="operator - (ComplexVector)"/>
        public
        ComplexVector
        Negate()
        {
            Complex[] v = new Complex[_length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = -_data[i];
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// In place unary minus of the <c>Vector</c>.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="Negate"/>
        /// <seealso cref="operator - (ComplexVector)"/>
        public
        void
        NegateInplace()
        {
            for(int i = 0; i < _length; i++)
            {
                _data[i] = -_data[i];
            }
        }

        /// <summary>
        /// Conjugate this vector.
        /// </summary>
        /// <returns>
        /// Vector ret[i] = real(this[i]) - imag(this[i])
        /// </returns>
        /// <seealso cref="ConjugateInplace"/>
        public
        ComplexVector
        Conjugate()
        {
            Complex[] v = new Complex[_length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = _data[i].Conjugate;
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// In place conjugation of this vector.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="Conjugate"/>
        public
        void
        ConjugateInplace()
        {
            for(int i = 0; i < _length; i++)
            {
                _data[i] = _data[i].Conjugate;
            }
        }

        /// <summary>
        /// Scale this complex vector with a complex scalar.
        /// </summary>
        /// <param name="scalar">The scalar to scale with</param>
        /// <returns>
        /// Vector ret[i] = this[i] * scalar
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded * operator.
        /// </remarks>
        /// <seealso cref="MultiplyInplace(Complex)"/>
        /// <seealso cref="operator * (ComplexVector, Complex)"/>
        public
        ComplexVector
        Multiply(
            Complex scalar
            )
        {
            Complex[] v = new Complex[_length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = _data[i] * scalar;
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// Multiplies in place this <c>Vector</c> by a scalar.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="Multiply(Complex)"/>
        /// <seealso cref="operator * (ComplexVector, Complex)"/>
        public
        void
        MultiplyInplace(
            Complex scalar
            )
        {
            for(int i = 0; i < _length; i++)
            {
                _data[i] *= scalar;
            }
        }

        #endregion

        #region Vector Products

        /// <summary>
        /// Scalar product of two vectors.
        /// </summary>
        /// <returns>
        /// Scalar ret = sum(u[i] * v[i])
        /// </returns>
        /// <seealso cref="ScalarMultiply(IVector&lt;Complex&gt;)"/>
        /// <seealso cref="operator * (ComplexVector, ComplexVector)"/>
        public static
        Complex
        ScalarProduct(
            IVector<Complex> u,
            IVector<Complex> v
            )
        {
            CheckMatchingVectorDimensions(u, v);

            Complex sum = 0;
            for(int i = 0; i < u.Length; i++)
            {
                sum += u[i] * v[i];
            }

            return sum;
        }

        /// <summary>
        /// Scalar product of two vectors.
        /// </summary>
        /// <returns>
        /// Scalar ret = sum(u[i] * v[i])
        /// </returns>
        /// <seealso cref="ScalarMultiply(IVector&lt;double&gt;)"/>
        /// <seealso cref="operator * (ComplexVector, Vector)"/>
        public static
        Complex
        ScalarProduct(
            IVector<Complex> u,
            IVector<double> v
            )
        {
            CheckMatchingVectorDimensions(u, v);

            Complex sum = 0;
            for(int i = 0; i < u.Length; i++)
            {
                sum += u[i] * v[i];
            }

            return sum;
        }

        /// <summary>
        /// Scalar product of this vector with another complex vector.
        /// </summary>
        /// <param name="b">The other complex vector.</param>
        /// <returns>
        /// Scalar ret = sum(this[i] * b[i])
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded * operator.
        /// </remarks>
        /// <seealso cref="ScalarProduct(IVector&lt;Complex&gt;,IVector&lt;Complex&gt;)"/>
        /// <seealso cref="operator * (ComplexVector, ComplexVector)"/>
        public
        Complex
        ScalarMultiply(
            IVector<Complex> b
            )
        {
            return ScalarProduct(this, b);
        }

        /// <summary>
        /// Scalar product of this vector with another real vector.
        /// </summary>
        /// <param name="b">The other real vector.</param>
        /// <returns>
        /// Scalar ret = sum(this[i] * b[i])
        /// </returns>
        /// <remarks>
        /// This method has the same effect as the overloaded * operator.
        /// </remarks>
        /// <seealso cref="ScalarProduct(IVector&lt;Complex&gt;,IVector&lt;double&gt;)"/>
        /// <seealso cref="operator * (ComplexVector, Vector)"/>
        public
        Complex
        ScalarMultiply(
            IVector<double> b
            )
        {
            return ScalarProduct(this, b);
        }

        /// <summary>
        /// Dyadic Product of two vectors.
        /// </summary>
        /// <returns>
        /// Matrix M[i,j] = u[i] * v[j].
        /// </returns>
        /// <seealso cref="TensorMultiply"/>
        public static
        ComplexMatrix
        DyadicProduct(
            IVector<Complex> u,
            IVector<Complex> v
            )
        {
            Complex[][] m = ComplexMatrix.CreateMatrixData(u.Length, v.Length);
            for(int i = 0; i < u.Length; i++)
            {
                for(int j = 0; j < v.Length; j++)
                {
                    m[i][j] = u[i] * v[j];
                }
            }

            return new ComplexMatrix(m);
        }

        /// <summary>
        /// Tensor Product (Dyadic) of this and another vector.
        /// </summary>
        /// <param name="b">The vector to operate on.</param>
        /// <returns>
        /// Matrix M[i,j] = this[i] * v[j].
        /// </returns>
        /// <seealso cref="DyadicProduct"/>
        public
        ComplexMatrix
        TensorMultiply(
            IVector<Complex> b
            )
        {
            return DyadicProduct(this, b);
        }

        /// <summary>
        /// Cross product of two 3-dimensional vectors.
        /// </summary>
        /// <returns>
        /// Vector ret = (u[2]v[3] - u[3]v[2], u[3]v[1] - u[1]v[3], u[1]v[2] - u[2]v[1]).
        /// </returns>
        /// <seealso cref="CrossMultiply"/>
        public static
        ComplexVector
        CrossProduct(
            IVector<Complex> u,
            IVector<Complex> v
            )
        {
            CheckMatchingVectorDimensions(u, v);
            if(3 != u.Length)
            {
                throw new ArgumentOutOfRangeException("u", Resources.ArgumentVectorThreeDimensional);
            }

            ComplexVector product = new ComplexVector(new Complex[] {
                u[1]*v[2] - u[2]*v[1],
                u[2]*v[0] - u[0]*v[2],
                u[0]*v[1] - u[1]*v[0]
                });

            return product;
        }

        /// <summary>
        /// Cross product of this vector with another vector.
        /// </summary>
        /// <param name="b">The other vector.</param>
        /// <returns>
        /// Vector ret = (this[2]b[3] - this[3]b[2], this[3]b[1] - this[1]b[3], this[1]b[2] - this[2]b[1]).
        /// </returns>
        /// <seealso cref="CrossProduct"/>
        public
        ComplexVector
        CrossMultiply(
            IVector<Complex> b
            )
        {
            return CrossProduct(this, b);
        }

        /// <summary>
        /// Array (element-by-element) product of two vectors.
        /// </summary>
        /// <returns>
        /// Vector ret[i] = u[i] * v[i]
        /// </returns>
        /// <seealso cref="ArrayMultiply(IVector&lt;Complex&gt;)"/>
        /// <seealso cref="ArrayMultiplyInplace(IVector&lt;Complex&gt;)"/>
        public static
        ComplexVector
        ArrayProduct(
            IVector<Complex> a,
            IVector<Complex> b
            )
        {
            CheckMatchingVectorDimensions(a, b);

            Complex[] v = new Complex[a.Length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = a[i] * b[i];
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// Array (element-by-element) product of two vectors.
        /// </summary>
        /// <returns>
        /// Vector ret[i] = u[i] * v[i]
        /// </returns>
        /// <seealso cref="ArrayMultiply(IVector&lt;double&gt;)"/>
        /// <seealso cref="ArrayMultiplyInplace(IVector&lt;double&gt;)"/>
        public static
        ComplexVector
        ArrayProduct(
            IVector<Complex> a,
            IVector<double> b
            )
        {
            CheckMatchingVectorDimensions(a, b);

            Complex[] v = new Complex[a.Length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = a[i] * b[i];
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// Array (element-by-element) product of this vector and another vector.
        /// </summary>
        /// <returns>
        /// Vector ret[i] = this[i] * b[i]
        /// </returns>
        /// <seealso cref="ArrayProduct(IVector&lt;Complex&gt;,IVector&lt;Complex&gt;)"/>
        /// <seealso cref="ArrayMultiplyInplace(IVector&lt;Complex&gt;)"/>
        public
        ComplexVector
        ArrayMultiply(
            IVector<Complex> b
            )
        {
            return ArrayProduct(this, b);
        }

        /// <summary>
        /// Array (element-by-element) product of this vector and another vector.
        /// </summary>
        /// <returns>
        /// Vector ret[i] = this[i] * b[i]
        /// </returns>
        /// <seealso cref="ArrayProduct(IVector&lt;Complex&gt;,IVector&lt;double&gt;)"/>
        /// <seealso cref="ArrayMultiplyInplace(IVector&lt;double&gt;)"/>
        public
        ComplexVector
        ArrayMultiply(
            IVector<double> b
            )
        {
            return ArrayProduct(this, b);
        }

        /// <summary>
        /// Multiply in place (element-by-element) another vector to this vector.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="ArrayProduct(IVector&lt;Complex&gt;,IVector&lt;Complex&gt;)"/>
        /// <seealso cref="ArrayMultiply(IVector&lt;Complex&gt;)"/>
        public
        void
        ArrayMultiplyInplace(
            IVector<Complex> b
            )
        {
            CheckMatchingVectorDimensions(this, b);

            for(int i = 0; i < _data.Length; i++)
            {
                _data[i] *= b[i];
            }
        }

        /// <summary>
        /// Multiply in place (element-by-element) another vector to this vector.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="ArrayProduct(IVector&lt;Complex&gt;,IVector&lt;double&gt;)"/>
        /// <seealso cref="ArrayMultiply(IVector&lt;double&gt;)"/>
        public
        void
        ArrayMultiplyInplace(
            IVector<double> b
            )
        {
            CheckMatchingVectorDimensions(this, b);

            for(int i = 0; i < _data.Length; i++)
            {
                _data[i] *= b[i];
            }
        }

        /// <summary>
        /// Array (element-by-element) raise to power.
        /// </summary>
        /// <returns>
        /// Vector ret[i] = this[i] ^ exponent
        /// </returns>
        /// <seealso cref="ArrayPowerInplace(Complex)"/>
        public
        ComplexVector
        ArrayPower(
            Complex exponent
            )
        {
            Complex[] newData = new Complex[_length];
            for(int i = 0; i < newData.Length; i++)
            {
                newData[i] = _data[i].Power(exponent);
            }

            return new ComplexVector(newData);
        }

        /// <summary>
        /// In place array (element-by-element) raise to power.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="ArrayPower(Complex)"/>
        public
        void
        ArrayPowerInplace(
            Complex exponent
            )
        {
            for(int i = 0; i < _data.Length; i++)
            {
                _data[i] = _data[i].Power(exponent);
            }
        }

        /// <summary>
        /// Array (element-by-element) quotient of two vectors.
        /// </summary>
        /// <returns>
        /// Vector ret[i] = u[i] / v[i]
        /// </returns>
        /// <seealso cref="ArrayDivide(IVector&lt;Complex&gt;)"/>
        /// <seealso cref="ArrayDivideInplace(IVector&lt;Complex&gt;)"/>
        public static
        ComplexVector
        ArrayQuotient(
            IVector<Complex> a,
            IVector<Complex> b
            )
        {
            CheckMatchingVectorDimensions(a, b);

            Complex[] v = new Complex[a.Length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = a[i] / b[i];
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// Array (element-by-element) quotient of two vectors.
        /// </summary>
        /// <returns>
        /// Vector ret[i] = u[i] / v[i]
        /// </returns>
        /// <seealso cref="ArrayDivide(IVector&lt;double&gt;)"/>
        /// <seealso cref="ArrayDivideInplace(IVector&lt;double&gt;)"/>
        public static
        ComplexVector
        ArrayQuotient(
            IVector<Complex> a,
            IVector<double> b
            )
        {
            CheckMatchingVectorDimensions(a, b);

            Complex[] v = new Complex[a.Length];
            for(int i = 0; i < v.Length; i++)
            {
                v[i] = a[i] / b[i];
            }

            return new ComplexVector(v);
        }

        /// <summary>
        /// Array (element-by-element) quotient of this vector and another complex vector.
        /// </summary>
        /// <returns>
        /// Vector ret[i] = this[i] / b[i]
        /// </returns>
        /// <seealso cref="ArrayQuotient(IVector&lt;Complex&gt;,IVector&lt;Complex&gt;)"/>
        /// <seealso cref="ArrayDivideInplace(IVector&lt;Complex&gt;)"/>
        public
        ComplexVector
        ArrayDivide(
            IVector<Complex> b
            )
        {
            return ArrayQuotient(this, b);
        }

        /// <summary>
        /// Array (element-by-element) quotient of this vector and another real vector.
        /// </summary>
        /// <returns>
        /// Vector ret[i] = this[i] / b[i]
        /// </returns>
        /// <seealso cref="ArrayQuotient(IVector&lt;Complex&gt;,IVector&lt;double&gt;)"/>
        /// <seealso cref="ArrayDivideInplace(IVector&lt;double&gt;)"/>
        public
        ComplexVector
        ArrayDivide(
            IVector<double> b
            )
        {
            return ArrayQuotient(this, b);
        }

        /// <summary>
        /// Divide in place (element-by-element) this vector by another complex vector.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="ArrayQuotient(IVector&lt;Complex&gt;,IVector&lt;Complex&gt;)"/>
        /// <seealso cref="ArrayDivide(IVector&lt;Complex&gt;)"/>
        public
        void
        ArrayDivideInplace(
            IVector<Complex> b
            )
        {
            CheckMatchingVectorDimensions(this, b);

            for(int i = 0; i < _data.Length; i++)
            {
                _data[i] /= b[i];
            }
        }

        /// <summary>
        /// Divide in place (element-by-element) this vector by another real vector.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="ArrayQuotient(IVector&lt;Complex&gt;,IVector&lt;double&gt;)"/>
        /// <seealso cref="ArrayDivide(IVector&lt;double&gt;)"/>
        public
        void
        ArrayDivideInplace(
            IVector<double> b
            )
        {
            CheckMatchingVectorDimensions(this, b);

            for(int i = 0; i < _data.Length; i++)
            {
                _data[i] /= b[i];
            }
        }

        /// <summary>
        /// Map an arbitrary function to all elements of this vector.
        /// </summary>
        /// <returns>
        /// Vector ret[i] = mapping(this[i])
        /// </returns>
        /// <seealso cref="ArrayMapInplace(Converter&lt;Complex,Complex&gt;)"/>
        public
        ComplexVector
        ArrayMap(
            Converter<Complex, Complex> mapping
            )
        {
            Complex[] newData = new Complex[_length];
            for(int i = 0; i < newData.Length; i++)
            {
                newData[i] = mapping(_data[i]);
            }

            return new ComplexVector(newData);
        }

        /// <summary>
        /// In place map an arbitrary function to all elements of this vector.
        /// </summary>
        /// <remarks>
        /// This method changes this vector.
        /// </remarks>
        /// <seealso cref="ArrayMap(Converter&lt;Complex,Complex&gt;)"/>
        public
        void
        ArrayMapInplace(
            Converter<Complex, Complex> mapping
            )
        {
            for(int i = 0; i < _data.Length; i++)
            {
                _data[i] = mapping(_data[i]);
            }
        }

        #endregion

        #region Vector Norms

        /// <summary>
        /// 1-Norm also known as Manhattan Norm or Taxicab Norm.
        /// </summary>
        /// <returns>
        /// Scalar ret = sum(abs(this[i]))
        /// </returns>
        public
        double
        Norm1()
        {
            double sum = 0;
            for(int i = 0; i < _data.Length; i++)
            {
                sum += _data[i].Modulus;
            }

            return sum;
        }

        #endregion

        #region Arithmetic Operator Overloading

        /// <summary>
        /// Addition Operator.
        /// </summary>
        public static
        ComplexVector
        operator +(
            ComplexVector u,
            ComplexVector v
            )
        {
            CheckMatchingVectorDimensions(u, v);
            return u.Add(v);
        }

        /// <summary>
        /// Addition Operator.
        /// </summary>
        public static
        ComplexVector
        operator +(
            ComplexVector complexVector,
            Vector realVector
            )
        {
            CheckMatchingVectorDimensions(complexVector, realVector);
            return complexVector.Add(realVector);
        }

        /// <summary>
        /// Addition Operator.
        /// </summary>
        public static
        ComplexVector
        operator +(
            ComplexVector complexVector,
            Complex scalar
            )
        {
            return complexVector.Add(scalar);
        }

        /// <summary>
        /// Subtraction Operator.
        /// </summary>
        public static
        ComplexVector
        operator -(
            ComplexVector u,
            ComplexVector v
            )
        {
            CheckMatchingVectorDimensions(u, v);
            return u.Subtract(v);
        }

        /// <summary>
        /// Subtraction Operator.
        /// </summary>
        public static
        ComplexVector
        operator -(
            ComplexVector complexVector,
            Vector realVector
            )
        {
            CheckMatchingVectorDimensions(complexVector, realVector);
            return complexVector.Subtract(realVector);
        }

        /// <summary>
        /// Subtraction Operator.
        /// </summary>
        public static
        ComplexVector
        operator -(
            ComplexVector complexVector,
            Complex scalar
            )
        {
            return complexVector.Subtract(scalar);
        }

        /// <summary>
        /// Negate a vectors
        /// </summary>
        public static
        ComplexVector
        operator -(
            ComplexVector v
            )
        {
            return v.Negate();
        }

        /// <summary>
        /// Scaling a vector by a scalar.
        /// </summary>
        public static
        ComplexVector
        operator *(
            Complex scalar,
            ComplexVector vector
            )
        {
            return vector.Multiply(scalar);
        }

        /// <summary>
        /// Scaling a vector by a scalar.
        /// </summary>
        public static
        ComplexVector
        operator *(
            ComplexVector vector,
            Complex scalar
            )
        {
            return vector.Multiply(scalar);
        }

        /// <summary>
        /// Scaling a vector by the inverse of a scalar.
        /// </summary>
        public static
        ComplexVector
        operator /(
            ComplexVector vector,
            Complex scalar
            )
        {
            return vector.Multiply(1 / scalar);
        }

        /// <summary>
        /// Scalar/dot product of two vectors.
        /// </summary>
        public static
        Complex
        operator *(
            ComplexVector u,
            ComplexVector v
            )
        {
            return ScalarProduct(u, v);
        }

        /// <summary>
        /// Scalar/dot product of two vectors.
        /// </summary>
        public static
        Complex
        operator *(
            ComplexVector u,
            Vector v
            )
        {
            return ScalarProduct(u, v);
        }

        #endregion

        #region Various Helpers & Infrastructure

        /// <summary>Check if size(A) == size(B) *</summary>
        private static
        void
        CheckMatchingVectorDimensions(
            IVector<Complex> A,
            IVector<Complex> B
            )
        {
            if(null == A)
            {
                throw new ArgumentNullException("A");
            }

            if(null == B)
            {
                throw new ArgumentNullException("B");
            }

            if(A.Length != B.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLengths);
            }
        }

        /// <summary>Check if size(A) == size(B) *</summary>
        private static
        void
        CheckMatchingVectorDimensions(
            IVector<Complex> A,
            IVector<double> B
            )
        {
            if(null == A)
            {
                throw new ArgumentNullException("A");
            }

            if(null == B)
            {
                throw new ArgumentNullException("B");
            }

            if(A.Length != B.Length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLengths);
            }
        }

        /// <summary>Returns a deep copy of this instance.</summary>
        public
        ComplexVector
        Clone()
        {
            return Create(_data);
        }

        /// <summary>
        /// Creates an exact copy of this matrix.
        /// </summary>
        object
        ICloneable.Clone()
        {
            return Create(_data);
        }

        /// <summary>
        /// Formats this vector to a human-readable string
        /// </summary>
        public override
        string
        ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            for(int i = 0; i < _data.Length; i++)
            {
                if(i != 0)
                {
                    sb.Append(',');
                }

                sb.Append(_data[i].ToString());
            }

            sb.Append("]");
            return sb.ToString();
        }

        #endregion

        #region IList<Complex> Interface Implementation

        /// <summary>
        /// Index of an element.
        /// </summary>
        int
        IList<Complex>.IndexOf(
            Complex item
            )
        {
            return Array.IndexOf(_data, item);
        }

        /// <summary>
        /// True if the vector contains some element.
        /// </summary>
        bool
        ICollection<Complex>.Contains(
            Complex item
            )
        {
            return Array.IndexOf(_data, item) >= 0;
        }

        /// <summary>
        /// Copy all elements to some array.
        /// </summary>
        void
        ICollection<Complex>.CopyTo(
            Complex[] array,
            int arrayIndex
            )
        {
            _data.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Length.
        /// </summary>
        int ICollection<Complex>.Count
        {
            get { return Length; }
        }

        /// <summary>
        /// Get a typed enumerator over all elements.
        /// </summary>
        IEnumerator<Complex>
        IEnumerable<Complex>.GetEnumerator()
        {
            return ((IEnumerable<Complex>)_data).GetEnumerator();
        }

        /// <summary>
        /// Get a non-typed enumerator over all elements.
        /// </summary>
        System.Collections.IEnumerator
        System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        /// <summary>
        /// False.
        /// </summary>
        bool ICollection<Complex>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Not Supported.
        /// </summary>
        void
        ICollection<Complex>.Add(
            Complex item
            )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not Supported.
        /// </summary>
        void
        IList<Complex>.Insert(
            int index,
            Complex item
            )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not Supported.
        /// </summary>
        bool
        ICollection<Complex>.Remove(
            Complex item
            )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not Supported.
        /// </summary>
        void
        IList<Complex>.RemoveAt(
            int index
            )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not Supported.
        /// </summary>
        void
        ICollection<Complex>.Clear()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
