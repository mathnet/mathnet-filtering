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
    [Obsolete]
    public enum InterpolationMode : int
    {
        /// <summary>
        /// Polynomial Interpolation
        /// </summary>
        ExpectNoPoles = 1,

        /// <summary>
        /// Rational Interpolation
        /// </summary>
        ExpectPoles = 2,

        /// <summary>
        /// Cubic Spline Interpolation
        /// </summary>
        Smooth = 8
    }

    /// <summary>
    /// Interpolation portal for the single dimension case.
    /// </summary>
    [Obsolete("Please use Interpolation instead. This class is obsolete and will be removed in future versions.")]
    public class InterpolationSingleDimension
    {
        SampleList _samples;
        IInterpolationAlgorithm _algorithm;
        bool _dirty = true; // delay preparation until first evaluation

        #region Construction

        /// <summary>
        /// Initialize the portal with samples from a sample list.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="PolynomialInterpolationAlgorithm"/>.
        /// </remarks>
        /// <param name="samples">Sample Points.</param>
        public
        InterpolationSingleDimension(
            SampleList samples
            )
        {
            _samples = samples;
            _samples.SampleAltered += samples_SampleAltered;
            _algorithm = new PolynomialInterpolationAlgorithm(); 
        }

        /// <summary>
        /// Initialize the portal with samples from points (t, x(t)).
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="PolynomialInterpolationAlgorithm"/>.
        /// </remarks>
        /// <param name="t">keys t, where x=f(t) or (t,x).</param>
        /// <param name="x">values x, where x=f(t) or (t,x).</param>
        public
        InterpolationSingleDimension(
            double[] t,
            double[] x
            )
        {
            _samples = new SampleList(t, x);
            _algorithm = new PolynomialInterpolationAlgorithm();
        }

        /// <summary>
        /// Initialize the portal with samples from a dictionary.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="PolynomialInterpolationAlgorithm"/>.
        /// </remarks>
        /// <param name="samples">Sample Points.</param>
        public
        InterpolationSingleDimension(
            IDictionary samples
            )
        {
            _samples = new SampleList(samples);
            _algorithm = new PolynomialInterpolationAlgorithm();
        }


        /// <summary>
        /// Initialize the portal with samples from a sample list and selects an algorithm that fits the chosen interpolation mode.
        /// </summary>
        /// <param name="samples">Sample Points.</param>
        /// <param name="mode">Interpolation Mode.</param>
        public
        InterpolationSingleDimension(
            SampleList samples,
            InterpolationMode mode
            )
        {
            _samples = samples;
            _samples.SampleAltered += samples_SampleAltered;
            _algorithm = SelectAlgorithm(mode);
        }

        /// <summary>
        /// Initialize the portal with samples from points (t, x(t)) and selects an algorithm that fits the chosen interpolation mode.
        /// </summary>
        /// <param name="t">keys t, where x=f(t) or (t,x).</param>
        /// <param name="x">values x, where x=f(t) or (t,x).</param>
        /// <param name="mode">Interpolation Mode.</param>
        public
        InterpolationSingleDimension(
            double[] t,
            double[] x,
            InterpolationMode mode
            )
        {
            _samples = new SampleList(t, x);
            _algorithm = SelectAlgorithm(mode);
        }

        /// <summary>
        /// Initialize the portal with samples from a sample list and selects an algorithm that fits the chosen interpolation mode with the given order.
        /// </summary>
        /// <param name="samples">Sample Points.</param>
        /// <param name="mode">Interpolation Mode.</param>
        /// <param name="maximumOrder">Maximum Interpolation Order.</param>
        public
        InterpolationSingleDimension(
            SampleList samples,
            InterpolationMode mode,
            int maximumOrder
            )
        {
            _samples = samples;
            _samples.SampleAltered += samples_SampleAltered;
            _algorithm = SelectAlgorithm(mode);
            _algorithm.MaximumOrder = maximumOrder;
        }

        /// <summary>
        /// Initialize the portal with samples from points (t, x(t)) and selects an algorithm that fits the chosen interpolation mode with the given order.
        /// </summary>
        /// <param name="t">keys t, where x=f(t) or (t,x).</param>
        /// <param name="x">values x, where x=f(t) or (t,x).</param>
        /// <param name="mode">Interpolation Mode.</param>
        /// <param name="maximumOrder">Maximum Interpolation Order.</param>
        public
        InterpolationSingleDimension(
            double[] t,
            double[] x,
            InterpolationMode mode,
            int maximumOrder
            )
        {
            _samples = new SampleList(t, x);
            _algorithm = SelectAlgorithm(mode);
            _algorithm.MaximumOrder = maximumOrder;
        }

        /// <summary>
        /// Initialize the portal with samples from a sample list and uses the specified algorithm.
        /// </summary>
        /// <param name="samples">Sample Points.</param>
        /// <param name="algorithm">Interpolation Algorithm.</param>
        public
        InterpolationSingleDimension(
            SampleList samples,
            IInterpolationAlgorithm algorithm
            )
        {
            _samples = samples;
            _samples.SampleAltered += samples_SampleAltered;
            _algorithm = algorithm;
        }

        /// <summary>
        /// Initialize the portal with samples from points (t, x(t)) and uses the specified algorithm.
        /// </summary>
        /// <param name="t">keys t, where x=f(t) or (t,x).</param>
        /// <param name="x">values x, where x=f(t) or (t,x).</param>
        /// <param name="algorithm">Interpolation Algorithm.</param>
        public
        InterpolationSingleDimension(
            double[] t,
            double[] x,
            IInterpolationAlgorithm algorithm
            )
        {
            _samples = new SampleList(t, x);
            _algorithm = algorithm;
        }

        /// <summary>
        /// Override this method to select custom interpolation algorithms.
        /// </summary>
        /// <param name="mode">Interpolation Mode.</param>
        protected virtual
        IInterpolationAlgorithm
        SelectAlgorithm(
            InterpolationMode mode
            )
        {
            switch(mode)
            {
                case InterpolationMode.ExpectNoPoles:
                    return new PolynomialInterpolationAlgorithm();
                case InterpolationMode.ExpectPoles:
                    return new RationalInterpolationAlgorithm();
                case InterpolationMode.Smooth:
                    throw new NotImplementedException();
                default:
                    return new PolynomialInterpolationAlgorithm();
            }
        }

        #endregion

        /// <summary>
        /// Interpolate at point t.
        /// </summary>
        public
        double
        Evaluate(
            double t
            )
        {
            if(_dirty)
            {
                _algorithm.Prepare(_samples);
                _dirty = false;
            }

            if(_samples.MinT <= t && t <= _samples.MaxT)
            {
                return _algorithm.Interpolate(t);
            }

            return _algorithm.Extrapolate(t);
        }

        /// <summary>
        /// Interpolate at point t and return the estimated error as a parameter.
        /// </summary>
        public
        double
        Evaluate(
            double t,
            out double errorEstimation
            )
        {
            if(_dirty)
            {
                _algorithm.Prepare(_samples);
                _dirty = false;
            }

            return _algorithm.Interpolate(t, out errorEstimation);
        }

        /// <summary>
        /// True if the selected algorithm supports error estimation.
        /// </summary>
        public bool SupportErrorEstimation
        {
            get { return _algorithm.SupportErrorEstimation; }
        }

        void
        samples_SampleAltered(
            object sender,
            SampleList.SampleAlteredEventArgs e
            )
        {
            _dirty = true; // require new preparation
        }
    }
}
