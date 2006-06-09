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

namespace MathNet.Symbolics.Backend.SystemBuilder
{
    public enum BuilderState : int
    {
        Idle = 0,
        System = 1,
        Signals = 2,
        Buses = 3,
        Ports = 4,
        SignalDetails = 5,
        InputSignals = 6,
        OutputSignals = 7,
        NamedSignals = 8,
        NamedBuses = 9
    }

    public class BuilderStateMachine
    {
        private BuilderState _state;

        public BuilderStateMachine()
        {
            _state = BuilderState.Idle;
        }

        public BuilderState CurrentState
        {
            get { return _state; }
            set { _state = value; }
        }

        public virtual void Reset()
        {
            _state = BuilderState.Idle;
        }

        public bool IsInsideSystem
        {
            get { return _state != BuilderState.Idle; }
        }

        public bool IsInsideGroup
        {
            get { return _state != BuilderState.Idle && _state != BuilderState.System; }
        }

        public bool CanAdvanceTo(BuilderState next)
        {
            if(next == BuilderState.Idle)
                return true;

            int n = (int)next;
            int s = (int)_state;

            return n >= s;
        }

        public void AdvanceTo(BuilderState next)
        {
            if(!CanAdvanceTo(next))
                throw new InvalidOperationException("Can't move from state " + _state.ToString() + " to state " + next.ToString() + ".");
            BuilderState before = _state;
            _state = next;
            OnAfterAdvance(before, next, before != BuilderState.Idle, before != BuilderState.Idle && before != BuilderState.System);
        }

        protected virtual void OnAfterAdvance(BuilderState before, BuilderState after, bool wasInsideSystem, bool wasInsideGroup)
        {
        }
    }
}
