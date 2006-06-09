using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Numerics
{
    /// <summary>
    /// A generic vector of two values, useful e.g. to return two values
    /// in a function without using out-parameters.
    /// </summary>
    public struct Tuple<TFirst, TSecond> : IEquatable<Tuple<TFirst, TSecond>>
        where TFirst : IEquatable<TFirst>
        where TSecond : IEquatable<TSecond>
    {
        private readonly TFirst _first;
        private readonly TSecond _second;

        public Tuple(TFirst first, TSecond second)
        {
            _first = first;
            _second = second;
        }

        public TFirst First
        {
            get { return _first; }
        }

        public TSecond Second
        {
            get { return _second; }
        }

        public bool Equals(Tuple<TFirst, TSecond> other)
        {
            return _first.Equals(other.First) && _second.Equals(other.Second);
        }
    }
}
