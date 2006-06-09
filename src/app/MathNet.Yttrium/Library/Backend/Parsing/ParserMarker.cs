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
using System.Text;

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Exceptions;

namespace MathNet.Symbolics.Backend.Parsing
{
    internal sealed class ParserMarker
    {
        private ParserBuffer buffer;
        private Int32Stack markers;

        private int markerOffset; // = 0;
        private int markerCount; // = 0;
        private int consumeOffset; // = 0;

        public ParserMarker(TextReader reader, Context context)
        {
            buffer = new ParserBuffer(16, 4096, reader, context);
            markers = new Int32Stack(16, 4096);
        }

        #region Consuming
        public void Consume()
        {
            consumeOffset++;
            if(markerCount == 0 && consumeOffset > (buffer.Size << 2))
            {
                buffer.RemoveBlock(consumeOffset);
                consumeOffset = 0;
            }
        }

        public LexerToken ConsumeGet()
        {
            LexerToken t = buffer.ElementAt(consumeOffset);
            Consume();
            return t;
        }

        public void Match(TokenTypes expected)
        {
            if(!buffer.ElementAt(consumeOffset).IsType(expected))
                throw new ParsingException(string.Format(MathNet.Symbolics.Properties.Resources.ex_Parsing_Failed_TokenMismatch, expected.ToString(), buffer.ElementAt(consumeOffset).Text, CurrentTokenNeighbourhood()));
            Consume();
        }
        public void Match(TokenTypes expected, string content)
        {
            if(!buffer.ElementAt(consumeOffset).IsType(expected) || !buffer.ElementAt(consumeOffset).Text.Equals(content))
                throw new ParsingException(string.Format(MathNet.Symbolics.Properties.Resources.ex_Parsing_Failed_TokenMismatchEx, expected.ToString(), buffer.ElementAt(consumeOffset).Text, CurrentTokenNeighbourhood(), content));
            Consume();
        }
        public void Match(string content)
        {
            if(!buffer.ElementAt(consumeOffset).Text.Equals(content))
                throw new ParsingException(string.Format(MathNet.Symbolics.Properties.Resources.ex_Parsing_Failed_TokenMismatch, content, buffer.ElementAt(consumeOffset).Text, CurrentTokenNeighbourhood()));
            Consume();
        }

        public LexerToken MatchGet(TokenTypes expected)
        {
            LexerToken t = buffer.ElementAt(consumeOffset);
            if(!t.IsType(expected))
                throw new ParsingException(string.Format(MathNet.Symbolics.Properties.Resources.ex_Parsing_Failed_TokenMismatch, expected.ToString(), t.Text, CurrentTokenNeighbourhood()));
            Consume();
            return t;
        }
        private string CurrentTokenNeighbourhood()
        {
            int start = Math.Max(0, consumeOffset - 4);
            int end = consumeOffset + 4;
            StringBuilder sb = new StringBuilder(buffer.ElementAt(start).Text);
            for(int i = start + 1; i < end; i++)
            {
                sb.Append(' ');
                sb.Append(buffer.ElementAt(i).Text);
            }
            return sb.ToString();
        }
        #endregion

        public bool IsEndOfFile
        {
            get { return buffer.ElementAt(consumeOffset).IsType(TokenTypes.EndOfFile); }
        }

        public LexerToken LookaheadToken(int index)
        {
            return buffer.ElementAt(consumeOffset + index);
        }

        public LexerToken LookaheadFistToken
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
        public void Reset(TextReader reader, Context context)
        {
            consumeOffset = 0;
            markerOffset = 0;
            buffer.Reset(reader, context);
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
