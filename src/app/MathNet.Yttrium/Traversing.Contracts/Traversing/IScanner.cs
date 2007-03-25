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

namespace MathNet.Symbolics.Traversing
{
    public interface IScanner
    {
        void Traverse(Signal rootSignal, IScanVisitor visitor, bool ignoreHold);
        void Traverse(Port rootPort, IScanVisitor visitor, bool ignoreHold);
        void Traverse(IEnumerable<Signal> rootSignals, IScanVisitor visitor, bool ignoreHold);
        void Traverse(Signal rootSignal, IScanStrategy strategy, IScanVisitor visitor, bool ignoreHold);
        void Traverse(Port rootPort, IScanStrategy strategy, IScanVisitor visitor, bool ignoreHold);
        void Traverse(IEnumerable<Signal> rootSignals, IScanStrategy strategy, IScanVisitor visitor, bool ignoreHold);


        bool ExistsSignal(Port rootPort, Predicate<Signal> match, bool ignoreHold);
        bool ExistsSignal(Signal rootSignal, Predicate<Signal> match, bool ignoreHold, out Signal foundSignal, out Port foundSignalTarget);
        bool ExistsSignal(Signal rootSignal, Predicate<Signal> match, bool ignoreHold);
        bool ExistsSignal(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold, out Signal foundSignal, out Port foundSignalTarget);
        bool ExistsSignal(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold);
        bool ExistsSignal(Port rootPort, Predicate<Signal> match, bool ignoreHold, out Signal foundSignal, out Port foundSignalTarget);

        bool ExistsPort(Port rootPort, Predicate<Port> match, bool ignoreHold, out Port foundPort, out Signal foundPortTarget);
        bool ExistsPort(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold);
        bool ExistsPort(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold, out Port foundPort, out Signal foundPortTarget);
        bool ExistsPort(Signal rootSignal, Predicate<Port> match, bool ignoreHold);
        bool ExistsPort(Signal rootSignal, Predicate<Port> match, bool ignoreHold, out Port foundPort, out Signal foundPortTarget);
        bool ExistsPort(Port rootPort, Predicate<Port> match, bool ignoreHold);


        bool TrueForAllSignals(Port rootPort, Predicate<Signal> match, bool ignoreHold);
        bool TrueForAllSignals(Signal rootSignal, Predicate<Signal> match, bool ignoreHold, out Signal failedSignal, out Port failedSignalTarget);
        bool TrueForAllSignals(Signal rootSignal, Predicate<Signal> match, bool ignoreHold);
        bool TrueForAllSignals(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold, out Signal failedSignal, out Port failedSignalTarget);
        bool TrueForAllSignals(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold);
        bool TrueForAllSignals(Port rootPort, Predicate<Signal> match, bool ignoreHold, out Signal failedSignal, out Port failedSignalTarget);

        bool TrueForAllPorts(Signal rootSignal, Predicate<Port> match, bool ignoreHold);
        bool TrueForAllPorts(Signal rootSignal, Predicate<Port> match, bool ignoreHold, out Port failedPort, out Signal failedPortTarget);
        bool TrueForAllPorts(Port rootPort, Predicate<Port> match, bool ignoreHold);
        bool TrueForAllPorts(Port rootPort, Predicate<Port> match, bool ignoreHold, out Port failedPort, out Signal failedPortTarget);
        bool TrueForAllPorts(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold);
        bool TrueForAllPorts(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold, out Port failedPort, out Signal failedPortTarget);


        void ForEachSignal(Signal rootSignal, ActionContinue<Signal> action, bool ignoreHold);
        void ForEachSignal(Port rootPort, ActionContinue<Signal> action, bool ignoreHold);
        void ForEachSignal(IEnumerable<Signal> rootSignals, ActionContinue<Signal> action, bool ignoreHold);
        void ForEachSignal(IEnumerable<Signal> rootSignals, Predicate<Signal> match, ActionContinue<Signal> action, bool ignoreHold);
        void ForEachSignal(Port rootPort, Predicate<Signal> match, ActionContinue<Signal> action, bool ignoreHold);
        void ForEachSignal(Signal rootSignal, Predicate<Signal> match, ActionContinue<Signal> action, bool ignoreHold);

        void ForEachPort(Signal rootSignal, ActionContinue<Port> action, bool ignoreHold);
        void ForEachPort(Signal rootSignal, Predicate<Port> match, ActionContinue<Port> action, bool ignoreHold);
        void ForEachPort(IEnumerable<Signal> rootSignals, ActionContinue<Port> action, bool ignoreHold);
        void ForEachPort(Port rootPort, ActionContinue<Port> action, bool ignoreHold);
        void ForEachPort(Port rootPort, Predicate<Port> match, ActionContinue<Port> action, bool ignoreHold);
        void ForEachPort(IEnumerable<Signal> rootSignals, Predicate<Port> match, ActionContinue<Port> action, bool ignoreHold);


        IList<Signal> FindAllSignals(Signal rootSignal, Predicate<Signal> match, bool ignoreHold);
        IList<Signal> FindAllSignals(Signal rootSignal, bool ignoreHold);
        IList<Signal> FindAllSignals(Port rootPort, bool ignoreHold);
        IList<Signal> FindAllSignals(IEnumerable<Signal> rootSignals, bool ignoreHold);
        IList<Signal> FindAllSignals(Port rootPort, Predicate<Signal> match, bool ignoreHold);
        IList<Signal> FindAllSignals(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold);

        IList<Port> FindAllPorts(Signal rootSignal, Predicate<Port> match, bool ignoreHold);
        IList<Port> FindAllPorts(Port rootPort, Predicate<Port> match, bool ignoreHold);
        IList<Port> FindAllPorts(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold);
        IList<Port> FindAllPorts(Signal rootSignal, bool ignoreHold);
        IList<Port> FindAllPorts(Port rootPort, bool ignoreHold);
        IList<Port> FindAllPorts(IEnumerable<Signal> rootSignals, bool ignoreHold);

        void FindAll(IEnumerable<Signal> rootSignals, bool ignoreHold, out IList<Signal> signals, out IList<Port> ports);
        void FindAll(Port rootPort, bool ignoreHold, out IList<Signal> signals, out IList<Port> ports);
        void FindAll(Signal rootSignal, bool ignoreHold, out IList<Signal> signals, out IList<Port> ports);

        Signal FindSignal(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold);
        Signal FindSignal(Port rootPort, Predicate<Signal> match, bool ignoreHold);
        Signal FindSignal(Signal rootSignal, Predicate<Signal> match, bool ignoreHold);

        Port FindPort(Signal rootSignal, Predicate<Port> match, bool ignoreHold);
        Port FindPort(Port rootPort, Predicate<Port> match, bool ignoreHold);
        Port FindPort(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold);


        List<List<Signal>> FindAllSignalPathsFrom(Port rootPort, Signal from, bool ignoreHold);
        List<List<Signal>> FindAllSignalPathsFrom(IEnumerable<Signal> rootSignals, Signal from, bool ignoreHold);
        List<List<Signal>> FindAllSignalPathsFrom(Signal rootSignal, Signal from, bool ignoreHold);

        List<List<Port>> FindAllPortPathsFrom(IEnumerable<Signal> rootSignals, Port from, bool ignoreHold);
        List<List<Port>> FindAllPortPathsFrom(Port rootPort, Port from, bool ignoreHold);
        List<List<Port>> FindAllPortPathsFrom(Signal rootSignal, Port from, bool ignoreHold);
    }
}
