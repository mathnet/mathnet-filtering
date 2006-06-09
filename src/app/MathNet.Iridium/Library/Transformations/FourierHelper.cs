#region MathNet Numerics, Copyright ©2004 Christoph Ruegg, Ben Houston 

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Christoph Ruegg, http://www.cdrnet.net,
// Based on Exocortex.DSP, Copyright Ben Houston, http://www.exocortex.org
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

namespace MathNet.Numerics.Transformations
{
	/// <summary>
	/// Helper class for fourier transformations.
	/// </summary>
	public sealed class FourierHelper
	{
		private const int maxLength = 4096;
		private const int minLength = 1;
		private const int maxBits = 12;
		private const int minBits = 0;

		private FourierHelper() {}

		#region Base 2 Exponential Functions
		/// <summary>
		/// Raises 2 to the provided integer exponent.
		/// </summary>
		public static int Pow2(int exponent) 
		{
			if(exponent>=0 && exponent<31) 
				return 1 << exponent;
			return 0;
		}
		/// <summary>
		/// Evaluates the logarithm to base 2 of the provided integer value.
		/// </summary>
		public static int Log2(int x) 
		{
			if(x<=65536) 
			{
				if(x<=256) 
				{
					if(x<=16) 
					{
						if(x<=4) 
						{	
							if(x<=2) 
							{
								if(x<=1) 
									return 0;
								return 1;	
							}
							return 2;				
						}
						if(x<=8)
							return 3;			
						return 4;				
					}
					if(x<=64) 
					{
						if(x<=32)
							return 5;	
						return 6;				
					}
					if(x<=128)
						return 7;		
					return 8;				
				}
				if(x<=4096) 
				{	
					if(x<=1024) 
					{	
						if(x<=512)
							return 9;			
						return 10;				
					}
					if(x<=2048)
						return 11;			
					return 12;				
				}
				if(x<=16384) 
				{
					if(x<=8192)
						return 13;			
					return 14;				
				}
				if(x<=32768)
					return 15;	
				return	16;	
			}
			if(x<=16777216) 
			{
				if(x<=1048576) 
				{
					if(x<=262144) 
					{	
						if(x<=131072)
							return 17;			
						return 18;				
					}
					if(x<=524288)
						return 19;			
					return 20;				
				}
				if(x<=4194304) 
				{
					if(x<=2097152)
						return 21;	
					return 22;				
				}
				if(x<=8388608)
					return 23;		
				return 24;				
			}
			if(x<=268435456) 
			{	
				if(x<=67108864) 
				{	
					if(x<=33554432)
						return 25;			
					return 26;				
				}
				if(x<=134217728)
					return 27;			
				return 28;				
			}
			if(x<=1073741824) 
			{
				if(x<=536870912)
					return 29;			
				return 30;				
			}
			//	since int is unsigned it can never be higher than 2,147,483,647
			//	if( x <= 2147483648 )
			//		return	31;	
			//	return	32;	
			return 31;
		}
		#endregion

		#region Reverse Bits
		static private int[][] reversedBitsLookup = new int[maxBits][];
		/// <summary>
		/// Permutates <c>numberOfBits</c> in ascending order
		/// and reverses each element's bits afterwards.
		/// </summary>
		static public int[] ReverseBits(int numberOfBits) 
		{
			System.Diagnostics.Debug.Assert(numberOfBits >= minBits);
			System.Diagnostics.Debug.Assert(numberOfBits <= maxBits);
			if(reversedBitsLookup[numberOfBits-1] == null) 
			{
				int len = Pow2(numberOfBits);
				int[] reversedBits = new int[len];
				for(int i=0;i<len;i++) 
				{
					int oldBits = i;
					int newBits = 0;
					for(int j=0;j<numberOfBits;j++) 
					{
						newBits = (newBits<<1) | (oldBits&1);
						oldBits = (oldBits>>1);
					}
					reversedBits[i] = newBits;
				}
				reversedBitsLookup[numberOfBits-1] = reversedBits;
			}
			return reversedBitsLookup[numberOfBits-1];
		}
		#endregion

		#region Real/Imag Coefficients (Complex Rotation)
		static private double[,][] realCoefficients = new double[maxBits+1,2][];
		static private double[,][] imagCoefficients = new double[maxBits+1,2][];
		/// <summary>
		/// Evaluates complex rotation coefficients if not already available
		/// and returns the (real) cosine lookup table.
		/// </summary>
		static public double[] RealCosineCoefficients(int level, bool forward)
		{
			if(realCoefficients[level,0] == null)
				BuildCoefficientsForLevels(level);
			return realCoefficients[level,forward ? 0 : 1];
		}
		/// <summary>
		/// Evaluates complex rotation coefficients if not already available
		/// and returns the (imaginary) sine lookup table.
		/// </summary>
		static public double[] ImaginarySineCoefficients(int level, bool forward)
		{
			if(imagCoefficients[level,0] == null)
				BuildCoefficientsForLevels(level);
			return imagCoefficients[level,forward ? 0 : 1];
		}
		/// <summary>
		/// Evaluates complex rotation coefficients if not already available.
		/// </summary>
		static public void BuildCoefficientsForLevels(int levels)
		{
			int N = 1;
			double uR, uI, angle, wR, wI, uwI;
			for(int level=1;level<=levels;level++) 
			{
				int M = N;
				N <<= 1;

				if(realCoefficients[level,0] != null)
					continue;

                // Forward Coefficients
				uR = 1;
				uI = 0;
				angle = Math.PI/M;
				wR = Math.Cos(angle);
				wI = Math.Sin(angle);

				realCoefficients[level,0] = new double[M];
				imagCoefficients[level,0] = new double[M];

				for(int j=0;j<M;j++) 
				{
					realCoefficients[level,0][j] = uR;
					imagCoefficients[level,0][j] = uI;
					uwI = uR*wI + uI*wR;
					uR = uR*wR - uI*wI;
					uI = uwI;
				}

				// Backward Coefficients
				uR = 1;
				uI = 0;
				angle = -Math.PI/M;
				wR = Math.Cos(angle);
				wI = Math.Sin(angle);

				realCoefficients[level,1] = new double[M];
				imagCoefficients[level,1] = new double[M];

				for(int j=0;j<M;j++) 
				{
					realCoefficients[level,1][j] = uR;
					imagCoefficients[level,1][j] = uI;
					uwI = uR*wI + uI*wR;
					uR = uR*wR - uI*wI;
					uI = uwI;
				}
			}
		}
		#endregion
	}
}
