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
    internal sealed class LexerMarker
    {
        private LexerBuffer buffer;
        private Int32Stack markers;

        private int markerOffset; // = 0;
        private int markerCount; // = 0;
        private int consumeOffset; // = 0;

        public LexerMarker(TextReader reader)
        {
            buffer = new LexerBuffer(16, 4096, reader);
            markers = new Int32Stack(16, 4096);
        }

        public void Consume()
        {
            consumeOffset++;
            if(markerCount == 0 && consumeOffset > (buffer.Size << 2))
            {
                buffer.RemoveBlock(consumeOffset);
                consumeOffset = 0;
            }
        }

        public char LookaheadCharacter(int index)
        {
            return buffer.ElementAt(consumeOffset + index);
        }

        public char LookaheadFistCharacter
        {
            get { return buffer.ElementAt(consumeOffset); }
        }

        #region Marker
        public void Mark()
        {
            if(markerCount++ == 0)
            {
                buffer.RemoveBlock(consumeOffset);
                consumeOffset = 0;
            }
            markerOffset = consumeOffset;
            markers.Push(consumeOffset);
        }

        public void CommitMark()
        {
            markerCount--;
            //Returns 0 if the last item was removed.
            markerOffset = markers.PopPeek();
            if(markerCount == 0)
            {
                buffer.RemoveBlock(consumeOffset);
                consumeOffset = 0;
            }
        }

        public void RollbackMark()
        {
            markerCount--;
            consumeOffset = markers.Pop();
            markerOffset = consumeOffset;
        }
        #endregion

        #region Reset
        /// <summary>Clear the buffer. Replace the current stream with a new one.</summary>
        public void Reset(TextReader reader)
        {
            consumeOffset = 0;
            markerOffset = 0;
            buffer.Reset(reader);
            markers.Reset();
        }
        public void Reset()
        {
            consumeOffset = 0;
            markerOffset = 0;
            buffer.Reset();
            markers.Reset();
        }
        #endregion
    }
}
