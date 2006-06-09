#region Copyright 2001-2006 Christoph Daniel Rüegg [GPL]
//Math.NET Symbolics: Yttrium, part of Math.NET
//Copyright (c) 2001-2006, Christoph Daniel Rueegg, http://cdrnet.net/.
//All rights reserved.
//This Math.NET package is available under the terms of the GPL.

//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.StdPackage
{
    [Flags, System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames")]
    public enum EAlgebraicStructure : int
    {
        AdditiveClose = 0x0001,
        AdditiveAssociative = 0x0002,
        AdditiveIdentityElement = 0x0004,
        AdditiveInverseElement = 0x0008,
        AdditiveCommutative = 0x0010,
        MultiplicativeClose = 0x0020,
        MultiplicativeAssociative = 0x0040,
        MultiplicativeIdentityElement = 0x0080,
        MultiplicativeInverseElement = 0x0100,
        MultiplicativeCommutative = 0x0200,
        DistributiveMultiplicativeOverAdditive = 0x0400,
        DistributiveAdditiveOverMultiplicative = 0x0800,
        Complement = 0x1000,

        Semigroup = AdditiveClose | AdditiveAssociative,
        Monoid = Semigroup | AdditiveIdentityElement,
        Group = Monoid | AdditiveInverseElement,

        AbelianSemigroup = Semigroup | AdditiveCommutative,
        AbelianMonoid = Monoid | AdditiveCommutative,
        AbelianGroup = Group | AdditiveCommutative,

        Ring = AbelianGroup | MultiplicativeClose | MultiplicativeAssociative | DistributiveMultiplicativeOverAdditive,
        RingWithUnity = Ring | MultiplicativeIdentityElement,
        CommutativeRing = Ring | MultiplicativeCommutative,
        CommutativeRingWithUnity = RingWithUnity | MultiplicativeCommutative,
        SkewField = RingWithUnity | MultiplicativeInverseElement, //aka Division Algebra
        Field = SkewField | MultiplicativeCommutative,

        Semiring = AbelianSemigroup | MultiplicativeClose | MultiplicativeAssociative | DistributiveMultiplicativeOverAdditive,
        CommutativeSemiring = Semiring | MultiplicativeCommutative,

        BooleanAlgebra = CommutativeSemiring | AdditiveIdentityElement | MultiplicativeIdentityElement | DistributiveAdditiveOverMultiplicative | Complement
    }

    #region Additive Close -> Add
    //public interface IAlgebraicAdditiveClose
    //{
    //    IAlgebraicAdditiveClose Add(IAlgebraicAdditiveClose other);
    //}
    public interface IAlgebraicAdditiveClose<T>
        where T : ValueStructure, IAlgebraicAdditiveClose<T>
    {
        T Add(T other);
    }
    #endregion

    #region Multiplicative Close -> Multiply
    //public interface IAlgebraicMultiplicativeClose
    //{
    //    IAlgebraicMultiplicativeClose Multiply(IAlgebraicMultiplicativeClose other);
    //}
    public interface IAlgebraicMultiplicativeClose<T>
        where T : ValueStructure, IAlgebraicMultiplicativeClose<T>
    {
        T Multiply(T other);
    }
    #endregion

    public interface IAlgebraicMonoid
    {
        bool IsAdditiveIdentity { get;}
        ValueStructure AdditiveIdentity { get;}
    }

    public interface IAlgebraicMonoid<T> : IAlgebraicMonoid, IAlgebraicAdditiveClose<T>
        where T : ValueStructure, IAlgebraicMonoid<T>
    {
    }

    public interface IAlgebraicGroup<T> : IAlgebraicMonoid<T>
        where T : ValueStructure, IAlgebraicGroup<T>
    {
        T Subtract(T other);
        T Negate();
    }

    public interface IAlgebraicAbelianGroup<T> : IAlgebraicGroup<T>
        where T : ValueStructure, IAlgebraicAbelianGroup<T>
    {
    }

    public interface IAlgebraicRingWithUnity : IAlgebraicMonoid
    {
        bool IsMultiplicativeIdentity { get;}
        ValueStructure MultiplicativeIdentity { get;}
    }

    public interface IAlgebraicRingWithUnity<T> : IAlgebraicRingWithUnity, IAlgebraicMonoid<T>, IAlgebraicMultiplicativeClose<T>
        where T : ValueStructure, IAlgebraicRingWithUnity<T>
    {
    }

    public interface IAlgebraicCommutativeRingWithUnity<T> : IAlgebraicRingWithUnity<T>
        where T : ValueStructure, IAlgebraicCommutativeRingWithUnity<T>
    {
    }

    public interface IAlgebraicSkewField<T> : IAlgebraicRingWithUnity<T>
        where T : ValueStructure, IAlgebraicSkewField<T>
    {
        T Divide(T other);
        T Invert();
    }

    public interface IAlgebraicField<T> : IAlgebraicSkewField<T>, IAlgebraicVectorSpace<T,T>
        where T : ValueStructure, IAlgebraicField<T>
    {
    }

    /// <summary>
    /// A vector space of elements TElement over a field TScalar.
    /// </summary>
    /// <typeparam name="TElement">Type of the vector elements.</typeparam>
    /// <typeparam name="TScalar">Type of the scalars.</typeparam>
    public interface IAlgebraicVectorSpace<TElement, TScalar> : IAlgebraicAbelianGroup<TElement>
        where TElement : ValueStructure, IAlgebraicAbelianGroup<TElement>
        where TScalar : ValueStructure, IAlgebraicField<TScalar>
    {
        TElement Scale(TScalar scalar);
    }
}
