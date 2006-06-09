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
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Backend.Templates
{
    public abstract class GenericSimpleArchitecture : Architecture, IArchitectureFactory
    {
        private Signal[] _internalSignals;
        private SignalSet _sensedSignals;

        /// <summary>Architecture Builder Constructor</summary>
        protected GenericSimpleArchitecture(MathIdentifier entityId, bool isMathematicalOperator) : base(entityId, entityId, isMathematicalOperator) { }

        /// <summary>Port Instance Constructor</summary>
        protected GenericSimpleArchitecture(MathIdentifier entityId, bool isMathematicalOperator, Port port, int internalSignalCount)
            : base(entityId, entityId, isMathematicalOperator)
        {
            _internalSignals = new Signal[internalSignalCount];
            for(int i = 0; i < _internalSignals.Length; i++)
                _internalSignals[i] = new Signal(port.Context);

            //System.Diagnostics.Debug.Assert(SupportsPort(port));
            SetPort(port);
            _sensedSignals = new SignalSet();
            SenseSignals(port.InputSignals, _internalSignals, port.Buses, port.OutputSignals);
            Action(port.InputSignals, port.OutputSignals, _internalSignals, port.Buses);
        }

        #region Signal Sensing
        protected abstract void SenseSignals(IList<Signal> inputSignals, IList<Signal> internalSignals, IList<Bus> buses, IList<Signal> outputSignals);

        protected void SenseSignal(Signal signal)
        {
            _sensedSignals.Add(signal);
            signal.SignalValueChanged += signal_SignalValueChanged;
        }

        protected void StopSenseSignal(Signal signal)
        {
            signal.SignalValueChanged -= signal_SignalValueChanged;
            _sensedSignals.Remove(signal);
        }

        private void signal_SignalValueChanged(object sender, EventArgs e)
        {
            Action(Port.InputSignals, Port.OutputSignals, _internalSignals, Port.Buses);
        }
        #endregion

        protected abstract void Action(IList<Signal> inputSignals, IList<Signal> outputSignals, IList<Signal> internalSignals, IList<Bus> buses);

        public abstract Architecture InstantiateToPort(Port port);

        public override void UnregisterArchitecture()
        {
            foreach(Signal signal in _sensedSignals)
                signal.SignalValueChanged -= signal_SignalValueChanged;
            _sensedSignals.Clear();
        }

        protected override void ReregisterArchitecture(Port oldPort, Port newPort)
        {
            SenseSignals(newPort.InputSignals, _internalSignals, newPort.Buses, newPort.OutputSignals);
            Action(newPort.InputSignals, newPort.OutputSignals, _internalSignals, newPort.Buses);
        }
    }
}
