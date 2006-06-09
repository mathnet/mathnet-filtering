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

namespace MathNet.Symbolics.Backend.Traversing
{
    public delegate bool ProcessSignal(Signal signal, Port parent, bool again, bool root);
    public delegate bool ProcessPort(Port port, Signal parent, bool again, bool root);
    public delegate bool ProcessLeafSignal(Signal signal, bool again);
    public delegate bool ProcessLeafPort(Port port, bool again);
    public delegate bool ProcessLeafBus(Bus bus, bool again);
    public delegate bool ProcessCycle(Port port, Signal target, Signal source);

    public interface IScanVisitor
    {
        /// <summary>
        /// Visit a signal. The signal is an input signal to the given parent port.
        /// </summary>
        /// <returns>false if no childs shall be processed.</returns>
        bool EnterSignal(Signal signal, Port parent, bool again, bool root);
        /// <summary>
        /// End visiting a signal, after all childs have been processed.
        /// </summary>
        /// <returns>false if finished.</returns>
        bool LeaveSignal(Signal signal, Port parent, bool again, bool root);

        /// <summary>
        /// Visit a port. The parent signal is an output signal of this port.
        /// </summary>
        /// <returns>false if no childs shall be processed.</returns>
        bool EnterPort(Port port, Signal parent, bool again, bool root);
        /// <summary>
        /// End visiting a port, after all childs have been processed.
        /// </summary>
        /// <returns>false if finished.</returns>
        bool LeavePort(Port port, Signal parent, bool again, bool root);

        /// <returns>false if finished.</returns>
        bool VisitLeaf(Signal signal, bool again);
        /// <returns>false if finished.</returns>
        bool VisitLeaf(Port port, bool again);
        /// <returns>false if finished.</returns>
        bool VisitLeaf(Bus bus, bool again);

        bool VisitCycle(Port port, Signal target, Signal source);
    }
}
