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

using MathNet.Symbolics.Events;

namespace MathNet.Symbolics
{
    public abstract class ValueNode : Node
    {
        private IValueStructure _presentStructure; // = null;
        private bool _hasEvent; // = false;

        public event EventHandler<ValueNodeEventArgs> ValueChanged;

        protected ValueNode()
        {
        }

        protected ValueNode(IValueStructure initialValue)
        {
            _presentStructure = initialValue;
        }

        /// <summary>
        /// The present value of this node.
        /// </summary>
        public IValueStructure Value
        {
            get { return _presentStructure; }
        }

        public MathIdentifier ValueTypeId
        {
            get { return _presentStructure.TypeId; }
        }

        /// <summary>
        /// True only in the scheduler process execution phase, and only if the
        /// there is an event related to this node, that is it's value has changed.
        /// </summary>
        public bool HasEvent
        {
            get { return _hasEvent; }
        }

        protected void SetHasEvent(bool value)
        {
            _hasEvent = value;
        }

        protected void SetPresentValue(IValueStructure value)
        {
            bool different = !(value == null && _presentStructure == null)
               && !(_presentStructure != null && value != null && _presentStructure.Equals(value));
            _presentStructure = value;
            //_properties.ValidatePropertiesAfterEvent(this);
        }

        protected void OnValueChanged()
        {
            //_context.NotifySignalValueChanged(this);
            EventHandler<ValueNodeEventArgs> handler = ValueChanged;
            if(handler != null)
                handler(this, new ValueNodeEventArgs(this));
        }

        public abstract void PostNewValue(IValueStructure newValue);
        public abstract void PostNewValue(IValueStructure newValue, TimeSpan delay);
        
    }
}
