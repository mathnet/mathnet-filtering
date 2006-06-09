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
using System.Xml;
using System.Text;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Numerics;

namespace MathNet.Symbolics.Backend.Containers
{
    public class BusSet : Set<Bus>
    {
        public BusSet() : base() { }
        public BusSet(IEnumerable<Bus> initial) : base(initial) { }
        public BusSet(params Bus[] initial) : base(initial) { }
        protected BusSet(IList<Bus> innerList) : base(innerList) { }
        public BusSet(int initialCount) : base(initialCount) { }

        public static BusSet ConvertFrom(Set<Bus> set)
        {
            BusSet ss = set as BusSet;
            if(ss == null)
                return new BusSet((IList<Bus>)set);
            else
                return ss;
        }

        #region Factory Methods
        protected override Set<Bus> CreateNewSet()
        {
            return new BusSet();
        }
        protected override ReadOnlySet<Bus> CreateNewReadOnlyWrapper(IList<Bus> list)
        {
            return new ReadOnlyBusSet(list);
        }
        #endregion

        public new ReadOnlyBusSet AsReadOnly
        {   // works thanks to the Factory Method pattern/trick
            get { return (ReadOnlyBusSet)base.AsReadOnly; }
        }

        #region InstanceId Conversion
        public InstanceIdSet ConvertAllToInstanceIds()
        {
            InstanceIdSet ids = new InstanceIdSet();
            foreach(Bus b in this)
                ids.Add(b.InstanceId);
            return ids;
        }
        public InstanceIdSet ConvertAllToInstanceIds(Converter<Bus, Guid> convert)
        {
            InstanceIdSet ids = new InstanceIdSet();
            foreach(Bus b in this)
                ids.Add(convert(b));
            return ids;
        }
        public static BusSet ConvertAllFromInstanceIds(InstanceIdSet idSet, Converter<Guid, Bus> convert)
        {
            BusSet bs = new BusSet();
            foreach(Guid id in idSet)
                bs.Add(convert(id));
            return bs;
        }
        #endregion
    }

    public class ReadOnlyBusSet : ReadOnlySet<Bus>
    {
        public ReadOnlyBusSet(IList<Bus> list) : base(list) { }

        #region Factory Methods
        protected override Set<Bus> CreateNewSet()
        {
            return new BusSet();
        }
        #endregion
    }
}
