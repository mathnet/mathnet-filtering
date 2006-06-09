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

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.Discovery;
using MathNet.Symbolics.Backend.Theorems;
using MathNet.Symbolics.Backend.Traversing;
using MathNet.Symbolics.StdPackage.Structures;

namespace MathNet.Symbolics.StdPackage.Flow
{
    [EntityImplementation("Transport", "Std")]
    public class TransportArchitecture : GenericSimpleArchitecture
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("Transport", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public TransportArchitecture() : base(_entityId, false) { }
        public TransportArchitecture(Port port) : base(_entityId, false, port, 0) { }

        protected override void SenseSignals(IList<Signal> inputSignals, IList<Signal> internalSignals, IList<Bus> buses, IList<Signal> outputSignals)
        {
            for(int i = 0; i < inputSignals.Count; i++)
                SenseSignal(inputSignals[i]);
        }

        protected override void Action(IList<Signal> inputSignals, IList<Signal> outputSignals, IList<Signal> internalSignals, IList<Bus> buses)
        {
            for(int i = 0; i < inputSignals.Count; i++)
                if(inputSignals[i].HasEvent)
                    outputSignals[i].PostNewValue(inputSignals[i].Value);
        }

        public override bool SupportsPort(Port port)
        {
            return port.BusCount == 0 && port.InputSignalCount == port.OutputSignalCount;
        }

        public override Architecture InstantiateToPort(Port port)
        {
            return new TransportArchitecture(port);
        }

        public static ITheorem[] BuildTheorems(Context context)
        {
            ITheorem[] theorems = new ITheorem[2];

            theorems[0] = new Analysis.DerivativeTransformation(context.Library.LookupEntity(_entityId),
                delegate(Port port, SignalSet manipulatedInputs, Signal variable, bool hasManipulatedInputs)
                {
                    return new SignalSet(manipulatedInputs);
                });

            theorems[1] = new Algebra.AutoSimplifyTransformation(context.Library.LookupEntity(_entityId),
                delegate(Port port)
                {
                    return ManipulationPlan.DoAlter;
                },
                delegate(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs)
                {
                    return new SignalSet(manipulatedInputs);
                });

            return theorems;
        }
    }
}
