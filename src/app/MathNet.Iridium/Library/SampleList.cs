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
using System.Collections.Generic;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics
{
    /// <summary>
    /// SampleList is a sorted list in ascending order for discrete function samples x=f(t) or value pairs (t,x).
    /// Multiple values x for the same t are replaced with the mean.
    /// </summary>
    /// <remarks>
    /// This list is designed for rare additions and removals only and is slow when adding
    /// items in not-ascending order. Better fill it directly in the constructor.
    /// </remarks>
    [Serializable]
    public class SampleList :
        ICloneable,
        ICollection,
        IDictionary,
        IEnumerable
    {
        int _size;
        int[] _sampleCount; // for mean calculation
        double[] _sampleT;
        double[] _sampleX;

        SampleList.KeyList _keyList;
        SampleList.ValueList _valueList;

        /// <summary>
        /// Event which notifies when a sample has been altered.
        /// </summary>
        public event EventHandler<SampleAlteredEventArgs> SampleAltered;

        #region Construction

        /// <summary>
        /// Create a new sample list.
        /// </summary>
        public
        SampleList()
        {
            _sampleCount = new int[16];
            _sampleT = new double[16];
            _sampleX = new double[16];
        }

        /// <summary>
        /// Create a new sample list.
        /// </summary>
        /// <param name="capacity">initial capacity</param>
        public
        SampleList(
            int capacity
            )
        {
            if(capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity", Resources.ArgumentNotNegative);
            }

            _sampleCount = new int[capacity];
            _sampleT = new double[capacity];
            _sampleX = new double[capacity];
        }

        /// <summary>
        /// Create a new sample list.
        /// </summary>
        public
        SampleList(
            IDictionary d
            )
            : this((d != null) ? d.Count : 16)
        {
            if(null == d)
            {
                throw new ArgumentNullException("d");
            }

            d.Keys.CopyTo(_sampleT, 0);
            d.Values.CopyTo(_sampleX, 0);
            Array.Sort(_sampleT, _sampleX);
            _size = d.Count;
        }

        /// <summary>
        /// Create a new sample list.
        /// </summary>
        public
        SampleList(
            IDictionary d, int capacity
            )
            : this((d != null) ? (capacity >= d.Count ? capacity : d.Count) : 16)
        {
            if(null == d)
            {
                throw new ArgumentNullException("d");
            }

            d.Keys.CopyTo(_sampleT, 0);
            d.Values.CopyTo(_sampleX, 0);
            Array.Sort(_sampleT, _sampleX);
            _size = d.Count;
        }

        /// <summary>
        /// Create a new sample list based on a copy of two arrays.
        /// </summary>
        /// <param name="t">keys t, where x=f(t) or (t,x).</param>
        /// <param name="x">values x, where x=f(t) or (t,x).</param>
        /// <exception cref="ArgumentOutOfRangeException">If the dimensionality of the two arrays doesn't match.</exception>
        public
        SampleList(
            IList<double> t,
            IList<double> x
            )
            : this((t != null) ? t.Count : 0)
        {
            if(null == t)
            {
                throw new ArgumentNullException("t");
            }

            if(null == x)
            {
                throw new ArgumentNullException("x");
            }

            if(t.Count != x.Count)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            t.CopyTo(_sampleT, 0);
            x.CopyTo(_sampleX, 0);
            Array.Sort(_sampleT, _sampleX);
            _size = t.Count;
        }

        #endregion

        #region Items

        /// <summary>
        /// Doubles the capacity. Do not use this method directly, use <see cref="Add"/> instead.
        /// </summary>
        void
        EnsureCapacity(
            int min
            )
        {
            int capacitySuggestion = (_sampleCount.Length < 8) ? 16 : (_sampleCount.Length << 1);
            this.Capacity = (capacitySuggestion < min) ? min : capacitySuggestion;
        }

        /// <summary>
        /// Append a sample to existing samples. Do not use this method directly, use <see cref="Add"/> instead.
        /// </summary>
        void
        AppendMean(
            int index,
            double x
            )
        {
            double cntBefore = _sampleCount[index];
            double cntAfter = _sampleCount[index] = _sampleCount[index] + 1;
            _sampleX[index] = (_sampleX[index] * cntBefore + x) / cntAfter;

            if(SampleAltered != null)
            {
                SampleAltered(this, new SampleAlteredEventArgs(_sampleT[index]));
            }
        }

        /// <summary>
        /// Insert a new sample. Do not use this method directly, use <see cref="Add"/> instead.
        /// </summary>
        void
        Insert(
            int index,
            double t,
            double x
            )
        {
            if(_size == _sampleCount.Length)
            {
                EnsureCapacity(_size + 1);
            }

            if(index < _size)
            {
                Array.Copy(_sampleCount, index, _sampleCount, index + 1, _size - index);
                Array.Copy(_sampleT, index, _sampleT, index + 1, _size - index);
                Array.Copy(_sampleX, index, _sampleX, index + 1, _size - index);
            }

            _sampleCount[index] = 1;
            _sampleT[index] = t;
            _sampleX[index] = x;
            _size++;

            if(SampleAltered != null)
            {
                SampleAltered(this, new SampleAlteredEventArgs(t));
            }
        }

        /// <summary>
        /// Add a new sample to the list. 
        /// </summary>
        /// <param name="t">key t, where x=f(t) or (t,x).</param>
        /// <param name="x">value x, where x=f(t) or (t,x).</param>
        public virtual
        void
        Add(
            double t,
            double x
            )
        {
            int index = Locate(t);
            if(index > 0 && Number.AlmostEqual(_sampleT[index], t))
            {
                AppendMean(index, x);
            }
            else if(index < _sampleT.Length - 1 && Number.AlmostEqual(_sampleT[index + 1], t))
            {
                AppendMean(index + 1, x);
            }
            else
            {
                Insert(index + 1, t, x);
            }
        }

        /// <summary>
        /// Remove the sample at the given index.
        /// </summary>
        public virtual
        void
        RemoveAt(
            int index
            )
        {
            double t = _sampleT[index];

            if(index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            _size--;
            if(index < _size)
            {
                Array.Copy(_sampleCount, index + 1, _sampleCount, index, _size - index);
                Array.Copy(_sampleT, index + 1, _sampleT, index, _size - index);
                Array.Copy(_sampleX, index + 1, _sampleX, index, _size - index);
            }

            _sampleCount[_size] = 0;

            if(SampleAltered != null)
            {
                SampleAltered(this, new SampleAlteredEventArgs(t));
            }
        }

        /// <summary>
        /// Remove all samples with a key exactly equal to t, where x=f(t) or (t,x).
        /// </summary>
        public virtual
        void
        Remove(
            double t
            )
        {
            int index = IndexOfT(t);
            if(index >= 0)
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// Remove all samples from the list.
        /// </summary>
        public virtual
        void
        Clear()
        {
            _size = 0;

            if(SampleAltered != null)
            {
                SampleAltered(this, new SampleAlteredEventArgs(double.NaN));
            }
        }

        /// <summary>
        /// The index of the sample with a key exactly equal to t.
        /// </summary>
        int
        IndexOfT(
            double t
            )
        {
            int index = Locate(t);
            if(Number.AlmostEqual(_sampleT[index], t))
            {
                return index;
            }

            if(Number.AlmostEqual(_sampleT[index + 1], t))
            {
                return index + 1;
            }

            return -1;
        }

        /// <summary>
        /// The index of the first sample with a value exactly equal to x.
        /// </summary>
        int
        IndexOfX(
            double x
            )
        {
            return Array.IndexOf(_sampleX, x, 0, _size);
        }

        /// <summary>
        /// Sets the X matching the given T (and sets its count to 1) or inserts a new sample.
        /// Do not use this to add a sample to the list!
        /// </summary>
        void
        SetX(
            double t,
            double x
            )
        {
            int index = Locate(t);
            if(Number.AlmostEqual(_sampleT[index], t))
            {
                _sampleX[index] = x;
                _sampleCount[index] = 1;
            }
            else if(Number.AlmostEqual(_sampleT[index + 1], t))
            {
                _sampleX[index + 1] = x;
                _sampleCount[index + 1] = 1;
            }
            else
            {
                Insert(index + 1, t, x);
            }
        }

        /// <summary>
        /// The smallest key t in the sample list, where x=f(t) or (t,x).
        /// </summary>
        public double MinT
        {
            get { return _sampleT[0]; }
        }

        /// <summary>
        /// The biggest key t in the sample list, where x=f(t) or (t,x).
        /// </summary>
        public double MaxT
        {
            get { return _sampleT[_size - 1]; }
        }

        /// <summary>
        /// Get the key t stored at the given index, where x=f(t) or (t,x).
        /// </summary>
        public
        double
        GetT(
            int index
            )
        {
            return _sampleT[index];
        }

        /// <summary>
        /// Get the value x stored at the given index, where x=f(t) or (t,x).
        /// </summary>
        public
        double
        GetX(
            int index
            )
        {
            return _sampleX[index];
        }

        #endregion

        #region Search

        /// <summary>
        /// Find the index i of a sample near t such that t[i] &lt;= t &lt;= t[i+1]. 
        /// </summary>
        /// <returns>The lower bound index of the interval, -1 or Size-1 if out of bounds.</returns>
        public
        int
        Locate(
            double t
            )
        {
            return LocateBisection(t, -1, _size);
        }

        /// <summary>
        /// Find the index i of a sample near t such that t[i] &lt;= t &lt;= t[i+1]. 
        /// </summary>
        /// <remarks>This method is faster if the expected index is near nearIndex (compared to the list size) but may be slower otherwise (worst case: factor 2 slower, best case: log2(n) faster).</remarks>
        /// <returns>The lower bound index of the interval, -1 or Size-1 if out of bounds.</returns>
        public
        int
        Locate(
            double t,
            int nearIndex
            )
        {
            if(nearIndex < 0 || nearIndex > _size - 1)
            {
                return LocateBisection(t, -1, _size);
            }

            int lower = nearIndex;
            int upper;
            int increment = 1;

            if(t >= _sampleT[lower]) 
            {
                // Hunt Up

                if(lower == _size - 1)
                {
                    return lower;
                }

                upper = lower + 1;
                while(t >= _sampleT[upper])
                {
                    lower = upper;
                    increment <<= 1;
                    upper = lower + increment;

                    if(upper > _size - 1)
                    {
                        upper = _size;
                        break;
                    }
                }
            }
            else 
            {
                // Hunt Down

                if(lower == 0)
                {
                    return -1;
                }

                upper = lower--;
                while(t < _sampleT[lower])
                {
                    upper = lower;
                    increment <<= 1;

                    if(increment > upper)
                    {
                        lower = -1;
                        break;
                    }
                    else
                    {
                        lower = upper - increment;
                    }
                }
            }

            return LocateBisection(t, lower, upper);
        }

        /// <summary>
        /// Bisection Search Helper. Do not use this method directly, use <see cref="Locate(double)"/> instead.
        /// </summary>
        int
        LocateBisection(
            double t,
            int lowerIndex,
            int upperIndex
            )
        {
            int midpointIndex;

            while(upperIndex - lowerIndex > 1)
            {
                midpointIndex = (upperIndex + lowerIndex) >> 1;

                if(t >= _sampleT[midpointIndex])
                {
                    lowerIndex = midpointIndex;
                }
                else
                {
                    upperIndex = midpointIndex;
                }
            }

            if(_size > 0 && Number.AlmostEqual(t, _sampleT[_size - 1]))
            {
                return _size - 2;
            }

            return lowerIndex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The count of unique samples supported.
        /// </summary>
        public int Capacity
        {
            get
            {
                return _sampleCount.Length;
            }

            set
            {
                if(value != _sampleCount.Length)
                {
                    if(value < _size)
                    {
                        throw new ArgumentOutOfRangeException("value");
                    }

                    if(value > 0)
                    {
                        int[] sampleCountNew = new int[value];
                        double[] sampleTNew = new double[value];
                        double[] sampleXNew = new double[value];

                        if(_size > 0)
                        {
                            Array.Copy(_sampleCount, 0, sampleCountNew, 0, _size);
                            Array.Copy(_sampleT, 0, sampleTNew, 0, _size);
                            Array.Copy(_sampleX, 0, sampleXNew, 0, _size);
                        }

                        _sampleCount = sampleCountNew;
                        _sampleT = sampleTNew;
                        _sampleX = sampleXNew;
                    }
                    else
                    {
                        _sampleCount = new int[16];
                        _sampleT = new double[16];
                        _sampleX = new double[16];
                    }
                }
            }
        }

        #endregion

        #region ICloneable Member

        /// <summary>
        /// Create a copy of this sample list.
        /// </summary>
        object
        ICloneable.Clone()
        {
            return Clone();
        }

        /// <summary>
        /// Create a copy of this sample list.
        /// </summary>
        public virtual
        SampleList
        Clone()
        {
            SampleList list = new SampleList(_size);
            Array.Copy(_sampleCount, 0, list._sampleCount, 0, _size);
            Array.Copy(_sampleT, 0, list._sampleT, 0, _size);
            Array.Copy(_sampleX, 0, list._sampleX, 0, _size);
            list._size = _size;
            return list;
        }

        #endregion

        #region ICollection Member

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        /// <summary>
        /// The count of unique samples stored.
        /// </summary>
        public int Count
        {
            get { return _size; }
        }

        void
        ICollection.CopyTo(
            Array array,
            int index
            )
        {
            if(array == null || array.Rank != 1)
            {
                throw new ArgumentException("array", Resources.ArgumentSingleDimensionArray);
            }

            if(index < 0 || (array.Length - index) < _size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            for(int i = 0; i < _size; i++)
            {
                DictionaryEntry entry = new DictionaryEntry(_sampleT[i], _sampleX[i]);
                array.SetValue(entry, i + index);
            }
        }

        #endregion

        #region IDictionary Member

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        object IDictionary.this[object key]
        {
            get
            {
                int index = this.IndexOfT(Convert.ToDouble(key));
                if(index >= 0)
                {
                    return this._sampleX[index];
                }

                return null;
            }

            set
            {
                if(key == null)
                {
                    throw new ArgumentNullException("key");
                }

                double dkey = Convert.ToDouble(key);
                double dvalue = Convert.ToDouble(value);
                SetX(dkey, dvalue);
            }
        }

        bool IDictionary.Contains(object key)
        {
            return ContainsT(Convert.ToDouble(key));
        }

        /// <summary>
        /// True if this sample list contains a sample at t
        /// </summary>
        public
        bool
        ContainsT(
            double t
            )
        {
            return IndexOfT(t) != -1;
        }
        
        /// <summary>
        /// True if this sample list contains a sample value x
        /// </summary>
        public
        bool
        ContainsX(
            double x
            )
        {
            return IndexOfX(x) != -1;
        }

        void 
        IDictionary.Add(
            object key,
            object value
            )
        {
            Add(Convert.ToDouble(key), Convert.ToDouble(value));
        }

        void 
        IDictionary.Remove(
            object key
            )
        {
            Remove(Convert.ToDouble(key));
        }

        IDictionaryEnumerator
        IDictionary.GetEnumerator()
        {
            return new SampleListEnumerator(this, 0, _size, SampleListEnumerator.EnumerationMode.DictEntry);
        }

        ICollection IDictionary.Keys
        {
            get
            {
                if(_keyList == null)
                {
                    _keyList = new SampleList.KeyList(this);
                }

                return _keyList;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                if(_valueList == null)
                {
                    _valueList = new SampleList.ValueList(this);
                }

                return _valueList;
            }
        }

        [Serializable]
        private sealed class KeyList : IList, ICollection, IEnumerable
        {
            private SampleList sampleList;

            internal KeyList(SampleList sampleList)
            {
                this.sampleList = sampleList;
            }

            public int Add(object key)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(object key)
            {
                return sampleList.ContainsT(Convert.ToDouble(key));
            }

            public void CopyTo(Array array, int index)
            {
                if(array == null || array.Rank != 1)
                {
                    throw new ArgumentException(Resources.ArgumentSingleDimensionArray, "array");
                }

                Array.Copy(sampleList._sampleT, 0, array, index, sampleList.Count);
            }

            public IEnumerator GetEnumerator()
            {
                return new SampleList.SampleListEnumerator(sampleList, 0, sampleList.Count, SampleList.SampleListEnumerator.EnumerationMode.Keys);
            }

            public int IndexOf(object key)
            {
                return sampleList.IndexOfT(Convert.ToDouble(key));
            }

            public void Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            public void Remove(object key)
            {
                throw new NotSupportedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            public int Count
            {
                get { return sampleList.Count; }
            }

            public bool IsFixedSize
            {
                get { return true; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool IsSynchronized
            {
                get { return ((ICollection)sampleList).IsSynchronized; }
            }

            public object this[int index]
            {
                get { return sampleList._sampleT[index]; }
                set { throw new NotSupportedException(); }
            }

            public object SyncRoot
            {
                get { return ((ICollection)sampleList).SyncRoot; }
            }
        }

        [Serializable]
        private sealed class ValueList : IList, ICollection, IEnumerable
        {
            private SampleList sampleList;

            internal ValueList(SampleList sampleList)
            {
                this.sampleList = sampleList;
            }

            public int Add(object value)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(object value)
            {
                return sampleList.ContainsX(Convert.ToDouble(value));
            }

            public void CopyTo(Array array, int index)
            {
                if(array == null || array.Rank != 1)
                {
                    throw new ArgumentException(Resources.ArgumentSingleDimensionArray, "array");
                }

                Array.Copy(sampleList._sampleX, 0, array, index, sampleList.Count);
            }

            public IEnumerator GetEnumerator()
            {
                return new SampleList.SampleListEnumerator(sampleList, 0, sampleList.Count, SampleList.SampleListEnumerator.EnumerationMode.Values);
            }

            public int IndexOf(object value)
            {
                return sampleList.IndexOfX(Convert.ToDouble(value));
            }

            public void Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            public void Remove(object value)
            {
                throw new NotSupportedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            public int Count
            {
                get { return sampleList.Count; }
            }

            public bool IsFixedSize
            {
                get { return true; }
            }

            public bool IsReadOnly
            {
                get { return true; }
            }

            public bool IsSynchronized
            {
                get { return ((ICollection)sampleList).IsSynchronized; }
            }

            public object this[int index]
            {
                get { return sampleList._sampleX[index]; }
                set { throw new NotSupportedException(); }
            }

            public object SyncRoot
            {
                get { return ((ICollection)sampleList).SyncRoot; }
            }
        }
        #endregion

        #region IEnumerable Member

        IEnumerator
        IEnumerable.GetEnumerator()
        {
            return new SampleListEnumerator(this, 0, _size, SampleListEnumerator.EnumerationMode.DictEntry);
        }

        [Serializable]
        private sealed class SampleListEnumerator : IDictionaryEnumerator, IEnumerator, ICloneable
        {
            private bool current;
            private int endIndex;
            private EnumerationMode mode;
            private int index;
            private object key;
            private SampleList sampleList;
            private int startIndex;
            private object value;

            internal enum EnumerationMode : int
            {
                /// <summary>Enumerate the keys.</summary>
                Keys = 0,

                /// <summary>Enumerate the values.</summary>
                Values = 1,

                /// <summary>Enumerate the dictionary entries.</summary>
                DictEntry = 2
            }

            internal SampleListEnumerator(SampleList sampleList, int index, int count, EnumerationMode mode)
            {
                this.sampleList = sampleList;
                this.index = index;
                this.startIndex = index;
                this.endIndex = index + count;
                this.mode = mode;
            }

            public object Clone()
            {
                return MemberwiseClone();
            }

            public bool MoveNext()
            {
                if(this.index < this.endIndex)
                {
                    this.key = sampleList._sampleT[this.index];
                    this.value = sampleList._sampleX[this.index];
                    this.index++;
                    this.current = true;
                    return true;
                }

                this.key = null;
                this.value = null;
                this.current = false;
                return false;
            }

            public void Reset()
            {
                this.index = this.startIndex;
                this.current = false;
                this.key = null;
                this.value = null;
            }

            public object Current
            {
                get
                {
                    if(!this.current)
                    {
                        throw new InvalidOperationException();
                    }

                    if(this.mode == EnumerationMode.Keys)
                    {
                        return this.key;
                    }

                    if(this.mode == EnumerationMode.Values)
                    {
                        return this.value;
                    }

                    return new DictionaryEntry(this.key, this.value);
                }
            }

            public DictionaryEntry Entry
            {
                get
                {
                    if(!this.current)
                    {
                        throw new InvalidOperationException();
                    }

                    return new DictionaryEntry(this.key, this.value);
                }
            }

            public object Key
            {
                get
                {
                    if(!this.current)
                    {
                        throw new InvalidOperationException();
                    }

                    return this.key;
                }
            }

            public object Value
            {
                get
                {
                    if(!this.current)
                    {
                        throw new InvalidOperationException();
                    }

                    return this.value;
                }
            }
        }
        #endregion

        #region Event Handler

        /// <summary>
        /// Event Argument for <see cref="SampleList.SampleAltered"/> Event.
        /// </summary>
        public class SampleAlteredEventArgs : EventArgs
        {
            double t;

            /// <summary>
            /// Instantiate new event arguments.
            /// </summary>
            /// <param name="t">The t-value of the x=f(t) or (t,x) samples.</param>
            public SampleAlteredEventArgs(double t)
            {
                this.t = t;
            }

            /// <summary>
            /// he t-value of the x=f(t) or (t,x) samples.
            /// </summary>
            public double T
            {
                get { return t; }
            }
        }

        #endregion
    }
}
