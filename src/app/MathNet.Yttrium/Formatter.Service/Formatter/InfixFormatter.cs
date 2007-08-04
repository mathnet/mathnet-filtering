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
using System.Collections.Generic;
using System.Text;

namespace MathNet.Symbolics.Formatter
{
    internal class InfixFormatter : IFormatter
    {
        public string Format(Signal signal, FormattingOptions options)
        {
            LinkedList<string> sequence = new LinkedList<string>();
            sequence.AddFirst("sentinel");
            FormatExpression(signal, sequence, options);
            sequence.RemoveFirst();

            StringBuilder sb = new StringBuilder(8 * sequence.Count); // heuristics
            foreach(string token in sequence)
                sb.Append(token);
            //sb.Replace("+-", "-");
            //sb.Replace("--", "+");
            return sb.ToString();
        }

        /// <returns>precedence group</returns>
        private int FormatExpression(Signal signal, LinkedList<string> sequence, FormattingOptions options)
        {
            if(signal.BehavesAsSourceSignal)
            {
                if(signal.Value != null)
                {
                    IFormattableLeaf leaf = signal.Value as IFormattableLeaf;
                    if(leaf != null)
                    {
                        int prec;
                        sequence.AddLast(leaf.Format(options, out prec));
                        return prec;
                    }
                    else
                    {
                        sequence.AddLast(signal.Value.ToString());
                        return -1;
                    }
                }
                if(!string.IsNullOrEmpty(signal.Label))
                {
                    sequence.AddLast(signal.Label);
                    return -1;
                }
                sequence.AddLast("signal");
                return -1;
            }

            Port port = signal.DrivenByPort;
            IEntity entity = port.Entity;
            int precedence = entity.PrecedenceGroup;
            InfixNotation notation = entity.Notation;

            if(notation == InfixNotation.PreOperator)
                sequence.AddLast(entity.Symbol);
            else if(notation == InfixNotation.None)
            {
                sequence.AddLast(entity.Symbol);
                sequence.AddLast("(");
            }

            bool notFirst = false;
            foreach(Signal s in port.InputSignals)
            {
                if(notFirst)
                    sequence.AddLast(entity.Symbol);
                else
                    notFirst = true;

                LinkedListNode<string> before = sequence.Last;
                int subPrecedence = FormatExpression(s, sequence, options);
                if(subPrecedence >= precedence && notation != InfixNotation.None)
                {
                    sequence.AddAfter(before, "(");
                    sequence.AddLast(")");
                }
            }

            if(notation == InfixNotation.PostOperator)
                sequence.AddLast(entity.Symbol);
            else if(notation == InfixNotation.None)
                sequence.AddLast(")");

            return precedence;
        }
    }
}
