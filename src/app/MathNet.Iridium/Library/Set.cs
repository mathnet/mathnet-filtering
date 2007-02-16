#region MathNet Numerics, Copyright ©2005 Christoph Ruegg

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2005,	Christoph Rüegg, http://www.cdrnet.net
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
using System.Collections.ObjectModel;

namespace MathNet.Numerics
{
    public interface ISet<T> : IList<T> where T : IEquatable<T>
    {
        T[] ToArray();
        /// <summary>Checks whether <c>c</c> is a subset of this set.</summary>
        bool IsSubset(IEnumerable<T> c);
        /// <summary>Checks whether this set is a subset of <c>c</c>, or if <c>c</c> is a superset of this set.</summary>
        bool IsSuperset(IEnumerable<T> c);
        bool HasEqualElements(IEnumerable<T> c);
        bool Exists(Predicate<T> match);
        bool Exists(Predicate<T> match, out T foundItem);
        bool TrueForAll(Predicate<T> match);
        void ForEach(Action<T> action);
        T Find(Predicate<T> match);
        int FindIndex(Predicate<T> match);
        int FindIndex(int startIndex, Predicate<T> match);
        int FindIndex(int startIndex, int count, Predicate<T> match);
        T FindLast(Predicate<T> match);
        int FindLastIndex(Predicate<T> match);
        int FindLastIndex(int startIndex, Predicate<T> match);
        int FindLastIndex(int startIndex, int count, Predicate<T> match);
        int LastIndexOf(T item);
        Set<T> FindAll(Predicate<T> match);
        Set<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> convert) where TOutput : IEquatable<TOutput>;

        void AddRange(IEnumerable<T> range);
        void AddDistinct(T item);
        void AddRangeDistinct(IEnumerable<T> range);
        void RemoveDuplicates();
        int RemoveAll(Predicate<T> match);

        bool IsReadonly { get;}
    }

    public class Set<T> : Collection<T>, ISet<T> where T : IEquatable<T>
    {
        public event EventHandler<SetChangedEventArgs<T>> OnSetChanged;
        protected ReadOnlySet<T> readonlyWrapper;

        public Set()
        {
        }

        public Set(IEnumerable<T> initial)
        {
            AddRange(initial);
        }

        public Set(params T[] initial)
        {
            AddRange(initial);
        }

        protected Set(IList<T> innerList)
            : base(innerList)
        {
        }

        public Set(int initialCount)
        //    : base((IList<T>)(new T[initialCount]))
        {
            T defnull = default(T);
            for(int i = 0; i < initialCount; i++)
                Add(defnull);
        }

        #region Factory Methods
        protected virtual Set<T> CreateNewSet()
        {
            return new Set<T>();
        }
        protected virtual ReadOnlySet<T> CreateNewReadOnlyWrapper(IList<T> list)
        {
            return new ReadOnlySet<T>(list);
        }
        #endregion

        public bool IsReadonly
        {
            get { return false; }
        }

        public ReadOnlySet<T> AsReadOnly
        {
            get
            {
                if(readonlyWrapper == null)
                    readonlyWrapper = CreateNewReadOnlyWrapper(base.Items);
                return readonlyWrapper;
            }
        }

        public T[] ToArray()
        {
            T[] array = new T[base.Count];
            base.CopyTo(array,0);
            return array;
        }

        #region Set Behaviour
        ///// <summary>
        ///// If true, the set elements have a specific order.
        ///// </summary>
        ///// <remarks>Two sequences with the same elements but in a different order are not equal.</remarks>
        //public bool IsSequence
        //{
        //}

        ///// <summary>
        ///// If true, the set may not contain any element more than once.
        ///// </summary>
        //public bool IsDistinct
        //{
        //}
        #endregion

        #region Element Manipulation
        public void AddRange(IEnumerable<T> range)
        {
            foreach(T item in range)
                base.Add(item);
        }

        public void AddDistinct(T item)
        {
            if(!Contains(item))
                base.Add(item);
        }

        public void AddRangeDistinct(IEnumerable<T> range)
        {
            foreach(T item in range)
                if(!base.Contains(item))
                    base.Add(item);
        }

        public void RemoveDuplicates()
        {
            Dictionary<T, object> table = new Dictionary<T, object>();
            for(int i = base.Count - 1; i >= 0; i--)
            {
                T item = base[i];
                if(table.ContainsKey(item))
                    base.RemoveAt(i);
                else
                    table.Add(item, null);
            }
        }

        public void ReplaceRange(IEnumerable<T> range)
        {
            int i = 0;
            foreach(T item in range)
                base[i++] = item;
        }

        protected override void InsertItem(int index, T item)
        {
            EventHandler<SetChangedEventArgs<T>> handler = OnSetChanged;
            if(handler == null)
                base.InsertItem(index, item);
            else
            {
                base.InsertItem(index, item);
                handler(this, SetChangedEventArgs<T>.Added(item, index));
                for(int i = index + 1; i < base.Count; i++)
                    handler(this, SetChangedEventArgs<T>.Moved(base[i], i - 1, i));
            }
        }
        protected override void RemoveItem(int index)
        {
            EventHandler<SetChangedEventArgs<T>> handler = OnSetChanged;
            if(handler == null)
                base.RemoveItem(index);
            else
            {
                handler(this, SetChangedEventArgs<T>.Removed(base[index], index));
                base.RemoveItem(index);
                for(int i = index; i < base.Count; i++)
                    handler(this, SetChangedEventArgs<T>.Moved(base[i], i + 1, i));
            }
        }
        protected override void SetItem(int index, T item)
        {
            EventHandler<SetChangedEventArgs<T>> handler = OnSetChanged;
            if(handler == null)
                base.SetItem(index, item);
            else
            {
                handler(this, SetChangedEventArgs<T>.Removed(base[index], index));
                base.SetItem(index, item);
                handler(this, SetChangedEventArgs<T>.Added(item, index));
            }
        }

        #endregion

        #region Subsets
        /// <summary>
        /// Checks whether <c>c</c> is a subset of this set.
        /// </summary>
        public bool IsSubset(IEnumerable<T> c)
        {
            foreach(T item in c)
                if(!base.Contains(item))
                    return false;
            return true;
        }

        /// <summary>
        /// Checks whether this set is a subset of <c>c</c>, or if <c>c</c> is a superset of this set.
        /// </summary>
        public bool IsSuperset(IEnumerable<T> c)
        {
            Dictionary<T, object> table = new Dictionary<T, object>();
            foreach(T item in c)
                if(!table.ContainsKey(item))
                    table.Add(item, null);
            foreach(T item in this)
                if(!table.ContainsKey(item))
                    return false;
            return true;
        }

        /// <remarks>ignores duplicate elements.</remarks>
        public bool HasEqualElements(IEnumerable<T> c)
        {
            // TODO: quite inelegant, find better algorithm.

            Dictionary<T, object> table = new Dictionary<T, object>();
            foreach(T item in c)
            {
                if(!table.ContainsKey(item))
                {
                    table.Add(item, null);
                    if(!base.Contains(item))
                        return false;
                }
            }
            foreach(T item in this)
                if(!table.ContainsKey(item))
                    return false;
            return true;
        }
        #endregion

        #region Concatenation (Not Distinct)
        /// <summary>
        /// Returns a collection resulting from the concatenation with the items of <c>c</c>.
        /// </summary>
        /// <remarks>Not distinct: the resulting collection may contain several identical elements.</remarks>
        public Set<T> Concatenate(IEnumerable<T> c)
        {
            return Concatenate(this, c);
        }
        public void ConcatenateInplace(IEnumerable<T> c)
        {
            AddRange(c);
        }
        /// <summary>
        /// Returns a collection resulting from the concatenation
        /// from <c>c1</c> and <c>c2</c>.
        /// </summary>
        /// <remarks>Not distinct: the resulting collection may contain several identical elements.</remarks>
        public static Set<T> Concatenate(IEnumerable<T> c1, IEnumerable<T> c2)
        {
            Set<T> s = new Set<T>(c1);
            s.AddRange(c2);
            return s;
        }
        #endregion

        #region Union (Distinct)
        /// <summary>
        /// Returns a collection resulting from the union with the items of <c>c</c>.
        /// </summary>
        /// <remarks>Distinct: the resulting collection may not contain several identical elements.</remarks>
        public Set<T> Union(IEnumerable<T> c)
        {
            return Union(this, c);
        }
        public void UnionInplace(IEnumerable<T> c)
        {
            RemoveDuplicates();
            AddRangeDistinct(c);
        }
        /// <summary>
        /// Returns a collection resulting from the union of the items
        /// of <c>c1</c> and <c>c2</c>.
        /// </summary>
        /// <remarks>Distinct: the resulting collection may not contain several identical elements.</remarks>
        public static Set<T> Union(IEnumerable<T> c1, IEnumerable<T> c2)
        {
            Set<T> s = new Set<T>();
            s.AddRangeDistinct(c1);
            s.AddRangeDistinct(c2);
            return s;
        }
        #endregion

        #region Intersection (Distinct)
        /// <summary>
        /// Returns a collection resulting from the intersection with the items of <c>c</c>.
        /// </summary>
        /// <remarks>Distinct: the resulting collection may not contain several identical elements.</remarks>
        public Set<T> Intersect(IEnumerable<T> c)
        {
            return Intersect(this, c);
        }
        public void IntersectInplace(IEnumerable<T> c)
        {
            Dictionary<T, object> table = new Dictionary<T, object>();
            foreach(T item in c)
                if(!table.ContainsKey(item))
                    table.Add(item, null);
            for(int i = base.Count - 1; i >= 0; i--)
            {
                if(!table.ContainsKey(base[i]))
                    base.RemoveAt(i);
            }
        }
        /// <summary>
        /// Returns a collection resulting from the intersection of the items
        /// of <c>c1</c> and <c>c2</c>.
        /// </summary>
        /// <remarks>Distinct: the resulting collection may not contain several identical elements.</remarks>
        public static Set<T> Intersect(IEnumerable<T> c1, IEnumerable<T> c2)
        {
            Dictionary<T, object> table = new Dictionary<T, object>();
            foreach(T item in c1)
                if(!table.ContainsKey(item))
                    table.Add(item, null);
            Set<T> s = new Set<T>();
            foreach(T item in c2)
                if(table.ContainsKey(item) && !s.Contains(item))
                    s.Add(item);
            return s;
        }
        #endregion

        #region Subtraction (Not Distinct)
        /// <summary>
        /// Returns a collection resulting from the subtraction of the items of <c>c</c>.
        /// </summary>
        /// <remarks>Not distinct: the resulting collection may contain several identical elements.</remarks>
        public Set<T> Subtract(IEnumerable<T> c)
        {
            return Subtract(this, c);
        }
        public void SubtractInplace(IEnumerable<T> c)
        {
            Dictionary<T, object> table = new Dictionary<T, object>();
            foreach(T item in c)
                if(!table.ContainsKey(item))
                    table.Add(item, null);
            for(int i = base.Count - 1; i >= 0; i--)
            {
                if(table.ContainsKey(base[i]))
                    base.RemoveAt(i);
            }
        }
        /// <summary>
        /// Returns a collection resulting from the subtraction of the items
        /// of <c>c2</c> to the collection <c>c1</c>.
        /// </summary>
        /// <remarks>Not distinct: the resulting collection may contain several identical elements.</remarks>
        public static Set<T> Subtract(IEnumerable<T> c1, IEnumerable<T> c2)
        {
            Dictionary<T, object> table = new Dictionary<T, object>();
            foreach(T item in c2)
                if(!table.ContainsKey(item))
                    table.Add(item, null);
            Set<T> s = new Set<T>();
            foreach(T item in c1)
                if(!table.ContainsKey(item))
                    s.Add(item);
            return s;
        }
        #endregion

        #region Lambdas
        public bool Exists(Predicate<T> match)
        {
            return (FindIndex(match) != -1);
        }
        public bool Exists(Predicate<T> match, out T foundItem)
        {
            int idx = FindIndex(match);
            if(idx == -1)
            {
                foundItem = default(T);
                return false;
            }
            else
            {
                foundItem = base[idx];
                return true;
            }
        }
        public bool TrueForAll(Predicate<T> match)
        {
            if(match == null) throw new ArgumentNullException("match");

            for(int i = 0; i < base.Count; i++)
                if(!match(base[i]))
                    return false;
            return true;
        }
        public void ForEach(Action<T> action)
        {
            if(action == null) throw new ArgumentNullException("action");

            for(int i = 0; i < base.Count; i++)
                action(base[i]);
        }
        #region Find Lambdas
        public T Find(Predicate<T> match)
        {
            if(match == null) throw new ArgumentNullException("match");

            for(int i = 0; i < base.Count; i++)
                if(match(base[i]))
                    return base[i];
            return default(T);
        }
        public int FindIndex(Predicate<T> match)
        {
            return FindIndex(0, base.Count, match);
        }
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return FindIndex(startIndex, base.Count - startIndex, match);
        }
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            if(startIndex > base.Count) throw new ArgumentOutOfRangeException("startIndex");
            if((count < 0) || (startIndex > (base.Count - count))) throw new ArgumentOutOfRangeException("count");
            if(match == null) throw new ArgumentNullException("match");

            int afterLast = startIndex + count;
            for(int i = startIndex; i < afterLast; i++)
                if(match(base[i]))
                    return i;
            return -1;
        }
        public T FindLast(Predicate<T> match)
        {
            if(match == null) throw new ArgumentNullException("match");

            for(int i = base.Count - 1; i >= 0; i--)
                if(match(base[i]))
                    return base[i];
            return default(T);
        }
        public int FindLastIndex(Predicate<T> match)
        {
            return FindLastIndex(base.Count - 1, base.Count, match);
        }
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return FindLastIndex(startIndex, startIndex + 1, match);
        }
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            if(startIndex >= base.Count) throw new ArgumentOutOfRangeException("startIndex");
            if((count < 0) || (((startIndex - count) + 1) < 0)) throw new ArgumentOutOfRangeException("count");
            if(match == null) throw new ArgumentNullException("match");

            int afterLast = startIndex - count;
            for(int i = startIndex; i > afterLast; i--)
                if(match(base[i]))
                    return i;
            return -1;
        }
        public int LastIndexOf(T item)
        {
            for(int i = base.Count - 1; i >= 0; i--)
                if(item.Equals(base[i]))
                    return i;
            return -1;
        }
        public Set<T> FindAll(Predicate<T> match)
        {
            if(match == null) throw new ArgumentNullException("match");
            
            Set<T> found = CreateNewSet();
            for(int i = 0; i < base.Count; i++)
                if(match(base[i]))
                    found.Add(base[i]);
            return found;
        }
        #endregion
        /// <returns>The number of removed items.</returns>
        public int RemoveAll(Predicate<T> match)
        {
            int cnt = 0;
            for(int i = base.Count - 1; i >= 0; i--)
                if(match(base[i]))
                {
                    cnt++;
                    base.RemoveAt(i);
                }
            return cnt;
        }
        #endregion

        #region Converter
        public Set<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> convert) where TOutput : IEquatable<TOutput>
        {
            Set<TOutput> ret = new Set<TOutput>();
            foreach(T item in this)
                ret.Add(convert(item));
            return ret;
        }
        #endregion
    }

    public class ReadOnlySet<T> : ReadOnlyCollection<T>, ISet<T> where T : IEquatable<T>
    {
        public ReadOnlySet(IList<T> list) : base(list) { }

        public T[] ToArray()
        {
            T[] array = new T[base.Count];
            base.CopyTo(array, 0);
            return array;
        }

        protected virtual Set<T> CreateNewSet()
        {
            return new Set<T>();
        }

        public bool IsReadonly
        {
            get { return true; }
        }

        #region Subsets
        /// <summary>
        /// Checks whether <c>c</c> is a subset of this set.
        /// </summary>
        public bool IsSubset(IEnumerable<T> c)
        {
            foreach(T item in c)
                if(!base.Contains(item))
                    return false;
            return true;
        }

        /// <summary>
        /// Checks whether this set is a subset of <c>c</c>, or if <c>c</c> is a superset of this set.
        /// </summary>
        public bool IsSuperset(IEnumerable<T> c)
        {
            Dictionary<T, object> table = new Dictionary<T, object>();
            foreach(T item in c)
                if(!table.ContainsKey(item))
                    table.Add(item, null);
            foreach(T item in this)
                if(!table.ContainsKey(item))
                    return false;
            return true;
        }

        /// <remarks>ignores duplicate elements.</remarks>
        public bool HasEqualElements(IEnumerable<T> c)
        {
            // TODO: quite inelegant, find better algorithm.

            Dictionary<T, object> table = new Dictionary<T, object>();
            foreach(T item in c)
            {
                if(!table.ContainsKey(item))
                {
                    table.Add(item, null);
                    if(!base.Contains(item))
                        return false;
                }
            }
            foreach(T item in this)
                if(!table.ContainsKey(item))
                    return false;
            return true;
        }
        #endregion

        #region Lambdas
        public bool Exists(Predicate<T> match)
        {
            return (FindIndex(match) != -1);
        }
        public bool Exists(Predicate<T> match, out T foundItem)
        {
            int idx = FindIndex(match);
            if(idx == -1)
            {
                foundItem = default(T);
                return false;
            }
            else
            {
                foundItem = base[idx];
                return true;
            }
        }
        public bool TrueForAll(Predicate<T> match)
        {
            if(match == null)
                throw new ArgumentNullException("match");
            for(int i = 0; i < base.Count; i++)
                if(!match(base[i]))
                    return false;
            return true;
        }
        public void ForEach(Action<T> action)
        {
            if(action == null)
                throw new ArgumentNullException("action");
            for(int i = 0; i < base.Count; i++)
                action(base[i]);
        }
        #region Find Lambdas
        public T Find(Predicate<T> match)
        {
            if(match == null)
                throw new ArgumentNullException("match");
            for(int i = 0; i < base.Count; i++)
                if(match(base[i]))
                    return base[i];
            return default(T);
        }
        public int FindIndex(Predicate<T> match)
        {
            return FindIndex(0, base.Count, match);
        }
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return FindIndex(startIndex, base.Count - startIndex, match);
        }
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            if(startIndex > base.Count)
                throw new ArgumentOutOfRangeException("startIndex");
            if((count < 0) || (startIndex > (base.Count - count)))
                throw new ArgumentOutOfRangeException("count");
            if(match == null)
                throw new ArgumentNullException("match");
            int afterLast = startIndex + count;
            for(int i = startIndex; i < afterLast; i++)
                if(match(base[i]))
                    return i;
            return -1;
        }
        public T FindLast(Predicate<T> match)
        {
            if(match == null)
                throw new ArgumentNullException("match");
            for(int i = base.Count - 1; i >= 0; i--)
                if(match(base[i]))
                    return base[i];
            return default(T);
        }
        public int FindLastIndex(Predicate<T> match)
        {
            return FindLastIndex(base.Count - 1, base.Count, match);
        }
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return FindLastIndex(startIndex, startIndex + 1, match);
        }
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            if(startIndex >= base.Count)
                throw new ArgumentOutOfRangeException("startIndex");
            if((count < 0) || (((startIndex - count) + 1) < 0))
                throw new ArgumentOutOfRangeException("count");
            if(match == null)
                throw new ArgumentNullException("match");
            int afterLast = startIndex - count;
            for(int i = startIndex; i > afterLast; i--)
                if(match(base[i]))
                    return i;
            return -1;
        }
        public int LastIndexOf(T item)
        {
            for(int i = base.Count - 1; i >= 0; i--)
                if(item.Equals(base[i]))
                    return i;
            return -1;
        }
        public Set<T> FindAll(Predicate<T> match)
        {
            if(match == null) throw new ArgumentNullException("match");

            Set<T> found = CreateNewSet();
            for(int i = 0; i < base.Count; i++)
                if(match(base[i]))
                    found.Add(base[i]);
            return found;
        }
        #endregion
        #endregion

        #region Not Supported
        void ISet<T>.AddRange(IEnumerable<T> range)
        {
            throw new NotSupportedException();
        }
        void ISet<T>.AddDistinct(T item)
        {
            throw new NotSupportedException();
        }
        void ISet<T>.AddRangeDistinct(IEnumerable<T> range)
        {
            throw new NotSupportedException();
        }
        void ISet<T>.RemoveDuplicates()
        {
            throw new NotSupportedException();
        }
        int ISet<T>.RemoveAll(Predicate<T> match)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region Converter
        public Set<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> convert) where TOutput : IEquatable<TOutput>
        {
            Set<TOutput> ret = new Set<TOutput>();
            foreach(T item in this)
                ret.Add(convert(item));
            return ret;
        }
        #endregion
    }

    public enum SetElementOperation
    {
        Added,
        Removed,
        Moved
    }

    public class SetChangedEventArgs<T> : EventArgs
    {
        private SetElementOperation op;
        private T element;
        private int indexBefore;
        private int indexAfter;

        private SetChangedEventArgs(SetElementOperation op, T element, int indexBefore, int indexAfter)
        {
            this.op = op;
            this.element = element;
            this.indexBefore = indexBefore;
            this.indexAfter = indexAfter;
        }

        public static SetChangedEventArgs<T> Moved(T element, int indexBefore, int indexAfter)
        {
            return new SetChangedEventArgs<T>(SetElementOperation.Moved, element, indexBefore, indexAfter);
        }
        public static SetChangedEventArgs<T> Added(T element, int index)
        {
            return new SetChangedEventArgs<T>(SetElementOperation.Added, element, -1, index);
        }
        public static SetChangedEventArgs<T> Removed(T element, int index)
        {
            return new SetChangedEventArgs<T>(SetElementOperation.Removed, element, index, -1);
        }

        public T Element
        {
            get { return element; }
        }

        public SetElementOperation Operation
        {
            get { return op; }
        }

        public bool ElementAdded
        {
            get { return op == SetElementOperation.Added; }
        }

        public bool ElementRemoved
        {
            get { return op == SetElementOperation.Removed; }
        }

        public bool ElementMoved
        {
            get { return op == SetElementOperation.Moved; }
        }

        public int IndexBefore
        {
            get { return indexBefore; }
        }

        public int IndexAfter
        {
            get { return indexAfter; }
        }
    }
}
