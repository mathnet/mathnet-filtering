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

using MathNet.Symbolics.Events;

namespace MathNet.Symbolics.Core
{
    public abstract class Process
    {
        private List<MathNet.Symbolics.Signal> sensitiveSignals;

        protected Process()
        {
            this.sensitiveSignals = new List<MathNet.Symbolics.Signal>(8);
        }

        private bool paused; // = false;
        public bool Paused
        {
            get { return paused; }
            set
            {
                if(value)
                    Pause();
                else
                    Continue();
            }
        }

        public abstract void Register(IList<MathNet.Symbolics.Signal> inputSignals, IList<MathNet.Symbolics.Signal> outputSignals, IList<MathNet.Symbolics.Signal> internalSignals, IList<MathNet.Symbolics.Bus> buses);

        public virtual void Unregister()
        {
            foreach(MathNet.Symbolics.Signal signal in sensitiveSignals)
                signal.ValueChanged -= signal_SignalValueChanged;
            sensitiveSignals.Clear();
        }

        public void Pause()
        {
            if(paused)
                return;
            foreach(MathNet.Symbolics.Signal signal in sensitiveSignals)
                signal.ValueChanged -= signal_SignalValueChanged;
            paused = true;
        }

        public void Continue()
        {
            if(!paused)
                return;
            foreach(MathNet.Symbolics.Signal signal in sensitiveSignals)
                signal.ValueChanged += signal_SignalValueChanged;
            paused = false;
        }

        public void ForceUpdate()
        {
            Action(true, null);
        }

        protected void SenseSignal(MathNet.Symbolics.Signal signal)
        {
            signal.ValueChanged += signal_SignalValueChanged;
            sensitiveSignals.Add(signal);
        }

        protected void StopSenseSignal(MathNet.Symbolics.Signal signal)
        {
            signal.ValueChanged -= signal_SignalValueChanged;
            sensitiveSignals.Remove(signal);

        }

        /// <param name="origin">note: may be null if not applicable</param>
        protected abstract void Action(bool isInit, MathNet.Symbolics.Signal origin);

        private void signal_SignalValueChanged(object sender, ValueNodeEventArgs e)
        {
            Action(false, (MathNet.Symbolics.Signal)e.ValueNode);
        }
    }
}
