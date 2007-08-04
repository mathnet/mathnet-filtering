#region Math.NET Yttrium (GPL) by Christoph Ruegg
// Math.NET Yttrium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2007, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Conversion;
using MathNet.Symbolics.Packages.Standard.Structures;

namespace MathNet.Symbolics.Packages.Standard.Arithmetics
{
    internal interface IAccumulator
    {
        IValueStructure Value { get; }
        IAccumulator Add(IValueStructure operand);
        IAccumulator Subtract(IValueStructure operand);
        IAccumulator Multiply(IValueStructure operand);
        IAccumulator Divide(IValueStructure operand);
        IAccumulator Negate();
        IAccumulator Invert();
        IAccumulator IntegerPower(int exponent);
        IAccumulator IntegerPower(IValueStructure exponent);
    }

    internal class Accumulator<T, TDivision> :
        IAccumulator
        where T : IAlgebraicIntegralDomain<T, TDivision>, IValueStructure //IAlgebraicCommutativeRingWithUnity<T>
        where TDivision : IAlgebraicIntegralDomain<TDivision, TDivision>, IValueStructure
    {
        private T _value;

        public Accumulator(T value)
        {
            _value = value;
        }

        public IValueStructure Value
        {
            get { return _value; }
        }

        public IAccumulator Add(IValueStructure operand)
        {
            object other;
            if(ValueConverter<T>.TryConvertLosslessFrom(operand, out other))
            {
                _value = _value.Add((T)other);
                return this;
            }
            else
            {
                IAccumulator acc = Escalate(operand.TypeId, _value, false);
                return acc.Add(operand);
            }
        }

        public IAccumulator Subtract(IValueStructure operand)
        {
            object other;
            if(ValueConverter<T>.TryConvertLosslessFrom(operand, out other))
            {
                _value = _value.Subtract((T)other);
                return this;
            }
            else
            {
                IAccumulator acc = Escalate(operand.TypeId, _value, false);
                return acc.Subtract(operand);
            }
        }

        public IAccumulator Negate()
        {
            _value = _value.Negate();
            return this;
        }

        public IAccumulator Multiply(IValueStructure operand)
        {
            object other;
            if(ValueConverter<T>.TryConvertLosslessFrom(operand, out other))
            {
                _value = _value.Multiply((T)other);
                return this;
            }
            else
            {
                IAccumulator acc = Escalate(operand.TypeId, _value, false);
                return acc.Multiply(operand);
            }
        }

        public IAccumulator Divide(IValueStructure operand)
        {
            object other;
            if(ValueConverter<T>.TryConvertLosslessFrom(operand, out other))
            {
                TDivision res = _value.Divide((T)other);
                if(_value is IAlgebraicDivisionExtension<T, T>)
                {
                    _value = (T)(object)res;
                    return this;
                }
                return Escalate<TDivision, TDivision>(res);
            }
            else
            {
                IAccumulator acc = Escalate(operand.TypeId, _value, true);
                return acc.Divide(operand);
            }
        }

        public IAccumulator Invert()
        {
            TDivision res = _value.Invert();
            if(_value is IAlgebraicDivisionExtension<T, T>)
            {
                _value = (T)(object)res;
                return this;
            }
            return Escalate<TDivision, TDivision>(res);
        }

        public IAccumulator IntegerPower(int exponent)
        {
            if(exponent >= 0)
            {
                _value = _value.PositiveIntegerPower(exponent);
                return this;
            }
            TDivision res = _value.IntegerPower(exponent);
            if(_value is IAlgebraicDivisionExtension<T, T>)
            {
                _value = (T)(object)res;
                return this;
            }
            return Escalate<TDivision, TDivision>(res);
        }

        public IAccumulator IntegerPower(IValueStructure operand)
        {
            object other;
            if(ValueConverter<IntegerValue>.TryConvertLosslessFrom(operand, out other))
                return IntegerPower((int)((IntegerValue)other).Value);
            if(ValueConverter<RationalValue>.TryConvertLosslessFrom(operand, out other))
            {
                RationalValue rv = (RationalValue)other;
                if(rv.IsInteger)
                    return IntegerPower((int)rv.NumeratorValue);
            }
            throw new NotSupportedException();
        }

        private static IAccumulator Escalate(MathIdentifier structure, IValueStructure value, bool requireField)
        {
            if(structure.Equals(IntegerValue.TypeIdentifier))
            {
                if(requireField)
                    return Escalate<RationalValue, RationalValue>(value);
                else
                    return Escalate<IntegerValue, RationalValue>(value);
            }
            if(structure.Equals(RationalValue.TypeIdentifier))
                return Escalate<RationalValue, RationalValue>(value);
            if(structure.Equals(RealValue.TypeIdentifier))
                return Escalate<RealValue, RealValue>(value);
            if(structure.Equals(ComplexValue.TypeIdentifier))
                return Escalate<ComplexValue, ComplexValue>(value);

            throw new NotSupportedException();
        }

        private static IAccumulator Escalate<TStructure, TStructureDivision>(IValueStructure value)
            where TStructure : IAlgebraicIntegralDomain<TStructure, TStructureDivision>, IValueStructure
            where TStructureDivision : IAlgebraicIntegralDomain<TStructureDivision, TStructureDivision>, IValueStructure
        {
            object val;
            if(ValueConverter<TStructure>.TryConvertLosslessFrom(value, out val))
                return new Accumulator<TStructure, TStructureDivision>((TStructure)val);
            else
                throw new NotSupportedException();
        }

        public static IAccumulator Create(IValueStructure value)
        {
            return Escalate(value.TypeId, value, false);
        }
    }
}
