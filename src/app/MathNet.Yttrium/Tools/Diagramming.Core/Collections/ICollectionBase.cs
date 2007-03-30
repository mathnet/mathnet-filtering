using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace Netron.Diagramming.Core
{
    /// <summary>
    /// Describes the CollectionBase collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICollectionBase<T> : ICollection<T>, IList<T>
    {
        #region Events
        /// <summary>
        /// Occurs when the collection is cleared.
        /// </summary>
        event EventHandler OnClear;
        /// <summary>
        /// Occurs when an item is added to the collection.
        /// </summary>
        event EventHandler<CollectionEventArgs<T>> OnItemAdded;
        /// <summary>
        /// Occurs when en item is removed from the collection.
        /// </summary>
        event EventHandler<CollectionEventArgs<T>> OnItemRemoved;
        #endregion

        #region Properties
      
        #endregion

        #region Methods
        /// <summary>
        /// Adds the range of items to the collection.
        /// </summary>
        /// <param name="items">The items.</param>
        void AddRange(CollectionBase<T> items);
        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns></returns>
        CollectionBase<T> Copy();
        /// <summary>
        /// Creates a deep copy of this instance.
        /// </summary>
        /// <returns></returns>
        CollectionBase<T> DeepCopy();
        /// <summary>
        /// Uses the given predicate to test the existence of a certain item in the collection.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        bool Exists(Predicate<T> predicate);
        /// <summary>
        /// Finds the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        T Find(Predicate<T> predicate);
        /// <summary>
        /// Uses the given Action to act on the collection items
        /// </summary>
        /// <param name="action">The action.</param>
        void ForEach(Action<T> action);
       
      
        ICollection<T> RemoveAll(Predicate<T> predicate);

        /// <summary>
        /// Converts the collection to an array.
        /// </summary>
        /// <returns></returns>
        T[] ToArray();
        /// <summary>
        /// Specific copy/paste utility function.
        /// </summary>
        /// <returns></returns>
        System.IO.MemoryStream ToStream();
        /// <summary>
        /// Returns a string representation of the collection.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns></returns>
        string ToString(string format, IFormatProvider formatProvider);
        /// <summary>
        /// Returns a string representation of the collection.
        /// </summary>
        /// <returns></returns>
        string ToString();
        /// <summary>
        /// Checks, using the given predicate, whether a certain property is true for all items in the collection.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        bool TrueForAll(Predicate<T> predicate);
        #endregion

        
        
    }
}
