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

using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Packages.ObjectModel;

namespace MathNet.Symbolics.Packages.Standard.Flow
{
    /// <summary>
    /// Clocked Register, the clock is the last input signal.
    /// </summary>
    //[EntityImplementation("Sync", "Std")]
    public class SyncArchitecture : ArchitectureBase, IArchitectureFactory
    {
        private static readonly MathIdentifier _entityId = new MathIdentifier("Sync", "Std");
        public static MathIdentifier EntityIdentifier
        {
            get { return _entityId; }
        }

        public SyncArchitecture() : base(_entityId, _entityId, false) { }
        public SyncArchitecture(Port port)
            : base(_entityId, _entityId, false)
        {
            //System.Diagnostics.Debug.Assert(SupportsPort(port));
            SetPort(port);

            port.InputSignals[port.InputSignalCount-1].ValueChanged  += SyncArchitecture_SignalValueChanged;
        }

        void SyncArchitecture_SignalValueChanged(object sender, EventArgs e)
        {
            for(int i = 0; i < Port.OutputSignals.Count; i++)
                Port.OutputSignals[i].PostNewValue(Port.InputSignals[i].Value);
        }

        public override bool SupportsPort(Port port)
        {
            if(port == null)
                throw new ArgumentNullException("port");

            return port.BusCount == 0 && port.InputSignalCount == port.OutputSignalCount+1;
        }

        public override void UnregisterArchitecture()
        {
            Port.InputSignals[Port.InputSignalCount - 1].ValueChanged -= SyncArchitecture_SignalValueChanged;
        }

        protected override void ReregisterArchitecture(Port oldPort, Port newPort)
        {
            newPort.InputSignals[newPort.InputSignalCount - 1].ValueChanged += SyncArchitecture_SignalValueChanged;
        }

        public IArchitecture InstantiateToPort(Port port)
        {
            return new SyncArchitecture(port);
        }
    }
}
