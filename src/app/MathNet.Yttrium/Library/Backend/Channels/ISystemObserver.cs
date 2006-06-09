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
using MathNet.Symbolics.Workplace;

namespace MathNet.Symbolics.Backend.Channels
{
    public interface ISystemObserver
    {
        bool AutoDetachOnSystemChanged { get;}
        bool AutoInitialize { get;}

        void AttachedToSystem(MathSystem system);
        void DetachedFromSystem(MathSystem system);
        void BeginInitialize();
        void EndInitialize();

        void OnSignalAdded(Signal signal, int index);
        void OnSignalRemoved(Signal signal, int index);
        void OnSignalMoved(Signal signal, int indexBefore, int indexAfter);
        
        void OnBusAdded(Bus bus, int index);
        void OnBusRemoved(Bus bus, int index);
        void OnBusMoved(Bus bus, int indexBefore, int indexAfter);
        
        void OnPortAdded(Port port, int index);
        void OnPortRemoved(Port port, int index);
        void OnPortMoved(Port port, int indexBefore, int indexAfter);

        void OnInputAdded(Signal signal, int index);
        void OnInputRemoved(Signal signal, int index);
        void OnInputMoved(Signal signal, int indexBefore, int indexAfter);
        
        void OnOutputAdded(Signal signal, int index);
        void OnOutputRemoved(Signal signal, int index);
        void OnOutputMoved(Signal signal, int indexBefore, int indexAfter);

        void OnPortDrivesSignal(Signal signal, Port port, int outputIndex);
        void OnPortDrivesSignalNoLonger(Signal signal, Port port, int outputIndex);

        void OnSignalDrivesPort(Signal signal, Port port, int inputIndex);
        void OnSignalDrivesPortNoLonger(Signal signal, Port port, int inputIndex);
        
        void OnBusAttachedToPort(Bus bus, Port port, int busIndex);
        void OnBusAttachedToPortNoLonger(Bus bus, Port port, int busIndex);
    }
}
