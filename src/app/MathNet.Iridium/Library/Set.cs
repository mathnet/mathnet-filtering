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
using System.Collections.ObjectModel;

namespace MathNet.Numerics
{
    /// <summary>
    /// A generic typed set.
    /// </summary>
    public interface ISet<T> :
        IList<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Create an array with all elements of this set.
        /// </summary>
        T[] ToArray();

        /// <summary>
        /// Checks whether <c>c</c> is a subset of this set.
        /// </summary>
        bool IsSubset(IEnumerable<T> c);

        /// <summary>
        /// Checks whether this set is a subset of <c>c</c>, or if <c>c</c> is a superset of this set.
        /// </summary>
        bool IsSuperset(IEnumerable<T> c);

        /// <summary>
        /// Checks whether this set has elements that are also in <c>c</c>.
        /// </summary>
        /// <remarks>Ingnores duplicate elements.</remarks>
        bool HasEqualElements(IEnumerable<T> c);

        /// <summary>
        /// Check whether this set has an element witch matches the predicate.
        /// </summary>
        bool Exists(Predicate<T> match);

        /// <summary>
        /// Check whether this set has an element witch matches the predicate, and returns it as foundItem-parameter.
        /// </summary>
        bool Exists(Predicate<T> match, out T foundItem);

        /// <summary>
        /// Checks wether all elements of this set match the predicate.
        /// </summary>
        bool TrueForAll(Predicate<T> match);

        /// <summary>
        /// Executes the action for all elements of this set.
        /// </summary>
        void ForEach(Action<T> action);


        /// <summary>
        /// Finds an element of this set that matches the predicate.
        /// </summary>
        T Find(Predicate<T> match);

        /// <summary>
        /// Finds the index of an element of this set that matches the predicate.
        /// </summary>
        int FindIndex(Predicate<T> match);

        /// <summary>
        /// Finds the index (after startIndex) of an element of this set that matches the predicate.
        /// </summary>
        int FindIndex(int startIndex, Predicate<T> match);

        /// <summary>
        /// Finds the index (between startIndex and startIndex+count-1) of an element of this set that matches the predicate.
        /// </summary>
        int FindIndex(int startIndex, int count, Predicate<T> match);

        /// <summary>
        /// Finds the last element of this set that matches the predicate.
        /// </summary>
        T FindLast(Predicate<T> match);

        /// <summary>
        /// Finds the index of the last element of this set that matches the predicate.
        /// </summary>
        int FindLastIndex(Predicate<T> match);

        /// <summary>
        /// Finds the index (after startIndex) of the last element of this set that matches the predicate.
        /// </summary>
        int FindLastIndex(int startIndex, Predicate<T> match);

        /// <summary>
        /// Finds the index (between startIndex and startIndex+count-1) of the last element of this set that matches the predicate.
        /// </summary>
        int FindLastIndex(int startIndex, int count, Predicate<T> match);


        /// <summary>
        /// Finds the last index of element <c>item</c>.
        /// </summary>
        int LastIndexOf(T item);

        /// <summary>
        /// Finds all elements of this set which match the predicate.
        /// </summary>
        Set<T> FindAll(Predicate<T> match);

        /// <summary>
        /// Maps <c>convert</c> to all elements of this set.
        /// </summary>
        Set<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> convert) where TOutput : IEquatable<TOutput>;


        /// <summary>
        /// Add all elements in <c>range</c> to this set.
        /// </summary>
        void AddRange(IEnumerable<T> range);

        /// <summary>
        /// Add <c>item</c> to this set, except if its already there.
        /// </summary>
        void AddDistinct(T item);

        /// <summary>
        /// Add all elements in <c>range</c> to this set, but skips duplicates.
        /// </summary>
        void AddRangeDistinct(IEnumerable<T> range);

        /// <summary>
        /// Remove al duplicate items from this set.
        /// </summary>
        void RemoveDuplicates();

        /// <summary>
        /// Remove all elements that match the predicate from this set.
        /// </summary>
        /// <returns>The number of removed items.</returns>
        int RemoveAll(Predicate<T> match);


        /// <summary>
        /// Sort all elements of this set with respect to the comparer.
        /// </summary>
        void Sort(IComparer<T> comparer);

        /// <summary>
        /// Sort the elements between index and index+count-1 of this set with respect to the comparer.
        /// </summary>
        void Sort(int index, int count, IComparer<T> comparer);
    }

    /// <summary>
    /// A generic typed writeable set.
    /// </summary>
    public class Set<T> :
        Collection<T>,
        ISet<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Event that notifies whenever the set changes.
        /// </summary>
        public event EventHandler<SetChangedEventArgs<T>> OnSetChanged;
        
        ReadOnlySet<T> readonlyWrapper;

        /// <summary>
        /// Create a new set.
        /// </summary>
        public
        Set()
        {
        }

        /// <summary>
        /// Create a new set.
        /// </summary>
        public
        Set(
            IEnumerable<T> initial
            )
        {
            AddRange(initial);
        }

        /// <summary>
        /// Create a new set.
        /// </summary>
        public
        Set(
            params T[] initial
            )
        {
            AddRange(initial);
        }

        /// <summary>
        /// Create a new set.
        /// </summary>
        protected
        Set(
            IList<T> innerList
            )
            : base(innerList)
        {
        }

        /// <summary>
        /// Create a new set.
        /// </summary>
        public
        Set(
            int initialCount
            )
        {
            T defnull = default(T);
            for(int i = 0; i < initialCount; i++)
            {
                Add(defnull);
            }
        }

        #region Factory Methods

        /// <summary>
        /// Override this when you derive from this class.
        /// </summary>
        protected virtual
        Set<T>
        CreateNewSet()
        {
            return new Set<T>();
        }

        /// <summary>
        /// Override this when you derive from this class.
        /// </summary>
        protected virtual
        ReadOnlySet<T>
        CreateNewReadOnlyWrapper(
            IList<T> list
            )
        {
            return new ReadOnlySet<T>(list);
        }

        #endregion

        /// <summary>
        /// Provides a read-only wrapper to this set. The wrapper will stay connected to this set's items but can't change them.
        /// </summary>
        public ReadOnlySet<T> AsReadOnly
        {
            get
            {
                if(readonlyWrapper == null)
                {
                    readonlyWrapper = CreateNewReadOnlyWrapper(Items);
                }

                return readonlyWrapper;
            }
        }

        /// <summary>
        /// Create an array with all elements of this set.
        /// </summary>
        public
        T[]
        ToArray()
        {
            T[] array = new T[Count];
            CopyTo(array, 0);
            return array;
        }

        #region Set Behaviour

        /////// <summary>
        /////// If true, the set elements have a specific order.
        /////// </summary>
        /////// <remarks>Two sequences with the same elements but in a different order are not equal.</remarks>
        ////public bool IsSequence
        ////{
        ////}

        /////// <summary>
        /////// If true, the set may not contain any element more than once.
        /////// </summary>
        ////public bool IsDistinct
        ////{
        ////}

        #endregion

        #region Element Manipulation

        /// <summary>
        /// Add all elements in <c>range</c> to this set.
        /// </summary>
        public
        void
        AddRange(
            IEnumerable<T> range
            )
        {
            foreach(T item in range)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Add <c>item</c> to this set, except if its already there.
        /// </summary>
        public
        void
        AddDistinct(
            T item
            )
        {
            if(!Contains(item))
            {
                Add(item);
            }
        }

        /// <summary>
        /// Add all elements in <c>range</c> to this set, but skips duplicates.
        /// </summary>
        public
        void
        AddRangeDistinct(
            IEnumerable<T> range
            )
        {
            foreach(T item in range)
            {
                if(!Contains(item))
                {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Remove al duplicate items from this set.
        /// </summary>
        public
        void
        RemoveDuplicates()
        {
            Dictionary<T, object> table = new Dictionary<T, object>();
            for(int i = Count - 1; i >= 0; i--)
            {
                T item = base[i];
                if(table.ContainsKey(item))
                {
                    RemoveAt(i);
                }
                else
                {
                    table.Add(item, null);
                }
            }
        }

        /// <summary>
        /// Replace elements starting from the first element with all elements in <c>range</c>.
        /// </summary>
        public
        void
        ReplaceRange(
            IEnumerable<T> range
            )
        {
            int i = 0;
            foreach(T item in range)
            {
                base[i++] = item;
            }
        }

        /// <summary>
        /// Called whenever an item is insert into this set.
        /// </summary>
        protected override
        void
        InsertItem(
            int index,
            T item
            )
        {
            EventHandler<SetChangedEventArgs<T>> handler = OnSetChanged;
            if(handler == null)
            {
                base.InsertItem(index, item);
            }
            else
            {
                base.InsertItem(index, item);
                handler(this, SetChangedEventArgs<T>.Added(item, index));
                for(int i = index + 1; i < Count; i++)
                {
                    handler(this, SetChangedEventArgs<T>.Moved(base[i], i - 1, i));
                }
            }
        }

        /// <summary>
        /// Called whenever an item is removed from this set.
        /// </summary>
        protected override
        void
        RemoveItem(
            int index
            )
        {
            EventHandler<SetChangedEventArgs<T>> handler = OnSetChanged;
            if(handler == null)
            {
                base.RemoveItem(index);
            }
            else
            {
                handler(this, SetChangedEventArgs<T>.Removed(base[index], index));
                base.RemoveItem(index);
                for(int i = index; i < Count; i++)
                {
                    handler(this, SetChangedEventArgs<T>.Moved(base[i], i + 1, i));
                }
            }
        }

        /// <summary>
        /// Called whenever an item is reset in this set.
        /// </summary>
        protected override
        void
        SetItem(
            int index,
            T item
            )
        {
            EventHandler<SetChangedEventArgs<T>> handler = OnSetChanged;
            if(handler == null)
            {
                base.SetItem(index, item);
            }
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
        public
        bool
        IsSubset(
            IEnumerable<T> c
            )
        {
            foreach(T item in c)
            {
                if(!Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether this set is a subset of <c>c</c>, or if <c>c</c> is a superset of this set.
        /// </summary>
        public
        bool
        IsSuperset(
            IEnumerable<T> c
            )
        {
            Dictionary<T, object> table = new Dictionary<T, object>();

            foreach(T item in c)
            {
                if(!table.ContainsKey(item))
                {
                    table.Add(item, null);
                }
            }

            foreach(T item in this)
            {
                if(!table.ContainsKey(item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether this set has elements that are also in <c>c</c>.
        /// </summary>
        /// <remarks>Ingnores duplicate elements.</remarks>
        public
        bool
        HasEqualElements(
            IEnumerable<T> c
            )
        {
            // TODO: quite inelegant, find better algorithm.

            Dictionary<T, object> table = new Dictionary<T, object>();

            foreach(T item in c)
            {
                if(!table.ContainsKey(item))
                {
                    table.Add(item, null);
                    if(!Contains(item))
                    {
                        return false;
                    }
                }
            }

            foreach(T item in this)
            {
                if(!table.ContainsKey(item))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Concatenation (Not Distinct)

        /// <summary>
        /// Returns a collection resulting from the concatenation with the items of <c>c</c>.
        /// </summary>
        /// <remarks>Not distinct: the resulting collection may contain several identical elements.</remarks>
        public
        Set<T>
        Concatenate(
            IEnumerable<T> c
            )
        {
            return Concatenate(this, c);
        }

        /// <summary>
        /// Concatenates this set with the items of <c>c</c>, by directly modifing this set.
        /// </summary>
        public
        void
        ConcatenateInplace(
            IEnumerable<T> c
            )
        {
            AddRange(c);
        }

        /// <summary>
        /// Returns a collection resulting from the concatenation
        /// from <c>c1</c> and <c>c2</c>.
        /// </summary>
        /// <remarks>Not distinct: the resulting collection may contain several identical elements.</remarks>
        public static
        Set<T>
        Concatenate(
            IEnumerable<T> c1,
            IEnumerable<T> c2
            )
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
        public
        Set<T>
        Union(
            IEnumerable<T> c
            )
        {
            return Union(this, c);
        }

        /// <summary>
        /// Creates the union of this set with the items of <c>c</c>, by directly modifing this set.
        /// </summary>
        public
        void
        UnionInplace(
            IEnumerable<T> c
            )
        {
            RemoveDuplicates();
            AddRangeDistinct(c);
        }

        /// <summary>
        /// Returns a collection resulting from the union of the items
        /// of <c>c1</c> and <c>c2</c>.
        /// </summary>
        /// <remarks>Distinct: the resulting collection may not contain several identical elements.</remarks>
        public static
        Set<T>
        Union(
            IEnumerable<T> c1,
            IEnumerable<T> c2
            )
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
        public
        Set<T>
        Intersect(
            IEnumerable<T> c
            )
        {
            return Intersect(this, c);
        }

        /// <summary>
        /// Creates the intersection of this set with the items of <c>c</c>, by directly modifing this set.
        /// </summary>
        public
        void
        IntersectInplace(
            IEnumerable<T> c
            )
        {
            Dictionary<T, object> table = new Dictionary<T, object>();

            foreach(T item in c)
            {
                if(!table.ContainsKey(item))
                {
                    table.Add(item, null);
                }
            }

            for(int i = Count - 1; i >= 0; i--)
            {
                if(!table.ContainsKey(base[i]))
                {
                    RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Returns a collection resulting from the intersection of the items
        /// of <c>c1</c> and <c>c2</c>.
        /// </summary>
        /// <remarks>Distinct: the resulting collection may not contain several identical elements.</remarks>
        public static
        Set<T>
        Intersect(
            IEnumerable<T> c1,
            IEnumerable<T> c2
            )
        {
            Dictionary<T, object> table = new Dictionary<T, object>();
            foreach(T item in c1)
            {
                if(!table.ContainsKey(item))
                {
                    table.Add(item, null);
                }
            }

            Set<T> s = new Set<T>();
            foreach(T item in c2)
            {
                if(table.ContainsKey(item) && !s.Contains(item))
                {
                    s.Add(item);
                }
            }

            return s;
        }

        #endregion

        #region Subtraction (Not Distinct)

        /// <summary>
        /// Returns a collection resulting from the subtraction of the items of <c>c</c>.
        /// </summary>
        /// <remarks>Not distinct: the resulting collection may contain several identical elements.</remarks>
        public
        Set<T>
        Subtract(
            IEnumerable<T> c
            )
        {
            return Subtract(this, c);
        }

        /// <summary>
        /// Creates the subtraction of this set with the items of <c>c</c>, by directly modifing this set.
        /// </summary>
        public
        void
        SubtractInplace(
            IEnumerable<T> c
            )
        {
            Dictionary<T, object> table = new Dictionary<T, object>();

            foreach(T item in c)
            {
                if(!table.ContainsKey(item))
                {
                    table.Add(item, null);
                }
            }

            for(int i = Count - 1; i >= 0; i--)
            {
                if(table.ContainsKey(base[i]))
                {
                    RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Returns a collection resulting from the subtraction of the items
        /// of <c>c2</c> to the collection <c>c1</c>.
        /// </summary>
        /// <remarks>Not distinct: the resulting collection may contain several identical elements.</remarks>
        public static
        Set<T>
        Subtract(
            IEnumerable<T> c1,
            IEnumerable<T> c2
            )
        {
            Dictionary<T, object> table = new Dictionary<T, object>();
            foreach(T item in c2)
            {
                if(!table.ContainsKey(item))
                {
                    table.Add(item, null);
                }
            }

            Set<T> s = new Set<T>();
            foreach(T item in c1)
            {
                if(!table.ContainsKey(item))
                {
                    s.Add(item);
                }
            }

            return s;
        }

        #endregion

        #region Lambdas

        /// <summary>
        /// Check whether this set has an element witch matches the predicate.
        /// </summary>
        public
        bool
        Exists(
            Predicate<T> match
            )
        {
            return (FindIndex(match) != -1);
        }

        /// <summary>
        /// Check whether this set has an element witch matches the predicate, and returns it as foundItem-parameter.
        /// </summary>
        public
        bool
        Exists(
            Predicate<T> match,
            out T foundItem
            )
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

        /// <summary>
        /// Checks wether all elements of this set match the predicate.
        /// </summary>
        public
        bool
        TrueForAll(
            Predicate<T> match
            )
        {
            if(match == null)
            {
                throw new ArgumentNullException("match");
            }

            for(int i = 0; i < Count; i++)
            {
                if(!match(base[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Executes the action for all elements of this set.
        /// </summary>
        public
        void
        ForEach(
            Action<T> action
            )
        {
            if(action == null)
            {
                throw new ArgumentNullException("action");
            }

            for(int i = 0; i < Count; i++)
            {
                action(base[i]);
            }
        }

        #region Find Lambdas

        /// <summary>
        /// Finds an element of this set that matches the predicate.
        /// </summary>
        public
        T
        Find(
            Predicate<T> match
            )
        {
            if(match == null)
            {
                throw new ArgumentNullException("match");
            }

            for(int i = 0; i < Count; i++)
            {
                if(match(base[i]))
                {
                    return base[i];
                }
            }

            return default(T);
        }

        /// <summary>
        /// Finds the index of an element of this set that matches the predicate.
        /// </summary>
        public
        int
        FindIndex(
            Predicate<T> match
            )
        {
            return FindIndex(0, Count, match);
        }

        /// <summary>
        /// Finds the index (after startIndex) of an element of this set that matches the predicate.
        /// </summary>
        public
        int
        FindIndex(
            int startIndex,
            Predicate<T> match
            )
        {
            return FindIndex(startIndex, Count - startIndex, match);
        }

        /// <summary>
        /// Finds the index (between startIndex and startIndex+count-1) of an element of this set that matches the predicate.
        /// </summary>
        public
        int
        FindIndex(
            int startIndex,
            int count,
            Predicate<T> match
            )
        {
            if(startIndex > Count)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            if((count < 0) || (startIndex > (Count - count)))
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if(match == null)
            {
                throw new ArgumentNullException("match");
            }

            int afterLast = startIndex + count;
            for(int i = startIndex; i < afterLast; i++)
            {
                if(match(base[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the last element of this set that matches the predicate.
        /// </summary>
        public
        T
        FindLast(
            Predicate<T> match
            )
        {
            if(match == null)
            {
                throw new ArgumentNullException("match");
            }

            for(int i = Count - 1; i >= 0; i--)
            {
                if(match(base[i]))
                {
                    return base[i];
                }
            }

            return default(T);
        }

        /// <summary>
        /// Finds the index of the last element of this set that matches the predicate.
        /// </summary>
        public
        int
        FindLastIndex(
            Predicate<T> match
            )
        {
            return FindLastIndex(Count - 1, Count, match);
        }

        /// <summary>
        /// Finds the index (after startIndex) of the last element of this set that matches the predicate.
        /// </summary>
        public
        int
        FindLastIndex(
            int startIndex,
            Predicate<T> match
            )
        {
            return FindLastIndex(startIndex, startIndex + 1, match);
        }

        /// <summary>
        /// Finds the index (between startIndex and startIndex+count-1) of the last element of this set that matches the predicate.
        /// </summary>
        public
        int
        FindLastIndex(
            int startIndex,
            int count,
            Predicate<T> match
            )
        {
            if(startIndex >= Count)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            if((count < 0) || (((startIndex - count) + 1) < 0))
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if(match == null)
            {
                throw new ArgumentNullException("match");
            }

            int afterLast = startIndex - count;
            for(int i = startIndex; i > afterLast; i--)
            {
                if(match(base[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the last index of element <c>item</c>.
        /// </summary>
        public
        int
        LastIndexOf(
            T item
            )
        {
            for(int i = Count - 1; i >= 0; i--)
            {
                if(item.Equals(base[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds all elements of this set which match the predicate.
        /// </summary>
        public
        Set<T>
        FindAll(
            Predicate<T> match
            )
        {
            if(match == null)
            {
                throw new ArgumentNullException("match");
            }

            Set<T> found = CreateNewSet();
            for(int i = 0; i < Count; i++)
            {
                if(match(base[i]))
                {
                    found.Add(base[i]);
                }
            }

            return found;
        }

        #endregion

        /// <summary>
        /// Remove all elements that match the predicate from this set.
        /// </summary>
        /// <returns>The number of removed items.</returns>
        public
        int
        RemoveAll(
            Predicate<T> match
            )
        {
            int cnt = 0;
            for(int i = Count - 1; i >= 0; i--)
            {
                if(match(base[i]))
                {
                    cnt++;
                    RemoveAt(i);
                }
            }

            return cnt;
        }

        #endregion

        #region Converter

        /// <summary>
        /// Maps <c>convert</c> to all elements of this set.
        /// </summary>
        public
        Set<TOutput>
        ConvertAll<TOutput>(
            Converter<T, TOutput> convert
            ) where TOutput : IEquatable<TOutput>
        {
            Set<TOutput> ret = new Set<TOutput>();

            foreach(T item in this)
            {
                ret.Add(convert(item));
            }

            return ret;
        }

        #endregion

        #region Sorting

        /// <summary>
        /// Sort all elements of this set with respect to the comparer.
        /// </summary>
        public
        void
        Sort(
            IComparer<T> comparer
            )
        {
            Sort(0, Count, comparer);
        }

        /// <summary>
        /// Sort the elements between index and index+count-1 of this set with respect to the comparer.
        /// </summary>
        public
        void
        Sort(
            int index,
            int count,
            IComparer<T> comparer
            )
        {
            IList<T> items = Items;

            List<T> list = items as List<T>;
            if(list != null)
            {
                list.Sort(index, count, comparer);
                return;
            }

            T[] array = items as T[];
            if(array != null)
            {
                Array.Sort<T>(array, index, count, comparer);
                return;
            }

            throw new NotSupportedException();
        }

        #endregion
    }

    /// <summary>
    /// A generic typed set which is read-only.
    /// </summary>
    public class ReadOnlySet<T> : 
        ReadOnlyCollection<T>,
        ISet<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Create a read-only set.
        /// </summary>
        public
        ReadOnlySet(
            IList<T> list
            ) : base(list)
        {
        }

        /// <summary>
        /// Create an array with all elements of this set.
        /// </summary>
        public
        T[]
        ToArray()
        {
            T[] array = new T[Count];
            CopyTo(array, 0);
            return array;
        }

        /// <summary>
        /// Override this when you derive from this class.
        /// </summary>
        protected virtual
        Set<T>
        CreateNewSet()
        {
            return new Set<T>();
        }

        #region Subsets

        /// <summary>
        /// Checks whether <c>c</c> is a subset of this set.
        /// </summary>
        public
        bool
        IsSubset(
            IEnumerable<T> c
            )
        {
            foreach(T item in c)
            {
                if(!Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether this set is a subset of <c>c</c>, or if <c>c</c> is a superset of this set.
        /// </summary>
        public
        bool
        IsSuperset(
            IEnumerable<T> c
            )
        {
            Dictionary<T, object> table = new Dictionary<T, object>();

            foreach(T item in c)
            {
                if(!table.ContainsKey(item))
                {
                    table.Add(item, null);
                }
            }

            foreach(T item in this)
            {
                if(!table.ContainsKey(item))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks whether this set has elements that are also in <c>c</c>.
        /// </summary>
        /// <remarks>Ingnores duplicate elements.</remarks>
        public
        bool
        HasEqualElements(
            IEnumerable<T> c
            )
        {
            // TODO: quite inelegant, find better algorithm.

            Dictionary<T, object> table = new Dictionary<T, object>();
            foreach(T item in c)
            {
                if(!table.ContainsKey(item))
                {
                    table.Add(item, null);
                    if(!Contains(item))
                    {
                        return false;
                    }
                }
            }

            foreach(T item in this)
            {
                if(!table.ContainsKey(item))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Lambdas

        /// <summary>
        /// Check whether this set has an element witch matches the predicate.
        /// </summary>
        public
        bool
        Exists(
            Predicate<T> match
            )
        {
            return (FindIndex(match) != -1);
        }

        /// <summary>
        /// Check whether this set has an element witch matches the predicate, and returns it as foundItem-parameter.
        /// </summary>
        public
        bool
        Exists(
            Predicate<T> match,
            out T foundItem
            )
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

        /// <summary>
        /// Checks wether all elements of this set match the predicate.
        /// </summary>
        public
        bool
        TrueForAll(
            Predicate<T> match
            )
        {
            if(match == null)
            {
                throw new ArgumentNullException("match");
            }

            for(int i = 0; i < Count; i++)
            {
                if(!match(base[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Executes the action for all elements of this set.
        /// </summary>
        public
        void
        ForEach(
            Action<T> action
            )
        {
            if(action == null)
            {
                throw new ArgumentNullException("action");
            }

            for(int i = 0; i < Count; i++)
            {
                action(base[i]);
            }
        }

        #region Find Lambdas

        /// <summary>
        /// Finds an element of this set that matches the predicate.
        /// </summary>
        public
        T
        Find(
            Predicate<T> match
            )
        {
            if(match == null)
            {
                throw new ArgumentNullException("match");
            }

            for(int i = 0; i < Count; i++)
            {
                if(match(base[i]))
                {
                    return base[i];
                }
            }

            return default(T);
        }

        /// <summary>
        /// Finds the index of an element of this set that matches the predicate.
        /// </summary>
        public
        int
        FindIndex(
            Predicate<T> match
            )
        {
            return FindIndex(0, Count, match);
        }

        /// <summary>
        /// Finds the index (after startIndex) of an element of this set that matches the predicate.
        /// </summary>
        public
        int
        FindIndex(
            int startIndex,
            Predicate<T> match
            )
        {
            return FindIndex(startIndex, Count - startIndex, match);
        }

        /// <summary>
        /// Finds the index (between startIndex and startIndex+count-1) of an element of this set that matches the predicate.
        /// </summary>
        public
        int
        FindIndex(
            int startIndex,
            int count,
            Predicate<T> match
            )
        {
            if(startIndex > Count)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            if((count < 0) || (startIndex > (Count - count)))
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if(match == null)
            {
                throw new ArgumentNullException("match");
            }

            int afterLast = startIndex + count;
            for(int i = startIndex; i < afterLast; i++)
            {
                if(match(base[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the last element of this set that matches the predicate.
        /// </summary>
        public
        T
        FindLast(
            Predicate<T> match
            )
        {
            if(match == null)
            {
                throw new ArgumentNullException("match");
            }

            for(int i = Count - 1; i >= 0; i--)
            {
                if(match(base[i]))
                {
                    return base[i];
                }
            }

            return default(T);
        }

        /// <summary>
        /// Finds the index of the last element of this set that matches the predicate.
        /// </summary>
        public
        int
        FindLastIndex(
            Predicate<T> match
            )
        {
            return FindLastIndex(Count - 1, Count, match);
        }

        /// <summary>
        /// Finds the index (after startIndex) of the last element of this set that matches the predicate.
        /// </summary>
        public
        int
        FindLastIndex(
            int startIndex,
            Predicate<T> match
            )
        {
            return FindLastIndex(startIndex, startIndex + 1, match);
        }

        /// <summary>
        /// Finds the index (between startIndex and startIndex+count-1) of the last element of this set that matches the predicate.
        /// </summary>
        public
        int
        FindLastIndex(
            int startIndex,
            int count,
            Predicate<T> match
            )
        {
            if(startIndex >= Count)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }

            if((count < 0) || (((startIndex - count) + 1) < 0))
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if(match == null)
            {
                throw new ArgumentNullException("match");
            }

            int afterLast = startIndex - count;
            for(int i = startIndex; i > afterLast; i--)
            {
                if(match(base[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the last index of element <c>item</c>.
        /// </summary>
        public
        int
        LastIndexOf(
            T item
            )
        {
            for(int i = Count - 1; i >= 0; i--)
            {
                if(item.Equals(base[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds all elements of this set which match the predicate.
        /// </summary>
        public
        Set<T>
        FindAll(
            Predicate<T> match
            )
        {
            if(match == null)
            {
                throw new ArgumentNullException("match");
            }

            Set<T> found = CreateNewSet();
            for(int i = 0; i < Count; i++)
            {
                if(match(base[i]))
                {
                    found.Add(base[i]);
                }
            }

            return found;
        }

        #endregion

        #endregion

        #region Not Supported

        /// <summary>
        /// Add all elements in <c>range</c> to this set.
        /// </summary>
        /// <remarks>
        /// This is not supported by this implementation.
        /// </remarks>
        /// <exception cref="NotSupportedException" />
        void
        ISet<T>.AddRange(
            IEnumerable<T> range
            )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Add <c>item</c> to this set, except if its already there.
        /// </summary>
        /// <remarks>
        /// This is not supported by this implementation.
        /// </remarks>
        /// <exception cref="NotSupportedException" />
        void
        ISet<T>.AddDistinct(
            T item
            )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Add all elements in <c>range</c> to this set, but skips duplicates.
        /// </summary>
        /// <remarks>
        /// This is not supported by this implementation.
        /// </remarks>
        /// <exception cref="NotSupportedException" />
        void
        ISet<T>.AddRangeDistinct(
            IEnumerable<T> range
            )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Remove al duplicate items from this set.
        /// </summary>
        /// <remarks>
        /// This is not supported by this implementation.
        /// </remarks>
        /// <exception cref="NotSupportedException" />
        void
        ISet<T>.RemoveDuplicates()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Remove all elements that match the predicate from this set.
        /// </summary>
        /// <remarks>
        /// This is not supported by this implementation.
        /// </remarks>
        /// <exception cref="NotSupportedException" />
        int
        ISet<T>.RemoveAll(
            Predicate<T> match
            )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sort all elements of this set with respect to the comparer.
        /// </summary>
        /// <remarks>
        /// This is not supported by this implementation.
        /// </remarks>
        /// <exception cref="NotSupportedException" />
        void
        ISet<T>.Sort(
            IComparer<T> comparer
            )
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sort the elements between index and index+count-1 of this set with respect to the comparer.
        /// </summary>
        /// <remarks>
        /// This is not supported by this implementation.
        /// </remarks>
        /// <exception cref="NotSupportedException" />
        void
        ISet<T>.Sort(
            int index,
            int count,
            IComparer<T> comparer
            )
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Converter

        /// <summary>
        /// Maps <c>convert</c> to all elements of this set.
        /// </summary>
        public
        Set<TOutput>
        ConvertAll<TOutput>(
            Converter<T, TOutput> convert
            ) where TOutput : IEquatable<TOutput>
        {
            Set<TOutput> ret = new Set<TOutput>();

            foreach(T item in this)
            {
                ret.Add(convert(item));
            }

            return ret;
        }

        #endregion
    }

    /// <summary>
    /// Set Element Operations
    /// </summary>
    public enum SetElementOperation
    {
        /// <summary>
        /// Add elements to the set
        /// </summary>
        Added,

        /// <summary>
        /// Remove elements to the set
        /// </summary>
        Removed,

        /// <summary>
        /// Move elements inside of the set
        /// </summary>
        Moved
    }

    /// <summary>
    /// Event argument used for notifying about changes in a set.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SetChangedEventArgs<T> :
        EventArgs
    {
        SetElementOperation op;
        T element;
        int indexBefore;
        int indexAfter;

        SetChangedEventArgs(
            SetElementOperation op,
            T element,
            int indexBefore,
            int indexAfter
            )
        {
            this.op = op;
            this.element = element;
            this.indexBefore = indexBefore;
            this.indexAfter = indexAfter;
        }

        /// <summary>
        /// Build changed event args when elevents have been moved inside of a set.
        /// </summary>
        /// <param name="element">The element which has been moved.</param>
        /// <param name="indexBefore">The previous index of the moved element.</param>
        /// <param name="indexAfter">The new index of the moved element.</param>
        public static
        SetChangedEventArgs<T>
        Moved(
            T element,
            int indexBefore,
            int indexAfter
            )
        {
            return new SetChangedEventArgs<T>(SetElementOperation.Moved, element, indexBefore, indexAfter);
        }

        /// <summary>
        /// Build changed event args when elevents have been added to a set.
        /// </summary>
        /// <param name="element">The element which as been added.</param>
        /// <param name="index">The index where the element was added.</param>
        public static
        SetChangedEventArgs<T>
        Added(
            T element,
            int index
            )
        {
            return new SetChangedEventArgs<T>(SetElementOperation.Added, element, -1, index);
        }

        /// <summary>
        /// Build changed event args when elevents have been removed from a set.
        /// </summary>
        /// <param name="element">The element which has been removed.</param>
        /// <param name="index">The previous index of the removed element.</param>
        public static
        SetChangedEventArgs<T>
        Removed(
            T element,
            int index
            )
        {
            return new SetChangedEventArgs<T>(SetElementOperation.Removed, element, index, -1);
        }

        /// <summary>
        /// The element that changed.
        /// </summary>
        public T Element
        {
            get { return element; }
        }

        /// <summary>
        /// The operation that was applied to the element.
        /// </summary>
        public SetElementOperation Operation
        {
            get { return op; }
        }

        /// <summary>
        /// True if the element was added to the set.
        /// </summary>
        public bool ElementAdded
        {
            get { return op == SetElementOperation.Added; }
        }

        /// <summary>
        /// True if the element was removed from the set.
        /// </summary>
        public bool ElementRemoved
        {
            get { return op == SetElementOperation.Removed; }
        }

        /// <summary>
        /// True if the element was moved inside of the set.
        /// </summary>
        public bool ElementMoved
        {
            get { return op == SetElementOperation.Moved; }
        }

        /// <summary>
        /// The index of the element before the operation.
        /// </summary>
        public int IndexBefore
        {
            get { return indexBefore; }
        }

        /// <summary>
        /// The index of the element after the operation.
        /// </summary>
        public int IndexAfter
        {
            get { return indexAfter; }
        }
    }
}
