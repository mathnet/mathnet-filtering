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

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.Discovery;
using MathNet.Symbolics.StdPackage.Structures;

namespace MathNet.Symbolics.StdPackage.Flow
{
    /// <summary>
    /// Clocked Register, the clock is the last input signal.
    /// </summary>
    [EntityImplementation("Sync", "Std")]
    public class SyncArchitecture : Architecture, IArchitectureFactory
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

            port.InputSignals[port.InputSignalCount-1].SignalValueChanged  += SyncArchitecture_SignalValueChanged;
        }

        void SyncArchitecture_SignalValueChanged(object sender, EventArgs e)
        {
            for(int i = 0; i < Port.OutputSignals.Count; i++)
                Port.OutputSignals[i].PostNewValue(Port.InputSignals[i].Value);
        }

        public override bool SupportsPort(Port port)
        {
            return port.BusCount == 0 && port.InputSignalCount == port.OutputSignalCount+1;
        }

        public override void UnregisterArchitecture()
        {
            Port.InputSignals[Port.InputSignalCount - 1].SignalValueChanged -= SyncArchitecture_SignalValueChanged;
        }

        protected override void ReregisterArchitecture(Port oldPort, Port newPort)
        {
            newPort.InputSignals[newPort.InputSignalCount - 1].SignalValueChanged += SyncArchitecture_SignalValueChanged;
        }

        public Architecture InstantiateToPort(Port port)
        {
            return new SyncArchitecture(port);
        }
    }
}
