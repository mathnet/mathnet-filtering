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
    /// <summary>
    /// A process for functions mapping a set of inputs to one output, which is mapped to several signals.
    /// </summary>
    public abstract class GenericFunctionProcess : Process
    {
        private readonly bool[] inInput, inInternal, outOutput, outInternal;
        private readonly int inCount, outCount; // = 0;
        private readonly Signal[] inputs, outputs;

        protected GenericFunctionProcess(bool[] inInput, bool[] inInternal, bool[] outOutput, bool[] outInternal)
        {
            this.inInput = inInput;
            this.inInternal = inInternal;
            this.outOutput = outOutput;
            this.outInternal = outInternal;

            //count mapped signals
            for(int i = 0; i < inInput.Length; i++)
                if(inInput[i]) inCount++;
            for(int i = 0; i < inInternal.Length; i++)
                if(inInternal[i]) inCount++;
            for(int i = 0; i < outOutput.Length; i++)
                if(outOutput[i]) outCount++;
            for(int i = 0; i < outInternal.Length; i++)
                if(outInternal[i]) outCount++;

            inputs = new Signal[inCount];
            outputs = new Signal[outCount];
        }
        protected GenericFunctionProcess(int firstInput, int lastInput, int output)
        {
            inInput = new bool[lastInput + 1];
            for(int i = firstInput; i <= lastInput; i++)
                inInput[i] = true;

            outOutput = new bool[output + 1];
            outOutput[output] = true;

            inInternal = new bool[] { };
            outInternal = new bool[] { };

            inCount = lastInput-firstInput+1;
            outCount = 1;

            inputs = new Signal[inCount];
            outputs = new Signal[outCount];
        }
        protected GenericFunctionProcess(int firstInput, int lastInput, int firstOutput, int lastOutput)
        {
            inInput = new bool[lastInput + 1];
            for(int i = firstInput; i <= lastInput; i++)
                inInput[i] = true;

            outOutput = new bool[lastOutput + 1];
            for(int i = firstOutput; i <= lastOutput; i++)
                outOutput[i] = true;

            inInternal = new bool[] { };
            outInternal = new bool[] { };

            inCount = lastInput - firstInput + 1;
            outCount = lastOutput - firstOutput + 1;

            inputs = new Signal[inCount];
            outputs = new Signal[outCount];
        }

        protected int InCount
        {
            get { return inCount; }
        }
        protected int OutCount
        {
            get { return outCount; }
        }
        protected Signal[] Inputs
        {
            get { return inputs; }
        }
        protected Signal[] Outputs
        {
            get { return outputs; }
        }

        public override void Register(IList<Signal> inputSignals, IList<Signal> outputSignals, IList<Signal> internalSignals, IList<Bus> buses)
        {
            int inputIndex = 0, outputIndex = 0;
            for(int i = 0; i < inputSignals.Count && i<inInput.Length; i++)
                if(inInput[i])
                {
                    inputs[inputIndex++] = inputSignals[i];
                    SenseSignal(inputSignals[i]);
                }
            for(int i = 0; i < outputSignals.Count && i < outOutput.Length; i++)
                if(outOutput[i])
                    outputs[outputIndex++] = outputSignals[i];
            for(int i = 0; i < internalSignals.Count; i++)
            {
                if(i<inInternal.Length && inInternal[i])
                {
                    inputs[inputIndex++] = internalSignals[i];
                    SenseSignal(internalSignals[i]);
                }
                if(i < outInternal.Length && outInternal[i])
                    outputs[outputIndex++] = internalSignals[i];
            }

            ForceUpdate();
        }

        protected void PublishToOutputs(ValueStructure value)
        {
            for(int i = 0; i < outputs.Length; i++)
                outputs[i].PostNewValue(value);
        }
        protected void PublishToOutputs(ValueStructure value, TimeSpan delay)
        {
            for(int i = 0; i < outputs.Length; i++)
                outputs[i].PostNewValue(value,delay);
        }
    }
}
