#region MathNet Numerics, Copyright ©2004 Joannes Vermorel

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Joannes Vermorel, http://www.vermorel.com
//						Jon Skeet, http://www.yoda.arachsys.com/csharp/
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
using System.Globalization;

namespace MathNet.Numerics
{
    //// TODO: check, maybe merge with Number class

    ///// <summary>
    ///// The class <c>Double</c> provides various utilities to handle
    ///// <c>double</c> values.
    ///// </summary>
    //public sealed class Double
    //{
    //    private Double() {}

    //    /// <summary>
    //    /// Converts the given double to a string representation of its
    //    /// exact decimal value.
    //    /// </summary>
    //    /// <param name="d">The double to convert.</param>
    //    /// <return>A string representation of the double's exact decimal value.</return>
    //    /// <remarks>
    //    /// <p>The .Net framework often round-up the <c>double</c> value
    //    /// when converting them into <c>string</c>. This behavior however
    //    /// might be troublesome for application. The method <c>ToExactString</c>
    //    /// provides, as the name suggests, an non-rounded string conversion
    //    /// of a <c>double</c> value.</p>
    //    /// </remarks>
    //    public static string ToExactString (double d)
    //    {
    //        if (double.IsInfinity(d)) return "PositiveInfinity";
    //        if (double.IsNegativeInfinity(d)) return "NegativeInfinity";
    //        if (double.IsNaN(d)) return "NaN";

    //        // Translate the double into sign, exponent and mantissa.
    //        long bits = BitConverter.DoubleToInt64Bits(d);
    //        bool negative = (bits >> 63) == -1;

    //        int exponent = (int) ((bits >> 52) & 0x7ffL);
    //        long mantissa = bits & 0xfffffffffffffL;

    //        // Subnormal numbers; exponent is effectively one higher,
    //        // but there's no extra normalisation bit in the mantissa
    //        if (exponent==0)
    //        {
    //            exponent++;
    //        }
    //            // Normal numbers; leave exponent as it is but add extra
    //            // bit to the front of the mantissa
    //        else
    //        {
    //            mantissa = mantissa | (1L<<52);
    //        }
        
    //        // Bias the exponent. It's actually biased by 1023, but we're
    //        // treating the mantissa as m.0 rather than 0.m, so we need
    //        // to subtract another 52 from it.
    //        exponent -= 1075;
        
    //        if (mantissa == 0) 
    //        {
    //            return "0";
    //        }
        
    //        /* Normalize */
    //        while((mantissa & 1) == 0) 
    //        {    /*  i.e., Mantissa is even */
    //            mantissa >>= 1;
    //            exponent++;
    //        }
        
    //        // Construct a new decimal expansion with the mantissa
    //        ArbitraryDecimal ad = new ArbitraryDecimal (mantissa);
        
    //        // If the exponent is less than 0, we need to repeatedly
    //        // divide by 2 - which is the equivalent of multiplying
    //        // by 5 and dividing by 10.
    //        if (exponent < 0) 
    //        {
    //            for (int i=0; i < -exponent; i++) ad.MultiplyBy(5);
    //            ad.Shift(-exponent);
    //        } 
    //            // Otherwise, we need to repeatedly multiply by 2
    //        else
    //        {
    //            for (int i=0; i < exponent; i++) ad.MultiplyBy(2);
    //        }
        
    //        // Finally, return the string with an appropriate sign
    //        if (negative) return "-"+ad.ToString();
    //        else return ad.ToString();
    //    }
    
    //    /// <summary>Private class used for manipulating.</summary>
    //    private class ArbitraryDecimal
    //    {
    //        /// <summary>Digits in the decimal expansion, one byte per digit</summary>
    //        byte[] digits;

    //        /// <summary> How many digits are *after* the decimal point</summary>
    //        int decimalPoint; //=0;

    //        /// <summary> 
    //        /// Constructs an arbitrary decimal expansion from the given long.
    //        /// The long must not be negative.
    //        /// </summary>
    //        internal ArbitraryDecimal (long x)
    //        {
    //            string tmp = x.ToString(CultureInfo.InvariantCulture);
    //            digits = new byte[tmp.Length];
    //            for (int i=0; i < tmp.Length; i++)
    //                digits[i] = (byte) (tmp[i]-'0');
    //            Normalize();
    //        }
        
    //        /// <summary>
    //        /// Multiplies the current expansion by the given amount, which should
    //        /// only be 2 or 5.
    //        /// </summary>
    //        internal void MultiplyBy(int amount)
    //        {
    //            byte[] result = new byte[digits.Length+1];
    //            for (int i=digits.Length-1; i >= 0; i--)
    //            {
    //                int resultDigit = digits[i]*amount+result[i+1];
    //                result[i]=(byte)(resultDigit/10);
    //                result[i+1]=(byte)(resultDigit%10);
    //            }
    //            if (result[0] != 0)
    //            {
    //                digits=result;
    //            }
    //            else
    //            {
    //                Array.Copy (result, 1, digits, 0, digits.Length);
    //            }
    //            Normalize();
    //        }
        
    //        /// <summary>
    //        /// Shifts the decimal point; a negative value makes
    //        /// the decimal expansion bigger (as fewer digits come after the
    //        /// decimal place) and a positive value makes the decimal
    //        /// expansion smaller.
    //        /// </summary>
    //        internal void Shift (int amount)
    //        {
    //            decimalPoint += amount;
    //        }

    //        /// <summary>
    //        /// Removes leading/trailing zeroes from the expansion.
    //        /// </summary>
    //        internal void Normalize()
    //        {
    //            int first;
    //            for (first=0; first < digits.Length; first++)
    //                if (digits[first]!=0)
    //                    break;
    //            int last;
    //            for (last=digits.Length-1; last >= 0; last--)
    //                if (digits[last]!=0)
    //                    break;
            
    //            if (first==0 && last==digits.Length-1)
    //                return;
            
    //            byte[] tmp = new byte[last-first+1];
    //            for (int i=0; i < tmp.Length; i++)
    //                tmp[i]=digits[i+first];
            
    //            decimalPoint -= digits.Length-(last+1);
    //            digits=tmp;
    //        }

    //        /// <summary>
    //        /// Converts the value to a proper decimal string representation.
    //        /// </summary>
    //        public override String ToString()
    //        {
    //            char[] digitString = new char[digits.Length];            
    //            for (int i=0; i < digits.Length; i++)
    //                digitString[i] = (char)(digits[i]+'0');
            
    //            // Simplest case - nothing after the decimal point,
    //            // and last real digit is non-zero, eg value=35
    //            if (decimalPoint==0)
    //            {
    //                return new string (digitString);
    //            }
            
    //            // Fairly simple case - nothing after the decimal
    //            // point, but some 0s to add, eg value=350
    //            if (decimalPoint < 0)
    //            {
    //                return new string (digitString)+
    //                    new string ('0', -decimalPoint);
    //            }
            
    //            // Nothing before the decimal point, eg 0.035
    //            if (decimalPoint >= digitString.Length)
    //            {
    //                return "0."+
    //                    new string ('0',(decimalPoint-digitString.Length))+
    //                    new string (digitString);
    //            }

    //            // Most complicated case - part of the string comes
    //            // before the decimal point, part comes after it,
    //            // eg 3.5
    //            return new string (digitString, 0, 
    //                digitString.Length-decimalPoint)+
    //                "."+
    //                new string (digitString,
    //                digitString.Length-decimalPoint, 
    //                decimalPoint);
    //        }
    //    }

//        #region Unit Testing Suite
//#if DEBUG

//        /// <summary>Testing suite for the <see cref="Double"/> class.</summary>
//        [TestFixture]
//        public class TestingSuite
//        {
//            private static Random random = new Random();

//            /// <summary>
//            /// Testing the method <see cref="Double.ToExactString"/>
//            /// </summary>
//            [Test] public void ToExactString()
//            {
//                for(int i = 0; i < 100; i++)
//                {
//                    double c = 2 * random.NextDouble() - 1;
//                    string s = Double.ToExactString(c);

//                    Assertion.AssertEquals("#A00 Unexpected parse result.", c, double.Parse(s));
//                }
//            }
//        }
//#endif
//        #endregion
//	}
}
