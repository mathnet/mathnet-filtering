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
using MathNet.Numerics;

namespace MathNet.Symbolics.Backend.Containers
{
    public class InstanceIdSet : Set<Guid>
    {
        public InstanceIdSet() : base() { }
        public InstanceIdSet(IEnumerable<Guid> initial) : base(initial) { }
        public InstanceIdSet(params Guid[] initial) : base(initial) { }
        protected InstanceIdSet(IList<Guid> innerList) : base(innerList) { }
        public InstanceIdSet(int initialCount) : base(initialCount) { }

        public static InstanceIdSet ConvertFrom(Set<Guid> set)
        {
            InstanceIdSet ss = set as InstanceIdSet;
            if(ss == null)
                return new InstanceIdSet((IList<Guid>)set);
            else
                return ss;
        }

        #region Factory Methods
        protected override Set<Guid> CreateNewSet()
        {
            return new InstanceIdSet();
        }
        protected override ReadOnlySet<Guid> CreateNewReadOnlyWrapper(IList<Guid> list)
        {
            return new ReadOnlyInstanceIdSet(list);
        }
        #endregion

        public new ReadOnlyInstanceIdSet AsReadOnly
        {   // works thanks to the Factory Method pattern/trick
            get { return (ReadOnlyInstanceIdSet)base.AsReadOnly; }
        }
    }

    public class ReadOnlyInstanceIdSet : ReadOnlySet<Guid>
    {
        public ReadOnlyInstanceIdSet(IList<Guid> list) : base(list) { }

        #region Factory Methods
        protected override Set<Guid> CreateNewSet()
        {
            return new InstanceIdSet();
        }
        #endregion
    }
}
