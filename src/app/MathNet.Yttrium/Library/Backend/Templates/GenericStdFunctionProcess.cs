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
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics.Backend.Templates
{
    public delegate ValueStructure DefaultValue();
    public delegate T ProcessAction<T>(T accumulation, T item);
    public delegate T ConvertFrom<T>(ValueStructure value);

    /// <summary>
    /// A process for standard functions mapping a set of inputs to one output, which is mapped to several signals.
    /// </summary>
    public class GenericStdFunctionProcess<T> : GenericFunctionProcess where T : ValueStructure
    {
        private DefaultValue defaultValue;
        private ProcessAction<T> processAction;
        private ConvertFrom<T> convertFrom;

        public GenericStdFunctionProcess(DefaultValue defaultValue, ProcessAction<T> processAction, ConvertFrom<T> convertFrom, bool[] inInput, bool[] inInternal, bool[] outOutput, bool[] outInternal)
            : base(inInput, inInternal, outOutput, outInternal)
        {
            this.defaultValue = defaultValue;
            this.processAction = processAction;
            this.convertFrom = convertFrom;
        }
        public GenericStdFunctionProcess(DefaultValue defaultValue, ProcessAction<T> processAction, bool[] inInput, bool[] inInternal, bool[] outOutput, bool[] outInternal)
            : base(inInput, inInternal, outOutput, outInternal)
        {
            this.defaultValue = defaultValue;
            this.processAction = processAction;
            this.convertFrom = delegate(ValueStructure value) { return (T)value; };
        }
        public GenericStdFunctionProcess(DefaultValue defaultValue, ProcessAction<T> processAction, ConvertFrom<T> convertFrom, int firstInput, int lastInput, int output)
            : base(firstInput, lastInput, output)
        {
            this.defaultValue = defaultValue;
            this.processAction = processAction;
            this.convertFrom = convertFrom;
        }
        public GenericStdFunctionProcess(DefaultValue defaultValue, ProcessAction<T> processAction, int firstInput, int lastInput, int output)
            : base(firstInput, lastInput, output)
        {
            this.defaultValue = defaultValue;
            this.processAction = processAction;
            this.convertFrom = delegate(ValueStructure value) { return (T)value; };
        }
        public GenericStdFunctionProcess(DefaultValue defaultValue, ConvertFrom<T> convertFrom, ProcessAction<T> processAction, int count)
            : base(0, count > 0 ? count-1 : 0, 0)
        {
            this.defaultValue = defaultValue;
            this.processAction = processAction;
            this.convertFrom = convertFrom;
        }

        protected override void Action(bool isInit, Signal origin)
        {
            if(Inputs.Length == 0)
                PublishToOutputs(defaultValue());
            else
            {
                T item = convertFrom(Inputs[0].Value);
                for(int i = 1; i < Inputs.Length; i++)
                    item = processAction(item,convertFrom(Inputs[i].Value));
                PublishToOutputs(item);
            }
        }
    }
}
