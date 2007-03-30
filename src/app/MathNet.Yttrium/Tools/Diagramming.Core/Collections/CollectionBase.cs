
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// CollectionBase is a base class that can be used to more easily implement the
    /// generic ICollection&lt;T&gt; and non-generic ICollection interfaces.
    /// </summary>
    /// <remarks>
    /// <para>To use CollectionBase as a base class, the derived class must override
    /// the Count, GetEnumerator, Add, Clear, and Remove methods. </para>
    /// <para>ICollection&lt;T&gt;.Contains need not be implemented by the
    /// derived class, but it should be strongly considered, because the CollectionBase implementation
    /// may not be very efficient.</para>
    /// </remarks>
    /// <typeparam name="T">The item type of the collection.</typeparam>

    [DebuggerDisplay("{DebuggerDisplayString()}")]
    public partial class CollectionBase<T> : ICollection<T>, ICollection, IList<T>, ICollectionBase<T>, IShowable 
    {
        #region Events
        /// <summary>
        /// Occurs when an item is added to the collection.
        /// </summary>
        public event EventHandler<CollectionEventArgs<T>> OnItemAdded;
        /// <summary>
        /// Occurs when an item is removed from the collection.
        /// </summary>
        public event EventHandler<CollectionEventArgs<T>> OnItemRemoved;
        /// <summary>
        /// Occurs when the collection is cleared.
        /// </summary>
        public event EventHandler OnClear;
        #endregion

        #region Fields
        /// <summary>
        /// the internal generic list on which this collection is based
        /// </summary>                                                                    
        protected List<T> innerList;
        /// <summary>
        /// whether this collection is readonly
        /// </summary>
        bool mReadOnly = false;
        #endregion

        #region Properties
        /// <summary>
        /// Indicates whether the collection is read-only. 
        /// </summary>
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return mReadOnly;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public virtual bool IsEmpty
        {
            get { return this.innerList.Count == 0; }
        }
        #endregion

        

        #region Constructor
        
        /// <summary>
        /// Creates a new CollectionBase. 
        /// </summary>
        public  CollectionBase()
        {
            innerList = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectionBase&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="baseCollection">The base collection.</param>
        public CollectionBase(ICollectionBase<T> baseCollection) : this()
        {
            innerList.AddRange(baseCollection);   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectionBase&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="baseCollection">The base collection.</param>
        /// <param name="readOnly">if set to <c>true</c> [read only].</param>
        public CollectionBase(ICollectionBase<T> baseCollection, bool readOnly)
            : this()
        {
            mReadOnly = readOnly;
            //innerList = new System.Collections.ObjectModel.ReadOnlyCollection<T>(baseCollection);
            innerList.AddRange(baseCollection);
        }

        
        

        #endregion

        /// <summary>
        /// Shows the string representation of the collection. The string representation contains
        /// a list of the items in the collection. Contained collections (except string) are expanded
        /// recursively.
        /// </summary>
        /// <returns>The string representation of the collection.</returns>
        public override string ToString()
        {
            return Algorithms.ToString(this);
        }
        /// <summary>
        /// Returns a string representation of this collection.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns></returns>
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            return Showing.ShowString(this, format, formatProvider);
        }

        #region ICollection<T> Members

        /// <summary>
        /// Must be overridden to allow adding items to this collection.
        /// </summary>
        /// <remarks><p>This method is not abstract, although derived classes should always
        /// override it. It is not abstract because some derived classes may wish to reimplement
        /// Add with a different return type (typically bool). In C#, this can be accomplished
        /// with code like the following:</p>
        /// <code>
        ///     public class MyCollection&lt;T&gt;: CollectionBase&lt;T&gt;, ICollection&lt;T&gt;
        ///     {
        ///         public new bool Add(T item) {
        ///             /* Add the item */
        ///         }
        ///  
        ///         void ICollection&lt;T&gt;.Add(T item) {
        ///             Add(item);
        ///         }
        ///     }
        /// </code>
        /// </remarks>
        /// <param name="item">Item to be added to the collection.</param>
        /// <exception cref="NotImplementedException">Always throws this exception to indicated
        /// that the method must be overridden or re-implemented in the derived class.</exception>
        public virtual void Add(T item)
        {
            if(item==null) throw new InconsistencyException("Adding 'null' to the collection is not allowed.");
            if (mReadOnly)
                throw new InconsistencyException("The collection is read only");
            this.innerList.Add(item);
            RaiseOnItemAdded(item);

        }
        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"></see>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"></see>.</param>
        /// <returns>
        /// The index of item if found in the list; otherwise, -1.
        /// </returns>
        public int IndexOf(T item)
        {
            return this.innerList.IndexOf(item);
        }

        /// <summary>
        /// Inserts the specified item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">A parameter of the generics Type T</param>
        public virtual void Insert(int index, T item)
        {
            if (mReadOnly)
                throw new InconsistencyException("The collection is read only");
            if(item.Equals(default(T)))
                return;
            this.innerList.Insert(index, item);
            RaiseOnItemAdded(item);
        }

        /// <summary>
        /// Adds a collection range to this collection.
        /// </summary>
        /// <param name="items">The items.</param>
        public virtual void AddRange(CollectionBase<T> items)
        {
            if (mReadOnly)
                throw new InconsistencyException("The collection is read only");
            this.innerList.AddRange(items);
            foreach (T item in items)
            {
                RaiseOnItemAdded(item);
            }
        }

        private void RaiseOnItemAdded(T item)
        {
            if (OnItemAdded != null)
                OnItemAdded(this, new CollectionEventArgs<T>(item));
        }

        private void RaiseOnItemRemoved(T item)
        {
            if (OnItemRemoved != null)
                OnItemRemoved(this, new CollectionEventArgs<T>(item));
        }
        private void RaiseOnClear()
        {
            if (OnClear != null)
                OnClear(this, EventArgs.Empty);
        }



        /// <summary>
        /// Must be overridden to allow clearing this collection.
        /// </summary>
        public virtual void Clear()
        {
            RaiseOnClear();
            innerList.Clear();
        }

        /// <summary>
        /// Must be overridden to allow removing items from this collection.
        /// </summary>
        /// <returns>True if <paramref name="item"/> existed in the collection and
        /// was removed. False if <paramref name="item"/> did not exist in the collection.</returns>
        public virtual bool Remove(T item)
        {
            
            bool result =  this.innerList.Remove(item);
            if (result)
            {
                RaiseOnItemRemoved(item);
            }
            return result;
        }

        /// <summary>
        /// Determines if the collection contains a particular item. This default implementation
        /// iterates all of the items in the collection via GetEnumerator, testing each item
        /// against <paramref name="item"/> using IComparable&lt;T&gt;.Equals or
        /// Object.Equals.
        /// </summary>
        /// <remarks>You should strongly consider overriding this method to provide
        /// a more efficient implementation, or if the default equality comparison
        /// is inappropriate.</remarks>
        /// <param name="item">The item to check for in the collection.</param>
        /// <returns>True if the collection contains <paramref name="item"/>, false otherwise.</returns>
        public virtual bool Contains(T item)
        {
            return this.innerList.Contains(item);

        }

        /// <summary>
        /// Copies all the items in the collection into an array. Implemented by
        /// using the enumerator returned from GetEnumerator to get all the items
        /// and copy them to the provided array.
        /// </summary>
        /// <param name="array">Array to copy to.</param>
        /// <param name="arrayIndex">Starting index in <paramref name="array"/> to copy to.</param>
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            int count = this.Count;

            if (count == 0)
                return;

            if (array == null)
                throw new ArgumentNullException("array");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", count, Resource1.ArgumentOutOfRange);
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex", arrayIndex, Resource1.ArgumentOutOfRange);
            if (arrayIndex >= array.Length || count > array.Length - arrayIndex)
                throw new ArgumentException("arrayIndex", Resource1.ArrayTooSmall);

            int index = arrayIndex, i = 0;
            foreach (T item in (ICollection<T>)this)
            {
                if (i >= count)
                    break;

                array[index] = item;
                ++index;
                ++i;
            }
        }

        /// <summary>
        /// Creates an array of the correct size, and copies all the items in the 
        /// collection into the array, by calling CopyTo.
        /// </summary>
        /// <returns>An array containing all the elements in the collection, in order.</returns>
        public virtual T[] ToArray()
        {
            int count = this.Count;

            T[] array = new T[count];
            CopyTo(array, 0);
            return array;
        }

        /// <summary>
        /// Must be overridden to provide the number of items in the collection.
        /// </summary>
        /// <value>The number of items in the collection.</value>
        public virtual int Count { get { return innerList.Count; } }



        #endregion

        #region Delegate operations

        /// <summary>
        /// Determines if the collection contains any item that satisfies the condition
        /// defined by <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A delegate that defines the condition to check for.</param>
        /// <returns>True if the collection contains one or more items that satisfy the condition
        /// defined by <paramref name="predicate"/>. False if the collection does not contain
        /// an item that satisfies <paramref name="predicate"/>.</returns>
        public virtual bool Exists(Predicate<T> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return Algorithms.Exists(this, predicate);
        }

        /// <summary>
        /// Determines if all of the items in the collection satisfy the condition
        /// defined by <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A delegate that defines the condition to check for.</param>
        /// <returns>True if all of the items in the collection satisfy the condition
        /// defined by <paramref name="predicate"/>, or if the collection is empty. False if one or more items
        /// in the collection do not satisfy <paramref name="predicate"/>.</returns>
        public virtual bool TrueForAll(Predicate<T> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return Algorithms.TrueForAll(this, predicate);
        }
         /*
        /// <summary>
        /// Counts the number of items in the collection that satisfy the condition
        /// defined by <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A delegate that defines the condition to check for.</param>
        /// <returns>The number of items in the collection that satisfy <paramref name="predicate"/>.</returns>
        public virtual int CountWhere(Predicate<T> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return Algorithms.CountWhere(this, predicate);
        }
          */
        /// <summary>
        /// Finds the first item in the collection satisfying the given predicate
        /// </summary>
        /// <param name="predicate">a searching predictae</param>
        /// <returns>an item satisfying the criteria (if any)</returns>
        public virtual T Find(Predicate<T> predicate)
        {
            foreach (T item in this.innerList)
            {
                if (predicate(item))
                    return item;
            }
            return default(T);
        }


        /// <summary>
        /// Removes all the items in the collection that satisfy the condition
        /// defined by <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A delegate that defines the condition to check for.</param>
        /// <returns>Returns a collection of the items that were removed, in sorted order.</returns>
        public virtual ICollection<T> RemoveAll(Predicate<T> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return Algorithms.RemoveWhere(this, predicate);
        }

        /// <summary>
        /// Performs the specified action on each item in this collection.
        /// </summary>
        /// <param name="action">An Action delegate which is invoked for each item in this collection.</param>
        public virtual void ForEach(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            Algorithms.ForEach(this, action);
        }



        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Must be overridden to enumerate all the members of the collection.
        /// </summary>
        /// <returns>A generic IEnumerator&lt;T&gt; that can be used
        /// to enumerate all the items in the collection.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.innerList.GetEnumerator();
        }


        #endregion

        #region ICollection Members

        /// <summary>
        /// Copies all the items in the collection into an array. Implemented by
        /// using the enumerator returned from GetEnumerator to get all the items
        /// and copy them to the provided array.
        /// </summary>
        /// <param name="array">Array to copy to.</param>
        /// <param name="index">Starting index in <paramref name="array"/> to copy to.</param>
        void ICollection.CopyTo(Array array, int index)
        {
            int count = this.Count;

            if (count == 0)
                return;

            if (array == null)
                throw new ArgumentNullException("array");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index", index, Resource1.ArgumentOutOfRange);
            if (index >= array.Length || count > array.Length - index)
                throw new ArgumentException("index", Resource1.ArrayTooSmall);

            int i = 0;
            foreach (object o in (ICollection)this)
            {
                if (i >= count)
                    break;

                array.SetValue(o, index);
                ++index;
                ++i;
            }
        }

        /// <summary>
        /// Indicates whether the collection is synchronized.
        /// </summary>
        /// <value>Always returns false, indicating that the collection is not synchronized.</value>
        bool ICollection.IsSynchronized
        {
            get { return IsSynchronized; }
        }

        /// <summary>
        /// See code analysis CA1033
        /// </summary>
        protected bool IsSynchronized
        {
            get { return false; }
        }
        /// <summary>
        /// Indicates the synchronization object for this collection.
        /// </summary>
        /// <value>Always returns this.</value>
        object ICollection.SyncRoot
        {
            get { return SyncRoot; }
        }

        /// <summary>
        /// See code analysis CA1033
        /// </summary>
        protected object SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Provides an IEnumerator that can be used to iterate all the members of the
        /// collection. This implementation uses the IEnumerator&lt;T&gt; that was overridden
        /// by the derived classes to enumerate the members of the collection.
        /// </summary>
        /// <returns>An IEnumerator that can be used to iterate the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T item in this)
            {
                yield return item;
            }
        }

        #endregion

        /// <summary>
        /// Display the contents of the collection in the debugger. This is intentionally private, it is called
        /// only from the debugger due to the presence of the DebuggerDisplay attribute. It is similar
        /// format to ToString(), but is limited to 250-300 characters or so, so as not to overload the debugger.
        /// </summary>
        /// <returns>The string representation of the items in the collection, similar in format to ToString().</returns>
        internal string DebuggerDisplayString()
        {
            const int MAXLENGTH = 250;

            System.Text.StringBuilder builder = new System.Text.StringBuilder();

            builder.Append('{');

            // Call ToString on each item and put it in.
            bool firstItem = true;
            foreach (T item in this)
            {
                if (builder.Length >= MAXLENGTH)
                {
                    builder.Append(",...");
                    break;
                }

                if (!firstItem)
                    builder.Append(',');

                if (item == null)
                    builder.Append("null");
                else
                    builder.Append(item.ToString());

                firstItem = false;
            }

            builder.Append('}');
            return builder.ToString();
        }
        /// <summary>
        /// Removes an item at the given index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this.innerList.RemoveAt(index);
        }
        /// <summary>
        /// Integer indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                return this.innerList[index];
            }
            set
            {
                this.innerList[index] = value;
            }
        }

        /// <summary>
        /// Returns a deep copy of this collection.
        /// <remarks>The returned collection is not attached to the <see cref="Model"/>
        /// and is as such on an in-memory collection of instances. You need to 'unwrap' the collection 
        /// in the model and, to make it visible, deploy it in the paintables collection of the model.
        /// </remarks>
        /// </summary>
        /// <returns></returns>
        public CollectionBase<T> DeepCopy()
        {
            /* This doesn't work seemingly....
            if (!typeof(T).IsSerializable)
                throw new InconsistencyException("The generic type on which the collection is based is not serializable, the collection cannot generate a deep copy.");
            */

            try
            {
                
                CollectionBase<T> newobj = null;
                MemoryStream stream = new MemoryStream();
                GenericFormatter<BinaryFormatter> f = new GenericFormatter<BinaryFormatter>();
                f.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                newobj = f.Deserialize<CollectionBase<T>>(stream);
                stream.Close();
                return newobj;
            }
            catch (Exception exc)
            {
                throw new InconsistencyException("The copy operation failed.", exc);
            }

        }
        /// <summary>
        /// Returns a copy of this instance.
        /// </summary>
        /// <returns></returns>
        public CollectionBase<T> Copy()
        {
            CollectionBase<T> copy = new CollectionBase<T>();
            foreach (T item in this.innerList)
            {
                copy.Add(item);
            }
            return copy;  
        }
        /// <summary>
        /// This method creates first a deep copy of the collection and puts the result in a <see cref="MemoryStream"/>. 
        /// See the <see cref="CopyTool"/> for details.
        /// </summary>
        /// <returns></returns>
        public MemoryStream ToStream()
        {
            try
            {

                MemoryStream stream = new MemoryStream();
                GenericFormatter<BinaryFormatter> f = new GenericFormatter<BinaryFormatter>();
                f.Serialize(stream, this);
                return stream;
            }
            catch (Exception exc)
            {
                throw new InconsistencyException("The ToStream() operation failed.", exc);
            }

        }

        /// <summary>
        /// Format <code>this</code> using at most approximately <code>rest</code> chars and
        /// append the result, possibly truncated, to stringbuilder.
        /// Subtract the actual number of used chars from <code>rest</code>.
        /// </summary>
        /// <param name="stringbuilder"></param>
        /// <param name="rest"></param>
        /// <param name="formatProvider"></param>
        /// <returns>
        /// True if the appended formatted string was complete (not truncated).
        /// </returns>
        public virtual bool Show(System.Text.StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider)
        {
            return true;
        }
      
    }

   
    


    
}
