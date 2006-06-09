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
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.StdPackage.Structures;

namespace MathNet.Symbolics.Backend.Templates
{
    public delegate Process[] GenerateProcesses(Port port);
    public delegate int GenerateInternalSignalCount(Port port);

    public abstract class GenericArchitectureFactory : IArchitectureFactory
    {
        #region Sub Types
        private struct ArchitectureItem
        {
            public ArchitectureItem(MathIdentifier id, bool isMathematicalOperator, Predicate<Port> portSupport, GenerateProcesses processGenerator, GenerateInternalSignalCount internalSignals)
            {
                this.Id = id;
                this.IsMathematicalOperator = isMathematicalOperator;
                this.PortSupport = portSupport;
                this.ProcessGenerator = processGenerator;
                this.InternalSignals = internalSignals;
            }

            public MathIdentifier Id;
            public bool IsMathematicalOperator;
            public Predicate<Port> PortSupport;
            public GenerateProcesses ProcessGenerator;
            public GenerateInternalSignalCount InternalSignals;
        }
        #endregion

        private List<ArchitectureItem> _items;
        private MathIdentifier _entityId;

        protected GenericArchitectureFactory(MathIdentifier entityId)
        {
            _entityId = entityId;
            _items = new List<ArchitectureItem>(4);
        }

        protected void AddArchitecture(MathIdentifier id, Predicate<Port> portSupport, GenerateProcesses processGenerator)
        {
            _items.Add(new ArchitectureItem(id, false, portSupport, processGenerator, delegate(Port port) { return 0; }));
        }
        protected void AddArchitecture(MathIdentifier id, Predicate<Port> portSupport, GenerateProcesses processGenerator, GenerateInternalSignalCount internalSignals)
        {
            _items.Add(new ArchitectureItem(id, false, portSupport, processGenerator, internalSignals));
        }
        protected void AddArchitecture(MathIdentifier id, bool isMathematicalOperator, Predicate<Port> portSupport, GenerateProcesses processGenerator, GenerateInternalSignalCount internalSignals)
        {
            _items.Add(new ArchitectureItem(id, isMathematicalOperator, portSupport, processGenerator, internalSignals));
        }

        public MathIdentifier EntityId
        {
            get { return _entityId; }
        }

        public bool SupportsPort(Port port)
        {
            if(!port.Entity.EntityId.Equals(_entityId))
                return false;

            foreach(ArchitectureItem item in _items)
            {
                if(item.PortSupport(port))
                    return true;
            }

            return false;
        }

        public Architecture InstantiateToPort(Port port)
        {
            foreach(ArchitectureItem item in _items)
            {
                if(item.PortSupport(port))
                    return new GenericArchitecture(item.Id, _entityId, item.IsMathematicalOperator,port,item.PortSupport,item.InternalSignals(port),item.ProcessGenerator(port));
            }
            throw new MathNet.Symbolics.Backend.Exceptions.ArchitectureNotAvailableException(port);
        }
    }
}
