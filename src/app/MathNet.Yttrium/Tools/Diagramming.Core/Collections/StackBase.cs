using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


namespace Netron.Diagramming.Core
{
    /// <summary>
    /// This is a wrapper around the <see cref="Stack"/> which communicates Push and Pop events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StackBase<T>
    {
        #region Fields
        /// <summary>
        /// the internal stack based on the .Net <see cref="Stack"/> implementation
        /// </summary>
        private Stack<T> InnerStack;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when an item is pushed onto the stack
        /// </summary>
        public event EventHandler<CollectionEventArgs<T>> OnItemPushed;
        /// <summary>
        /// Occurs when an item is popped from the stack
        /// </summary>
        public event EventHandler<CollectionEventArgs<T>> OnItemPopped;
        #endregion;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:StackBase&lt;T&gt;"/> class.
        /// </summary>
        public StackBase()
        {
            InnerStack = new Stack<T>();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Gets the number of elements in the stack
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get { return InnerStack.Count; }
        }


        /// <summary>
        /// Pushes the specified item.
        /// </summary>
        /// <param name="item">A parameter of the generics Type T</param>
        public void Push(T item)
        {

            this.InnerStack.Push(item);
            RaiseOnItemPushed(item);

        }

        /// <summary>
        /// Pops the next item from the stack
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            T item =
             InnerStack.Pop();
            RaiseOnItemPopped(item);
            return item;
        }

        /// <summary>
        /// Peeks the next item in the stack
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return InnerStack.Peek();
        }


        /// <summary>
        /// Converts the stack to an array.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            return InnerStack.ToArray();
        }


        /// <summary>
        /// Raises the <see cref="OnItemPushed"/>.
        /// </summary>
        /// <param name="item">A parameter of the generics Type T</param>
        private void RaiseOnItemPushed(T item)
        {
            EventHandler<CollectionEventArgs<T>> handler = OnItemPushed;
            handler(this, new CollectionEventArgs<T>(item));
        }

        /// <summary>
        /// Raises the on OnItemPopped event.
        /// </summary>
        /// <param name="item">A parameter of the generics Type T</param>
        private void RaiseOnItemPopped(T item)
        {
            EventHandler<CollectionEventArgs<T>> handler = OnItemPopped;
            handler(this, new CollectionEventArgs<T>(item));
        }


        #endregion

    }
}
