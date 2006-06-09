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

namespace MathNet.Symbolics.Backend.Simulation
{
    /// <summary>Update the Signal Value (int the signal assignment phase).</summary>
    /// <returns>True if the new value is different to the present value.</returns>
    public delegate bool SetValue(ValueStructure newValue);
    /// <summary>Set the HasEvent Flag (constant over the process phase).</summary>
    public delegate void SetEventFlag(bool flagEvent);
    /// <summary>Notify driven processes the new value.</summary>
    public delegate void NotifyNewValue();

    public struct SignalEventItem
    {
        private readonly Signal _signal;
        private readonly ValueStructure _newValue;
        private readonly SetValue _setValue;
        private readonly SetEventFlag _setEventFlag;
        private readonly NotifyNewValue _notifyNewValue;

        public SignalEventItem(Signal signal, ValueStructure newValue, SetValue setValue, SetEventFlag setEventFlag, NotifyNewValue notifyNewValue)
        {
            _signal = signal;
            _newValue = newValue;
            _setEventFlag = setEventFlag;
            _setValue = setValue;
            _notifyNewValue = notifyNewValue;
        }

        public Signal Signal
        {
            get { return _signal; }
        }

        public ValueStructure NewValue
        {
            get { return _newValue; }
        }

        public SetValue SetValue
        {
            get { return _setValue; }
        }
        
        public SetEventFlag SetEventFlag
        {
            get { return _setEventFlag; }
        }

        public NotifyNewValue NotifyNewValue
        {
            get { return _notifyNewValue; }
        }
    }

    public struct TimedSignalEventItem
    {
        private readonly SignalEventItem _item;
        private TimeSpan _timeSpan;

        public TimedSignalEventItem(SignalEventItem item, TimeSpan timeSpan)
        {
            _item = item;
            _timeSpan = timeSpan;
        }

        public SignalEventItem Item
        {
            get { return _item; }
        }

        public TimeSpan TimeSpan
        {
            get { return _timeSpan; }
            set { _timeSpan = value; }
        }
    }
}
