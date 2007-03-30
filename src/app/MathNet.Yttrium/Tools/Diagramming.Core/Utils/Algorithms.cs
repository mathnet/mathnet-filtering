//******************************
// Written by Peter Golde
// Copyright (connector) 2004-2005, Wintellect
//
// Use and restribution of this code is subject to the license agreement 
// contained in the file "License.txt" accompanying this file.
//******************************

using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 419  // Ambigious cref in XML comment

namespace Netron.Diagramming.Core
{ 

    /// <summary>
    /// Algorithms contains a number of static methods that implement
    /// algorithms that work on collections. Most of the methods deal with
    /// the standard generic collection interfaces such as IEnumerable&lt;T&gt;,
    /// ICollection&lt;T&gt; and IList&lt;T&gt;.
    /// </summary>
    public static class Algorithms
    {

        /// <summary>
        /// Create an IEnumerable that enumerates an array. Make sure that only enumerable stuff
        /// is used and no downcasts to ICollection are taken advantage of.
        /// </summary>
        /// <param name="array">An array.</param>
        /// <returns>An IEnumerable cast of the array</returns>
        /// <typeparam name="T">a </typeparam>
        /// <example>The following code
        /// <code>
        /// 
        /// </code>
        /// </example>
        public static IEnumerable<T> EnumerableFromArray<T>(T[] array)
        {
            foreach (T t in array)
                yield return t;
        }


      
        #region String representations

        /// <summary>
        /// Gets a string representation of the elements in the collection.
        /// The string representation starts with "{", has a list of items separated
        /// by commas (","), and ends with "}". Each item in the collection is 
        /// converted to a string by calling its ToString method (null is represented by "null").
        /// Contained collections (except strings) are recursively converted to strings by this method.
        /// </summary>
        /// <param name="collection">A collection to get the string representation of.</param>
        /// <returns>The string representation of the collection. If <paramref name="collection"/> is null, then the string "null" is returned.</returns>
        internal static string ToString<T>(IEnumerable<T> collection)
        {
            return collection.GetType().Name;// ToString(collection);
        }

   

        /// <summary>
        /// Gets a string representation of the mappings in a dictionary.
        /// The string representation starts with "{", has a list of mappings separated
        /// by commas (", "), and ends with "}". Each mapping is represented
        /// by "key->value". Each key and value in the dictionary is 
        /// converted to a string by calling its ToString method (null is represented by "null").
        /// Contained collections (except strings) are recursively converted to strings by this method.
        /// </summary>
        /// <param name="dictionary">A dictionary to get the string representation of.</param>
        /// <returns>The string representation of the collection, or "null" 
        /// if <paramref name="dictionary"/> is null.</returns>
        internal static string ToString<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            bool firstItem = true;

            if (dictionary == null)
                return "null";

            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            builder.Append("{");

            // Call ToString on each item and put it in.
            foreach (KeyValuePair<TKey, TValue> pair in dictionary) {
                if (!firstItem)
                    builder.Append(", ");

                if (pair.Key == null)
                    builder.Append("null");
               
                else
                    builder.Append(pair.Key.ToString());

                builder.Append("->");

                if (pair.Value == null)
                    builder.Append("null");
               
                else
                    builder.Append(pair.Value.ToString());


                firstItem = false;
            }

            builder.Append("}");
            return builder.ToString();
        }

        #endregion String representations

  

  

   

        #region Predicate operations

        /// <summary>
        /// Determines if a collection contains any item that satisfies the condition
        /// defined by <paramref name="predicate"/>.
        /// </summary>
        /// <param name="collection">The collection to check all the items in.</param>
        /// <param name="predicate">A delegate that defines the condition to check for.</param>
        /// <returns>True if the collection contains one or more items that satisfy the condition
        /// defined by <paramref name="predicate"/>. False if the collection does not contain
        /// an item that satisfies <paramref name="predicate"/>.</returns>
        internal static bool Exists<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            foreach (T item in collection) {
                if (predicate(item))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if all of the items in the collection satisfy the condition
        /// defined by <paramref name="predicate"/>.
        /// </summary>
        /// <param name="collection">The collection to check all the items in.</param>
        /// <param name="predicate">A delegate that defines the condition to check for.</param>
        /// <returns>True if all of the items in the collection satisfy the condition
        /// defined by <paramref name="predicate"/>, or if the collection is empty. False if one or more items
        /// in the collection do not satisfy <paramref name="predicate"/>.</returns>
        internal static bool TrueForAll<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            foreach (T item in collection) {
                if (!predicate(item))
                    return false;
            }

            return true;
        }
         /*
        /// <summary>
        /// Counts the number of items in the collection that satisfy the condition
        /// defined by <paramref name="predicate"/>.
        /// </summary>
        /// <param name="collection">The collection to count items in.</param>
        /// <param name="predicate">A delegate that defines the condition to check for.</param>
        /// <returns>The number of items in the collection that satisfy <paramref name="predicate"/>.</returns>
        public static int CountWhere<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            int count = 0;
            foreach (T item in collection)
            {
                if (predicate(item))
                    ++count;
            }

            return count;
        }
        */
        /// <summary>
        /// Removes all the items in the collection that satisfy the condition
        /// defined by <paramref name="predicate"/>.
        /// </summary>
        /// <remarks>If the collection if an array or implements IList&lt;T&gt;, an efficient algorithm that
        /// compacts items is used. If not, then ICollection&lt;T&gt;.Remove is used
        /// to remove items from the collection. If the collection is an array or fixed-size list,
        /// the non-removed elements are placed, in order, at the beginning of
        /// the list, and the remaining list items are filled with a default value (0 or null).</remarks>
        /// <param name="collection">The collection to check all the items in.</param>
        /// <param name="predicate">A delegate that defines the condition to check for.</param>
        /// <returns>Returns a collection of the items that were removed. This collection contains the
        /// items in the same order that they orginally appeared in <paramref name="collection"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        internal static ICollection<T> RemoveWhere<T>(ICollection<T> collection, Predicate<T> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (predicate == null)
                throw new ArgumentNullException("predicate");
          

            IList<T> list = collection as IList<T>;
            if (list != null) {
                T item;
                int i = -1, j = 0;
                int listCount = list.Count;
                List<T> removed = new List<T>();

                // Remove item where predicate is true, compressing items to lower in the list. This is much more
                // efficient than the naive algorithm that uses IList<T>.Remove().
                while (j < listCount) {
                    item = list[j];
                    if (predicate(item)) {
                        removed.Add(item);
                    }
                    else {
                        ++i;
                        if (i != j)
                            list[i] = item;
                    }
                    ++j;
                }

                ++i;
                if (i < listCount) {
                    // remove items from the end.
                    if (list is IList && ((IList)list).IsFixedSize) {
                        // An array or similar. Null out the last elements.
                        while (i < listCount)
                            list[i++] = default(T);
                    }
                    else {
                        // Normal list.
                        while (i < listCount) {
                            list.RemoveAt(listCount - 1);
                            --listCount;
                        }
                    }
                }

                return removed;
            }
            else {
                // We have to copy all the items to remove to a List, because collections can't be modifed 
                // during an enumeration.
                List<T> removed = new List<T>();

                foreach (T item in collection)
                    if (predicate(item))
                        removed.Add(item);

                foreach (T item in removed)
                    collection.Remove(item);

                return removed;
            }
        }
        /*
        /// <summary>
        /// Convert a collection of items by applying a delegate to each item in the collection. The resulting collection
        /// contains the result of applying <paramref name="converter"/> to each item in <paramref name="sourceCollection"/>, in
        /// order.
        /// </summary>
        /// <typeparam name="TSource">The type of items in the collection to convert.</typeparam>
        /// <typeparam name="TDest">The type each item is being converted to.</typeparam>
        /// <param name="sourceCollection">The collection of item being converted.</param>
        /// <param name="converter">A delegate to the method to call, passing each item in <paramref name="sourceCollection"/>.</param>
        /// <returns>The resulting collection from applying <paramref name="converter"/> to each item in <paramref name="sourceCollection"/>, in
        /// order.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="sourceCollection"/> or <paramref name="converter"/> is null.</exception>
        public static IEnumerable<TDest> Convert<TSource, TDest>(IEnumerable<TSource> sourceCollection, Converter<TSource, TDest> converter)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException("sourceCollection");
            if (converter == null)
                throw new ArgumentNullException("converter");

            foreach (TSource sourceItem in sourceCollection)
                yield return converter(sourceItem);
        }
          */
        /// <summary>
        /// Creates a delegate that converts keys to values by used a dictionary to map values. Keys
        /// that a not present in the dictionary are converted to the default value (zero or null).
        /// </summary>
        /// <remarks>This delegate can be used as a parameter in Convert or ConvertAll methods to convert
        /// entire collections.</remarks>
        /// <param name="dictionary">The dictionary used to perform the conversion.</param>
        /// <returns>A delegate to a method that converts keys to values. </returns>
        internal static Converter<TKey, TValue> GetDictionaryConverter<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            return GetDictionaryConverter(dictionary, default(TValue));
        }

        /// <summary>
        /// Creates a delegate that converts keys to values by used a dictionary to map values. Keys
        /// that a not present in the dictionary are converted to a supplied default value.
        /// </summary>
        /// <remarks>This delegate can be used as a parameter in Convert or ConvertAll methods to convert
        /// entire collections.</remarks>
        /// <param name="dictionary">The dictionary used to perform the conversion.</param>
        /// <param name="defaultValue">The result of the conversion for keys that are not present in the dictionary.</param>
        /// <returns>A delegate to a method that converts keys to values. </returns>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is null.</exception>
        internal static Converter<TKey, TValue> GetDictionaryConverter<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TValue defaultValue)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            return delegate(TKey key) {
                TValue value;
                if (dictionary.TryGetValue(key, out value))
                    return value;
                else
                    return defaultValue;
            };
        }

        /// <summary>
        /// Performs the specified action on each item in a collection.
        /// </summary>
        /// <param name="collection">The collection to process.</param>
        /// <param name="action">An Action delegate which is invoked for each item in <paramref name="collection"/>.</param>
        internal static void ForEach<T>(IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (action == null)
                throw new ArgumentNullException("action");

            foreach (T item in collection)
                action(item);
        }


        #endregion Predicate operations



        #region Sorting
        /// <summary>
        /// Sorts a list or array in place.
        /// </summary>
        /// <remarks><para>The Quicksort algorithms is used to sort the items. In virtually all cases,
        /// this takes time O(N log N), where N is the number of items in the list.</para>
        /// <para>Values are compared by using the IComparable&lt;T&gt;
        /// interfaces implementation on the type T.</para>
        /// <para>Although arrays cast to IList&lt;T&gt; are normally read-only, this method
        /// will work correctly and modify an array passed as <paramref name="list"/>.</para></remarks>
        /// <param name="list">The list or array to sort.</param>
        public static void SortInPlace<T>(IList<T> list)
            where T : IComparable<T>
        {
            SortInPlace<T>(list, Comparer<T>.Default);
        }

        /// <summary>
        /// Sorts a list or array in place. A supplied IComparer&lt;T&gt; is used
        /// to compare the items in the list. 
        /// </summary>
        /// <remarks><para>The Quicksort algorithms is used to sort the items. In virtually all cases,
        /// this takes time O(N log N), where N is the number of items in the list.</para>
        /// <para>Although arrays cast to IList&lt;T&gt; are normally read-only, this method
        /// will work correctly and modify an array passed as <paramref name="list"/>.</para></remarks>
        /// <param name="list">The list or array to sort.</param>
        /// <param name="comparer">The comparer instance used to compare items in the collection. Only
        /// the Compare method is used.</param>
        public static void SortInPlace<T>(IList<T> list, IComparer<T> comparer)
        {
            if(list == null)
                throw new ArgumentNullException("Cannot sort a 'null' list.");
            if(comparer == null)
                throw new ArgumentNullException("The comparer is 'null'.");

            // If we have an array, use the built-in array sort (faster than going through IList accessors
            // with virtual calls).
            if(list is T[])
            {
                Array.Sort((T[]) list, comparer);
                return;
            }

            if(list.IsReadOnly)
                throw new ArgumentException("The list is readonly.", "list");

            // Instead of a recursive procedure, we use an explicit stack to hold
            // ranges that we still need to sort.
            int[] leftStack = new int[32], rightStack = new int[32];
            int stackPtr = 0;

            int l = 0;                       // the inclusive left edge of the current range we are sorting.
            int r = list.Count - 1;    // the inclusive right edge of the current range we are sorting.
            T partition;                   // The partition value.

            // Loop until we have nothing left to sort. On each iteration, l and r contains the bounds
            // of something to sort (unless r <= l), and leftStack/rightStack have a stack of unsorted
            // pieces (unles stackPtr == 0).
            for(; ; )
            {
                if(l == r - 1)
                {
                    // We have exactly 2 elements to sort. Compare them and swap if needed.
                    T e1, e2;
                    e1 = list[l];
                    e2 = list[r];
                    if(comparer.Compare(e1, e2) > 0)
                    {
                        list[r] = e1;
                        list[l] = e2;
                    }
                    l = r;     // sort complete, find other work from the stack.
                }
                else if(l < r)
                {
                    // Sort the items in the inclusive range l .. r

                    // Get the left, middle, and right-most elements and sort them, yielding e1=smallest, e2=median, e3=largest
                    int m = l + (r - l) / 2;
                    T e1 = list[l], e2 = list[m], e3 = list[r], temp;
                    if(comparer.Compare(e1, e2) > 0)
                    {
                        temp = e1;
                        e1 = e2;
                        e2 = temp;
                    }
                    if(comparer.Compare(e1, e3) > 0)
                    {
                        temp = e3;
                        e3 = e2;
                        e2 = e1;
                        e1 = temp;
                    }
                    else if(comparer.Compare(e2, e3) > 0)
                    {
                        temp = e2;
                        e2 = e3;
                        e3 = temp;
                    }

                    if(l == r - 2)
                    {
                        // We have exactly 3 elements to sort, and we've done that. Store back and we're done.
                        list[l] = e1;
                        list[m] = e2;
                        list[r] = e3;
                        l = r;  // sort complete, find other work from the stack.
                    }
                    else
                    {
                        // Put the smallest at the left, largest in the middle, and the median at the right (which is the partitioning value)
                        list[l] = e1;
                        list[m] = e3;
                        list[r] = partition = e2;

                        // Partition into three parts, items <= partition, items == partition, and items >= partition
                        int i = l, j = r;
                        T item_i, item_j;
                        for(; ; )
                        {
                            do
                            {
                                ++i;
                                item_i = list[i];
                            } while(comparer.Compare(item_i, partition) < 0);

                            do
                            {
                                --j;
                                item_j = list[j];
                            } while(comparer.Compare(item_j, partition) > 0);

                            if(j < i)
                                break;

                            list[i] = item_j;
                            list[j] = item_i; // swap items to continue the partition.
                        }

                        // Move the partition value into place.
                        list[r] = item_i;
                        list[i] = partition;
                        ++i;

                        // We have partitioned the list. 
                        //    Items in the inclusive range l .. j are <= partition.
                        //    Items in the inclusive range i .. r are >= partition.
                        //    Items in the inclusive range j+1 .. i - 1 are == partition (and in the correct final position).
                        // We now need to sort l .. j and i .. r.
                        // To do this, we stack one of the lists for later processing, and change l and r to the other list.
                        // If we always stack the larger of the two sub-parts, the stack cannot get greater
                        // than log2(Count) in size; i.e., a 32-element stack is enough for the maximum list size.
                        if((j - l) > (r - i))
                        {
                            // The right partition is smaller. Stack the left, and get ready to sort the right.
                            leftStack[stackPtr] = l;
                            rightStack[stackPtr] = j;
                            l = i;
                        }
                        else
                        {
                            // The left partition is smaller. Stack the right, and get ready to sort the left.
                            leftStack[stackPtr] = i;
                            rightStack[stackPtr] = r;
                            r = j;
                        }
                        ++stackPtr;
                    }
                }
                else if(stackPtr > 0)
                {
                    // We have a stacked sub-list to sort. Pop it off and sort it.
                    --stackPtr;
                    l = leftStack[stackPtr];
                    r = rightStack[stackPtr];
                }
                else
                {
                    // We have nothing left to sort.
                    break;
                }
            }
        }

      
        #endregion

    }
}
