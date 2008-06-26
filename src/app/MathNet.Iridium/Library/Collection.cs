#region Math.NET Iridium (LGPL) by Vermorel
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2004-2008, Joannes Vermorel, http://www.vermorel.com
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
using MathNet.Numerics.Properties;

namespace MathNet.Numerics
{
    /// <summary>
    /// The class <c>Collection</c> contains several utilities performing
    /// some basic collection operations (like union, intersection...).
    /// </summary>
    public sealed class Collection
    {
        /// <summary>
        /// Preventing the instantiation of the <see cref="Collection"/> class.
        /// </summary>
        private Collection()
        {
        }

        /// <summary>
        /// The class <c>ConcatCollection</c> is used to perform the 
        /// mathematical concatenation between two collections.
        /// </summary>
        /// <seealso cref="Collection.Concat"/>
        private sealed class ConcatCollection : ICollection
        {
            private sealed class ConcatEnumerator : IEnumerator
            {
                private IEnumerator enumerator1, enumerator2;

                private bool isEnumator1Current;

                public ConcatEnumerator(ConcatCollection union)
                {
                    this.enumerator1 = union.c1.GetEnumerator();
                    this.enumerator2 = union.c2.GetEnumerator();
                    this.isEnumator1Current = true;
                }

                public void Reset()
                {
                    enumerator1.Reset();
                    enumerator2.Reset();
                    isEnumator1Current = true;
                }

                public object Current
                {
                    get
                    {
                        if(isEnumator1Current)
                        {
                            return enumerator1.Current;
                        }
                        
                        return enumerator2.Current;
                    }
                }

                public bool MoveNext()
                {
                    if(isEnumator1Current && enumerator1.MoveNext())
                    {
                        return true;
                    }

                    isEnumator1Current = false;
                    return enumerator2.MoveNext();
                }
            }

            private ICollection c1, c2;

            public ConcatCollection(ICollection c1, ICollection c2)
            {
                this.c1 = c1;
                this.c2 = c2;
            }


            public bool IsSynchronized
            {
                get { return c1.IsSynchronized && c2.IsSynchronized; }
            }

            public int Count
            {
                get { return c1.Count + c2.Count; }
            }

            public void CopyTo(Array array, int index)
            {
                int indexArray = index;

                foreach(object obj in c1)
                {
                    array.SetValue(obj, indexArray++);
                }

                foreach(object obj in c2)
                {
                    array.SetValue(obj, indexArray++);
                }
            }

            public object SyncRoot
            {
                get { return c1.SyncRoot; }
            }

            public IEnumerator GetEnumerator()
            {
                return new ConcatEnumerator(this);
            }
        }


        /// <summary>
        /// The class <c>InterCollection</c> is used to perform the
        /// mathematical intersection between two collections.
        /// </summary>
        /// <seealso cref="Collection.Inter"/>
        [Obsolete("Use Set<T>.Intersect() instead.", false)]
        private sealed class InterCollection : ICollection
        {
            private ArrayList intersection;

            public InterCollection(ICollection c1, ICollection c2)
            {
                // swap in order to have <c>c1.Count <= c2.Count</c>
                if(c1.Count > c2.Count)
                {
                    ICollection c1Bis = c1;
                    c1 = c2;
                    c2 = c1Bis;
                }

                Hashtable table = new Hashtable(c1.Count);
                foreach(object obj in c1)
                {
                    if(!table.Contains(obj))
                    {
                        table.Add(obj, null);
                    }
                }

                // building the intersection
                intersection = new ArrayList();
                foreach(object obj in c2)
                {
                    if(table.Contains(obj))
                    {
                        intersection.Add(obj);
                        table.Remove(obj);
                    }
                }

                intersection.TrimToSize();
            }

            #region ICollection Members

            public IEnumerator GetEnumerator()
            {
                return intersection.GetEnumerator();
            }

            public bool IsSynchronized
            {
                get { return intersection.IsSynchronized; }
            }

            public int Count
            {
                get { return intersection.Count; }
            }

            public void CopyTo(Array array, int index)
            {
                intersection.CopyTo(array, index);
            }

            public object SyncRoot
            {
                get { return intersection.SyncRoot; }
            }

            #endregion
        }

        /// <summary>
        /// The class <c>UnionCollection</c> is used to perform the
        /// mathematical union between two collections.
        /// </summary>
        [Obsolete("Use Set<T>.Union() instead.", false)]
        private sealed class UnionCollection : ICollection
        {
            private ArrayList union;

            public UnionCollection(ICollection c1, ICollection c2)
            {
                Hashtable table1 = new Hashtable(c1.Count);
                foreach(object obj in c1)
                {
                    if(!table1.Contains(obj))
                    {
                        table1.Add(obj, null);
                    }
                }

                Hashtable table2 = new Hashtable(c2.Count);
                foreach(object obj in c2)
                {
                    if(!table2.Contains(obj))
                    {
                        table2.Add(obj, null);
                    }
                }

                // building the union
                union = new ArrayList(Math.Max(table1.Count, table2.Count));
                union.AddRange(table1.Keys);
                foreach(object obj in c2)
                {
                    if(!table1.Contains(obj))
                    {
                        union.Add(obj);
                    }
                }

                union.TrimToSize();
            }

            #region ICollection Members

            public bool IsSynchronized
            {
                get { return union.IsSynchronized; }
            }

            public int Count
            {
                get { return union.Count; }
            }

            public void CopyTo(Array array, int index)
            {
                union.CopyTo(array, index);
            }

            public IEnumerator GetEnumerator()
            {
                return union.GetEnumerator();
            }

            public object SyncRoot
            {
                get { return union.SyncRoot; }
            }

            #endregion
        }

        /// <summary>
        /// The collection <c>MinusCollection</c> is used to perform
        /// the mathematical subtraction of two collections.
        /// </summary>
        /// <seealso cref="Collection.Minus"/>
        [Obsolete("Use Set<T>.Subtract() instead.", false)]
        private sealed class MinusCollection : ICollection
        {
            private ArrayList minus;

            public MinusCollection(ICollection c1, ICollection c2)
            {
                Hashtable table1 = new Hashtable(c1.Count);
                foreach(object obj in c1)
                {
                    if(!table1.Contains(obj))
                    {
                        table1.Add(obj, null);
                    }
                }

                Hashtable table2 = new Hashtable(c2.Count);
                foreach(object obj in c2)
                {
                    if(!table2.Contains(obj))
                    {
                        table2.Add(obj, null);
                    }
                }

                // building minus collection
                minus = new ArrayList(Math.Max(c1.Count - c2.Count, 10));
                foreach(object obj in table1.Keys)
                {
                    if(!table2.Contains(obj))
                    {
                        minus.Add(obj);
                    }
                }

                minus.TrimToSize();
            }

            #region ICollection Members

            public bool IsSynchronized
            {
                get { return minus.IsSynchronized; }
            }

            public int Count
            {
                get { return minus.Count; }
            }

            public void CopyTo(Array array, int index)
            {
                minus.CopyTo(array, index);
            }

            public IEnumerator GetEnumerator()
            {
                return minus.GetEnumerator();
            }

            public object SyncRoot
            {
                get { return minus.SyncRoot; }
            }

            #endregion
        }

        /// <summary>
        /// Returns a collection resulting from the concatenation from
        /// <c>c1</c> and <c>c2</c>.
        /// </summary>
        /// <param name="c1">Should not be null.</param>
        /// <param name="c2">Should not be null.</param>
        /// <remarks>The call is performed in <c>O(1)</c> computational time, the
        /// concatenated collection is not built explicitly.</remarks>
        public static ICollection Concat(ICollection c1, ICollection c2)
        {
            if(c1 == null)
            {
                throw new ArgumentNullException("c1", string.Format(Resources.ArgumentNull, "c1"));
            }

            if(c2 == null)
            {
                throw new ArgumentNullException("c2", string.Format(Resources.ArgumentNull, "c2"));
            }

            return new ConcatCollection(c1, c2);
        }

        /// <summary>
        /// Returns a collection resulting from the mathematical intersection
        /// of <c>c1</c> and <c>c2</c>.
        /// </summary>
        /// <param name="c1">Should not be null.</param>
        /// <param name="c2">Should not be null.</param>
        /// <remarks>
        /// <p>The call is performed in <c>O(c1.Count+c2.Count)</c> and
        /// the intersection is built explicitly.</p>
        /// <p>The resulting collection will not contain several identical elements.</p>
        /// <p>Example: Inter({1;1;2;3},{0;1;1;3;4}) = {1;3}.</p>
        /// </remarks>
        [Obsolete("Use Set<T>.Intersect() instead.", false)]
        public static ICollection Inter(ICollection c1, ICollection c2)
        {
            if(c1 == null)
            {
                throw new ArgumentNullException("c1", string.Format(Resources.ArgumentNull, "c1"));
            }

            if(c2 == null)
            {
                throw new ArgumentNullException("c2", string.Format(Resources.ArgumentNull, "c2"));
            }

            return new InterCollection(c1, c2);
        }

        /// <summary>
        /// Returns a collection resulting from the subtraction of
        /// the items of <c>c2</c> to the collection <c>c1</c>. 
        /// </summary>
        /// <param name="c1">Should not be null.</param>
        /// <param name="c2">Should not be null.</param>
        /// <remarks>The call is performed in <c>O(c1.Count+c2.Count)</c></remarks>
        [Obsolete("Use Set<T>.Subtract() instead.", false)]
        public static ICollection Minus(ICollection c1, ICollection c2)
        {
            if(c1 == null)
            {
                throw new ArgumentNullException("c1", string.Format(Resources.ArgumentNull, "c1"));
            }

            if(c2 == null)
            {
                throw new ArgumentNullException("c2", string.Format(Resources.ArgumentNull, "c2"));
            }

            return new MinusCollection(c1, c2);
        }

        /// <summary>
        /// Returns the cartesian product of the two collections <c>c1</c>
        /// and <c>c2</c>.
        /// </summary>
        /// <param name="c1">Should not be null.</param>
        /// <param name="c2">Should not be null.</param>
        public static ICollection Product(ICollection c1, ICollection c2)
        {
            if(c1 == null)
            {
                throw new ArgumentNullException("c1", string.Format(Resources.ArgumentNull, "c1"));
            }

            if(c2 == null)
            {
                throw new ArgumentNullException("c2", string.Format(Resources.ArgumentNull, "c2"));
            }

            return null;
        }

        /// <summary>
        /// Returns a collection resulting from the union of the items
        /// of <c>c1</c> and <c>c2</c>.
        /// </summary>
        /// <param name="c1">Should not be null.</param>
        /// <param name="c2">Should not be null.</param>
        /// <remarks>
        /// <p>The call is performed in <c>O(c1.Count+c2.Count)</c>
        /// computational time.</p>
        /// <p>The resulting collection will not contain several identical elements.</p>
        /// <p>Example: Union({1;1;3},{0;1;2;3}) = {0;1;2;3}</p>
        /// </remarks>
        [Obsolete("Use Set<T>.Union() instead.", false)]
        public static ICollection Union(ICollection c1, ICollection c2)
        {
            if(c1 == null)
            {
                throw new ArgumentNullException("c1", string.Format(Resources.ArgumentNull, "c1"));
            }

            if(c2 == null)
            {
                throw new ArgumentNullException("c2", string.Format(Resources.ArgumentNull, "c2"));
            }

            return new UnionCollection(c1, c2);
        }


        #region NUnit testing suite
#if DEBUG2
        /// <summary>
        /// Testing the class <see cref="Collection"/>.
        /// </summary>
        [TestFixture]
        public class TestingSuite
        {
            /// <summary>
            /// Testing the method <see cref="ConcatCollection.Count"/>.
            /// </summary>
            [Test] public void ConcatCount()
            {
                int[] array0 = new int[0], array1 = new int[7], array2 = new int[13];

                Assert.AreEqual(array0.Length + array0.Length,
                    (new ConcatCollection(array0, array0)).Count, "#A00");
                Assert.AreEqual(array0.Length + array1.Length,
                    (new ConcatCollection(array0, array1)).Count, "#A01");
                Assert.AreEqual(array0.Length + array2.Length,
                    (new ConcatCollection(array0, array2)).Count, "#A02");

                Assert.AreEqual(array1.Length + array0.Length,
                    (new ConcatCollection(array1, array0)).Count, "#A03");
                Assert.AreEqual(array1.Length + array1.Length,
                    (new ConcatCollection(array1, array1)).Count, "#A04");
                Assert.AreEqual(array1.Length + array2.Length,
                    (new ConcatCollection(array1, array2)).Count, "#A05");
            }

            /// <summary>
            /// Testing the method <see cref="ConcatCollection.GetEnumerator"/>.
            /// </summary>
            [Test] public void ConcatGetEnumerator()
            {
                // generating two arrays
                int[] array1 = new int[10], array2 = new int[13];
                for(int i = 0; i < array1.Length; i++) array1[i] = i;
                for(int i = 0; i < array2.Length; i++) array2[i] = i + array1.Length;

                ConcatCollection union = new ConcatCollection(array1, array2);

                int index = 0;

                foreach(int value in union)
                {
                    Assert.AreEqual(index++, value, "#A00 Unexpected value in collection.");
                }

                Assert.AreEqual(array1.Length + array2.Length, index, 
                    "#A01 Unexpected count of enumerated element in collection.");
            }

            /// <summary>
            /// Testing the method <see cref="InterCollection.GetEnumerator"/>
            /// </summary>
            [Test] public void InterGetEnumerator()
            {
                int LENGTH = 100;
                int[] array1 = new int[LENGTH], array2 = new int[LENGTH];

                for(int i = 0; i < LENGTH; i++)
                {
                    array1[i] = i;
                    array2[i] = i / 2;
                }

                ICollection intersection = Collection.Inter(array1, array2);

                Assert.AreEqual(LENGTH / 2, intersection.Count,
                    "#A00 Unexpected intersection count.");

                foreach(int i in intersection)
                    Assert.IsTrue(i >= 0 && i <= LENGTH,
                        "#A01-" + i + " Unexpected intersection item.");
            }

            /// <summary>
            /// Testing the method <see cref="MinusCollection.GetEnumerator"/>
            /// </summary>
            [Test] public void MinusGetEnumerator()
            {
                int LENGTH = 100;
                int[] array1 = new int[LENGTH], array2 = new int[LENGTH];

                for(int i = 0; i < LENGTH; i++)
                {
                    array1[i] = i;
                    array2[i] = i / 2;
                }

                ICollection minus = Collection.Minus(array1, array2);
                
                Assert.AreEqual(LENGTH / 2, minus.Count,
                    "#A00 Unexpected minus count.");

                foreach(int i in minus)
                    Assert.IsTrue(i >= LENGTH / 2,
                        "#A01-" + i + " Unexpected minus item.");
            }

            /// <summary>
            /// Testing the method <see cref="UnionCollection.GetEnumerator"/>.
            /// </summary>
            [Test] public void UnionGetEnumerator()
            {
                int LENGTH = 100;

                int[] array1 = new int[LENGTH], array2 = new int[LENGTH];

                for(int i = 0; i < LENGTH; i++)
                {
                    array1[i] = i;
                    array2[i] = i / 2;
                }

                ICollection union = Collection.Union(array1, array2);

                Assert.AreEqual(LENGTH, union.Count, 
                    "#A00 Unexpected union count.");

                foreach(int i in union)
                    Assert.IsTrue(i >= 0 && i < LENGTH,
                        "#A01-" + i + " Unexpected union item.");
            }
        }
#endif
        #endregion
    }
}
