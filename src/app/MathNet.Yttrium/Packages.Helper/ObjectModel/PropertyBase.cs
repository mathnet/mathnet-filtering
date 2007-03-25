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
using System.Xml;
using System.Reflection;

using MathNet.Symbolics.Backend.Utils;

namespace MathNet.Symbolics.Packages.ObjectModel
{
    public abstract class PropertyBase : IProperty
    {
        public abstract MathIdentifier TypeId { get;}

        public abstract bool StillValidAfterEvent(Signal signal);
        public abstract bool StillValidAfterDrive(Signal signal);
        public abstract bool StillValidAfterUndrive(Signal signal);

        public virtual bool ReferencesCoreObjects
        {
            get { return false; }
        }

        public virtual IEnumerable<Signal> CollectSignals()
        {
            yield break;
            //return SingletonProvider<EmptyIterator<Signal>>.Instance;
        }
        public virtual IEnumerable<Bus> CollectBuses()
        {
            yield break;
            //return SingletonProvider<EmptyIterator<Bus>>.Instance;
        }

        #region Equality
        public abstract bool Equals(IProperty other);
        public override bool Equals(object obj)
        {
            IProperty vs = obj as IProperty;
            if(vs != null)
                return Equals(vs);
            return false;
        }

        public bool EqualsById(IProperty other)
        {
            return other != null && TypeId.Equals(other.TypeId);
        }

        public bool EqualsById(MathIdentifier otherPropertyId)
        {
            return TypeId.Equals(otherPropertyId);
        }
        #endregion

        public override string ToString()
        {
            return TypeId.ToString();
        }

        // SEE MICROKERNEL SERIALIZER
        public virtual void Serialize(XmlWriter writer, IDictionary<Guid, Guid> signalMappings, IDictionary<Guid, Guid> busMappings)
        {
        }
        //private static Property Deserialize(IContext context, XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses);

        

        
    }
}
