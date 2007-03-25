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
using System.Text;

using MathNet.Symbolics.Backend;
using MathNet.Numerics;

namespace MathNet.Symbolics.Backend.Containers
{
    public class PortSet : Set<Port>, IPortSet
    {
        public PortSet() : base() { }
        public PortSet(IEnumerable<Port> initial) : base(initial) { }
        public PortSet(params Port[] initial) : base(initial) { }
        protected PortSet(IList<Port> innerList) : base(innerList) { }
        public PortSet(int initialCount) : base(initialCount) { }

        public static PortSet ConvertFrom(Set<Port> set)
        {
            PortSet ps = set as PortSet;
            if(ps == null)
                return new PortSet((IList<Port>)set);
            else
                return ps;
        }

        #region Factory Methods
        protected override Set<Port> CreateNewSet()
        {
            return new PortSet();
        }
        protected override ReadOnlySet<Port> CreateNewReadOnlyWrapper(IList<Port> list)
        {
            return new ReadOnlyPortSet(list);
        }
        #endregion

        public new ReadOnlyPortSet AsReadOnly
        {   // works thanks to the Factory Method pattern/trick
            get { return (ReadOnlyPortSet)base.AsReadOnly; }
        }

        #region InstanceId Conversion
        public InstanceIdSet ConvertAllToInstanceIds()
        {
            InstanceIdSet ids = new InstanceIdSet();
            foreach(Port p in this)
                ids.Add(p.InstanceId);
            return ids;
        }
        public InstanceIdSet ConvertAllToInstanceIds(Converter<Port, Guid> convert)
        {
            InstanceIdSet ids = new InstanceIdSet();
            foreach(Port p in this)
                ids.Add(convert(p));
            return ids;
        }
        public static PortSet ConvertAllFromInstanceIds(InstanceIdSet idSet, Converter<Guid, Port> convert)
        {
            PortSet ps = new PortSet();
            foreach(Guid id in idSet)
                ps.Add(convert(id));
            return ps;
        }
        #endregion
    }

    public class ReadOnlyPortSet : ReadOnlySet<Port>, IPortSet
    {
        public ReadOnlyPortSet(IList<Port> list) : base(list) { }

        #region Factory Methods
        protected override Set<Port> CreateNewSet()
        {
            return new PortSet();
        }
        #endregion

        #region InstanceId Conversion
        public InstanceIdSet ConvertAllToInstanceIds()
        {
            InstanceIdSet ids = new InstanceIdSet();
            foreach(Port p in this)
                ids.Add(p.InstanceId);
            return ids;
        }
        public InstanceIdSet ConvertAllToInstanceIds(Converter<Port, Guid> convert)
        {
            InstanceIdSet ids = new InstanceIdSet();
            foreach(Port p in this)
                ids.Add(convert(p));
            return ids;
        }
        #endregion
    }
}
