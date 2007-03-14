#region MathNet Numerics, Copyright ©2004 Christoph Ruegg 

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Christoph Ruegg, http://www.cdrnet.net
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
using System.Collections;
using MathNet.Numerics;

namespace MathNet.Numerics.Interpolation
{
	public enum InterpolationMode : int
	{
		/// <summary>Polynomial Interpolation</summary>
		ExpectNoPoles = 1,
		/// <summary>Rational Interpolation</summary>
		ExpectPoles = 2,
		/// <summary>Cubic Spline Interpolation</summary>
		Smooth = 8
	}

	public class InterpolationSingleDimension
	{
		private SampleList samples;
		private IInterpolationAlgorithm algorithm;
		private bool dirty = true; //delay preparation until first evaluation

		#region Construction
		public InterpolationSingleDimension(SampleList samples)
		{
			this.samples = samples;
			this.samples.SampleAltered += samples_SampleAltered;
			this.algorithm = new PolynomialInterpolationAlgorithm(4);
		}
		public InterpolationSingleDimension(IDictionary samples)
		{
			this.samples = new SampleList(samples);
			this.samples.SampleAltered += samples_SampleAltered;
			this.algorithm = new PolynomialInterpolationAlgorithm(4);
		}
		public InterpolationSingleDimension(SampleList samples, InterpolationMode mode)
		{
			this.samples = samples;
			this.samples.SampleAltered += samples_SampleAltered;
			this.algorithm = SelectAlgorithm(mode,4);
		}
		public InterpolationSingleDimension(SampleList samples, InterpolationMode mode, int order)
		{
			this.samples = samples;
			this.samples.SampleAltered += samples_SampleAltered;
			this.algorithm = SelectAlgorithm(mode,order);
		}
		public InterpolationSingleDimension(SampleList samples, IInterpolationAlgorithm algorithm)
		{
			this.samples = samples;
			this.samples.SampleAltered += samples_SampleAltered;
			this.algorithm = algorithm;
		}

		protected virtual IInterpolationAlgorithm SelectAlgorithm(InterpolationMode mode, int order)
		{
			switch(mode)
			{
				case InterpolationMode.ExpectNoPoles:
					return new PolynomialInterpolationAlgorithm(order);
				case InterpolationMode.ExpectPoles:
					return new RationalInterpolationAlgorithm(order);
                //case InterpolationMode.Smooth:
                //    return new CubicSplineInterpolationAlgorithm(order);
				default:
					return new PolynomialInterpolationAlgorithm(order);
			}
		}
		#endregion

		public double Evaluate(double t)
		{
			if(dirty)
			{
				algorithm.Prepare(samples);
				dirty = false;
			}
			if(samples.MinT <= t && t <= samples.MaxT)
				return algorithm.Interpolate(t);
			else
				return algorithm.Extrapolate(t);
		}

		public double Evaluate(double t, out double errorEstimation)
		{
			if(dirty)
			{
				algorithm.Prepare(samples);
				dirty = false;
			}
			return algorithm.Interpolate(t, out errorEstimation);
		}

		public bool SupportErrorEstimation
		{
			get {return algorithm.SupportErrorEstimation;}
		}

		private void samples_SampleAltered(object sender, SampleList.SampleAlteredEventArgs e)
		{
			dirty = true; //require new preparation
		}
	}
}
