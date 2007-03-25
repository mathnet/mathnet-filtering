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
using System.Reflection;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Backend.Properties
{
    // TODO: Redesign (inherit?)

    public class PropertyBag : IEnumerable<IProperty>
    {
        private readonly List<IProperty> _table;
        private readonly IdentifierDictionary<IProperty> _properties;

        public PropertyBag()
        {
            _table = new List<IProperty>(8);
            _properties = new IdentifierDictionary<IProperty>(4, 8);
        }

        public int Count
        {
            get { return _table.Count; }
        }

        public IProperty this[int index]
        {
            get { return _table[index]; }
        }

        public bool ContainsProperty(MathIdentifier propertyId)
        {
            return _properties.ContainsKey(propertyId);
        }

        public IProperty LookupProperty(MathIdentifier propertyId)
        {
            return _properties.GetValue(propertyId);
        }

        public bool TryLookupProperty(MathIdentifier propertyId, out IProperty value)
        {
            return _properties.TryGetValue(propertyId, out value);
        }
        
        public bool TryLookupProperty<T>(MathIdentifier propertyId, out T value) where T : IProperty
        {
            return _properties.TryGetValue<T>(propertyId, out value);
        }

        public void AddProperty(IProperty property)
        {
            _properties.Add(property.TypeId, property);
            _table.Add(property);
        }
        public void AddPropertyRange(IEnumerable<IProperty> properties)
        {
            foreach(IProperty property in properties)
                AddProperty(property);
        }

        public void RemoveProperty(IProperty property)
        {
            _properties.Remove(property.TypeId);
            _table.Remove(property);
        }
        public void RemoveProperty(MathIdentifier propertyId)
        {
            IProperty property = _properties.GetValue(propertyId);
            _properties.Remove(property.TypeId);
            _table.Remove(property);
        }

        public void RemoveAllProperties()
        {
            _table.Clear();
            _properties.Clear();
        }

        public void ValidatePropertiesAfterEvent(Signal signal)
        {
            for(int i = _table.Count - 1; i >= 0; i--)
            {
                IProperty property = _table[i];
                if(!property.StillValidAfterEvent(signal))
                    RemoveProperty(property);
            }
        }

        public void ValidatePropertiesAfterDrive(Signal signal)
        {
            for(int i = _table.Count - 1; i >= 0; i--)
            {
                IProperty property = _table[i];
                if(!property.StillValidAfterDrive(signal))
                    RemoveProperty(property);
            }
        }

        public void ValidatePropertiesAfterUndrive(Signal signal)
        {
            for(int i = _table.Count - 1; i >= 0; i--)
            {
                IProperty property = _table[i];
                if(!property.StillValidAfterUndrive(signal))
                    RemoveProperty(property);
            }
        }

        #region Enumeration
        public IEnumerator<IProperty> GetEnumerator()
        {
            return _table.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _table.GetEnumerator();
        }
        #endregion
    }
}
