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
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.Discovery;
using MathNet.Symbolics.StdPackage.Structures;

namespace MathNet.Symbolics.StdPackage.Flow
{
    [EntityImplementation("Clock", "Std")]
    public class ClockArchitecture : GenericSimpleArchitecture
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("Clock", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        private TimeSpan[] periods;

        public ClockArchitecture() : base(_entityId, false) { }
        public ClockArchitecture(Port port) : base(_entityId, false, port, 0) { }

        protected override void SenseSignals(IList<Signal> inputSignals, IList<Signal> internalSignals, IList<Bus> buses, IList<Signal> outputSignals)
        {
            periods = new TimeSpan[inputSignals.Count];
            for(int i = 0; i < periods.Length; i++)
            {
                SenseSignal(inputSignals[i]);
                SenseSignal(outputSignals[i]);
                if(UpdatePeriods(i,inputSignals[i]))
                    PostDelayedClock(i,outputSignals[i]);
            }  
        }

        /// <returns>true if an event shall be posted.</returns>
        private bool UpdatePeriods(int index, Signal inputSignal)
        {
            if(RealValue.Converter.CanConvertLosslessFrom(inputSignal.Value))
            {
                periods[index] = TimeSpan.FromSeconds(1.0 / RealValue.ConvertFrom(inputSignal.Value).Value);
                return true;
            }
            else
            {
                periods[index] = TimeSpan.Zero;
                return false;
            }
        }

        private void PostDelayedClock(int index, Signal outputSignal)
        {
            TimeSpan p = periods[index];
            if(p.Equals(TimeSpan.Zero))
                return;
            ToggleValue tv = outputSignal.Value as ToggleValue;
            if(tv == null)
                tv = ToggleValue.InitialToggle;
            outputSignal.PostNewValue(tv.Toggle(), p);
        }

        protected override void Action(IList<Signal> inputSignals, IList<Signal> outputSignals, IList<Signal> internalSignals, IList<Bus> buses)
        {
            int cnt = inputSignals.Count;
            for(int i = 0; i < cnt; i++)
                if(inputSignals[i].HasEvent && UpdatePeriods(i, inputSignals[i]) || outputSignals[i].HasEvent)
                    PostDelayedClock(i, outputSignals[i]);
        }

        public override bool SupportsPort(Port port)
        {
            return port.BusCount == 0 && port.InputSignalCount == port.OutputSignalCount
                && port.InputSignals.TrueForAll(Std.IsAlwaysReal);
        }

        public override Architecture InstantiateToPort(Port port)
        {
            return new ClockArchitecture(port);
        }
    }
}
