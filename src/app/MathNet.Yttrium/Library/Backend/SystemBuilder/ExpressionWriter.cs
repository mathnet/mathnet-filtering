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
using MathNet.Symbolics.Backend.Properties;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Workplace;

namespace MathNet.Symbolics.Backend.SystemBuilder
{
    /// <summary>
    /// Concrete System Builder for constructing an (incomplete) expression
    /// strings compatible with the parser infrastructure.
    /// </summary>
    public class ExpressionWriter : ISystemBuilder
    {
        /*
         * Works, but is incomplete (TODO)
         */ 

        private Queue<string> _writtenExpressions;
        private Dictionary<Guid, string> _signalMappings;
        private Dictionary<Guid, string> _busMappings;
        private StringBuilder _current;

        public ExpressionWriter()
        {
            _writtenExpressions = new Queue<string>();
            _signalMappings = new Dictionary<Guid, string>();
            _busMappings = new Dictionary<Guid, string>();
        }

        public Queue<string> WrittenExpressions
        {
            get { return _writtenExpressions; }
        }

        public void BeginBuildSystem(int inputSignalCount, int outputSignalCount, int busCount)
        {
            _current = new StringBuilder();
        }

        public Guid BuildSignal(string label, bool hold, bool isSource)
        {
            Guid guid = Guid.NewGuid();
            if(string.IsNullOrEmpty(label))
                label = "S" + guid.ToString("N");
            _signalMappings.Add(guid, label);
            _current.AppendFormat("signal {0};", label);
            _current.AppendLine();
            return guid;
        }

        public Guid BuildBus(string label)
        {
            Guid guid = Guid.NewGuid();
            if(string.IsNullOrEmpty(label))
                label = "S" + guid.ToString("N");
            _busMappings.Add(guid, label);
            _current.AppendFormat("bus {0};", label);
            _current.AppendLine();
            return guid;
        }

        public Guid BuildPort(MathIdentifier entityId, InstanceIdSet inputSignals, InstanceIdSet outputSignals, InstanceIdSet buses)
        {
            Guid guid = Guid.NewGuid();
            _current.AppendFormat("instanciate {0}", entityId.ToString());
            if(inputSignals.Count > 0)
            {
                _current.Append(" in ");
                for(int i = 0; i < inputSignals.Count; i++)
                {
                    if(i > 0) _current.Append(',');
                    _current.Append(_signalMappings[inputSignals[i]]);
                }
            }
            if(outputSignals.Count > 0)
            {
                _current.Append(" out ");
                for(int i = 0; i < outputSignals.Count; i++)
                {
                    if(i > 0) _current.Append(',');
                    _current.Append(_signalMappings[outputSignals[i]]);
                }
            }
            if(buses.Count > 0)
            {
                _current.Append(" bus ");
                for(int i = 0; i < buses.Count; i++)
                {
                    if(i > 0) _current.Append(',');
                    _current.Append(_busMappings[buses[i]]);
                }
            }
            _current.AppendLine(";");
            return guid;
        }

        public void AppendSignalValue(Guid iid, StructurePack value)
        {
            //_current.AppendFormat("{0} <- {1}", _signalMappings[iid], ??);
        }

        public void AppendSignalProperty(Guid iid, PropertyPack property)
        {
            //_current.AppendFormat("assume {0} is {1}", _signalMappings[iid], ??);
        }

        public void AppendSignalConstraint(Guid iid, PropertyPack constraint)
        {
            //_current.AppendFormat("assume {0} is {1}", _signalMappings[iid], ??);
        }

        public void AppendSystemInputSignal(Guid iid)
        {
        }

        public void AppendSystemOutputSignal(Guid iid)
        {
        }

        public void AppendSystemNamedSignal(Guid iid, string name)
        {
        }

        public void AppendSystemNamedBus(Guid iid, string name)
        {
        }

        public void EndBuildSystem()
        {
            _writtenExpressions.Enqueue(_current.ToString());
            _current = null;
        }
    }
}
