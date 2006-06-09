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

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.SystemBuilder;

namespace MathNet.Symbolics.Core
{
    public class Bus : IEquatable<Bus>
    {
        private readonly Guid _iid;
        private readonly Context _context;
        private string _label = string.Empty;

        internal Bus(Context context)
        {
            _context = context;
            _iid = _context.GenerateInstanceId();
            context.NotifyNewBusConstructed(this);
        }

        public Guid InstanceId
        {
            get { return _iid; }
        }

        public Context Context
        {
            get { return _context; }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        #region System Builder
        internal Guid AcceptSystemBuilderBefore(ISystemBuilder builder)
        {
            return builder.BuildBus(_label);
        }
        internal void AcceptSystemBuilderAfter(ISystemBuilder builder, Dictionary<Guid, Guid> signalMappings, Dictionary<Guid, Guid> busMappings)
        {
        }
        #endregion

        #region Instance Equality
        /// <remarks>Two buses are equal only if they are the same instance.</remarks>
        public bool Equals(Bus other)
        {
            return other != null && _iid.Equals(other._iid);
        }
        /// <remarks>Two buses are equal only if they are the same instance.</remarks>
        public override bool Equals(object obj)
        {
            Bus other = obj as Bus;
            if(other == null)
                return false;
            else
                return _iid.Equals(other._iid);
        }
        public override int GetHashCode()
        {
            return _iid.GetHashCode();
        }
        #endregion
    }
}