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

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Workplace;

namespace MathNet.Symbolics.Backend.Parsing
{
    public class Parser
    {
        private ParserScanner scanner;

        public Parser()
        {
        }

        public void Interpret(TextReader reader, MathSystem system)
        {
            if(scanner == null)
                scanner = new ParserScanner(reader, system);
            else
                scanner.Reset(reader, system);
            scanner.AllStatements();
        }

        public void Interpret(string expression, MathSystem system)
        {
            Interpret(new StringReader(expression), system);
        }

        public void Interpret(FileInfo file, MathSystem system)
        {
            Interpret(file.OpenText(), system);
        }
    }
}
