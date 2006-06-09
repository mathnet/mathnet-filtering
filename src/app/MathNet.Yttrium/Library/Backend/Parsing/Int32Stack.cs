#region Copyright 2001-2006 Christoph Daniel Rüegg [GPL]
//Math.NET Symbolics: Yttrium, part of Math.NET
//Copyright (c) 2001-2006, Christoph Daniel Rueegg, http://cdrnet.net/.
//All rights reserved.
//This Math.NET package is available under the terms of the GPL.

//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program; if not, write to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;

using MathNet.Symbolics.Backend.Exceptions;

namespace MathNet.Symbolics.Backend.Parsing
{
    // TODO: Replace with .NET 2.0 generic stack

    /// <summary>
    /// Int32 Stack with auto expansion.
    /// </summary>
    internal sealed class Int32Stack
    {
        private int[] stack;
        private int count;
        private int maxSize;

        public Int32Stack(int minSize, int maxSize)
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
            get { return count - 1; }
        }

        /// <summary>The current size of the stack.</summary>
        public int Size
        {
            get { return stack.Length - 1; }
        }

        private void Init(int size)
        {
            stack = new int[size];
            stack[0] = 0;
            count = 1;
        }

        /// <summary>Add a new item to the top of the stack.</summary>
        /// <param name="item">The item to add.</param>
        public void Push(int item)
        {
            if(count == stack.Length)
                Expand();
            stack[count++] = item;
        }

        /// <summary>Returns and removes the item on top of the stack.</summary>
        /// <returns>The item that was on the stack's top.</returns>
        public int Pop()
        {
            return stack[--count];
        }

        /// <summary>Returns (without removing) the item on top of the stack.</summary>
        /// <returns>The item that is on the stack's top.</returns>
        public int Peek()
        {
            return stack[count - 1];
        }

        /// <summary>Removes the item on top of the stack and returns the new item on top if the stack</summary>
        /// <returns>The item that now is the stack's top (after removement).</returns>
        public int PopPeek()
        {
            count--;
            return stack[count - 1];
        }

        /// <summary>Removes the item on top of the stack.</summary>
        public void Remove()
        {
            count--;
        }

        /// <summary>Gets items form the stack, starting from the top. Zero Based.</summary>
        public int this[int index]
        {
            get { return stack[count - 1 - index]; }
        }

        private void Expand()
        {
            if(maxSize > 0 && stack.Length * 2 > maxSize)
                throw new ParsingException("Parsing failed. Maximum marker stack size exceeded.");
            int[] newStack = new int[stack.Length * 2];
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
