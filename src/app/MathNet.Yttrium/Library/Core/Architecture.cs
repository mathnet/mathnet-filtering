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

using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Core
{
    public abstract class Architecture
    {
        private MathIdentifier _id;
        private bool _isInstance; // = false;
        private bool _isMathematicalOperator;
        private MathIdentifier _entityId;
        private Port _port; // = null;

        protected Architecture(MathIdentifier id, MathIdentifier entityId, bool isMathematicalOperator)
        {
            _id = id;
            _entityId = entityId;
            _isMathematicalOperator = isMathematicalOperator;
        }

        public bool IsInstance
        {
            get { return _isInstance; }
        }

        public bool IsMathematicalOperator
        {
            get { return _isMathematicalOperator; }
        }

        public MathIdentifier Identifier
        {
            get { return _id; }
        }

        public MathIdentifier EntityId
        {
            get { return _entityId; }
        }

        public Port Port
        {
            get { return _port; }
        }

        protected void SetPort(Port port)
        {
            if(port == null) throw new ArgumentNullException("port");
            _isInstance = true;
            _port = port;
        }

        public virtual bool SupportsPort(Port port)
        {
            if(port == null) throw new ArgumentNullException("port");
            return port.Entity.EntityId.Equals(_entityId);
        }

        public bool RebindToPortIfSupported(Port newPort)
        {
            if(newPort == null) throw new ArgumentNullException("newPort");
            if(newPort.Equals(_port))
                return true;
            if(SupportsPort(newPort))
            {
                Port oldPort = _port;
                UnregisterArchitecture();
                _port = newPort;
                ReregisterArchitecture(oldPort, newPort);
                return true;
            }
            return false;
        }

        protected abstract void ReregisterArchitecture(Port oldPort, Port newPort);

        public abstract void UnregisterArchitecture();

        public virtual ISignalSet ExecuteMathematicalOperator()
        {
            return _port.OutputSignals;
        }

        /*internal Signal AutomaticSimplifyOutput(Signal signal)
        {
            for(int i = 0; i < port.OutputSignalCount; i++)
            {
                if(port[i] == signal)
                    return AutomaticSimplifyOutput(i);
            }
            return signal;
        }

        protected virtual Signal AutomaticSimplifyOutput(int outputIndex)
        {
            return port[outputIndex];
        }*/

        public override string ToString()
        {
            if(_port == null)
                return _id.ToString();
            else
                return _port.Entity.ToString() + "(" + _id.ToString() + ")";
        }
    }
}