#region Math.NET Yttrium (GPL) by Christoph Ruegg
// Math.NET Yttrium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2007, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;

namespace MathNet.Symbolics.Interpreter
{
    // TODO: Replace with .NET 2.0 generic stack

    /// <summary>
    /// Entity Stack with auto expansion.
    /// </summary>
    public sealed class EntityStack
    {
        private IEntity[] stack;
        private int count;
        private int maxSize;

        public EntityStack(int minSize, int maxSize)
        {
            this.maxSize = maxSize;
            // Find first power of 2 >= to requested size
            int size;
            if(minSize < 0)
            {
                Init(16);
                return;
            }
            // check for overflow
            if(minSize >= (Int32.MaxValue / 2))
            {
                Init(Int32.MaxValue);
                return;
            }
            for(size = 2; size < minSize; size *= 2) ;
            Init(size);
        }

        /// <summary>The current count of elements in the stack</summary>
        public int Count
        {
            get { return count; }
        }

        /// <summary>The current size of the stack.</summary>
        public int Size
        {
            get { return stack.Length; }
        }

        private void Init(int size)
        {
            stack = new IEntity[size];
            count = 0;
        }

        /// <summary>Add a new item to the top of the stack.</summary>
        /// <param name="item">The item to add.</param>
        public void Push(IEntity item)
        {
            if(count == stack.Length)
                Expand();
            stack[count++] = item;
        }

        /// <summary>Returns and removes the item on top of the stack.</summary>
        /// <returns>The item that was on the stack's top.</returns>
        public IEntity Pop()
        {
            IEntity token = stack[--count];
            stack[count] = null; //remove reference to allow GC to do its job...
            return token;
        }

        /// <summary>Returns (without removing) the item on top of the stack.</summary>
        /// <returns>The item that is on the stack's top.</returns>
        public IEntity Peek()
        {
            return stack[count - 1];
        }

        /// <summary>Removes the item on top of the stack and returns the new item on top if the stack</summary>
        /// <returns>The item that now is the stack's top (after removement).</returns>
        public IEntity PopPeek()
        {
            stack[--count] = null; //remove reference to allow GC to do its job...
            return stack[count - 1];
        }

        /// <summary>Removes the item on top of the stack.</summary>
        public void Remove()
        {
            stack[--count] = null; //remove reference to allow GC to do its job...
        }

        /// <summary>Gets items form the stack, starting from the top. Zero Based.</summary>
        public IEntity this[int index]
        {
            get { return stack[count - 1 - index]; }
        }

        private void Expand()
        {
            if(maxSize > 0 && stack.Length * 2 > maxSize)
                throw new MathNet.Symbolics.Exceptions.ParsingException("Parsing failed. Maximum token stack size exceeded.");
            IEntity[] newStack = new IEntity[stack.Length * 2];
            for(int i = 0; i < stack.Length; i++)
                newStack[i] = stack[i];
            stack = newStack;
        }

        /// <summary>Reset the stack, remove all items.</summary>
        public void Reset()
        {
            count = 1;
        }
    }
}
