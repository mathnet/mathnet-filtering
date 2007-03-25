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
using System.IO;

using MathNet.Symbolics;

namespace MathNet.Symbolics.Interpreter
{
    public class Parser : IParser
    {
        private ParserScanner scanner;

        public Parser()
        {
        }

        public void Interpret(TextReader reader, IMathSystem system)
        {
            if(scanner == null)
                scanner = new ParserScanner(reader, system);
            else
                scanner.Reset(reader, system);
            scanner.AllStatements();
        }

        public void Interpret(string expression, IMathSystem system)
        {
            Interpret(new StringReader(expression), system);
        }

        public void Interpret(FileInfo file, IMathSystem system)
        {
            if(file == null)
                throw new ArgumentNullException("file");

            Interpret(file.OpenText(), system);
        }
    }
}
