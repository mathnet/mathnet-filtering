//-----------------------------------------------------------------------
// <copyright file="LocalStrings.cs" company="Math.NET Project">
//    Copyright (c) 2002-2008, Christoph Rüegg.
//    All Right Reserved.
// </copyright>
// <author>
//    Christoph Rüegg, http://christoph.ruegg.name
// </author>
// <product>
//    Math.NET Iridium, part of the Math.NET Project.
//    http://mathnet.opensourcedotnet.info
// </product>
// <license type="opensource" name="LGPL" version="2 or later">
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation; either version 2 of the License, or
//    any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// </license>
//-----------------------------------------------------------------------

using System;

namespace MathNet.Numerics.Properties
{
    /*
    Note (ruegg, 2008-11-11): The best practice for internationalization is to use the
    ResourceManager class with Resources.resx files to automatically build resource satellite assemblies.
    On the other hand, for Mono it is recommended to use Mono.Unix with the Catalog class
    and *.po files. Since we want to cross-compile on both Microsoft.NET and Mono and because
    the only text we deal about is exception messages, I decided to no longer support either
    of those two approaches and only provide english exception messages from now on
    (translated exception messages are IMO a bad idea anyway).
    */

    internal static class LocalStrings
    {
        internal static string
        ArgumentHistogramContainsNot(double value)
        {
            return string.Format("The histogram does not contains the value {0}.", value);
        }

        internal static string
        ArgumentInIntervalXYInclusive(double left, double right)
        {
            return string.Format("Value is expected to be between {0} and {1} (including {0} and {1}).", left, right);
        }

        internal static string
        ArgumentMatrixIndexOutOfRange
        {
            get { return "The matrix indices must not be out of range of the given matrix."; }
        }

        internal static string
        ArgumentMatrixNotRankDeficient
        {
            get { return "Matrix must not be rank deficient."; }
        }

        internal static string
        ArgumentMatrixNotSingular
        {
            get { return "Matrix must not be singular."; }
        }

        internal static string
        ArgumentMatrixSameColumnDimension
        {
            get { return "Matrix column dimensions must agree."; }
        }

        internal static string
        ArgumentMatrixSameDimensions
        {
            get { return "Matrix dimensions must agree."; }
        }

        internal static string
        ArgumentMatrixSameRowDimension
        {
            get { return "Matrix row dimensions must agree."; }
        }

        internal static string
        ArgumentMatrixSingleColumn
        {
            get { return "Matrix must have exactly one column."; }
        }

        internal static string
        ArgumentMatrixSingleColumnRow
        {
            get { return "Matrix must have exactly one column and row, thus have only one cell."; }
        }

        internal static string
        ArgumentMatrixSingleRow
        {
            get { return "Matrix must have exactly one row."; }
        }

        internal static string
        ArgumentMatrixSquare
        {
            get { return "Matrix must be square."; }
        }

        internal static string
        ArgumentMatrixSymmetric
        {
            get { return "Matrix must be symmetric."; }
        }

        internal static string
        ArgumentMatrixSymmetricPositiveDefinite
        {
            get { return "Matrix must be symmetric positive definite."; }
        }

        internal static string
        ArgumentNotInfinityNaN
        {
            get { return "Value must neither be infinite nor NaN."; }
        }

        internal static string
        ArgumentNotNegative
        {
            get { return "Value must not be negative (zero is ok)."; }
        }

        internal static string
        ArgumentNull(string what)
        {
            return string.Format("{0} is a null reference (Nothing in Visual Basic).", what);
        }

        internal static string
        ArgumentOutOfRangeGreater(string what, double lowerBound)
        {
            return string.Format("{0} must be greater than {1}.", what, lowerBound);
        }

        internal static string
        ArgumentOutOfRangeGreaterEqual(string what, double lowerBound)
        {
            return string.Format("{0} must be greater than or equal to {1}.", what, lowerBound);
        }

        internal static string
        ArgumentOutOfRangeGreaterEqual(string what, string lowerBound)
        {
            return string.Format("{0} must be greater than or equal to {1}.", what, lowerBound);
        }

        internal static string
        ArgumentParameterSetInvalid
        {
            get { return "The chosen parameter set is invalid (probably some value is out of range)."; }
        }

        internal static string
        ArgumentParseComplexNumber
        {
            get { return "The given expression does not represent a complex number."; }
        }

        internal static string
        ArgumentPositive
        {
            get { return "Value must be positive (and not zero)."; }
        }

        internal static string
        ArgumentPowerOfTwo
        {
            get { return "Size must be a Power of Two."; }
        }

        internal static string
        ArgumentPowerOfTwoEveryDimension
        {
            get { return "Size must be a Power of Two in every dimension."; }
        }

        internal static string
        ArgumentEven
        {
            get { return "Value must be even."; }
        }

        internal static string
        ArgumentOdd
        {
            get { return "Value must be odd."; }
        }

        internal static string
        ArgumentRangeLessEqual(string left, string right, string range)
        {
            return string.Format("The range between {0} and {1} must be less than or equal to {2}.", left, right, range);
        }

        internal static string
        ArgumentSingleDimensionArray
        {
            get { return "Array must have exactly one dimension (and not be null)."; }
        }

        internal static string
        ArgumentTooLarge
        {
            get { return "Value is too large."; }
        }

        internal static string
        ArgumentTooLargeForIterationLimit
        {
            get { return "Value is too large for the current iteration limit."; }
        }

        internal static string
        ArgumentTypeMismatch
        {
            get { return "Type mismatch."; }
        }

        internal static string
        ArgumentVectorLengthsMultipleOf(string what)
        {
            return string.Format("Array length must be a multiple of {0}.", what);
        }

        internal static string
        ArgumentVectorsSameLengths
        {
            get { return "All vectors must have the same dimensionality."; }
        }

        internal static string
        ArgumentVectorThreeDimensional
        {
            get { return "The vector must have 3 dimensions."; }
        }

        internal static string
        ArgumentEnumerableNotEmpty
        {
            get { return "Enumerable must provide at least one item."; }
        }

        internal static string
        FeaturePlannedButNotImplementedYet
        {
            get { return "This feature is not implemented yet (but is planned)."; }
        }

        internal static string
        InvalidLeftBoundaryCondition
        {
            get { return "Invalid Left Boundary Condition."; }
        }

        internal static string
        InvalidOperationAccumulatorEmpty
        {
            get { return "The operation could not be performed because the accumulator is empty."; }
        }

        internal static string
        InvalidOperationHistogramEmpty
        {
            get { return "The operation could not be performed because the histogram is empty."; }
        }

        internal static string
        InvalidOperationHistogramNotEnoughPoints
        {
            get { return "Not enough points in the distribution."; }
        }

        internal static string
        InvalidOperationNoSamplesProvided
        {
            get { return "No Samples Provided. Preparation Required."; }
        }

        internal static string
        InvalidRightBoundaryCondition
        {
            get { return "Invalid Right Boundary Condition."; }
        }

        internal static string
        SpecialCasePlannedButNotImplementedYet
        {
            get { return "This special case is not supported yet (but is planned)."; }
        }
    }
}
