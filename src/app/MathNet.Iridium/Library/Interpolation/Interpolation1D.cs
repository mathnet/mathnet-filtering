#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
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
    /// <summary>
    /// Interpolation Characteristics
    /// </summary>
    public enum InterpolationMode : int
    {
        /// <summary>Polynomial Interpolation</summary>
        ExpectNoPoles = 1,
        /// <summary>Rational Interpolation</summary>
        ExpectPoles = 2,
        /// <summary>Cubic Spline Interpolation</summary>
        Smooth = 8
    }

    /// <summary>
    /// Interpolation portal for the single dimension case.
    /// </summary>
    public class InterpolationSingleDimension
    {
        private SampleList samples;
        private IInterpolationAlgorithm algorithm;
        private bool dirty = true; //delay preparation until first evaluation

        #region Construction
        /// <summary>
        /// Initialize the portal with samples from a sample list.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="PolynomialInterpolationAlgorithm"/>.
        /// </remarks>
        public InterpolationSingleDimension(SampleList samples)
        {
            this.samples = samples;
            this.samples.SampleAltered += samples_SampleAltered;
            this.algorithm = new PolynomialInterpolationAlgorithm(4);
        }
        /// <summary>
        /// Initialize the portal with samples from a dictionary.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="PolynomialInterpolationAlgorithm"/>.
        /// </remarks>
        public InterpolationSingleDimension(IDictionary samples)
        {
            this.samples = new SampleList(samples);
            this.samples.SampleAltered += samples_SampleAltered;
            this.algorithm = new PolynomialInterpolationAlgorithm(4);
        }
        /// <summary>
        /// Initialize the portal with samples from a sample list and selects an algorithm that fits the chosen interpolation mode.
        /// </summary>
        public InterpolationSingleDimension(SampleList samples, InterpolationMode mode)
        {
            this.samples = samples;
            this.samples.SampleAltered += samples_SampleAltered;
            this.algorithm = SelectAlgorithm(mode, 4);
        }
        /// <summary>
        /// Initialize the portal with samples from a sample list and selects an algorithm that fits the chosen interpolation mode with the given order.
        /// </summary>
        public InterpolationSingleDimension(SampleList samples, InterpolationMode mode, int order)
        {
            this.samples = samples;
            this.samples.SampleAltered += samples_SampleAltered;
            this.algorithm = SelectAlgorithm(mode, order);
        }
        /// <summary>
        /// Initialize the portal with samples from a sample list and uses the specified algorithm.
        /// </summary>
        public InterpolationSingleDimension(SampleList samples, IInterpolationAlgorithm algorithm)
        {
            this.samples = samples;
            this.samples.SampleAltered += samples_SampleAltered;
            this.algorithm = algorithm;
        }

        /// <summary>
        /// Override this method to select custom interpolation algorithms.
        /// </summary>
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
        /// <summary>
        /// Interpolate at point t.
        /// </summary>
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

        /// <summary>
        /// Interpolate at point t and return the estimated error as a parameter.
        /// </summary>
        public double Evaluate(double t, out double errorEstimation)
        {
            if(dirty)
            {
                algorithm.Prepare(samples);
                dirty = false;
            }
            return algorithm.Interpolate(t, out errorEstimation);
        }

        /// <summary>
        /// True if the selected algorithm supports error estimation.
        /// </summary>
        public bool SupportErrorEstimation
        {
            get { return algorithm.SupportErrorEstimation; }
        }

        private void samples_SampleAltered(object sender, SampleList.SampleAlteredEventArgs e)
        {
            dirty = true; //require new preparation
        }
    }
}
