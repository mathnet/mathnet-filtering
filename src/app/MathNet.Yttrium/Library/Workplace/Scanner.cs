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
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Backend.Theorems;
using MathNet.Symbolics.Backend.Traversing;

namespace MathNet.Symbolics.Workplace
{
    public static class Scanner
    {
        public static void Traverse(Signal rootSignal, ScanStrategy strategy, IScanVisitor visitor, bool ignoreHold)
        {
            if(strategy == null) throw new ArgumentNullException("strategy");
            strategy.Traverse(rootSignal, visitor, ignoreHold);
        }
        public static void Traverse(Port rootPort, ScanStrategy strategy, IScanVisitor visitor, bool ignoreHold)
        {
            if(strategy == null) throw new ArgumentNullException("strategy");
            strategy.Traverse(rootPort, visitor, ignoreHold);
        }
        public static void Traverse(IEnumerable<Signal> rootSignals, ScanStrategy strategy, IScanVisitor visitor, bool ignoreHold)
        {
            if(strategy == null) throw new ArgumentNullException("strategy");
            strategy.Traverse(rootSignals, visitor, ignoreHold);
        }

        #region For-Each Lambdas -> Action
        #region Unconditional
        public static void ForEachSignal(Signal rootSignal, ActionContinue<Signal> action, bool ignoreHold)
        {
            SignalActionVisitor visitor = new SignalActionVisitor(action);
            AllSignalsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
        }
        public static void ForEachSignal(Port rootPort, ActionContinue<Signal> action, bool ignoreHold)
        {
            SignalActionVisitor visitor = new SignalActionVisitor(action);
            AllSignalsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
        }
        public static void ForEachSignal(IEnumerable<Signal> rootSignals, ActionContinue<Signal> action, bool ignoreHold)
        {
            SignalActionVisitor visitor = new SignalActionVisitor(action);
            AllSignalsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
        }

        public static void ForEachPort(Signal rootSignal, ActionContinue<Port> action, bool ignoreHold)
        {
            PortActionVisitor visitor = new PortActionVisitor(action);
            AllPortsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
        }
        public static void ForEachPort(Port rootPort, ActionContinue<Port> action, bool ignoreHold)
        {
            PortActionVisitor visitor = new PortActionVisitor(action);
            AllPortsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
        }
        public static void ForEachPort(IEnumerable<Signal> rootSignals, ActionContinue<Port> action, bool ignoreHold)
        {
            PortActionVisitor visitor = new PortActionVisitor(action);
            AllPortsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
        }
        #endregion
        #region Conditional
        public static void ForEachSignal(Signal rootSignal, Predicate<Signal> match, ActionContinue<Signal> action, bool ignoreHold)
        {
            SignalConditionalActionVisitor visitor = new SignalConditionalActionVisitor(action, match);
            AllSignalsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
        }
        public static void ForEachSignal(Port rootPort, Predicate<Signal> match, ActionContinue<Signal> action, bool ignoreHold)
        {
            SignalConditionalActionVisitor visitor = new SignalConditionalActionVisitor(action, match);
            AllSignalsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
        }
        public static void ForEachSignal(IEnumerable<Signal> rootSignals, Predicate<Signal> match, ActionContinue<Signal> action, bool ignoreHold)
        {
            SignalConditionalActionVisitor visitor = new SignalConditionalActionVisitor(action, match);
            AllSignalsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
        }

        public static void ForEachPort(Signal rootSignal, Predicate<Port> match, ActionContinue<Port> action, bool ignoreHold)
        {
            PortConditionalActionVisitor visitor = new PortConditionalActionVisitor(action, match);
            AllPortsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
        }
        public static void ForEachPort(Port rootPort, Predicate<Port> match, ActionContinue<Port> action, bool ignoreHold)
        {
            PortConditionalActionVisitor visitor = new PortConditionalActionVisitor(action, match);
            AllPortsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
        }
        public static void ForEachPort(IEnumerable<Signal> rootSignals, Predicate<Port> match, ActionContinue<Port> action, bool ignoreHold)
        {
            PortConditionalActionVisitor visitor = new PortConditionalActionVisitor(action, match);
            AllPortsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
        }
        #endregion
        #endregion

        #region Exists Lambdas -> bool
        #region Signal
        public static bool ExistsSignal(Signal rootSignal, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return ExistsSignal(rootSignal, match, ignoreHold, out signal, out port);
        }
        public static bool ExistsSignal(Signal rootSignal, Predicate<Signal> match, bool ignoreHold, out Signal foundSignal, out Port foundSignalTarget)
        {
            ExistsSignalVisitor visitor = new ExistsSignalVisitor(match);
            AllSignalsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
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

        public static bool ExistsSignal(Port rootPort, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return ExistsSignal(rootPort, match, ignoreHold, out signal, out port);
        }
        public static bool ExistsSignal(Port rootPort, Predicate<Signal> match, bool ignoreHold, out Signal foundSignal, out Port foundSignalTarget)
        {
            ExistsSignalVisitor visitor = new ExistsSignalVisitor(match);
            AllSignalsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
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

        public static bool ExistsSignal(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return ExistsSignal(rootSignals, match, ignoreHold, out signal, out port);
        }
        public static bool ExistsSignal(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold, out Signal foundSignal, out Port foundSignalTarget)
        {
            ExistsSignalVisitor visitor = new ExistsSignalVisitor(match);
            AllSignalsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
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
        public static bool ExistsPort(Signal rootSignal, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return ExistsPort(rootSignal, match, ignoreHold, out port, out signal);
        }
        public static bool ExistsPort(Signal rootSignal, Predicate<Port> match, bool ignoreHold, out Port foundPort, out Signal foundPortTarget)
        {
            ExistsPortVisitor visitor = new ExistsPortVisitor(match);
            AllPortsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
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

        public static bool ExistsPort(Port rootPort, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return ExistsPort(rootPort, match, ignoreHold, out port, out signal);
        }
        public static bool ExistsPort(Port rootPort, Predicate<Port> match, bool ignoreHold, out Port foundPort, out Signal foundPortTarget)
        {
            ExistsPortVisitor visitor = new ExistsPortVisitor(match);
            AllPortsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
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

        public static bool ExistsPort(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return ExistsPort(rootSignals, match, ignoreHold, out port, out signal);
        }
        public static bool ExistsPort(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold, out Port foundPort, out Signal foundPortTarget)
        {
            ExistsPortVisitor visitor = new ExistsPortVisitor(match);
            AllPortsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
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
        public static bool TrueForAllSignals(Signal rootSignal, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return TrueForAllSignals(rootSignal, match, ignoreHold, out signal, out port);
        }
        public static bool TrueForAllSignals(Signal rootSignal, Predicate<Signal> match, bool ignoreHold, out Signal failedSignal, out Port failedSignalTarget)
        {
            TrueForAllSignalsVisitor visitor = new TrueForAllSignalsVisitor(match);
            AllSignalsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
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

        public static bool TrueForAllSignals(Port rootPort, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return TrueForAllSignals(rootPort, match, ignoreHold, out signal, out port);
        }
        public static bool TrueForAllSignals(Port rootPort, Predicate<Signal> match, bool ignoreHold, out Signal failedSignal, out Port failedSignalTarget)
        {
            TrueForAllSignalsVisitor visitor = new TrueForAllSignalsVisitor(match);
            AllSignalsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
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

        public static bool TrueForAllSignals(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return TrueForAllSignals(rootSignals, match, ignoreHold, out signal, out port);
        }
        public static bool TrueForAllSignals(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold, out Signal failedSignal, out Port failedSignalTarget)
        {
            TrueForAllSignalsVisitor visitor = new TrueForAllSignalsVisitor(match);
            AllSignalsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
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
        public static bool TrueForAllPorts(Signal rootSignal, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return TrueForAllPorts(rootSignal, match, ignoreHold, out port, out signal);
        }
        public static bool TrueForAllPorts(Signal rootSignal, Predicate<Port> match, bool ignoreHold, out Port failedPort, out Signal failedPortTarget)
        {
            TrueForAllPortsVisitor visitor = new TrueForAllPortsVisitor(match);
            AllPortsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
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

        public static bool TrueForAllPorts(Port rootPort, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return TrueForAllPorts(rootPort, match, ignoreHold, out port, out signal);
        }
        public static bool TrueForAllPorts(Port rootPort, Predicate<Port> match, bool ignoreHold, out Port failedPort, out Signal failedPortTarget)
        {
            TrueForAllPortsVisitor visitor = new TrueForAllPortsVisitor(match);
            AllPortsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
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

        public static bool TrueForAllPorts(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            return TrueForAllPorts(rootSignals, match, ignoreHold, out port, out signal);
        }
        public static bool TrueForAllPorts(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold, out Port failedPort, out Signal failedPortTarget)
        {
            TrueForAllPortsVisitor visitor = new TrueForAllPortsVisitor(match);
            AllPortsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
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

        #region Find-All Lambdas -> List
        #region Unconditional
        public static SignalSet FindAllSignals(Signal rootSignal, bool ignoreHold)
        {
            CollectVisitor visitor = new CollectVisitor();
            AllSignalsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
            return visitor.Signals;
        }
        public static SignalSet FindAllSignals(Port rootPort, bool ignoreHold)
        {
            CollectVisitor visitor = new CollectVisitor();
            AllSignalsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
            return visitor.Signals;
        }
        public static SignalSet FindAllSignals(IEnumerable<Signal> rootSignals, bool ignoreHold)
        {
            CollectVisitor visitor = new CollectVisitor();
            AllSignalsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
            return visitor.Signals;
        }

        public static PortSet FindAllPorts(Signal rootSignal, bool ignoreHold)
        {
            CollectVisitor visitor = new CollectVisitor();
            AllPortsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
            return visitor.Ports;
        }
        public static PortSet FindAllPorts(Port rootPort, bool ignoreHold)
        {
            CollectVisitor visitor = new CollectVisitor();
            AllPortsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
            return visitor.Ports;
        }
        public static PortSet FindAllPorts(IEnumerable<Signal> rootSignals, bool ignoreHold)
        {
            CollectVisitor visitor = new CollectVisitor();
            AllPortsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
            return visitor.Ports;
        }

        public static void FindAll(Signal rootSignal, bool ignoreHold, out SignalSet signals, out PortSet ports)
        {
            CollectVisitor visitor = new CollectVisitor();
            AllSpanningTreeStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
            signals = visitor.Signals;
            ports = visitor.Ports;
        }
        public static void FindAll(Port rootPort, bool ignoreHold, out SignalSet signals, out PortSet ports)
        {
            CollectVisitor visitor = new CollectVisitor();
            AllSpanningTreeStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
            signals = visitor.Signals;
            ports = visitor.Ports;
        }
        public static void FindAll(IEnumerable<Signal> rootSignals, bool ignoreHold, out SignalSet signals, out PortSet ports)
        {
            CollectVisitor visitor = new CollectVisitor();
            AllSpanningTreeStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
            signals = visitor.Signals;
            ports = visitor.Ports;
        }
        #endregion
        #region Conditional
        public static SignalSet FindAllSignals(Signal rootSignal, Predicate<Signal> match, bool ignoreHold)
        {
            ConditionalCollectVisitor visitor = new ConditionalCollectVisitor(match);
            AllSignalsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
            return visitor.Signals;
        }
        public static SignalSet FindAllSignals(Port rootPort, Predicate<Signal> match, bool ignoreHold)
        {
            ConditionalCollectVisitor visitor = new ConditionalCollectVisitor(match);
            AllSignalsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
            return visitor.Signals;
        }
        public static SignalSet FindAllSignals(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold)
        {
            ConditionalCollectVisitor visitor = new ConditionalCollectVisitor(match);
            AllSignalsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
            return visitor.Signals;
        }

        public static PortSet FindAllPorts(Signal rootSignal, Predicate<Port> match, bool ignoreHold)
        {
            ConditionalCollectVisitor visitor = new ConditionalCollectVisitor(match);
            AllPortsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
            return visitor.Ports;
        }
        public static PortSet FindAllPorts(Port rootPort, Predicate<Port> match, bool ignoreHold)
        {
            ConditionalCollectVisitor visitor = new ConditionalCollectVisitor(match);
            AllPortsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
            return visitor.Ports;
        }
        public static PortSet FindAllPorts(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold)
        {
            ConditionalCollectVisitor visitor = new ConditionalCollectVisitor(match);
            AllPortsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
            return visitor.Ports;
        }
        #endregion
        #endregion

        #region Find Lambdas -> Signal/Port/Bus
        public static Signal FindSignal(Signal rootSignal, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            if(ExistsSignal(rootSignal, match, ignoreHold, out signal, out port))
                return signal;
            else
                throw new MathNet.Symbolics.Backend.Exceptions.NotFoundException();
        }
        public static Signal FindSignal(Port rootPort, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            if(ExistsSignal(rootPort, match, ignoreHold, out signal, out port))
                return signal;
            else
                throw new MathNet.Symbolics.Backend.Exceptions.NotFoundException();
        }
        public static Signal FindSignal(IEnumerable<Signal> rootSignals, Predicate<Signal> match, bool ignoreHold)
        {
            Signal signal; Port port;
            if(ExistsSignal(rootSignals, match, ignoreHold, out signal, out port))
                return signal;
            else
                throw new MathNet.Symbolics.Backend.Exceptions.NotFoundException();
        }

        public static Port FindPort(Signal rootSignal, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            if(ExistsPort(rootSignal, match, ignoreHold, out port, out signal))
                return port;
            else
                throw new MathNet.Symbolics.Backend.Exceptions.NotFoundException();
        }
        public static Port FindPort(Port rootPort, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            if(ExistsPort(rootPort, match, ignoreHold, out port, out signal))
                return port;
            else
                throw new MathNet.Symbolics.Backend.Exceptions.NotFoundException();
        }
        public static Port FindPort(IEnumerable<Signal> rootSignals, Predicate<Port> match, bool ignoreHold)
        {
            Signal signal; Port port;
            if(ExistsPort(rootSignals, match, ignoreHold, out port, out signal))
                return port;
            else
                throw new MathNet.Symbolics.Backend.Exceptions.NotFoundException();
        }
        #endregion

        #region Collect Paths -> List of Lists of Signal/Port
        public static List<List<Signal>> FindAllSignalPathsFrom(Signal rootSignal, Signal from, bool ignoreHold)
        {
            SignalPathCollectVisitor visitor = new SignalPathCollectVisitor(from);
            AllPathsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
            return visitor.Paths;
        }
        public static List<List<Signal>> FindAllSignalPathsFrom(Port rootPort, Signal from, bool ignoreHold)
        {
            SignalPathCollectVisitor visitor = new SignalPathCollectVisitor(from);
            AllPathsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
            return visitor.Paths;
        }
        public static List<List<Signal>> FindAllSignalPathsFrom(IEnumerable<Signal> rootSignals, Signal from, bool ignoreHold)
        {
            SignalPathCollectVisitor visitor = new SignalPathCollectVisitor(from);
            AllPathsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
            return visitor.Paths;
        }

        public static List<List<Port>> FindAllPortPathsFrom(Signal rootSignal, Port from, bool ignoreHold)
        {
            PortPathCollectVisitor visitor = new PortPathCollectVisitor(from);
            AllPathsStrategy.Instance.Traverse(rootSignal, visitor, ignoreHold);
            return visitor.Paths;
        }
        public static List<List<Port>> FindAllPortPathsFrom(Port rootPort, Port from, bool ignoreHold)
        {
            PortPathCollectVisitor visitor = new PortPathCollectVisitor(from);
            AllPathsStrategy.Instance.Traverse(rootPort, visitor, ignoreHold);
            return visitor.Paths;
        }
        public static List<List<Port>> FindAllPortPathsFrom(IEnumerable<Signal> rootSignals, Port from, bool ignoreHold)
        {
            PortPathCollectVisitor visitor = new PortPathCollectVisitor(from);
            AllPathsStrategy.Instance.Traverse(rootSignals, visitor, ignoreHold);
            return visitor.Paths;
        }
        #endregion

        #region Manipulator
        public static Signal Manipulate(Signal rootSignal, IManipulationVisitor visitor, bool ignoreHold)
        {
            return Manipulator.Manipulate(rootSignal, visitor, ignoreHold);
        }
        public static SignalSet Manipulate(IEnumerable<Signal> rootSignals, IManipulationVisitor visitor, bool ignoreHold)
        {
            return Manipulator.Manipulate(rootSignals, visitor, ignoreHold);
        }

        #region Basic Manipulation
        public static Signal Manipulate(Signal rootSignal, EstimatePlan plan, ManipulatePort manipulatePort, ManipulateSignal manipulateSignal, bool ignoreHold)
        {
            IManipulationVisitor visitor = new BasicManipulationVisitor(plan, manipulatePort, manipulateSignal);
            return Manipulator.Manipulate(rootSignal, visitor, ignoreHold);
        }
        public static SignalSet Manipulate(IEnumerable<Signal> rootSignals, EstimatePlan plan, ManipulatePort manipulatePort, ManipulateSignal manipulateSignal, bool ignoreHold)
        {
            IManipulationVisitor visitor = new BasicManipulationVisitor(plan, manipulatePort, manipulateSignal);
            return Manipulator.Manipulate(rootSignals, visitor, ignoreHold);
        }
        #endregion

        #region Transformation Theorems
        public static Signal Transform(Signal signal, Context context, MathIdentifier transformationTypeId, bool ignoreHold)
        {
            TransformationTypeTable transTable = context.Library.LookupTransformationTheoremType(transformationTypeId);
            TransformationManipulationVisitor visitor = new TransformationManipulationVisitor(transTable);
            return Manipulator.Manipulate(signal, visitor, ignoreHold);
        }
        public static Signal Transform(Signal signal, Context context, MathIdentifier transformationTypeId, ConfigureTransformation configure, bool ignoreHold)
        {
            TransformationTypeTable transTable = context.Library.LookupTransformationTheoremType(transformationTypeId);
            TransformationManipulationVisitor visitor = new TransformationManipulationVisitor(transTable, configure);
            return Manipulator.Manipulate(signal, visitor, ignoreHold);
        }

        public static SignalSet Transform(IEnumerable<Signal> signals, Context context, MathIdentifier transformationTypeId, bool ignoreHold)
        {
            TransformationTypeTable transTable = context.Library.LookupTransformationTheoremType(transformationTypeId);
            TransformationManipulationVisitor visitor = new TransformationManipulationVisitor(transTable);
            return Manipulator.Manipulate(signals, visitor, ignoreHold);
        }
        public static SignalSet Transform(IEnumerable<Signal> signals, Context context, MathIdentifier transformationTypeId, ConfigureTransformation configure, bool ignoreHold)
        {
            TransformationTypeTable transTable = context.Library.LookupTransformationTheoremType(transformationTypeId);
            TransformationManipulationVisitor visitor = new TransformationManipulationVisitor(transTable, configure);
            return Manipulator.Manipulate(signals, visitor, ignoreHold);
        }
        #endregion

        #region Substitute
        public static Signal Substitute(Signal rootSignal, Signal subject, Signal replacement)
        {
            IManipulationVisitor visitor = CreateSubstituteVisitor(subject, replacement);
            return Manipulator.Manipulate(rootSignal, visitor, false);
        }
        public static SignalSet Substitute(IEnumerable<Signal> rootSignals, Signal subject, Signal replacement)
        {
            IManipulationVisitor visitor = CreateSubstituteVisitor(subject, replacement);
            return Manipulator.Manipulate(rootSignals, visitor, false);
        }
        private static IManipulationVisitor CreateSubstituteVisitor(Signal subject, Signal replacement)
        {
            return new BasicManipulationVisitor(
                delegate(Port p)
                {   // ## ESTIMATE PLAN
                    if(p.InputSignals.Contains(subject))
                        return ManipulationPlan.DoAlter;
                    else
                        return ManipulationPlan.CloneIfChildsAltered;
                },
                delegate(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs)
                {   // ## MANIPULATE PORT

                    /* NOTE: manipulatedInputs could contain sentinels, that's why
                     * we use the original port inputs instead. */

                    ReadOnlySignalSet inputs = port.InputSignals; 
                    for(int i = 0; i < inputs.Count; i++)
                        if(subject.Equals(inputs[i]))
                            manipulatedInputs[i] = replacement;
                    return port.CloneWithNewInputs(manipulatedInputs).OutputSignals;
                },
                delegate(Signal original, Signal replaced, bool isReplaced)
                {   // ## POST-MANIPULATE SIGNAL
                    if(subject.Equals(replaced))
                        return replacement;
                    else
                        return replaced;
                });
        }
        #endregion

        #endregion
    }
}
