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
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Iridium.Test
{
    public static class NumericAssert
    {
        public static void AreAlmostEqual(Matrix expected, Matrix actual, string message)
        {
            Assert.DoAssert(new AlmostEqualAsserter(expected.Norm1(), actual.Norm1(), (expected-actual).Norm1(), 10 * Number.DefaultRelativeAccuracy, message));
        }

        public static void AreAlmostEqual(double expected, double actual, double relativeAccuracy, string message)
        {
            Assert.DoAssert(new AlmostEqualAsserter(expected, actual, relativeAccuracy, message));
        }

        public static void AreAlmostEqual(double expected, double actual, string message)
        {
            Assert.DoAssert(new AlmostEqualAsserter(expected, actual, message));
        }
    }

    public class AlmostEqualAsserter : ComparisonAsserter
    {
        double _relativeAccuracy;
        double _difference;

        public AlmostEqualAsserter(double expected, double actual, double difference, double relativeAccuracy, string message, params object[] args)
            : base(expected, actual, message, args)
        {
            _relativeAccuracy = relativeAccuracy;
            _difference = difference;
        }

        public AlmostEqualAsserter(double expected, double actual, double relativeAccuracy, string message, params object[] args)
            : this(expected, actual, expected - actual, relativeAccuracy, message, args)
        {
        }

        public AlmostEqualAsserter(double expected, double actual, string message, params object[] args)
            : this(expected, actual, expected - actual, Number.DefaultRelativeAccuracy, message, args)
        {
        }

        public override bool Test()
        {
            double actual = (double)base.actual;
            double expected = (double)base.expected;

            if(!Number.AlmostEqualNorm(actual, expected, _difference, _relativeAccuracy))
            {
                FailureMessage.DisplayDifferences(expected, actual, false);
                return false;
            }

            return true;
        }
    }
}
