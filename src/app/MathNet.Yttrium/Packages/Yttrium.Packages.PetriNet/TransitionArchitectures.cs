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

using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Library;

namespace MathNet.Symbolics.Packages.PetriNet
{
    public class TransitionArchitectures : GenericSimpleArchitecture
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("Transition", "PetriNet");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public TransitionArchitectures() : base(_entityId, false) { }
        public TransitionArchitectures(Port port) : base(_entityId, false, port, 0) { }

        protected override void SenseSignals(IList<Signal> inputSignals, IList<Signal> internalSignals, IList<Bus> buses, IList<Signal> outputSignals)
        {
            for(int i = 0; i < inputSignals.Count; i++)
                SenseSignal(inputSignals[i]);
        }

        protected override void Action(IList<Signal> inputSignals, IList<Signal> outputSignals, IList<Signal> internalSignals, IList<Bus> buses)
        {
            foreach(Signal input in inputSignals)
            {
                TokenValue token = input.Value as TokenValue;
                if(token == null || token.Value < 1)
                    return;
            }

            // apparently all nodes ok, so we fire:

            foreach(Signal input in inputSignals)
                input.PostNewValue(((TokenValue)input.Value).Decrement());
            foreach(Signal output in outputSignals)
                output.PostNewValue(((output.Value as TokenValue) ?? TokenValue.Zero).Increment());
        }

        public override bool SupportsPort(Port port)
        {
            if(port == null)
                throw new ArgumentNullException("port");

            // all inputs must be empty or of type TokenValue
            return !port.InputSignals.Exists(delegate(Signal s) { return (s.Value as TokenValue) == null; }); 
        }

        public override IArchitecture InstantiateToPort(Port port)
        {
            return new TransitionArchitectures(port);
        }

        public static void RegisterTheorems(ILibrary library)
        {
        }
    }
}
