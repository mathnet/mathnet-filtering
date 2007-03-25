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
using System.Xml;

namespace MathNet.Symbolics
{
    /// <summary>
    /// A user defined object storing data, that support storing references of signals, buses and ports.
    /// </summary>
    /// <remarks>
    /// Any class implementing this interface shall also implement a static deserialization factory:
    /// private static YourType Deserialize(XmlReader reader, Dictionary<Guid, Signal> signals, Dictionary<Guid, Bus> buses);
    /// </remarks>
    public interface ICustomData
    {
        MathIdentifier TypeId { get;}

        bool ReferencesCoreObjects { get;}
        IEnumerable<Signal> CollectSignals();
        IEnumerable<Bus> CollectBuses();
        //IEnumerable<Port> CollectPorts();

        void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings);
        //void Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses);

    }
}
