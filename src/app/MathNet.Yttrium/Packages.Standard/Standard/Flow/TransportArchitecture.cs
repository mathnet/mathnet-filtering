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

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Manipulation;
using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.Packages.Standard.Flow
{
    //[EntityImplementation("Transport", "Std")]
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
            if(port == null)
                throw new ArgumentNullException("port");

            return port.BusCount == 0 && port.InputSignalCount == port.OutputSignalCount;
        }

        public override IArchitecture InstantiateToPort(Port port)
        {
            return new TransportArchitecture(port);
        }

        public static void RegisterTheorems(ILibrary library)
        {
            Analysis.DerivativeTransformation.Provider.Add(
                new Analysis.DerivativeTransformation(_entityId,
                delegate(Port port, SignalSet manipulatedInputs, Signal variable, bool hasManipulatedInputs)
                {
                    return new SignalSet(manipulatedInputs);
                }));

            Algebra.AutoSimplifyTransformation.Provider.Add(
                new Algebra.AutoSimplifyTransformation(_entityId,
                delegate(Port port)
                {
                    return ManipulationPlan.DoAlter;
                },
                delegate(Port port, SignalSet manipulatedInputs, bool hasManipulatedInputs)
                {
                    return new SignalSet(manipulatedInputs);
                }));
        }
    }
}
