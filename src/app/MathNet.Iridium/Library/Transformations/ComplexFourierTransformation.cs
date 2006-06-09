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
	/// The <c>ComplexFourierTransformation</c> provides algorithms
	/// for one, two and three dimensional fast fourier transformations
	/// (FFT) on complex vectors.
	/// </summary>
	public class ComplexFourierTransformation : ComplexTransformation
	{
		/// <summary>
		/// Creates a real-value based fast fourier transformation instance
		/// with the data provided
		/// </summary>
		public ComplexFourierTransformation(Complex[] data) : base(data) {}

		/// <summary>
		/// The core transformation implementation for one dimension.
		/// </summary>
		/// <param name="forward">Indicates the transformation direction.</param>
		protected override void TransformCore(bool forward)
		{
			int length = viewData.Length;
			int ln = FourierHelper.Log2(length);

			ReorderData();
			
			// successive doubling
			int N = 1;
			for(int level=1;level<=ln;level++) 
			{
				int M = N;
				N <<= 1;

				double[] realCosine = FourierHelper.RealCosineCoefficients(level,forward);
				double[] imagSine = FourierHelper.ImaginarySineCoefficients(level,forward); 

				for(int j=0;j<M;j++) 
				{
					double uR = realCosine[j];
					double uI = imagSine[j];

					for(int even=j;even<length;even+=N) 
					{
						int odd	 = even + M;
						
						double	r = viewData[odd].Real;
						double	i = viewData[odd].Imag;

						double	odduR = r * uR - i * uI;
						double	odduI = r * uI + i * uR;

						r = viewData[even].Real;
						i = viewData[even].Imag;
						
						viewData[even].Real	= r+odduR;
						viewData[even].Imag = i+odduI;

						viewData[odd].Real = r-odduR;
						viewData[odd].Imag = i-odduI;
					}
				}
			}
		}

		private void ReorderData() 
		{
			int length = viewData.Length;
            int[] reversedBits = FourierHelper.ReverseBits(FourierHelper.Log2(length));
			for(int i=0;i<length;i++) 
			{
				int swap = reversedBits[i];
				if(swap > i)
					SwapViewData(i,swap);
			}
		}
	}
}
