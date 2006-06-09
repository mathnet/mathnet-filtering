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
    public interface ISignalSet : ISet<Signal>
    {
        void PostNewValues(IEnumerable<ValueStructure> newValues);
        void PostNewValues(IEnumerable<ValueStructure> newValues, TimeSpan delay);
        void PostNewValues(IEnumerable<ValueStructure> newValues, IEnumerable<TimeSpan> delays);
    }

    public class SignalSet : Set<Signal>, ISignalSet
    {
        public SignalSet() : base() { }
        public SignalSet(IEnumerable<Signal> initial) : base(initial) { }
        public SignalSet(params Signal[] initial) : base(initial) { }
        protected SignalSet(IList<Signal> innerList) : base(innerList) { }
        public SignalSet(int initialCount) : base(initialCount) { }

        public static SignalSet ConvertFrom(Set<Signal> set)
        {
            SignalSet ss = set as SignalSet;
            if(ss == null)
                return new SignalSet((IList<Signal>)set);
            else
                return ss;
        }

        #region Factory Methods
        protected override Set<Signal> CreateNewSet()
        {
            return new SignalSet();
        }
        protected override ReadOnlySet<Signal> CreateNewReadOnlyWrapper(IList<Signal> list)
        {
            return new ReadOnlySignalSet(list);
        }
        #endregion

        public new ReadOnlySignalSet AsReadOnly
        {   // works thanks to the Factory Method pattern/trick
            get { return (ReadOnlySignalSet)base.AsReadOnly; }
        }

        #region InstanceId Conversion
        public InstanceIdSet ConvertAllToInstanceIds()
        {
            InstanceIdSet ids = new InstanceIdSet();
            foreach(Signal s in this)
                ids.Add(s.InstanceId);
            return ids;
        }
        public InstanceIdSet ConvertAllToInstanceIds(Converter<Signal,Guid> convert)
        {
            InstanceIdSet ids = new InstanceIdSet();
            foreach(Signal s in this)
                ids.Add(convert(s));
            return ids;
        }
        public static SignalSet ConvertAllFromInstanceIds(InstanceIdSet idSet, Converter<Guid, Signal> convert)
        {
            SignalSet ss = new SignalSet();
            foreach(Guid id in idSet)
                ss.Add(convert(id));
            return ss;
        }
        #endregion

        public void PostNewValues(IEnumerable<ValueStructure> newValues)
        {
            int idx = 0;
            foreach(ValueStructure value in newValues)
                base[idx++].PostNewValue(value);
        }

        public void PostNewValues(IEnumerable<ValueStructure> newValues, TimeSpan delay)
        {
            int idx = 0;
            foreach(ValueStructure value in newValues)
                base[idx++].PostNewValue(value,delay);
        }

        public void PostNewValues(IEnumerable<ValueStructure> newValues, IEnumerable<TimeSpan> delays)
        {
            int idx = 0;
            IEnumerator<TimeSpan> delayEnumerator = delays.GetEnumerator();
            foreach(ValueStructure value in newValues)
            {
                delayEnumerator.MoveNext();
                base[idx++].PostNewValue(value, delayEnumerator.Current);
            }
        }
    }

    public class ReadOnlySignalSet : ReadOnlySet<Signal>, ISignalSet
    {
        public ReadOnlySignalSet(IList<Signal> list) : base(list) { }

        #region Factory Methods
        protected override Set<Signal> CreateNewSet()
        {
            return new SignalSet();
        }
        #endregion

        public void PostNewValues(IEnumerable<ValueStructure> newValues)
        {
            int idx = 0;
            foreach(ValueStructure value in newValues)
                base[idx++].PostNewValue(value);
        }

        public void PostNewValues(IEnumerable<ValueStructure> newValues, TimeSpan delay)
        {
            int idx = 0;
            foreach(ValueStructure value in newValues)
                base[idx++].PostNewValue(value, delay);
        }

        public void PostNewValues(IEnumerable<ValueStructure> newValues, IEnumerable<TimeSpan> delays)
        {
            int idx = 0;
            IEnumerator<TimeSpan> delayEnumerator = delays.GetEnumerator();
            foreach(ValueStructure value in newValues)
            {
                delayEnumerator.MoveNext();
                base[idx++].PostNewValue(value, delayEnumerator.Current);
            }
        }
    }
}
