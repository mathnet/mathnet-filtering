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

using MathNet.Symbolics.Exceptions;

namespace MathNet.Symbolics.Traversing
{
    public class Scanner : IScanner
    {
        public void Traverse(Signal rootSignal, IScanVisitor visitor, bool ignoreHold)
        {
            if(visitor == null) throw new ArgumentNullException("visitor");
            visitor.DefaultStrategy.Traverse(rootSignal, visitor, ignoreHold);
        }
        public void Traverse(Port rootPort, IScanVisitor visitor, bool ignoreHold)
        {
            if(visitor == null) throw new ArgumentNullException("visitor");
            visitor.DefaultStrategy.Traverse(rootPort, visitor, ignoreHold);
        }
        public void Traverse(IEnumerable<Signal> rootSignals, IScanVisitor visitor, bool ignoreHold)
        {
            if(visitor == null) throw new ArgumentNullException("visitor");
            visitor.DefaultStrategy.Traverse(rootSignals, visitor, ignoreHold);
        }

        public void Traverse(Signal rootSignal, IScanStrategy strategy, IScanVisitor visitor, bool ignoreHold)
        {
            if(visitor == null) throw new ArgumentNullException("visitor");
            if(strategy == null) throw new ArgumentNullException("strategy");
            strategy.Traverse(rootSignal, visitor, ignoreHold);
        }
        public void Traverse(Port rootPort, IScanStrategy strategy, IScanVisitor visitor, bool ignoreHold)
        {
            if(visitor == null) throw new ArgumentNullException("visitor");
            if(strategy == null) throw new ArgumentNullException("strategy");
            strategy.Traverse(rootPort, visitor, ignoreHold);
        }
        public void Traverse(IEnumerable<Signal> rootSignals, IScanStrategy strategy, IScanVisitor visitor, bool ignoreHold)
        {
            if(visitor == null) throw new ArgumentNullException("visitor");
            if(strategy == null) throw new ArgumentNullException("strategy");
            strategy.Traverse(rootSignals, visitor, ignoreHold);
        }

        #region Exists Lambdas -> bool
        #region Signal
        public bool ExistsSignal(Signal rootSignal, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return ExistsSignal(rootSignal, match, ignoreHold, out signal, out port);
        }
        public bool ExistsSignal(Signal rootSignal, Predicate<Signal> match, bool ignoreHold, out Signal foundSignal, out Port foundSignalTarget)
        {
            IExistsSignalVisitor visitor = Binder.GetInstance<IExistsSignalVisitor, Predicate<Signal>>(match);
            visitor.DefaultStrategy.Traverse(rootSignal, visitor, ignoreHold);
            if(visitor.Exists)
            {
                foundSignal = visitor.FoundSignal;
                foundSignalTarget = visitor.FoundSignalTarget;
                return true;
            }
            else
            {
                foundSignal = null;
                foundSignalTarget = null;
                return false;
            }
        }

        public bool ExistsSignal(Port rootPort, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return ExistsSignal(rootPort, match, ignoreHold, out signal, out port);
        }
        public bool ExistsSignal(Port rootPort, Predicate<Signal> match, bool ignoreHold, out Signal foundSignal, out Port foundSignalTarget)
        {
            IExistsSignalVisitor visitor = Binder.GetInstance<IExistsSignalVisitor, Predicate<Signal>>(match);
            visitor.DefaultStrategy.Traverse(rootPort, visitor, ignoreHold);
            if(visitor.Exists)
            {
                foundSignal = visitor.FoundSignal;
                foundSignalTarget = visitor.FoundSignalTarget;
                return true;
            }
            else
            {
                foundSignal = null;
                foundSignalTarget = null;
                return false;
            }
        }

        public bool ExistsSignal(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return ExistsSignal(rootSignals, match, ignoreHold, out signal, out port);
        }
        public bool ExistsSignal(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold, out Signal foundSignal, out Port foundSignalTarget)
        {
            IExistsSignalVisitor visitor = Binder.GetInstance<IExistsSignalVisitor, Predicate<Signal>>(match);
            visitor.DefaultStrategy.Traverse(rootSignals, visitor, ignoreHold);
            if(visitor.Exists)
            {
                foundSignal = visitor.FoundSignal;
                foundSignalTarget = visitor.FoundSignalTarget;
                return true;
            }
            else
            {
                foundSignal = null;
                foundSignalTarget = null;
                return false;
            }
        }
        #endregion
        #region Port
        public bool ExistsPort(Signal rootSignal, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return ExistsPort(rootSignal, match, ignoreHold, out port, out signal);
        }
        public bool ExistsPort(Signal rootSignal, Predicate<Port> match, bool ignoreHold, out Port foundPort, out Signal foundPortTarget)
        {
            IExistsPortVisitor visitor = Binder.GetInstance<IExistsPortVisitor, Predicate<Port>>(match);
            visitor.DefaultStrategy.Traverse(rootSignal, visitor, ignoreHold);
            if(visitor.Exists)
            {
                foundPort = visitor.FoundPort;
                foundPortTarget = visitor.FoundPortTarget;
                return true;
            }
            else
            {
                foundPort = null;
                foundPortTarget = null;
                return false;
            }
        }

        public bool ExistsPort(Port rootPort, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return ExistsPort(rootPort, match, ignoreHold, out port, out signal);
        }
        public bool ExistsPort(Port rootPort, Predicate<Port> match, bool ignoreHold, out Port foundPort, out Signal foundPortTarget)
        {
            IExistsPortVisitor visitor = Binder.GetInstance<IExistsPortVisitor, Predicate<Port>>(match);
            visitor.DefaultStrategy.Traverse(rootPort, visitor, ignoreHold);
            if(visitor.Exists)
            {
                foundPort = visitor.FoundPort;
                foundPortTarget = visitor.FoundPortTarget;
                return true;
            }
            else
            {
                foundPort = null;
                foundPortTarget = null;
                return false;
            }
        }

        public bool ExistsPort(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return ExistsPort(rootSignals, match, ignoreHold, out port, out signal);
        }
        public bool ExistsPort(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold, out Port foundPort, out Signal foundPortTarget)
        {
            IExistsPortVisitor visitor = Binder.GetInstance<IExistsPortVisitor, Predicate<Port>>(match);
            visitor.DefaultStrategy.Traverse(rootSignals, visitor, ignoreHold);
            if(visitor.Exists)
            {
                foundPort = visitor.FoundPort;
                foundPortTarget = visitor.FoundPortTarget;
                return true;
            }
            else
            {
                foundPort = null;
                foundPortTarget = null;
                return false;
            }
        }
        #endregion
        #endregion

        #region True-For-All Lambdas -> bool
        #region Signal
        public bool TrueForAllSignals(Signal rootSignal, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return TrueForAllSignals(rootSignal, match, ignoreHold, out signal, out port);
        }
        public bool TrueForAllSignals(Signal rootSignal, Predicate<Signal> match, bool ignoreHold, out Signal failedSignal, out Port failedSignalTarget)
        {
            ITrueForAllSignalsVisitor visitor = Binder.GetInstance<ITrueForAllSignalsVisitor, Predicate<Signal>>(match);
            visitor.DefaultStrategy.Traverse(rootSignal, visitor, ignoreHold);
            if(visitor.TrueForAll)
            {
                failedSignal = null;
                failedSignalTarget = null;
                return true;
            }
            else
            {
                failedSignal = visitor.FailedSignal;
                failedSignalTarget = visitor.FailedSignalTarget;
                return false;
            }
        }

        public bool TrueForAllSignals(Port rootPort, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return TrueForAllSignals(rootPort, match, ignoreHold, out signal, out port);
        }
        public bool TrueForAllSignals(Port rootPort, Predicate<Signal> match, bool ignoreHold, out Signal failedSignal, out Port failedSignalTarget)
        {
            ITrueForAllSignalsVisitor visitor = Binder.GetInstance<ITrueForAllSignalsVisitor, Predicate<Signal>>(match);
            visitor.DefaultStrategy.Traverse(rootPort, visitor, ignoreHold);
            if(visitor.TrueForAll)
            {
                failedSignal = null;
                failedSignalTarget = null;
                return true;
            }
            else
            {
                failedSignal = visitor.FailedSignal;
                failedSignalTarget = visitor.FailedSignalTarget;
                return false;
            }
        }

        public bool TrueForAllSignals(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return TrueForAllSignals(rootSignals, match, ignoreHold, out signal, out port);
        }
        public bool TrueForAllSignals(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold, out Signal failedSignal, out Port failedSignalTarget)
        {
            ITrueForAllSignalsVisitor visitor = Binder.GetInstance<ITrueForAllSignalsVisitor, Predicate<Signal>>(match);
            visitor.DefaultStrategy.Traverse(rootSignals, visitor, ignoreHold);
            if(visitor.TrueForAll)
            {
                failedSignal = null;
                failedSignalTarget = null;
                return true;
            }
            else
            {
                failedSignal = visitor.FailedSignal;
                failedSignalTarget = visitor.FailedSignalTarget;
                return false;
            }
        }
        #endregion
        #region Port
        public bool TrueForAllPorts(Signal rootSignal, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return TrueForAllPorts(rootSignal, match, ignoreHold, out port, out signal);
        }
        public bool TrueForAllPorts(Signal rootSignal, Predicate<Port> match, bool ignoreHold, out Port failedPort, out Signal failedPortTarget)
        {
            ITrueForAllPortsVisitor visitor = Binder.GetInstance<ITrueForAllPortsVisitor, Predicate<Port>>(match);
            visitor.DefaultStrategy.Traverse(rootSignal, visitor, ignoreHold);
            if(visitor.TrueForAll)
            {
                failedPort = null;
                failedPortTarget = null;
                return true;
            }
            else
            {
                failedPort = visitor.FailedPort;
                failedPortTarget = visitor.FailedPortTarget;
                return false;
            }
        }

        public bool TrueForAllPorts(Port rootPort, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return TrueForAllPorts(rootPort, match, ignoreHold, out port, out signal);
        }
        public bool TrueForAllPorts(Port rootPort, Predicate<Port> match, bool ignoreHold, out Port failedPort, out Signal failedPortTarget)
        {
            ITrueForAllPortsVisitor visitor = Binder.GetInstance<ITrueForAllPortsVisitor, Predicate<Port>>(match);
            visitor.DefaultStrategy.Traverse(rootPort, visitor, ignoreHold);
            if(visitor.TrueForAll)
            {
                failedPort = null;
                failedPortTarget = null;
                return true;
            }
            else
            {
                failedPort = visitor.FailedPort;
                failedPortTarget = visitor.FailedPortTarget;
                return false;
            }
        }

        public bool TrueForAllPorts(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return TrueForAllPorts(rootSignals, match, ignoreHold, out port, out signal);
        }
        public bool TrueForAllPorts(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold, out Port failedPort, out Signal failedPortTarget)
        {
            ITrueForAllPortsVisitor visitor = Binder.GetInstance<ITrueForAllPortsVisitor, Predicate<Port>>(match);
            visitor.DefaultStrategy.Traverse(rootSignals, visitor, ignoreHold);
            if(visitor.TrueForAll)
            {
                failedPort = null;
                failedPortTarget = null;
                return true;
            }
            else
            {
                failedPort = visitor.FailedPort;
                failedPortTarget = visitor.FailedPortTarget;
                return false;
            }
        }
        #endregion
        #endregion

        #region For-Each Lambdas -> Action
        #region Unconditional
        public void ForEachSignal(Signal rootSignal, ActionContinue<Signal> action, bool ignoreHold)
        {
            IScanVisitor visitor = Binder.GetInstance<IScanVisitor, ActionContinue<Signal>>(action);
            visitor.DefaultStrategy.Traverse(rootSignal, visitor, ignoreHold);
        }
        public void ForEachSignal(Port rootPort, ActionContinue<Signal> action, bool ignoreHold)
        {
            IScanVisitor visitor = Binder.GetInstance<IScanVisitor, ActionContinue<Signal>>(action);
            visitor.DefaultStrategy.Traverse(rootPort, visitor, ignoreHold);
        }
        public void ForEachSignal(IEnumerable<Signal> rootSignals, ActionContinue<Signal> action, bool ignoreHold)
        {
            IScanVisitor visitor = Binder.GetInstance<IScanVisitor, ActionContinue<Signal>>(action);
            visitor.DefaultStrategy.Traverse(rootSignals, visitor, ignoreHold);
        }

        public void ForEachPort(Signal rootSignal, ActionContinue<Port> action, bool ignoreHold)
        {
            IScanVisitor visitor = Binder.GetInstance<IScanVisitor, ActionContinue<Port>>(action);
            visitor.DefaultStrategy.Traverse(rootSignal, visitor, ignoreHold);
        }
        public void ForEachPort(Port rootPort, ActionContinue<Port> action, bool ignoreHold)
        {
            IScanVisitor visitor = Binder.GetInstance<IScanVisitor, ActionContinue<Port>>(action);
            visitor.DefaultStrategy.Traverse(rootPort, visitor, ignoreHold);
        }
        public void ForEachPort(IEnumerable<Signal> rootSignals, ActionContinue<Port> action, bool ignoreHold)
        {
            IScanVisitor visitor = Binder.GetInstance<IScanVisitor, ActionContinue<Port>>(action);
            visitor.DefaultStrategy.Traverse(rootSignals, visitor, ignoreHold);
        }
        #endregion
        #region Conditional
        public void ForEachSignal(Signal rootSignal, Predicate<Signal> match, ActionContinue<Signal> action, bool ignoreHold)
        {
            IScanVisitor visitor = Binder.GetInstance<IScanVisitor, ActionContinue<Signal>, Predicate<Signal>>(action, match);
            visitor.DefaultStrategy.Traverse(rootSignal, visitor, ignoreHold);
        }
        public void ForEachSignal(Port rootPort, Predicate<Signal> match, ActionContinue<Signal> action, bool ignoreHold)
        {
            IScanVisitor visitor = Binder.GetInstance<IScanVisitor, ActionContinue<Signal>, Predicate<Signal>>(action, match);
            visitor.DefaultStrategy.Traverse(rootPort, visitor, ignoreHold);
        }
        public void ForEachSignal(IEnumerable<Signal> rootSignals, Predicate<Signal> match, ActionContinue<Signal> action, bool ignoreHold)
        {
            IScanVisitor visitor = Binder.GetInstance<IScanVisitor, ActionContinue<Signal>, Predicate<Signal>>(action, match);
            visitor.DefaultStrategy.Traverse(rootSignals, visitor, ignoreHold);
        }

        public void ForEachPort(Signal rootSignal, Predicate<Port> match, ActionContinue<Port> action, bool ignoreHold)
        {
            IScanVisitor visitor = Binder.GetInstance<IScanVisitor, ActionContinue<Port>, Predicate<Port>>(action, match);
            visitor.DefaultStrategy.Traverse(rootSignal, visitor, ignoreHold);
        }
        public void ForEachPort(Port rootPort, Predicate<Port> match, ActionContinue<Port> action, bool ignoreHold)
        {
            IScanVisitor visitor = Binder.GetInstance<IScanVisitor, ActionContinue<Port>, Predicate<Port>>(action, match);
            visitor.DefaultStrategy.Traverse(rootPort, visitor, ignoreHold);
        }
        public void ForEachPort(IEnumerable<Signal> rootSignals, Predicate<Port> match, ActionContinue<Port> action, bool ignoreHold)
        {
            IScanVisitor visitor = Binder.GetInstance<IScanVisitor, ActionContinue<Port>, Predicate<Port>>(action, match);
            visitor.DefaultStrategy.Traverse(rootSignals, visitor, ignoreHold);
        }
        #endregion
        #endregion

        #region Find-All Lambdas -> List
        #region Unconditional
        public IList<Signal> FindAllSignals(Signal rootSignal, bool ignoreHold)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor>();
            IScanStrategy strategy = Binder.GetSpecificInstance<IScanStrategy>(new MathIdentifier("AllSignalsStrategy", "Traversing"));
            strategy.Traverse(rootSignal, visitor, ignoreHold);
            return visitor.Signals;
        }
        public IList<Signal> FindAllSignals(Port rootPort, bool ignoreHold)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor>();
            IScanStrategy strategy = Binder.GetSpecificInstance<IScanStrategy>(new MathIdentifier("AllSignalsStrategy", "Traversing"));
            strategy.Traverse(rootPort, visitor, ignoreHold);
            return visitor.Signals;
        }
        public IList<Signal> FindAllSignals(IEnumerable<Signal> rootSignals, bool ignoreHold)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor>();
            IScanStrategy strategy = Binder.GetSpecificInstance<IScanStrategy>(new MathIdentifier("AllSignalsStrategy", "Traversing"));
            strategy.Traverse(rootSignals, visitor, ignoreHold);
            return visitor.Signals;
        }

        public IList<Port> FindAllPorts(Signal rootSignal, bool ignoreHold)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor>();
            IScanStrategy strategy = Binder.GetSpecificInstance<IScanStrategy>(new MathIdentifier("AllPortsStrategy", "Traversing"));
            strategy.Traverse(rootSignal, visitor, ignoreHold);
            return visitor.Ports;
        }
        public IList<Port> FindAllPorts(Port rootPort, bool ignoreHold)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor>();
            IScanStrategy strategy = Binder.GetSpecificInstance<IScanStrategy>(new MathIdentifier("AllPortsStrategy", "Traversing"));
            strategy.Traverse(rootPort, visitor, ignoreHold);
            return visitor.Ports;
        }
        public IList<Port> FindAllPorts(IEnumerable<Signal> rootSignals, bool ignoreHold)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor>();
            IScanStrategy strategy = Binder.GetSpecificInstance<IScanStrategy>(new MathIdentifier("AllPortsStrategy", "Traversing"));
            strategy.Traverse(rootSignals, visitor, ignoreHold);
            return visitor.Ports;
        }

        public void FindAll(Signal rootSignal, bool ignoreHold, out IList<Signal> signals, out IList<Port> ports)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor>();
            visitor.DefaultStrategy.Traverse(rootSignal, visitor, ignoreHold);
            signals = visitor.Signals;
            ports = visitor.Ports;
        }
        public void FindAll(Port rootPort, bool ignoreHold, out IList<Signal> signals, out IList<Port> ports)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor>();
            visitor.DefaultStrategy.Traverse(rootPort, visitor, ignoreHold);
            signals = visitor.Signals;
            ports = visitor.Ports;
        }
        public void FindAll(IEnumerable<Signal> rootSignals, bool ignoreHold, out IList<Signal> signals, out IList<Port> ports)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor>();
            visitor.DefaultStrategy.Traverse(rootSignals, visitor, ignoreHold);
            signals = visitor.Signals;
            ports = visitor.Ports;
        }
        #endregion
        #region Conditional
        public IList<Signal> FindAllSignals(Signal rootSignal, Predicate<Signal> match, bool ignoreHold)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor,Predicate<Signal>>(match);
            IScanStrategy strategy = Binder.GetSpecificInstance<IScanStrategy>(new MathIdentifier("AllSignalsStrategy", "Traversing"));
            strategy.Traverse(rootSignal, visitor, ignoreHold);
            return visitor.Signals;
        }
        public IList<Signal> FindAllSignals(Port rootPort, Predicate<Signal> match, bool ignoreHold)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor, Predicate<Signal>>(match);
            IScanStrategy strategy = Binder.GetSpecificInstance<IScanStrategy>(new MathIdentifier("AllSignalsStrategy", "Traversing"));
            strategy.Traverse(rootPort, visitor, ignoreHold);
            return visitor.Signals;
        }
        public IList<Signal> FindAllSignals(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor, Predicate<Signal>>(match);
            IScanStrategy strategy = Binder.GetSpecificInstance<IScanStrategy>(new MathIdentifier("AllSignalsStrategy", "Traversing"));
            strategy.Traverse(rootSignals, visitor, ignoreHold);
            return visitor.Signals;
        }

        public IList<Port> FindAllPorts(Signal rootSignal, Predicate<Port> match, bool ignoreHold)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor, Predicate<Port>>(match);
            IScanStrategy strategy = Binder.GetSpecificInstance<IScanStrategy>(new MathIdentifier("AllPortsStrategy", "Traversing"));
            strategy.Traverse(rootSignal, visitor, ignoreHold);
            return visitor.Ports;
        }
        public IList<Port> FindAllPorts(Port rootPort, Predicate<Port> match, bool ignoreHold)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor, Predicate<Port>>(match);
            IScanStrategy strategy = Binder.GetSpecificInstance<IScanStrategy>(new MathIdentifier("AllPortsStrategy", "Traversing"));
            strategy.Traverse(rootPort, visitor, ignoreHold);
            return visitor.Ports;
        }
        public IList<Port> FindAllPorts(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold)
        {
            ICollectVisitor visitor = Binder.GetInstance<ICollectVisitor, Predicate<Port>>(match);
            IScanStrategy strategy = Binder.GetSpecificInstance<IScanStrategy>(new MathIdentifier("AllPortsStrategy", "Traversing"));
            strategy.Traverse(rootSignals, visitor, ignoreHold);
            return visitor.Ports;
        }
        #endregion
        #endregion

        #region Find Lambdas -> Signal/Port/Bus
        public Signal FindSignal(Signal rootSignal, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            if(ExistsSignal(rootSignal, match, ignoreHold, out signal, out port))
                return signal;
            else
                throw new TraversingException("signal not found");
        }
        public Signal FindSignal(Port rootPort, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            if(ExistsSignal(rootPort, match, ignoreHold, out signal, out port))
                return signal;
            else
                throw new TraversingException("signal not found");
        }
        public Signal FindSignal(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            if(ExistsSignal(rootSignals, match, ignoreHold, out signal, out port))
                return signal;
            else
                throw new TraversingException("signal not found");
        }

        public Port FindPort(Signal rootSignal, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            if(ExistsPort(rootSignal, match, ignoreHold, out port, out signal))
                return port;
            else
                throw new TraversingException("port not found");
        }
        public Port FindPort(Port rootPort, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            if(ExistsPort(rootPort, match, ignoreHold, out port, out signal))
                return port;
            else
                throw new TraversingException("port not found");
        }
        public Port FindPort(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            if(ExistsPort(rootSignals, match, ignoreHold, out port, out signal))
                return port;
            else
                throw new TraversingException("port not found");
        }
        #endregion

        #region Collect Paths -> List of Lists of Signal/Port
        public List<List<Signal>> FindAllSignalPathsFrom(Signal rootSignal, Signal from, bool ignoreHold)
        {
            ISignalPathCollectVisitor visitor = Binder.GetInstance<ISignalPathCollectVisitor, Signal>(from);
            visitor.DefaultStrategy.Traverse(rootSignal, visitor, ignoreHold);
            return visitor.Paths;
        }
        public List<List<Signal>> FindAllSignalPathsFrom(Port rootPort, Signal from, bool ignoreHold)
        {
            ISignalPathCollectVisitor visitor = Binder.GetInstance<ISignalPathCollectVisitor, Signal>(from);
            visitor.DefaultStrategy.Traverse(rootPort, visitor, ignoreHold);
            return visitor.Paths;
        }
        public List<List<Signal>> FindAllSignalPathsFrom(IEnumerable<Signal> rootSignals, Signal from, bool ignoreHold)
        {
            ISignalPathCollectVisitor visitor = Binder.GetInstance<ISignalPathCollectVisitor, Signal>(from);
            visitor.DefaultStrategy.Traverse(rootSignals, visitor, ignoreHold);
            return visitor.Paths;
        }

        public List<List<Port>> FindAllPortPathsFrom(Signal rootSignal, Port from, bool ignoreHold)
        {
            IPortPathCollectVisitor visitor = Binder.GetInstance<IPortPathCollectVisitor, Port>(from);
            visitor.DefaultStrategy.Traverse(rootSignal, visitor, ignoreHold);
            return visitor.Paths;
        }
        public List<List<Port>> FindAllPortPathsFrom(Port rootPort, Port from, bool ignoreHold)
        {
            IPortPathCollectVisitor visitor = Binder.GetInstance<IPortPathCollectVisitor, Port>(from);
            visitor.DefaultStrategy.Traverse(rootPort, visitor, ignoreHold);
            return visitor.Paths;
        }
        public List<List<Port>> FindAllPortPathsFrom(IEnumerable<Signal> rootSignals, Port from, bool ignoreHold)
        {
            IPortPathCollectVisitor visitor = Binder.GetInstance<IPortPathCollectVisitor, Port>(from);
            visitor.DefaultStrategy.Traverse(rootSignals, visitor, ignoreHold);
            return visitor.Paths;
        }
        #endregion
    }
}
