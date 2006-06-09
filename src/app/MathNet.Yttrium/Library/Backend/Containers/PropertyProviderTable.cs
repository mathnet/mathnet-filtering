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
using MathNet.Symbolics.Backend.Theorems;

namespace MathNet.Symbolics.Backend.Containers
{
    public sealed class PropertyProviderTable : IEnumerable<IPropagationTheorem>
    {
        // TODO: consider a more usable/standardized interface. inherit?

        private readonly MathIdentifier _propertyTypeId;
        private readonly List<IPropagationTheorem> _table;

        public PropertyProviderTable(MathIdentifier propertyTypeId)
        {
            _propertyTypeId = propertyTypeId;
            _table = new List<IPropagationTheorem>(16);
        }

        public MathIdentifier ProvidedPropertyId
        {
            get { return _propertyTypeId; }
        }

        public bool ContainsTheorem(IPropagationTheorem theorem)
        {
            return _table.Contains(theorem);
        }

        public bool WouldBePropagatedTo(Signal target)
        {
            foreach(IPropagationTheorem theorem in _table)
                if(theorem.WouldBePropagatedTo(target))
                    return true;
            return false;
        }

        /// <returns>true if the property is attached to the signal after this call.</returns>
        public bool PropagatePropertyIfApplicable(Signal target)
        {
            if(target.Properties.ContainsProperty(_propertyTypeId))
                return true;
            foreach(IPropagationTheorem theorem in _table)
                if(theorem.PropagatePropertyIfApplicable(target))
                    return true;
            return false;
        }

        /// <summary>Sets or Removes the property conditioned on the available theorems.</summary>
        /// <returns>true if the property is attached to the signal after this call.</returns>
        public bool UpdateProperty(Signal target)
        {
            if(target.Properties.ContainsProperty(_propertyTypeId))
            {
                if(WouldBePropagatedTo(target))
                    return true;
                else
                {
                    target.Properties.RemoveProperty(_propertyTypeId);
                    return false;
                }
            }
            else
                return PropagatePropertyIfApplicable(target);
        }

        public void AddTheorem(IPropagationTheorem theorem)
        {
            _table.Add(theorem);
        }

        #region Enumeration
        IEnumerator<IPropagationTheorem> IEnumerable<IPropagationTheorem>.GetEnumerator()
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
