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
	/// The <c>RealFourierTransformation</c> provides algorithms
	/// for one, two and three dimensional fast fourier transformations
	/// (FFT) on real vectors.
	/// </summary>
	public class RealFourierTransformation : RealTransformation
	{
		/// <summary>
		/// Creates a real-value based fast fourier transformation instance
		/// with the data provided
		/// </summary>
		public RealFourierTransformation(double[] data) : base(data) {}

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

					for(int evenT=j;evenT<length;evenT+=N) 
					{
						int even = evenT << 1;
						int odd	 = (evenT + M) << 1;
						
						double	r = viewData[odd];
						double	i = viewData[odd+1];

						double	odduR = r * uR - i * uI;
						double	odduI = r * uI + i * uR;

						r = viewData[even];
						i = viewData[even+1];
						
						viewData[even] = r+odduR;
						viewData[even+1] = i+odduI;

						viewData[odd] = r-odduR;
						viewData[odd+1] = i-odduI;
					}
				}
			}
		}

		private void ReorderData() 
		{
			int length = viewData.Length>>1;
			int[] reversedBits = FourierHelper.ReverseBits(FourierHelper.Log2(length));
			for(int i=0;i<length;i++) 
			{
				int swap = reversedBits[i];
				if(swap > i) 
				{
					SwapViewData(i<<1,swap<<1);
					SwapViewData((i<<1)+1,(swap<<1)+1);
				}
			}
		}
	}
}
