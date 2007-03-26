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
using System.Text;

using MathNet.Symbolics.Traversing;
using MathNet.Symbolics.Traversing.Visitors;

namespace MathNet.Symbolics
{
    internal class VisitorFactory :
        IFactory<IScanner>,
        IFactory<IExistsSignalVisitor, Predicate<Signal>>,
        IFactory<IExistsPortVisitor, Predicate<Port>>,
        IFactory<ITrueForAllSignalsVisitor, Predicate<Signal>>,
        IFactory<ITrueForAllPortsVisitor, Predicate<Port>>,
        IFactory<ISignalPathCollectVisitor, Signal>,
        IFactory<IPortPathCollectVisitor, Port>,
        IFactory<ICollectVisitor>,
        IFactory<ICollectVisitor, Predicate<Signal>>,
        IFactory<ICollectVisitor, Predicate<Port>>,
        IFactory<ICollectVisitor, Predicate<Bus>>,
        IFactory<ICollectVisitor, Predicate<Signal>, Predicate<Port>, Predicate<Bus>>,
        IFactory<IScanVisitor, ActionContinue<Signal>>,
        IFactory<IScanVisitor, ActionContinue<Port>>,
        IFactory<IScanVisitor, ActionContinue<Signal>, Predicate<Signal>>,
        IFactory<IScanVisitor, ActionContinue<Port>, Predicate<Port>>,
        IFactory<IScanVisitor, ProcessSignal, ProcessSignal, ProcessPort, ProcessPort, ProcessLeafSignal, ProcessLeafPort, ProcessLeafBus, ProcessCycle>
    {
        private VisitorFactory() { }

        // SCANNER
        IScanner IFactory<IScanner>.GetInstance()
        {
            return new Scanner();
        }

        // ExistsSignalVisitor
        IExistsSignalVisitor IFactory<IExistsSignalVisitor>.GetInstance()
        {
            throw new NotSupportedException("expected parameters: Predicate<Signal>");
        }
        IExistsSignalVisitor IFactory<IExistsSignalVisitor, Predicate<Signal>>.GetInstance(Predicate<Signal> match)
        {
            return new ExistsSignalVisitor(match);
        }

        // ExistsPortVisitor
        IExistsPortVisitor IFactory<IExistsPortVisitor>.GetInstance()
        {
            throw new NotSupportedException("expected parameters: Predicate<Port>");
        }
        IExistsPortVisitor IFactory<IExistsPortVisitor, Predicate<Port>>.GetInstance(Predicate<Port> match)
        {
            return new ExistsPortVisitor(match);
        }

        // TrueForAllSignalsVisitor
        ITrueForAllSignalsVisitor IFactory<ITrueForAllSignalsVisitor>.GetInstance()
        {
            throw new NotSupportedException("expected parameters: Predicate<Signal>");
        }
        ITrueForAllSignalsVisitor IFactory<ITrueForAllSignalsVisitor, Predicate<Signal>>.GetInstance(Predicate<Signal> match)
        {
            return new TrueForAllSignalsVisitor(match);
        }

        // TrueForAllPortsVisitor
        ITrueForAllPortsVisitor IFactory<ITrueForAllPortsVisitor>.GetInstance()
        {
            throw new NotSupportedException("expected parameters: Predicate<Port>");
        }
        ITrueForAllPortsVisitor IFactory<ITrueForAllPortsVisitor, Predicate<Port>>.GetInstance(Predicate<Port> match)
        {
            return new TrueForAllPortsVisitor(match);
        }

        // SignalPathCollectVisitor
        ISignalPathCollectVisitor IFactory<ISignalPathCollectVisitor>.GetInstance()
        {
            throw new NotSupportedException("expected parameters: ISignal");
        }
        ISignalPathCollectVisitor IFactory<ISignalPathCollectVisitor, Signal>.GetInstance(Signal source)
        {
            return new SignalPathCollectVisitor(source);
        }

        // SignalPathCollectVisitor
        IPortPathCollectVisitor IFactory<IPortPathCollectVisitor>.GetInstance()
        {
            throw new NotSupportedException("expected parameters: Port");
        }
        IPortPathCollectVisitor IFactory<IPortPathCollectVisitor, Port>.GetInstance(Port source)
        {
            return new PortPathCollectVisitor(source);
        }

        // CollectVisitor
        ICollectVisitor IFactory<ICollectVisitor>.GetInstance()
        {
            return new CollectVisitor();
        }

        // ConditionalCollectVisitor
        ICollectVisitor IFactory<ICollectVisitor, Predicate<Signal>>.GetInstance(Predicate<Signal> match)
        {
            return new ConditionalCollectVisitor(match);
        }
        ICollectVisitor IFactory<ICollectVisitor, Predicate<Port>>.GetInstance(Predicate<Port> match)
        {
            return new ConditionalCollectVisitor(match);
        }
        ICollectVisitor IFactory<ICollectVisitor, Predicate<Bus>>.GetInstance(Predicate<Bus> match)
        {
            return new ConditionalCollectVisitor(match);
        }
        ICollectVisitor IFactory<ICollectVisitor, Predicate<Signal>, Predicate<Port>, Predicate<Bus>>.GetInstance(Predicate<Signal> p1, Predicate<Port> p2, Predicate<Bus> p3)
        {
            return new ConditionalCollectVisitor(p1, p2, p3);
        }


        IScanVisitor IFactory<IScanVisitor>.GetInstance()
        {
            throw new NotSupportedException("expected parameters");
        }

        // SignalActionVisitor
        IScanVisitor IFactory<IScanVisitor, ActionContinue<Signal>>.GetInstance(ActionContinue<Signal> action)
        {
            return new SignalActionVisitor(action);
        }

        // PortActionVisitor
        IScanVisitor IFactory<IScanVisitor, ActionContinue<Port>>.GetInstance(ActionContinue<Port> action)
        {
            return new PortActionVisitor(action);
        }

        // SignalConditionalActionVisitor
        IScanVisitor IFactory<IScanVisitor, ActionContinue<Signal>, Predicate<Signal>>.GetInstance(ActionContinue<Signal> action, Predicate<Signal> match)
        {
            return new SignalConditionalActionVisitor(action, match);
        }

        // PortConditionalActionVisitor
        IScanVisitor IFactory<IScanVisitor, ActionContinue<Port>, Predicate<Port>>.GetInstance(ActionContinue<Port> action, Predicate<Port> match)
        {
            return new PortConditionalActionVisitor(action, match);
        }

        // BasicVisitor
        IScanVisitor IFactory<IScanVisitor, ProcessSignal, ProcessSignal, ProcessPort, ProcessPort, ProcessLeafSignal, ProcessLeafPort, ProcessLeafBus, ProcessCycle>.GetInstance(ProcessSignal enterSignal, ProcessSignal leaveSignal, ProcessPort enterPort, ProcessPort leavePort, ProcessLeafSignal leafSignal, ProcessLeafPort leafPort, ProcessLeafBus leafBus, ProcessCycle cycle)
        {
            return new BasicScanVisitor(enterSignal,leaveSignal,enterPort,leavePort,leafSignal,leafPort,leafBus,cycle);
        }
    }
}