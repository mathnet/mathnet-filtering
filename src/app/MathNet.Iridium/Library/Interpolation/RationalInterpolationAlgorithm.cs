#region Math.NET Iridium (LGPL) by Ruegg + Contributors
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
//
// Contribution: Numerical Recipes in C++, Second Edition [2003]
//               Handbook of Mathematical Functions [1965]
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
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.Interpolation
{
    /// <summary>
    /// Lagrange Rational Interpolation using Bulirsch &amp; Stoer's Algorithm.
    /// </summary>
    [Obsolete("Please use Interpolation or directly one of the newer implementation in the Algorithms namespace instead. The direct replacement is LimitedOrderRationalInterpolation. This class is obsolete and will be removed in future versions.")]
    public class RationalInterpolationAlgorithm : IInterpolationAlgorithm
    {
        SampleList _samples;
        int _maximumOrder;
        int _effectiveOrder;

        /// <summary>
        /// Create a rational interpolation algorithm with full order.
        /// </summary>
        public
        RationalInterpolationAlgorithm()
        {
            _maximumOrder = int.MaxValue;
            _effectiveOrder = -1;
        }

        /// <summary>
        /// Create a rational interpolation algorithm with the given order.
        /// </summary>
        public
        RationalInterpolationAlgorithm(
            int maximumOrder
            )
        {
            _maximumOrder = maximumOrder;
            _effectiveOrder = -1;
        }

        /// <summary>
        /// Precompute/optimize the algoritm for the given sample set.
        /// </summary>
        public
        void
        Prepare(
            SampleList samples
            )
        {
            if(null == samples)
            {
                throw new ArgumentNullException("samples");
            }

            _samples = samples;
            _effectiveOrder = Math.Min(_maximumOrder, samples.Count);
        }

        /// <summary>
        /// The maxium interpolation order.
        /// </summary>
        /// <seealso cref="EffectiveOrder"/>
        public int MaximumOrder
        {
            get
            {
                return _maximumOrder;
            }

            set
            {
                if(value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                if(_maximumOrder == value)
                {
                    return;
                }

                if(null == _samples)
                {
                    _maximumOrder = value;
                    _effectiveOrder = -1;
                    return;
                }

                _maximumOrder = value;
                _effectiveOrder = Math.Min(value, _samples.Count);
            }
        }

        /// <summary>
        /// The interpolation order that is effectively used.
        /// </summary>
        /// <seealso cref="MaximumOrder"/>
        public int EffectiveOrder
        {
            get { return _effectiveOrder; }
        }

        /// <summary>
        /// Interpolate at point t.
        /// </summary>
        public
        double
        Interpolate(
            double t
            )
        {
            double error;
            return Interpolate(t, out error);
        }

        /// <summary>
        /// Interpolate at point t and return the estimated error as error-parameter.
        /// </summary>
        public
        double
        Interpolate(
            double t,
            out double error
            )
        {
            if(null == _samples)
            {
                throw new InvalidOperationException(Resources.InvalidOperationNoSamplesProvided);
            }

            const double tiny = 1.0e-15;
            int closestIndex;
            int offset = SuggestOffset(t, out closestIndex);
            double[] c = new double[_effectiveOrder];
            double[] d = new double[_effectiveOrder];
            int ns = closestIndex - offset;
            double den, ho, hp;
            double x = 0;
            error = 0;

            if(_samples.GetT(closestIndex) == t)
            {
                return _samples.GetX(closestIndex);
            }

            for(int i = 0; i < _effectiveOrder; i++)
            {
                c[i] = _samples.GetX(offset + i);
                d[i] = c[i] + tiny; // prevent rare zero-over-zero condition
            }

            x = _samples.GetX(offset + ns--);
            for(int level = 1; level < _effectiveOrder; level++)
            {
                for(int i = 0; i < _effectiveOrder - level; i++)
                {
                    hp = _samples.GetT(offset + i + level) - t;
                    ho = (_samples.GetT(offset + i) - t) * d[i] / hp;

                    den = ho - c[i + 1];
                    if(den == 0)
                    {
                        // TODO (cdr, 2006-06-09): Check sign (positive or negative infinity?)
                        error = 0;
                        return double.PositiveInfinity; // or is it NegativeInfinity?
                    }

                    den = (c[i + 1] - d[i]) / den;
                    d[i] = c[i + 1] * den;
                    c[i] = ho * den;
                }

                error = (2 * (ns + 1) < (_effectiveOrder - level) ? c[ns + 1] : d[ns--]);
                x += error;
            }

            return x;
        }

        int
        SuggestOffset(
            double t,
            out int closestIndex
            )
        {
            closestIndex = Math.Max(_samples.Locate(t), 0);
            int ret = Math.Min(
                Math.Max(
                    closestIndex - (_effectiveOrder - 1) / 2,
                    0
                    ),
                _samples.Count - _effectiveOrder
                );

            if(closestIndex < (_samples.Count - 1))
            {
                double dist1 = Math.Abs(t - _samples.GetT(closestIndex));
                double dist2 = Math.Abs(t - _samples.GetT(closestIndex + 1));

                if(dist1 > dist2)
                {
                    closestIndex++;
                }
            }

            return ret;
        }

        /// <summary>
        /// Extrapolate at point t.
        /// </summary>
        public
        double
        Extrapolate(
            double t
            )
        {
            return Interpolate(t);
        }

        /// <summary>
        /// True if the alorithm supports error estimation.
        /// </summary>
        /// <remarks>
        /// Always true for this algorithm.
        /// </remarks>
        public bool SupportErrorEstimation
        {
            get { return true; }
        }
    }
}
