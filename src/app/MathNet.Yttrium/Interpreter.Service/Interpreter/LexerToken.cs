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
    [Flags]
    public enum TokenTypes : long
    {
        Unknown = 0x00000000,
        System = 0x00000001,
        EndOfFile = 0x00000002,
        EndOfLine = 0x00000004,
        Integer = 0x00000008,
        Real = 0x00000010,
        Literal = 0x00000020,

        TextIdentifier = 0x00000040,
        SystemTextIdentifier = TextIdentifier | System,
        SymbolIdentifier = 0x00000080,
        SystemSymbolIdentifier = SymbolIdentifier | System,
        MathIdentifier = 0x00000100,

        Left = 0x00000200,
        Right = 0x00000400,

        ListEncapsulation = 0x00000800, //()
        LeftList = ListEncapsulation | Left,
        RightList = ListEncapsulation | Right,

        VectorEncapsulation = 0x00001000, //[]
        LeftVector = VectorEncapsulation | Left,
        RightVector = VectorEncapsulation | Right,

        SetEncapsulation = 0x00002000, //{}
        LeftSet = SetEncapsulation | Left,
        RightSet = SetEncapsulation | Right,

        ScalarEncapsulation = 0x00004000, //<>
        LeftScalar = ScalarEncapsulation | Left,
        RightScalar = ScalarEncapsulation | Right,

        Separator = 0x00008000, //,
        Executor = 0x00010000, //;

        DefineKeyword = 0x00100000 | SystemTextIdentifier,
        InstantiateKeyword = 0x00200000 | SystemTextIdentifier,
        SignalKeyword = 0x00400000 | SystemTextIdentifier,
        BusKeyword = 0x00800000 | SystemTextIdentifier,

        Assignment = 0x10000000 | SystemSymbolIdentifier,
        Association = 0x20000000 | SystemSymbolIdentifier
    }

    public sealed class LexerToken
    {
        private string text;
        private TokenTypes types;

        public LexerToken(string text)
        {
            this.text = text;
            //types = TokenTypes.Unknown; //default value
        }

        public LexerToken(string text, TokenTypes types)
        {
            this.text = text;
            this.types = types;
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public TokenTypes Types
        {
            get { return types; }
            set { types = value; }
        }

        public bool IsType(TokenTypes types)
        {
            return (this.types & types) == types;
        }

        public override string ToString()
        {
            return "Token '" + text + "' - " + types.ToString();
        }
    }
}
