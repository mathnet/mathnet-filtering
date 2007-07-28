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
    /// <summary>
    /// Represents an Yttrium Signal. Signals are the core elements; yttrium is
    /// all about signals, their values and their relations to other signals.
    /// </summary>
    public abstract class Signal : ValueNode, IEquatable<Signal>
    {
        protected Signal()
            : base()
        {
        }

        protected Signal(IValueStructure initialValue)
            : base(initialValue)
        {
        }

        #region Event Aspects
        public static readonly NodeEvent SignalInputTreeChangedEvent
            = NodeEvent.Register(new MathIdentifier("SignalInputTreeChanged", "Core"),
                                 typeof(EventHandler<SignalEventArgs>),
                                 typeof(Signal));
        public event EventHandler<SignalEventArgs> SignalInputTreeChanged
        {
            add { AddHandler(SignalInputTreeChangedEvent, value); }
            remove { RemoveHandler(SignalInputTreeChangedEvent, value); }
        }
        protected virtual void OnInputTreeChanged()
        {
            RaiseEvent(SignalInputTreeChangedEvent, new SignalEventArgs(this));
        }
        #endregion

        #region Drive State
        public abstract bool IsDriven
        {
            get;
        }

        public abstract bool Hold
        {
            get;
            set;
        }

        public abstract bool IsSourceSignal
        {
            get;
            set;
        }

        /// <summary>
        /// True if this signal is driven by a port and, if <paramref name="ignoreHold"/> is false, the signal is not being hold.
        /// </summary>
        /// <param name="ignoreHold">if true, this method returns the same value as the <see cref="IsDriven"/> property.</param>
        public bool BehavesAsBeingDriven(bool ignoreHold)
        {
            return IsDriven && (ignoreHold || !Hold);
        }

        /// <summary>
        /// True if this signal is either a source signal or just behaves so because of being hold.
        /// </summary>
        public bool BehavesAsSourceSignal
        {
            get { return IsSourceSignal || Hold; }
        }
        #endregion

        #region Graph Structure
        public abstract Port DrivenByPort
        {
            get;
        }

        /// <summary>
        /// True if the signal is driven by a port and this port has the specified entity.
        /// </summary>
        public bool IsDrivenByPortEntity(MathIdentifier entityId)
        {
            if(!IsDriven)
                return false;
            return DrivenByPort.Entity.EntityId.Equals(entityId);
        }

        /// <summary>
        /// True if the signal is driven by a port and this port has the specified entity.
        /// </summary>
        public bool IsDrivenByPortEntity(string entityLabel, string entityDomain)
        {
            if(!IsDriven)
                return false;
            return DrivenByPort.Entity.EntityId.Equals(entityLabel, entityDomain);
        }

        public abstract bool IsCyclic
        {
            get;
        }

        public abstract int Cycles
        {
            get;
        }

        public abstract bool DependsOn(Signal signal);
        public abstract bool DependsOn(Port port);
        public abstract bool DependsOn(MathIdentifier portEntity);
        #endregion

        public abstract bool HasProperty(MathIdentifier propertyId);

        public abstract bool AskForProperty(MathIdentifier propertyType);

        public bool AskForProperty(string propertyLabel, string propertyDomain)
        {
            return AskForProperty(new MathIdentifier(propertyLabel, propertyDomain));
        }

        public abstract void AddConstraint(IProperty property);

        public abstract void RemoveAllConstraints();

        #region Operators: Builder Shortcuts

        protected abstract Signal AddSignalCore(Signal summand);
        protected abstract Signal NegateSignalCore();
        protected abstract Signal SubtractSignalCore(Signal subtrahend);
        protected abstract Signal MultiplySignalCore(Signal multiplier);
        protected abstract Signal DivideSignalCore(Signal divisor);

        /// <summary>
        /// Shortcut for the binary addition operation.
        /// </summary>
        public static Signal operator +(Signal summand1, Signal summand2)
        {
            if(summand1 == null) throw new ArgumentNullException("summand1");
            return summand1.AddSignalCore(summand2);
        }
        /// <summary>
        /// Unary add, just returns the signal (does nothing).
        /// </summary>
        public static Signal operator +(Signal summand)
        {
            return summand;
        }

        /// <summary>
        /// Shortcut for the binary subtraction operation.
        /// </summary>
        public static Signal operator -(Signal minuend, Signal subtrahend)
        {
            if(minuend == null) throw new ArgumentNullException("minuend");
            return minuend.SubtractSignalCore(subtrahend);
        }
        /// <summary>
        /// Shortcut for the unary subtraction operation.
        /// </summary>
        public static Signal operator -(Signal subtrahend)
        {
            if(subtrahend == null) throw new ArgumentNullException("subtrahend");
            return subtrahend.NegateSignalCore();
        }

        /// <summary>
        /// Shortcut for the binary multiplication operation.
        /// </summary>
        public static Signal operator *(Signal multiplicand, Signal multiplier)
        {
            if(multiplicand == null) throw new ArgumentNullException("multiplicand");
            return multiplicand.MultiplySignalCore(multiplier);
        }

        /// <summary>
        /// Shortcut for the binary division operation.
        /// </summary>
        public static Signal operator /(Signal dividend, Signal divisor)
        {
            if(dividend == null) throw new ArgumentNullException("dividend");
            return dividend.DivideSignalCore(divisor);
        }
        #endregion

        public bool Equals(Signal other)
        {
            return base.Equals((Node)other);
        }
    }
}
