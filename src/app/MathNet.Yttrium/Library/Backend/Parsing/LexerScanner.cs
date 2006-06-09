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
using System.Text;
using System.IO;
using System.Globalization;

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Exceptions;

namespace MathNet.Symbolics.Backend.Parsing
{
    public sealed class LexerScanner
    {
        private LexerMarker lexer;
        private Context context;
        private const char eof = char.MaxValue;

        private char executorCharacter, seperatorCharacter;
        private EncapsulationFormat listEncapsulation, vectorEncapsulation, setEncapsulation, scalarEncapsulation, literalEncapsulation;
        private readonly NumberFormatInfo numberFormat = Context.NumberFormat;

        public LexerScanner(TextReader reader, Context context)
        {
            this.lexer = new LexerMarker(reader);
            this.context = context;

            this.executorCharacter = context.ExecutorCharacter;
            this.seperatorCharacter = context.SeparatorCharacter;
            this.listEncapsulation = context.ListEncapsulation;
            this.vectorEncapsulation = context.VectorEncapsulation;
            this.setEncapsulation = context.SetEncapsulation;
            this.scalarEncapsulation = context.ScalarEncapsulation;
            this.literalEncapsulation = context.LiteralEncapsulation;
        }

        public LexerToken NextToken()
        {
            char beginChar = lexer.LookaheadFistCharacter;
            if(IsWhitespaceCharacter(beginChar))
            {
                ScanSkipWhiteSpace();
                beginChar = lexer.LookaheadFistCharacter;
            }
            if(IsOpeningLiteralCharacter(beginChar))
                return ScanLiteral();
            if(IsNumberCharacter(beginChar))
                return ScanNumber();
            if(IsMarkerCharacter(beginChar))
                return ScanMarkers();
            if(IsBeginTextIdentifierCharacter(beginChar))
                return ScanTextIdentifier();
            if(IsOpeningPunctuationCharacter(beginChar))
                return ScanOpeningPunctuation();
            if(IsClosingPunctuationCharacter(beginChar))
                return ScanClosingPunctuation();
            if(IsSeparatorExecutorCharacter(beginChar))
                return ScanSeparatorExecutor();
            if(IsEndOfLineCharacter(beginChar))
                return ScanEndOfLine();
            if(IsEndOfFileCharacter(beginChar))
                return ScanEndOfFile();
            if(IsSymbolIdentifierCharacter(beginChar))
                return ScanSymbolIdentifier();
            throw new ParsingException("Parsing failed. Lexer Scanner detected unexpected token '" + beginChar + "'");
        }

        #region Scanners
        private void ScanSkipWhiteSpace()
        {
            while(IsWhitespaceCharacter(lexer.LookaheadFistCharacter))
                lexer.Consume();
        }

        private static LexerToken ScanEndOfFile()
        {
            return new LexerToken(eof.ToString(), TokenTypes.EndOfFile);
        }

        private LexerToken ScanEndOfLine()
        {
            StringBuilder sb = new StringBuilder();
            do
            {
                while(IsWhitespaceCharacter(lexer.LookaheadFistCharacter))
                    lexer.Consume();
                while(IsEndOfLineCharacter(lexer.LookaheadFistCharacter))
                {
                    sb.Append(lexer.LookaheadFistCharacter);
                    lexer.Consume();
                }
            }
            while(IsWhitespaceCharacter(lexer.LookaheadFistCharacter));
            //return new LexerToken("\n",TokenTypes.EndOfLine);
            return NextToken();
        }

        private LexerToken ScanSeparatorExecutor()
        {
            char c = lexer.LookaheadFistCharacter;
            lexer.Consume();
            if(c == executorCharacter)
                return new LexerToken(executorCharacter.ToString(), TokenTypes.Executor);
            return new LexerToken(seperatorCharacter.ToString(), TokenTypes.Separator);
        }

        private LexerToken ScanMarkers()
        {
            if(lexer.LookaheadCharacter(1) == '_' && IsTextIdentifierCharacter(lexer.LookaheadCharacter(2))
                || lexer.LookaheadCharacter(1) != '_' && IsTextIdentifierCharacter(lexer.LookaheadCharacter(1)))
            {
                char c = lexer.LookaheadFistCharacter;
                lexer.Consume();
                LexerToken t = ScanTextIdentifier();
                t.Text = c + t.Text;
                //t.Types |= TokenTypes.Marked;
                return t;
            }
            return ScanSymbolIdentifier();
        }

        private LexerToken ScanTextIdentifier()
        {
            StringBuilder sb = new StringBuilder();
            lexer.Mark();
            bool mathId = false;
            while(IsTextIdentifierCharacter(lexer.LookaheadFistCharacter))
            {
                sb.Append(lexer.LookaheadFistCharacter);
                lexer.Consume();
            }
            if(lexer.LookaheadFistCharacter == '.' && IsBeginTextIdentifierCharacter(lexer.LookaheadCharacter(1)))
            {
                mathId = true;
                sb.Append('.');
                lexer.Consume();
                while(IsTextIdentifierCharacter(lexer.LookaheadFistCharacter))
                {
                    sb.Append(lexer.LookaheadFistCharacter);
                    lexer.Consume();
                }
            }
            if(sb.Length == 1 && sb[0] == '_')
            {
                lexer.RollbackMark();
                return ScanSymbolIdentifier();
            }
            else
            {
                lexer.CommitMark();
                string s = sb.ToString();
                switch(s)
                {
                    case "define":
                        return new LexerToken(s, TokenTypes.DefineKeyword);
                    case "instantiate":
                        return new LexerToken(s, TokenTypes.InstantiateKeyword);
                    case "signal":
                        return new LexerToken(s, TokenTypes.SignalKeyword);
                    case "bus":
                        return new LexerToken(s, TokenTypes.BusKeyword);
                }
                if(mathId)
                    return new LexerToken(s, TokenTypes.MathIdentifier);
                else
                    return new LexerToken(s, TokenTypes.TextIdentifier);
            }
        }

        private LexerToken ScanLiteral()
        {
            StringBuilder sb = new StringBuilder();
            lexer.Consume();
            while(true)
            {
                char c = lexer.LookaheadFistCharacter;
                if(IsClosingLiteralCharacter(c))
                {
                    if(IsClosingLiteralCharacter(lexer.LookaheadCharacter(1)))
                    { // character de-stuffing: "" -> " 
                        sb.Append(c);
                        lexer.Consume();
                        lexer.Consume();
                        continue;
                    }
                    else
                        break;
                }
                if(c == eof)
                    throw new ParsingException("Parsing failed. Unexpected end of file. Did you forget to close a literal encapsulation?");
                sb.Append(c);
                lexer.Consume();
            }
            lexer.Consume();
            return new LexerToken(sb.ToString(), TokenTypes.Literal);
        }

        private string ScanIntegerHelper()
        {
            StringBuilder sb = new StringBuilder();
            while(IsNumberCharacter(lexer.LookaheadFistCharacter))
            {
                sb.Append(lexer.LookaheadFistCharacter);
                lexer.Consume();
            }
            return sb.ToString();
        }

        private LexerToken ScanNumber()
        {
            string part1 = ScanIntegerHelper();
            if(lexer.LookaheadFistCharacter == numberFormat.NumberDecimalSeparator[0] && IsNumberCharacter(lexer.LookaheadCharacter(1)))
            {
                lexer.Consume();
                string part2 = ScanIntegerHelper();
                if(lexer.LookaheadFistCharacter == 'e')
                {
                    lexer.Mark();
                    lexer.Consume();
                    if(IsNumberCharacter(lexer.LookaheadFistCharacter))
                    {
                        string part3 = ScanIntegerHelper();
                        lexer.CommitMark();
                        return new LexerToken(part1 + numberFormat.NumberDecimalSeparator + part2 + 'e' + part3, TokenTypes.Real);
                    }
                    if(lexer.LookaheadFistCharacter == numberFormat.NegativeSign[0] && IsNumberCharacter(lexer.LookaheadCharacter(1)))
                    {
                        lexer.Consume();
                        string part3 = ScanIntegerHelper();
                        lexer.CommitMark();
                        return new LexerToken(part1 + numberFormat.NumberDecimalSeparator + part2 + "e" + numberFormat.NegativeSign + part3, TokenTypes.Real);
                    }
                    lexer.RollbackMark();
                }
                return new LexerToken(part1 + numberFormat.NumberDecimalSeparator + part2, TokenTypes.Real);
            }
            return new LexerToken(part1, TokenTypes.Integer);
        }

        private LexerToken ScanSymbolIdentifier()
        {
            if(lexer.LookaheadFistCharacter == '<' && lexer.LookaheadCharacter(1) == '-')
            {
                lexer.Consume(); lexer.Consume();
                return new LexerToken("<-", TokenTypes.Assignment);
            }
            else if(lexer.LookaheadFistCharacter == '-' && lexer.LookaheadCharacter(1) == '>')
            {
                lexer.Consume(); lexer.Consume();
                return new LexerToken("->", TokenTypes.Association);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(lexer.LookaheadFistCharacter);
            lexer.Consume();
            while(IsSymbolIdentifierCharacter(lexer.LookaheadFistCharacter)
                && context.Library.Entities.ContainsSymbol(sb.ToString() + lexer.LookaheadFistCharacter))
            {
                sb.Append(lexer.LookaheadFistCharacter);
                lexer.Consume();
            }
            //if(sb[0] == '@' || sb[0] == '#')
            //	return new LexerToken(sb.ToString(),TokenTypes.SymbolIdentifier | TokenTypes.Marked);
            return new LexerToken(sb.ToString(), TokenTypes.SymbolIdentifier);
        }

        private LexerToken ScanOpeningPunctuation()
        {
            if(lexer.LookaheadFistCharacter == '<' && lexer.LookaheadCharacter(1) == '-')
            {
                lexer.Consume(); lexer.Consume();
                return new LexerToken("<-", TokenTypes.Assignment);
            }
            else if(lexer.LookaheadFistCharacter == '-' && lexer.LookaheadCharacter(1) == '>')
            {
                lexer.Consume(); lexer.Consume();
                return new LexerToken("->", TokenTypes.Association);
            }

            char c = lexer.LookaheadFistCharacter;
            lexer.Consume();
            if(c == scalarEncapsulation.Prefix)
                return new LexerToken(scalarEncapsulation.Prefix.ToString(), IsSymbolIdentifierCharacter(c) ? TokenTypes.LeftScalar | TokenTypes.SymbolIdentifier : TokenTypes.LeftScalar);
            if(c == setEncapsulation.Prefix)
                return new LexerToken(setEncapsulation.Prefix.ToString(), TokenTypes.LeftSet);
            if(c == vectorEncapsulation.Prefix)
                return new LexerToken(vectorEncapsulation.Prefix.ToString(), TokenTypes.LeftVector);
            return new LexerToken(listEncapsulation.Prefix.ToString(), TokenTypes.LeftList);
        }

        private LexerToken ScanClosingPunctuation()
        {
            if(lexer.LookaheadFistCharacter == '<' && lexer.LookaheadCharacter(1) == '-')
            {
                lexer.Consume(); lexer.Consume();
                return new LexerToken("<-", TokenTypes.Assignment);
            }
            else if(lexer.LookaheadFistCharacter == '-' && lexer.LookaheadCharacter(1) == '>')
            {
                lexer.Consume(); lexer.Consume();
                return new LexerToken("->", TokenTypes.Association);
            }

            char c = lexer.LookaheadFistCharacter;
            lexer.Consume();
            if(c == scalarEncapsulation.Postfix)
                return new LexerToken(scalarEncapsulation.Postfix.ToString(), IsSymbolIdentifierCharacter(c) ? TokenTypes.RightScalar | TokenTypes.SymbolIdentifier : TokenTypes.RightScalar);
            if(c == setEncapsulation.Postfix)
                return new LexerToken(setEncapsulation.Postfix.ToString(), TokenTypes.RightSet);
            if(c == vectorEncapsulation.Postfix)
                return new LexerToken(vectorEncapsulation.Postfix.ToString(), TokenTypes.RightVector);
            return new LexerToken(listEncapsulation.Postfix.ToString(), TokenTypes.RightList);
        }
        #endregion

        #region Selectors
        private static bool IsNumberCharacter(char c)
        {
            //0..9
            return (c >= '0' && c <= '9');
        }

        private static bool IsTextIdentifierCharacter(char c)
        {//a..zA..Z0..9_
            return (c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c == '_');
        }

        private static bool IsBeginTextIdentifierCharacter(char c)
        {//a..zA..Z_
            return (c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c == '_');
        }

        private static bool IsSymbolIdentifierCharacter(char c)
        {
            UnicodeCategory cat = char.GetUnicodeCategory(c);
            switch(cat)
            {
                case UnicodeCategory.OtherPunctuation:
                case UnicodeCategory.CurrencySymbol:
                case UnicodeCategory.MathSymbol:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.ModifierSymbol:
                case UnicodeCategory.DashPunctuation:
                case UnicodeCategory.ConnectorPunctuation:
                case UnicodeCategory.OtherSymbol:
                    return true;
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.UppercaseLetter:
                    return !IsBeginTextIdentifierCharacter(c);
                default:
                    return false;
            }
        }

        private static bool IsMarkerCharacter(char c)
        {
            return c == '@' || c == '#';
        }

        private bool IsSeparatorExecutorCharacter(char c)
        {
            return c == executorCharacter || c == seperatorCharacter;
        }

        private bool IsOpeningPunctuationCharacter(char c)
        {
            return c == listEncapsulation.Prefix || c == vectorEncapsulation.Prefix
                || c == setEncapsulation.Prefix || c == scalarEncapsulation.Prefix;
        }

        private bool IsClosingPunctuationCharacter(char c)
        {
            return c == listEncapsulation.Postfix || c == vectorEncapsulation.Postfix
                || c == setEncapsulation.Postfix || c == scalarEncapsulation.Postfix;
        }

        private bool IsOpeningLiteralCharacter(char c)
        {
            return c == literalEncapsulation.Prefix;
        }

        private bool IsClosingLiteralCharacter(char c)
        {
            return c == literalEncapsulation.Postfix;
        }

        private static bool IsEndOfFileCharacter(char c)
        {
            return c == eof;
        }

        private static bool IsEndOfLineCharacter(char c)
        {
            return c == '\n' || c == '\r';
        }

        private static bool IsWhitespaceCharacter(char c)
        {
            return char.IsWhiteSpace(c);
        }
        //private bool IsSystemTextIdentifier(string s)
        //{
        //    return s == "define" || s == "instantiate";
                
        //        //s.Equals("if") || s.Equals("then") || s.Equals("else")
        //        //|| s.Equals("elseif") || s.Equals("while") || s.Equals("do")
        //        //|| s.Equals("for") || s.Equals("end");
        //}
        #endregion

        #region Reset
        /// <summary>Clear the buffer. Replace the current stream with a new one.</summary>
        public void Reset(TextReader reader, Context context)
        {
            lexer.Reset(reader);
            this.context = context;

            this.executorCharacter = context.ExecutorCharacter;
            this.seperatorCharacter = context.SeparatorCharacter;
            this.listEncapsulation = context.ListEncapsulation;
            this.vectorEncapsulation = context.VectorEncapsulation;
            this.setEncapsulation = context.SetEncapsulation;
            this.scalarEncapsulation = context.ScalarEncapsulation;
            this.literalEncapsulation = context.LiteralEncapsulation;
        }
        public void Reset()
        {
            lexer.Reset();
        }
        #endregion
    }
}
