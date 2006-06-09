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

using MathNet.Symbolics.Workplace;
using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Backend.Traversing
{
    public static class Manipulator
    {
        private class ManipulatorPlanReducer : AbstractScanVisitor
        {
            Dictionary<Guid, ManipulationPlan> _plans;
            bool _ignoreHold;

            public ManipulatorPlanReducer(Dictionary<Guid, ManipulationPlan> plans, bool ignoreHold)
            {
                _plans = plans;
                _ignoreHold = ignoreHold;
            }

            public override bool EnterPort(Port port, Signal parent, bool again, bool root)
            {
                // ## DIVE DEEPER ONLY ON UNCERTAIN PORTS
                if(_plans[port.InstanceId] != ManipulationPlan.AlterIfChildsAltered
                    && _plans[port.InstanceId] != ManipulationPlan.CloneIfChildsAltered)
                    return false;
                return base.EnterPort(port, parent, again, root);
            }

            public override bool VisitCycle(Port port, Signal target, Signal source)
            {
                // ## ON CYCLES, TEST WHETHER THE PORT DEPENDS ON ANY ALTERED PORTS AND ADOPT
                if(Scanner.ExistsPort(port, delegate(Port p) { return _plans[p.InstanceId] == ManipulationPlan.DoAlter; }, _ignoreHold))
                    _plans[port.InstanceId] = ManipulationPlan.DoAlter;
                else
                    _plans[port.InstanceId] = ManipulationPlan.DontAlter;
                return base.VisitCycle(port, target, source);
            }

            public override bool VisitLeaf(Port port, bool again)
            {
                // ## REMOVE PLANS ON UNCERTAIN PORTS WITHOUT CHILDS
                if(_plans[port.InstanceId] == ManipulationPlan.AlterIfChildsAltered
                    || _plans[port.InstanceId] == ManipulationPlan.CloneIfChildsAltered)
                    _plans[port.InstanceId] = ManipulationPlan.DontAlter;
                return base.VisitLeaf(port, again);
            }

            public override bool LeavePort(Port port, Signal parent, bool again, bool root)
            {
                // ## PROPAGATE "DONT ALTER" AND "DO ALTER"
                bool allDont = true;
                bool oneDoes = false;
                foreach(Signal s in port.InputSignals)
                    if(s.BehavesAsBeingDriven(_ignoreHold))
                    {
                        allDont = allDont && (_plans[s.DrivenByPort.InstanceId] == ManipulationPlan.DontAlter);
                        oneDoes = oneDoes || (_plans[s.DrivenByPort.InstanceId] == ManipulationPlan.DoAlter);
                    }
                if(oneDoes)
                    _plans[port.InstanceId] = ManipulationPlan.DoAlter;
                else if(allDont)
                    _plans[port.InstanceId] = ManipulationPlan.DontAlter;
                return base.LeavePort(port, parent, again, root);
            }
        }

        private class ManipulatorPlanExecutor : AbstractScanVisitor
        {
            IManipulationVisitor _visitor;
            Dictionary<Guid, ManipulationPlan> _plans;
            Dictionary<Guid, Signal> _signalRep;
            Dictionary<Guid, Signal> _sentinels;

            public ManipulatorPlanExecutor(Dictionary<Guid, ManipulationPlan> plans, Dictionary<Guid, Signal> signalReplacements, Dictionary<Guid, Signal> sentinels, IManipulationVisitor visitor)
            {
                _visitor = visitor;
                _plans = plans;
                _signalRep = signalReplacements;
                _sentinels = sentinels;
            }

            public override bool EnterSignal(Signal signal, Port parent, bool again, bool root)
            {
                // ## DONT DIVE DEEPER ON ALREADY REPLACED SIGNALS
                if(_signalRep.ContainsKey(signal.InstanceId))
                    return false;
                return base.EnterSignal(signal, parent, again, root);
            }

            public override bool EnterPort(Port port, Signal parent, bool again, bool root)
            {
                // ## DONT DIVE DEEPER ON "DONT ALTER" PORTS
                if(_plans[port.InstanceId] == ManipulationPlan.DontAlter)
                    return false;
                return base.EnterPort(port, parent, again, root);
            }

            public override bool VisitCycle(Port port, Signal target, Signal source)
            {
                // ## INSERT DUMMY SENTINEL SIGNALS TO BREAK UP CYCLES
                if(!_signalRep.ContainsKey(source.InstanceId)) // not replaced/sentinel yet
                {
                    Signal sentinel = new Signal(source.Context, source.Value);
                    sentinel.Label = "Sentinel_" + sentinel.InstanceId.ToString();
                    _sentinels.Add(sentinel.InstanceId, source);
                    _signalRep.Add(source.InstanceId, sentinel);
                }
                return base.VisitCycle(port, target, source);
            }

            public override bool LeavePort(Port port, Signal parent, bool again, bool root)
            {
                if(_plans[port.InstanceId] == ManipulationPlan.DontAlter)
                    return true;

                // ## MANIPULATE PORT
                bool isManipulated;
                SignalSet preManip = BuildManipulatedInputsList(port, out isManipulated);
                IEnumerable<Signal> postManip;
                if(_plans[port.InstanceId] == ManipulationPlan.CloneIfChildsAltered)
                {
                    Port newPort = port.CloneWithNewInputs(preManip);
                    postManip = newPort.OutputSignals;
                }
                else
                    postManip = _visitor.ManipulatePort(port, preManip, isManipulated);

                // ## WRITE BACK OUTPUT SIGNAL REPLACEMENTS
                ReadOnlySignalSet outputs = port.OutputSignals;
                int i = 0;
                foreach(Signal rep in postManip)
                    _signalRep[outputs[i++].InstanceId] = rep;

                return base.LeavePort(port, parent, again, root);
            }

            public override bool LeaveSignal(Signal signal, Port parent, bool again, bool root)
            {
                // ## POST-MANIPULATE (REPLACED) SIGNALS
                Signal rep;
                if(_signalRep.TryGetValue(signal.InstanceId, out rep))
                    _signalRep[signal.InstanceId] = _visitor.ManipulateSignal(signal, rep, !signal.Equals(rep));
                else
                    _signalRep[signal.InstanceId] = _visitor.ManipulateSignal(signal, signal, false);

                return base.LeaveSignal(signal, parent, again, root);
            }

            private SignalSet BuildManipulatedInputsList(Port port, out bool isManipulated)
            {
                ReadOnlySignalSet inputs = port.InputSignals;
                SignalSet manip = new SignalSet();
                isManipulated = false;
                foreach(Signal s in inputs)
                {
                    Signal rep;
                    if(_signalRep.TryGetValue(s.InstanceId, out rep))
                    {
                        if(!s.Equals(rep))
                            isManipulated = true;
                        manip.Add(rep);
                    }
                    else
                        manip.Add(s);
                }
                return manip;
            }
        }

        public static Signal Manipulate(Signal rootSignal, IManipulationVisitor visitor, bool ignoreHold)
        {
            Dictionary<Guid, ManipulationPlan> plans = new Dictionary<Guid, ManipulationPlan>();
            Dictionary<Guid, Signal> signalReplacements = new Dictionary<Guid, Signal>();
            Dictionary<Guid, Signal> sentinels = new Dictionary<Guid, Signal>();

            // ## ESTIMATE MANIPULATION PLAN
            Scanner.ForEachPort(rootSignal, delegate(Port p)
            {
                if(!plans.ContainsKey(p.InstanceId))
                    plans.Add(p.InstanceId, visitor.EstimatePlan(p));
                return true;
            }, ignoreHold);
            
            // ## OPTIMIZE MANIPULATION PLAN (cycle analysis)
            ManipulatorPlanReducer reducer = new ManipulatorPlanReducer(plans, ignoreHold);
            AllPathsStrategy.Instance.Traverse(rootSignal, reducer, ignoreHold);

            // ## EXECUTE MANIPULATION
            ManipulatorPlanExecutor executor = new ManipulatorPlanExecutor(plans, signalReplacements, sentinels, visitor);
            AllPathsStrategy.Instance.Traverse(rootSignal, executor, ignoreHold);

            // ## SELECT NEW SYSTEM
            Signal ret;
            if(!signalReplacements.TryGetValue(rootSignal.InstanceId, out ret))
                ret = rootSignal;

            // ## FIX SENTINELS ON SELECTED NEW SYSTEM
            Scanner.ForEachPort(ret, delegate(Port p)
            {
                // look for sentinels on all input signals
                ReadOnlySignalSet inputs = p.InputSignals;
                for(int i = 0; i < inputs.Count; i++)
                {
                    Signal input = inputs[i];
                    if(FixSentinel(ref input, sentinels, signalReplacements))
                        p.ReplaceInputSignalBinding(i, input);
                }
                return true;
            }, ignoreHold);
            FixSentinel(ref ret, sentinels, signalReplacements);

            // ## RETURN SELECTED NEW SYSTEM AS RESULT
            return ret;
        }

        public static SignalSet Manipulate(IEnumerable<Signal> rootSignals, IManipulationVisitor visitor, bool ignoreHold)
        {
            Dictionary<Guid, ManipulationPlan> plans = new Dictionary<Guid, ManipulationPlan>();
            Dictionary<Guid, Signal> signalReplacements = new Dictionary<Guid, Signal>();
            Dictionary<Guid, Signal> sentinels = new Dictionary<Guid, Signal>();

            // ## ESTIMATE MANIPULATION PLAN
            Scanner.ForEachPort(rootSignals, delegate(Port p)
            {
                if(!plans.ContainsKey(p.InstanceId))
                    plans.Add(p.InstanceId, visitor.EstimatePlan(p));
                return true;
            }, ignoreHold);

            // ## OPTIMIZE MANIPULATION PLAN (cycle analysis)
            ManipulatorPlanReducer reducer = new ManipulatorPlanReducer(plans, ignoreHold);
            AllPathsStrategy.Instance.Traverse(rootSignals, reducer, ignoreHold);

            // ## EXECUTE MANIPULATION
            ManipulatorPlanExecutor executor = new ManipulatorPlanExecutor(plans, signalReplacements, sentinels, visitor);
            AllPathsStrategy.Instance.Traverse(rootSignals, executor, ignoreHold);

            // ## SELECT NEW SYSTEM
            SignalSet ret = new SignalSet();
            foreach(Signal root in rootSignals)
            {
                Signal r;
                if(!signalReplacements.TryGetValue(root.InstanceId, out r))
                    r = root;
                ret.Add(r);
            }

            // ## FIX SENTINELS ON SELECTED NEW SYSTEM
            Scanner.ForEachPort(ret, delegate(Port p)
            {
                // look for sentinels on all input signals
                ReadOnlySignalSet inputs = p.InputSignals;
                for(int i = 0; i < inputs.Count; i++)
                {
                    Signal input = inputs[i];
                    if(FixSentinel(ref input, sentinels, signalReplacements))
                        p.ReplaceInputSignalBinding(i, input);
                }
                return true;
            }, ignoreHold);

            for(int i = 0; i < ret.Count; i++)
            {
                Signal r = ret[i];
                if(FixSentinel(ref r, sentinels, signalReplacements))
                    ret[i] = r;
            }

            // ## RETURN SELECTED NEW SYSTEM AS RESULT
            return ret;
        }

        private static bool FixSentinel(ref Signal signal, Dictionary<Guid, Signal> sentinels, Dictionary<Guid, Signal> signalReplacements)
        {
            bool ret = false;
            while(sentinels.ContainsKey(signal.InstanceId)) // is a sentinel
            {
                ret = true;
                Signal original = sentinels[signal.InstanceId];
                Signal final;
                if(signalReplacements.TryGetValue(original.InstanceId, out final) && !signal.Equals(final))
                    signal = final; // sentinel was replaced -> replace with replacement
                else
                    signal = original; // sentinel was not replaced -> replace with original
            }
            return ret; // is not a sentinel
        }
    }
}
