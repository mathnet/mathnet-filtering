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
using System.Xml;
using System.Reflection;
using System.Collections.Generic;

using MathNet.Symbolics.Backend;

namespace MathNet.Symbolics.Core
{
    public enum InfixNotation : int
    {
        None = 0,
        LeftAssociativeInnerOperator = 1,
        RightAssociativeInnerOperator = 2,
        PreOperator = 3,
        PostOperator = 4
    }

    [Serializable]
    public class Entity : IEquatable<Entity>
    {
        private readonly string _symbol;
        private readonly MathIdentifier _id;
        private readonly InfixNotation _notation;
        private readonly int _precedenceGroup;
        private readonly bool _isGeneric; // = false;
        private readonly string[] _inputSignalLabels;
        private readonly string[] _outputSignalLabels;
        private readonly string[] _busLabels;

        public Entity(string symbol, string label, string domain, InfixNotation notation, int precedence, bool isGeneric)
            : this(symbol, new MathIdentifier(label, domain), notation, precedence, isGeneric, new string[0]) { }
        public Entity(string symbol, MathIdentifier id, InfixNotation notation, int precedence, bool isGeneric)
            : this(symbol, id, notation, precedence, isGeneric, new string[0]) { }
        public Entity(string symbol, string label, string domain, InfixNotation notation, int precedence, bool isGeneric, string[] buses)
            : this(symbol, new MathIdentifier(label, domain), notation, precedence, isGeneric, buses) { }
        public Entity(string symbol, MathIdentifier id, InfixNotation notation, int precedence, bool isGeneric, string[] buses)
        {
            _id = id;
            _symbol = symbol;
            _inputSignalLabels = new string[0];
            _outputSignalLabels = new string[0];
            _busLabels = buses;
            _isGeneric = isGeneric;
            _notation = notation;
            _precedenceGroup = precedence;
        }
        public Entity(string symbol, string label, string domain, InfixNotation notation, int precedence, string[] inputSignals, string[] outputSignals)
            : this(symbol, label, domain, notation, precedence, inputSignals, outputSignals, new string[0]) { }
        public Entity(string symbol, MathIdentifier id, InfixNotation notation, int precedence, string[] inputSignals, string[] outputSignals)
            : this(symbol, id, notation, precedence, inputSignals, outputSignals, new string[0]) { }
        public Entity(string symbol, string label, string domain, InfixNotation notation, int precedence, string[] inputSignals, string[] outputSignals, string[] buses)
            : this(symbol, new MathIdentifier(label, domain), notation, precedence, inputSignals, outputSignals, buses) { }
        public Entity(string symbol, MathIdentifier id, InfixNotation notation, int precedence, string[] inputSignals, string[] outputSignals, string[] buses)
        {
            _id = id;
            _symbol = symbol;
            _inputSignalLabels = inputSignals;
            _outputSignalLabels = outputSignals;
            _busLabels = buses;
            _notation = notation;
            _precedenceGroup = precedence;
        }

        public string Symbol
        {
            get { return _symbol; }
        }

        public MathIdentifier EntityId
        {
            get { return _id; }
        }

        public InfixNotation Notation
        {
            get { return _notation; }
        }

        public int PrecedenceGroup
        {
            get { return _precedenceGroup; }
        }

        public bool IsGeneric
        {
            get { return _isGeneric; }
        }

        public string[] InputSignals
        {
            get { return _inputSignalLabels; }
        }

        public string[] OutputSignals
        {
            get { return _outputSignalLabels; }
        }

        public string[] Buses
        {
            get { return _busLabels; }
        }

        public virtual Entity CompileGenericEntity(int inputSignalsCount, int busesCount)
        {
            return this;
        }

        public Port InstantiatePort(Context context, params Signal[] inputSignals)
        {
            return InstantiatePort(context, inputSignals, null, null);
        }
        public Port InstantiatePort(Context context, IList<Signal> inputSignals)
        {
            return InstantiatePort(context, inputSignals, null, null);
        }
        public Port InstantiatePort(Context context, IList<Signal> inputSignals, IEnumerable<Signal> outputSignals)
        {
            return InstantiatePort(context, inputSignals, outputSignals, null);
        }
        public virtual Port InstantiatePort(Context context, IList<Signal> inputSignals, IEnumerable<Signal> outputSignals, IList<Bus> buses)
        {
            Entity entity = this;

            if(_isGeneric)
            {
                if(inputSignals == null)
                    throw new MathNet.Symbolics.Backend.Exceptions.GenericEntityPortNotInstantiableException();
                entity = CompileGenericEntity(inputSignals.Count, buses != null ? buses.Count : 0);
            }

            if(entity.IsGeneric)
                throw new MathNet.Symbolics.Backend.Exceptions.GenericEntityPortNotInstantiableException();

            if((inputSignals != null && inputSignals.Count != entity._inputSignalLabels.Length) || (buses != null && buses.Count != entity._busLabels.Length))
                throw new MathNet.Symbolics.Backend.Exceptions.EntitySignalMismatchException();

            Port port;
            if(outputSignals == null)
                port = new Port(context, entity);
            else
                port = new Port(context, entity, outputSignals);
            if(inputSignals != null)
                port.BindInputSignals(inputSignals);
            if(buses != null && buses.Count > 0)
                port.BindBuses(buses);
            port.EnsureArchitectureLink(); //Try to find an architecture already

            return port;
        }
        public Port InstantiateUnboundPort(Context context)
        {
            return InstantiatePort(context, null, null, new Bus[] { });
        }

        public override int GetHashCode()
        {
            return EntityId.GetHashCode();
        }

        public override string ToString()
        {
            return EntityId.ToString() + ": '" + _symbol + "'";
        }

        public bool Equals(Entity other)
        {
            return other != null && EntityId.Equals(other.EntityId) && _symbol == other._symbol; // && isGeneric == other.isGeneric
            //&& inputSignalLabels.Length == other.inputSignalLabels.Length && outputSignalLabels.Length == other.outputSignalLabels.Length && busLabels.Length == other.busLabels.Length;
        }
        public override bool Equals(object obj)
        {
            Entity entity = obj as Entity;
            return entity != null && Equals(entity);
        }
        public bool EqualsById(Entity other)
        {
            return other != null && EntityId.Equals(other.EntityId);
        }
        public bool EqualsById(MathIdentifier otherEntityId)
        {
            return EntityId.Equals(otherEntityId);
        }
    }
}
