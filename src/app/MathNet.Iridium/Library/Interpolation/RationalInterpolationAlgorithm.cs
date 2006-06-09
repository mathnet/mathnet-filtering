#region MathNet Numerics, Copyright ©2004 Christoph Ruegg 

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Christoph Ruegg, http://www.cdrnet.net
// Partially based on ideas from Numerical Recipes in C++, Second Edition [2003],
// as well as Handbook of Mathematical Functions [1965].
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
using MathNet.Numerics;

namespace MathNet.Numerics.Interpolation
{
	/// <summary>
	/// Lagrange Rational Interpolation using Bulirsch & Stoer's Algorithm.
	/// </summary>
	public class RationalInterpolationAlgorithm : IInterpolationAlgorithm
	{
		private SampleList samples;
		private int order;
		private int effectiveOrder;

		public RationalInterpolationAlgorithm(int order)
		{
			this.order = order;
			this.effectiveOrder = order;
		}

		public void Prepare(SampleList samples)
		{
			this.samples = samples;
			this.effectiveOrder = Math.Min(order,samples.Count);
		}

		public double Interpolate(double t)
		{
			double error;
			return Interpolate(t, out error);
		}

		public double Interpolate(double t, out double error)
		{
			if(samples == null)
				throw new InvalidOperationException("No Samples Provided. Preparation Required.");

			const double tiny = 1.0e-15;
			int closestIndex;
			int offset = SuggestOffset(t,out closestIndex);
			double[] c = new double[effectiveOrder];
			double[] d = new double[effectiveOrder];
			int ns = closestIndex-offset;
			double den,ho,hp;
			double x = 0;
			error = 0;

			if(samples.GetT(closestIndex) == t)
				return samples.GetX(closestIndex);

			for(int i=0;i<effectiveOrder;i++)
			{
				c[i] = samples.GetX(offset+i);
				d[i] = c[i] + tiny; //prevent rare zero-over-zero condition
			}

			x = samples.GetX(offset+ns--);
			for(int level=1;level<effectiveOrder;level++)
			{
				for(int i=0;i<effectiveOrder-level;i++)
				{
					hp = samples.GetT(offset+i+level)-t;
					ho = (samples.GetT(offset+i)-t)*d[i]/hp;
					den = ho-c[i+1];
					if(den == 0)
					{
#warning: Check sign (positive or negative infinity?)
						error = 0;
						return double.PositiveInfinity; //or is it NegativeInfinity?
					}
					den = (c[i+1]-d[i])/den;
					d[i] = c[i+1]*den;
					c[i] = ho*den;
				}
				error = (2*(ns+1) < (effectiveOrder-level) ? c[ns+1] : d[ns--]);
				x += error;
			}

			return x;
		}

		private int SuggestOffset(double t, out int closestIndex)
		{
			closestIndex = Math.Max(samples.Locate(t),0);
			int ret = Math.Min(Math.Max(closestIndex-(effectiveOrder-1)/2,0),samples.Count-effectiveOrder);
			if(Math.Abs(t-samples.GetT(closestIndex)) > Math.Abs(t-samples.GetT(closestIndex+1)))
				closestIndex++;
			return ret;
		}

		public double Extrapolate(double t)
		{
			return Interpolate(t);
		}

		public bool SupportErrorEstimation
		{
			get {return true;}
		}
	}
}
