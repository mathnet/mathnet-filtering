using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Numerics
{
    /// <summary>
    /// Sorting algorithms for single, tuple and triple lists.
    /// </summary>
    public static class Sorting
    {
        /// <summary>
        /// Sort a list of keys, inplace.
        /// </summary>
        /// <param name="keys">List to sort.</param>
        public static
        void
        Sort<T>(
            IList<T> keys
            )
        {
            Sort(keys, Comparer<T>.Default);
        }

        /// <summary>
        /// Sort a list of keys and items with respect to the keys, inplace.
        /// </summary>
        /// <param name="keys">List to sort.</param>
        /// <param name="items">List to permutate the same way as the key list.</param>
        public static
        void
        Sort<TKey, TItem>(
            IList<TKey> keys,
            IList<TItem> items
            )
        {
            Sort(keys, items, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Sort a list of keys, items1 and items2 with respect to the keys, inplace.
        /// </summary>
        /// <param name="keys">List to sort.</param>
        /// <param name="items1">First list to permutate the same way as the key list.</param>
        /// <param name="items2">Second list to permutate the same way as the key list.</param>
        public static
        void
        Sort<TKey, TItem1, TItem2>(
            IList<TKey> keys,
            IList<TItem1> items1,
            IList<TItem2> items2
            )
        {
            Sort(keys, items1, items2, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Sort a list of keys, inplace.
        /// </summary>
        /// <param name="keys">List to sort.</param>
        /// <param name="comparer">Comparison, defining the sort order.</param>
        public static
        void
        Sort<T>(
            IList<T> keys,
            IComparer<T> comparer
            )
        {
            if(null == keys)
            {
                throw new ArgumentNullException("keys");
            }

            if(null == comparer)
            {
                throw new ArgumentNullException("comparer");
            }

            // basic cases
            if(keys.Count <= 1)
            {
                return;
            }

            if(keys.Count == 2)
            {
                if(comparer.Compare(keys[0], keys[1]) > 0)
                {
                    Swap(keys, 0, 1);
                }

                return;
            }

            // generic list case
            List<T> list = keys as List<T>;
            if(null != list)
            {
                list.Sort(comparer);
                return;
            }

            // array case
            T[] array = keys as T[];
            if(null != array)
            {
                Array.Sort(array, comparer);
                return;
            }

            // local sort implementation
            QuickSort(keys, comparer, 0, keys.Count - 1);
        }

        /// <summary>
        /// Sort a list of keys and items with respect to the keys, inplace.
        /// </summary>
        /// <param name="keys">List to sort.</param>
        /// <param name="items">List to permutate the same way as the key list.</param>
        /// <param name="comparer">Comparison, defining the sort order.</param>
        public static
        void
        Sort<TKey, TItem>(
            IList<TKey> keys,
            IList<TItem> items,
            IComparer<TKey> comparer
            )
        {
            if(null == keys)
            {
                throw new ArgumentNullException("keys");
            }

            if(null == items)
            {
                throw new ArgumentNullException("items");
            }

            if(null == comparer)
            {
                throw new ArgumentNullException("comparer");
            }

            // array case
            TKey[] keysArray = keys as TKey[];
            TItem[] itemsArray = items as TItem[];
            if((null != keysArray) && (null != itemsArray))
            {
                Array.Sort(keysArray, itemsArray, comparer);
                return;
            }

            // local sort implementation
            QuickSort(keys, items, comparer, 0, keys.Count - 1);
        }

        /// <summary>
        /// Sort a list of keys, items1 and items2 with respect to the keys, inplace.
        /// </summary>
        /// <param name="keys">List to sort.</param>
        /// <param name="items1">First list to permutate the same way as the key list.</param>
        /// <param name="items2">Second list to permutate the same way as the key list.</param>
        /// <param name="comparer">Comparison, defining the sort order.</param>
        public static
        void
        Sort<TKey, TItem1, TItem2>(
            IList<TKey> keys,
            IList<TItem1> items1,
            IList<TItem2> items2,
            IComparer<TKey> comparer
            )
        {
            if(null == keys)
            {
                throw new ArgumentNullException("keys");
            }

            if(null == items1)
            {
                throw new ArgumentNullException("items1");
            }

            if(null == items2)
            {
                throw new ArgumentNullException("items2");
            }

            if(null == comparer)
            {
                throw new ArgumentNullException("comparer");
            }

            // local sort implementation
            QuickSort(keys, items1, items2, comparer, 0, keys.Count - 1);
        }

        static
        void
        QuickSort<T>(
            IList<T> keys,
            IComparer<T> comparer,
            int left,
            int right
            )
        {
            do
            {
                int a = left;
                int b = right;
                int p = a + ((b - a) >> 1); // midpoint

                if(comparer.Compare(keys[a], keys[p]) > 0)
                {
                    Swap(keys, a, p);
                }

                if(comparer.Compare(keys[a], keys[b]) > 0)
                {
                    Swap(keys, a, b);
                }

                if(comparer.Compare(keys[p], keys[b]) > 0)
                {
                    Swap(keys, p, b);
                }

                T pivot = keys[p];

                do
                {
                    while(comparer.Compare(keys[a], pivot) < 0)
                    {
                        a++;
                    }

                    while(comparer.Compare(pivot, keys[b]) < 0)
                    {
                        b--;
                    }

                    if(a > b)
                    {
                        break;
                    }

                    if(a < b)
                    {
                        Swap(keys, a, b);
                    }

                    a++;
                    b--;
                }
                while(a <= b);

                if((b - left) <= (right - a))
                {
                    if(left < b)
                    {
                        QuickSort(keys, comparer, left, b);
                    }

                    left = a;
                }
                else
                {
                    if(a < right)
                    {
                        QuickSort(keys, comparer, a, right);
                    }

                    right = b;
                }
            }
            while(left < right);
        }

        static
        void
        QuickSort<T, TItems>(
            IList<T> keys,
            IList<TItems> items,
            IComparer<T> comparer,
            int left,
            int right
            )
        {
            do
            {
                int a = left;
                int b = right;
                int p = a + ((b - a) >> 1); // midpoint

                if(comparer.Compare(keys[a], keys[p]) > 0)
                {
                    Swap(keys, a, p);
                    Swap(items, a, p);
                }

                if(comparer.Compare(keys[a], keys[b]) > 0)
                {
                    Swap(keys, a, b);
                    Swap(items, a, b);
                }

                if(comparer.Compare(keys[p], keys[b]) > 0)
                {
                    Swap(keys, p, b);
                    Swap(items, p, b);
                }

                T pivot = keys[p];

                do
                {
                    while(comparer.Compare(keys[a], pivot) < 0)
                    {
                        a++;
                    }

                    while(comparer.Compare(pivot, keys[b]) < 0)
                    {
                        b--;
                    }

                    if(a > b)
                    {
                        break;
                    }

                    if(a < b)
                    {
                        Swap(keys, a, b);
                        Swap(items, a, b);
                    }

                    a++;
                    b--;
                }
                while(a <= b);

                if((b - left) <= (right - a))
                {
                    if(left < b)
                    {
                        QuickSort(keys, items, comparer, left, b);
                    }

                    left = a;
                }
                else
                {
                    if(a < right)
                    {
                        QuickSort(keys, items, comparer, a, right);
                    }

                    right = b;
                }
            }
            while(left < right);
        }

        static
        void
        QuickSort<T, TItems1, TItems2>(
            IList<T> keys,
            IList<TItems1> items1,
            IList<TItems2> items2,
            IComparer<T> comparer,
            int left,
            int right
            )
        {
            do
            {
                int a = left;
                int b = right;
                int p = a + ((b - a) >> 1); // midpoint

                if(comparer.Compare(keys[a], keys[p]) > 0)
                {
                    Swap(keys, a, p);
                    Swap(items1, a, p);
                    Swap(items2, a, p);
                }

                if(comparer.Compare(keys[a], keys[b]) > 0)
                {
                    Swap(keys, a, b);
                    Swap(items1, a, b);
                    Swap(items2, a, b);
                }

                if(comparer.Compare(keys[p], keys[b]) > 0)
                {
                    Swap(keys, p, b);
                    Swap(items1, p, b);
                    Swap(items2, p, b);
                }

                T pivot = keys[p];

                do
                {
                    while(comparer.Compare(keys[a], pivot) < 0)
                    {
                        a++;
                    }

                    while(comparer.Compare(pivot, keys[b]) < 0)
                    {
                        b--;
                    }

                    if(a > b)
                    {
                        break;
                    }

                    if(a < b)
                    {
                        Swap(keys, a, b);
                        Swap(items1, a, b);
                        Swap(items2, a, b);
                    }

                    a++;
                    b--;
                }
                while(a <= b);

                if((b - left) <= (right - a))
                {
                    if(left < b)
                    {
                        QuickSort(keys, items1, items2, comparer, left, b);
                    }

                    left = a;
                }
                else
                {
                    if(a < right)
                    {
                        QuickSort(keys, items1, items2, comparer, a, right);
                    }

                    right = b;
                }
            }
            while(left < right);
        }

        static
        void
        Swap<T>(
            IList<T> keys,
            int a,
            int b
            )
        {
            T local = keys[a];
            keys[a] = keys[b];
            keys[b] = local;
        }
    }
}
