#region MathNet Numerics, Copyright ©2004 Christoph Ruegg 

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Christoph Ruegg, http://www.cdrnet.net,
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

namespace MathNet.Numerics
{
	/// <summary>
	/// Rational represents a finite order rational with positive
	/// powers and constant real coefficients for both numerator and denominator.
	/// </summary>
	public class Rational : IComparable, ICloneable
	{
		private Polynomial numerator, denominator;

		//TODO: Implement polynomial factorization to normalize rationals

		#region Constructors
		/// <summary>Create a new rational by polynomials (directly linked, clone them manually before applying if needed)</summary>
		public Rational(Polynomial numerator, Polynomial denominator)
		{
			this.numerator = numerator.Clone();
			this.denominator = denominator.Clone();
		}

		/// <summary>Create a new rational by orders</summary>
		public Rational(int numeratorOrder, int denominatorOrder)
		{
			numerator = new Polynomial(numeratorOrder);
			denominator = new Polynomial(denominatorOrder);
		}

		/// <summary>Create a new rational by coefficients</summary>
		public Rational(double[] numeratorCoefficients, double[] denominatorCoefficients)
		{
			numerator = new Polynomial(numeratorCoefficients);
			denominator = new Polynomial(denominatorCoefficients);
		}

		/// <summary>Create a new rational by copy</summary>
		/// <param name="copy">A rational to copy from.</param>
		public Rational(Rational copy)
		{
			numerator = new Polynomial(copy.numerator);
			denominator = new Polynomial(copy.denominator);
		}
		#endregion

		#region .NET Integration: Hashing, Equality, Ordering, Cloning
		public override int GetHashCode()
		{
			return numerator.GetHashCode() ^ denominator.GetHashCode();
		}
		public override bool Equals(Object obj)
		{
			if (obj == null || !(obj is Rational))
				return false;
			return Equals((Rational)obj);
		}
		public bool Equals(Rational rational)
		{
			return numerator.Equals(rational.numerator) && denominator.Equals(rational.denominator);
		}
		public static bool Equals(Rational rational1, Rational rational2)
		{
			return rational1.Equals(rational2);
		}
		public int CompareTo(object obj)
		{
			if(obj == null)
				return 1;
			if(!(obj is Rational))
				throw new ArgumentException("Type mismatch: rational expected.","obj");
			return CompareTo((Rational)obj);
		}
		public int CompareTo(Rational rational)
		{
			int n = numerator.CompareTo(rational.numerator);
			if(n == 0)
				n = denominator.CompareTo(rational.denominator);
			return n;
		}
		object ICloneable.Clone()
		{
			return Clone();
		}
		public Rational Clone()
		{
			return new Rational(this);
		}
		#endregion

		#region String Formatting and Parsing
		public string ToString(string baseVariable)
		{
			return "(" + numerator.ToString(baseVariable) + ")/(" + denominator.ToString(baseVariable) + ")";
		}
		public override string ToString()
		{
			return ToString("x");
		}
		#endregion

		#region Accessors
		public void Normalize()
		{
			numerator.Normalize();
			denominator.Normalize();
		}
		public Polynomial Numerator
		{
			get {return numerator;}
			set {numerator = value;}
		}
		public Polynomial Denominator
		{
			get {return denominator;}
			set {denominator = value;}
		}
		public int NumeratorOrder
		{
			get {return numerator.Order;}
		}
		public int DenominatorOrder
		{
			get {return denominator.Order;}
		}
		#endregion

		#region Operators
		public static bool operator== (Rational rational1, Rational rational2)
		{
			return rational1.Equals(rational2);
		}
		public static bool operator!= (Rational rational1, Rational rational2)
		{
			return !rational1.Equals(rational2);
		}
		public static bool operator> (Rational rational1, Rational rational2)
		{
			return rational1.CompareTo(rational2) == 1;
		}
		public static bool operator< (Rational rational1, Rational rational2)
		{
			return rational1.CompareTo(rational2) == -1;
		}
		public static bool operator>= (Rational rational1, Rational rational2)
		{
			int res = rational1.CompareTo(rational2);
			return res == 1 || res == 0;
		}
		public static bool operator<= (Rational rational1, Rational rational2)
		{
			int res = rational1.CompareTo(rational2);
			return res == -1 || res == 0;
		}

		public static Rational operator+ (Rational rational1, Rational rational2)
		{
			return rational1.Add(rational2);
		}
		public static Rational operator+ (Rational rational, Polynomial polynomial)
		{
			Rational ret = new Rational(rational);
			ret.AddInplace(polynomial);
			return ret;
		}
		public static Rational operator+ (Polynomial polynomial, Rational rational)
		{
			Rational ret = new Rational(rational);
			ret.AddInplace(polynomial);
			return ret;
		}
		public static Rational operator+ (Rational rational, double n)
		{
			Rational ret = new Rational(rational);
			ret.AddInplace(n);
			return ret;
		}
		public static Rational operator+ (double n, Rational rational)
		{
			Rational ret = new Rational(rational);
			ret.AddInplace(n);
			return ret;
		}
		public static Rational operator+ (Rational rational)
		{
			return rational;
		}

		public static Rational operator- (Rational rational1, Rational rational2)
		{
			return rational1.Subtract(rational2);
		}
		public static Rational operator- (Rational rational, Polynomial polynomial)
		{
			Rational ret = new Rational(rational);
			ret.SubtractInplace(polynomial);
			return ret;
		}
		public static Rational operator- (Polynomial polynomial, Rational rational)
		{
			Rational ret = new Rational(rational);
			ret.NegateInplace();
			ret.AddInplace(polynomial);
			return ret;
		}
		public static Rational operator- (Rational rational, double n)
		{
			Rational ret = new Rational(rational);
			ret.SubtractInplace(n);
			return ret;
		}
		public static Rational operator- (double n, Rational rational)
		{
			Rational ret = new Rational(rational);
			ret.NegateInplace();
			ret.AddInplace(n);
			return ret;
		}
		public static Rational operator- (Rational rational)
		{
			Rational ret = new Rational(rational);
			ret.NegateInplace();
			return ret;
		}

		public static Rational operator* (Rational rational1, Rational rational2)
		{
			return rational1.Multiply(rational2);
		}
		public static Rational operator* (Rational rational, Polynomial polynomial)
		{
			return rational.Multiply(polynomial);
		}
		public static Rational operator* (Polynomial polynomial, Rational rational)
		{
			return rational.Multiply(polynomial);
		}
		public static Rational operator* (Rational rational, double n)
		{
			Rational ret = new Rational(rational);
			ret.MultiplyInplace(n);
			return ret;
		}
		public static Rational operator* (double n, Rational rational)
		{
			Rational ret = new Rational(rational);
			ret.MultiplyInplace(n);
			return ret;
		}

		public static Rational operator/ (Rational rational1, Rational rational2)
		{
			return rational1.Divide(rational2);
		}
		public static Rational operator/ (Rational rational, Polynomial polynomial)
		{
			return rational.Divide(polynomial);
		}
		public static Rational operator/ (Polynomial polynomial, Rational rational)
		{
			Rational ret = rational.Divide(polynomial);
			ret.InvertInplace();
			return ret;
		}
		public static Rational operator/ (Rational rational, double n)
		{
			Rational ret = new Rational(rational);
			ret.DivideInplace(n);
			return ret;
		}
		public static Rational operator/ (double n, Rational rational)
		{
			Rational ret = new Rational(rational);
			ret.InvertInplace();
			ret.MultiplyInplace(n);
			return ret;
		}
		#endregion

		#region Inplace Arithmetic Methods
		public Rational Add(Rational rational)
		{
			if(denominator.Equals(rational.denominator))
				return new Rational(numerator+rational.numerator,denominator.Clone());
			Polynomial num = numerator*rational.denominator + rational.numerator*denominator;
			Polynomial denom = denominator*rational.denominator;
			return new Rational(num,denom);
		}
		public void AddInplace(Polynomial polynomial)
		{
			numerator.AddInplace(denominator*polynomial);
		}
		public void AddInplace(double n)
		{
			numerator.AddInplace(denominator*n);
		}

		public Rational Subtract(Rational rational)
		{
			if(denominator.Equals(rational.denominator))
				return new Rational(numerator-rational.numerator,denominator.Clone());
			Polynomial num = numerator*rational.denominator - rational.numerator*denominator;
			Polynomial denom = denominator*rational.denominator;
			return new Rational(num,denom);
		}
		public void SubtractInplace(Polynomial polynomial)
		{
			numerator.SubtractInplace(denominator*polynomial);
		}
		public void SubtractInplace(double n)
		{
			numerator.SubtractInplace(denominator*n);
		}
		public void NegateInplace()
		{
			numerator.NegateInplace();
		}

		public Rational Multiply(Rational rational)
		{
			return new Rational(numerator*rational.numerator,denominator*rational.denominator);
		}
		public Rational Multiply(Polynomial polynomial)
		{
			return new Rational(numerator*polynomial,denominator.Clone());
		}
		public void MultiplyInplace(double n)
		{
			numerator.MultiplyInplace(n);
		}

		public Rational Divide(Rational rational)
		{
			return new Rational(numerator*rational.denominator,denominator*rational.numerator);
		}
		public Rational Divide(Polynomial polynomial)
		{
			return new Rational(numerator.Clone(),denominator*polynomial);
		}
		public void DivideInplace(double n)
		{
			denominator.MultiplyInplace(n);
		}
		public void InvertInplace()
		{
			Polynomial temp = denominator;
			denominator = numerator;
			numerator = temp;
		}
		#endregion

		#region Evaluation
		public double Evaluate(double value)
		{
			// TODO: correct Rational.Evaluate implementation
			// the formula here below is accurate iff the Rational cannot be 'simplified'

			return numerator.Evaluate(value)/denominator.Evaluate(value);
		}
		#endregion
	}
}