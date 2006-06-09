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
using System.Text;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Theorems;

namespace MathNet.Symbolics.Backend.Containers
{
    [Serializable]
    public class IdentifierDictionary<T> : Dictionary<string, Dictionary<string, T>>, IEnumerable<T>
    {
        private readonly int _labelCapacity;

        public IdentifierDictionary(int domainCapacity, int labelCapacity) : base(domainCapacity, Context.IdentifierComparer)
        {
            _labelCapacity = labelCapacity;
        }

        public bool ContainsId(MathIdentifier id)
        {
            Dictionary<string, T> inner;
            if(!base.TryGetValue(id.Domain, out inner))
                return false;
            else
                return inner.ContainsKey(id.Label);
        }

        public T GetValue(MathIdentifier id)
        {
            T value;
            if(TryGetValue(id, out value))
                return value;
            else
                throw new MathNet.Symbolics.Backend.Exceptions.NotFoundException();
        }
        public bool TryGetValue(MathIdentifier id, out T value)
        {
            Dictionary<string, T> inner;
            if(base.TryGetValue(id.Domain, out inner))
                return inner.TryGetValue(id.Label, out value);
            value = default(T);
            return false;
        }

        public bool TryGetValue<TSub>(MathIdentifier id, out TSub value) where TSub : T
        {
            Dictionary<string, T> inner;
            T tmp;
            if(base.TryGetValue(id.Domain, out inner) && inner.TryGetValue(id.Label, out tmp))
            {
                if(tmp is TSub)
                {
                    value = (TSub)tmp;
                    return true;
                }
            }
            value = default(TSub);
            return false;
        }

        public T FindValue(Predicate<T> match)
        {
            T value;
            if(TryFindValue(match, out value))
                return value;
            else
                throw new MathNet.Symbolics.Backend.Exceptions.NotFoundException();
        }
        public bool TryFindValue(Predicate<T> match, out T value)
        {
            foreach(T item in SelectAll())
                if(match(item))
                {
                    value = item;
                    return true;
                }
            value = default(T);
            return false;
        }

        public string FindDomainOfLabel(string label)
        {
            string domain;
            if(TryFindDomainOfLabel(label, out domain))
                return domain;
            else
                throw new MathNet.Symbolics.Backend.Exceptions.NotFoundException();
        }
        public bool TryFindDomainOfLabel(string label, out string domain)
        {
            foreach(KeyValuePair<string, Dictionary<string, T>> inner in this)
                if(inner.Value.ContainsKey(label))
                {
                    domain = inner.Key;
                    return true;
                }
            domain = null;
            return false;
        }

        public IEnumerable<T> SelectAll()
        {
            foreach(KeyValuePair<string, Dictionary<string, T>> inner in this)
                foreach(KeyValuePair<string, T> value in inner.Value)
                    yield return value.Value;
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return SelectAll().GetEnumerator();
        }

        public virtual void Add(MathIdentifier id, T value)
        {
            Dictionary<string, T> inner;
            if(!base.TryGetValue(id.Domain, out inner))
            {
                inner = new Dictionary<string, T>(_labelCapacity, Context.IdentifierComparer);
                Add(id.Domain, inner);
            }
            inner.Add(id.Label, value);
        }

        public virtual void Remove(MathIdentifier id)
        {
            Dictionary<string, T> inner;
            if(base.TryGetValue(id.Domain, out inner))
                inner.Remove(id.Label);
        }
    }
}
