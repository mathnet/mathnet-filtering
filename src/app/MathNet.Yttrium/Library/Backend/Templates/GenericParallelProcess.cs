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
    public abstract class GenericParallelProcess : Process
    {
        private readonly int count, firstInput, firstOutput;
        private readonly bool inputIsInternal, outputIsInternal;
        private readonly Signal[] inputs, outputs;

        protected GenericParallelProcess(int firstInput, int firstOutput, int count, bool inputIsInternal, bool outputIsInternal)
        {
            this.firstInput = firstInput;
            this.firstOutput = firstOutput;
            this.count = count;
            this.inputIsInternal = inputIsInternal;
            this.outputIsInternal = outputIsInternal;

            inputs = new Signal[count];
            outputs = new Signal[count];
        }

        public override void Register(IList<Signal> inputSignals, IList<Signal> outputSignals, IList<Signal> internalSignals, IList<Bus> buses)
        {
            for(int i = 0; i < count; i++)
            {
                inputs[i] = inputIsInternal ? internalSignals[firstInput + i] : inputSignals[firstInput + i];
                outputs[i] = outputIsInternal ? internalSignals[firstOutput + i] : outputSignals[firstOutput + i];
                SenseSignal(inputs[i]);
            }

            ForceUpdate();
        }

        protected override void Action(bool isInit, Signal origin)
        {
            for(int i = 0; i < count; i++)
                if(inputs[i].HasEvent || isInit)
                    Process(inputs[i], outputs[i]);
        }

        protected abstract void Process(Signal input, Signal output);
    }
}
