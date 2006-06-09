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
using System.IO;

using MathNet.Symbolics.Backend.Exceptions;

namespace MathNet.Symbolics.Backend.Parsing
{
    /// <summary>
    /// Circular char fifo/queue buffer with auto expansion.
    /// </summary>
    internal sealed class LexerBuffer
    {
        private TextReader reader;
        private char[] buffer;
        private int sizeLessOne;
        private int offset; //physical index of front token
        private int count;
        private int maxSize;

        public LexerBuffer(int minSize, int maxSize, TextReader reader)
        {
            this.maxSize = maxSize;
            this.reader = reader;
            Init(minSize);
        }

        public int Count
        {
            get { return count; }
        }

        public int Size
        {
            get { return sizeLessOne + 1; }
        }

        /// <summary>Fetch a token from the queue by index</summary>
        /// <param name="index">The index of the token to fetch, where zero is the token at the front of the queue</param>
        public char ElementAt(int index)
        {
            if(count <= index)
                RefillBuffer(index - count + 1);
            return buffer[(offset + index) & sizeLessOne];
        }

        #region Append and Remove Buffer Content
        /// <summary>Add token to end of the queue</summary>
        /// <param name="token">The token to add</param>
        private void Append(char token)
        {
            if(count == buffer.Length)
                Expand();
            buffer[(offset + count) & sizeLessOne] = token;
            count++;
        }

        /// <summary>Remove char from the front of the queue</summary>
        public void RemoveFirst()
        {
            if(count == 0)
                RefillBuffer(1);
            offset = (offset + 1) & sizeLessOne;
            count--;
        }

        /// <summary>Remove count of chars from the front of the queue</summary>
        public void RemoveBlock(int amount)
        {
            if(count < amount)
                RefillBuffer(amount - count);
            offset = (offset + amount) & sizeLessOne;
            count -= amount;
        }
        #endregion

        #region Buffer Maintenance
        /// <summary>Expand the token buffer by doubling its capacity</summary>
        private void Expand()
        {
            if(maxSize > 0 && buffer.Length * 2 > maxSize)
                throw new ParsingException("Parsing failed. Maximum lexer buffer size exceeded.");
            char[] newBuffer = new char[buffer.Length * 2];
            for(int i = 0; i < buffer.Length; i++)
                newBuffer[i] = buffer[(offset + i) & sizeLessOne];
            buffer = newBuffer;
            sizeLessOne = buffer.Length - 1;
            offset = 0;
        }

        private void RefillBuffer(int minimumDeltaCount)
        {
            //This method is that big because we try to read faster
            //in large blocks instead of every single character in a loop,
            //so we need to handle different buffer states different ...
            while(minimumDeltaCount > sizeLessOne + 1 - count)
                Expand();
            if(count == 0)
            {
                offset = 0;
                count = reader.Read(buffer, 0, sizeLessOne + 1);
                if(count < minimumDeltaCount)
                    count += reader.ReadBlock(buffer, count, sizeLessOne + 1 - count);
                if(count < minimumDeltaCount)
                {
                    FillBufferEof(minimumDeltaCount - count);
                    return;
                }
            }
            else
            {
                int tail = (offset + count) & sizeLessOne;
                if(tail > offset)
                {
                    int read = reader.Read(buffer, tail, sizeLessOne + 1 - tail);
                    if(read < minimumDeltaCount && read < sizeLessOne + 1 - tail)
                        read += reader.ReadBlock(buffer, tail + read, sizeLessOne + 1 - tail - read);
                    count += read;
                    if(read < minimumDeltaCount)
                    {
                        if(read < sizeLessOne + 1 - tail)
                        {
                            FillBufferEof(sizeLessOne + 1 - tail - read);
                            return;
                        }
                        int read2 = reader.Read(buffer, 0, offset);
                        if(read + read2 < minimumDeltaCount)
                            read2 += reader.ReadBlock(buffer, read2, offset - read2);
                        count += read2;
                        if(read + read2 < minimumDeltaCount)
                        {
                            FillBufferEof(minimumDeltaCount - (read + read2));
                            return;
                        }
                    }
                }
                else
                {
                    int read = reader.Read(buffer, tail, offset - tail);
                    if(read < minimumDeltaCount)
                        read += reader.ReadBlock(buffer, tail + read, offset - tail - read);
                    count += read;
                    if(read < minimumDeltaCount)
                    {
                        FillBufferEof(minimumDeltaCount - read);
                        return;
                    }
                }
            }
        }
        private void FillBufferEof(int amount)
        {
            char eof = char.MaxValue;
            for(int i = 0; i < amount; i++)
                buffer[(offset + count++) & sizeLessOne] = eof;
        }

        private void Init(int minSize)
        {
            int size;
            if(minSize < 0)
            {
                size = 16;
            }
            else if(minSize >= (Int32.MaxValue / 2))
            {
                size = Int32.MaxValue;
            }
            else
            {
                for(size = 2; size < minSize; size *= 2) ;
            }
            buffer = new char[size];
            sizeLessOne = size - 1;
            offset = 0;
            count = 0;
        }
        #endregion

        #region Reset
        /// <summary>Clear the queue. Replace the current stream with a new one.</summary>
        public void Reset(TextReader reader)
        {
            offset = 0;
            count = 0;
            this.reader = reader;
        }
        /// <summary>Clear the queue. Leaving the previous buffer alone.</summary>
        public void Reset()
        {
            offset = 0;
            count = 0;
        }
        #endregion
    }
}
