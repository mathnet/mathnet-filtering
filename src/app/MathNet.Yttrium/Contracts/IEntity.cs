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

namespace MathNet.Symbolics
{
    public enum InfixNotation : int
    {
        None = 0,
        LeftAssociativeInnerOperator = 1,
        RightAssociativeInnerOperator = 2,
        PreOperator = 3,
        PostOperator = 4
    }

    public interface IEntity : IEquatable<IEntity>
    {
        MathIdentifier EntityId { get;}
        bool EqualsById(IEntity other);
        bool EqualsById(MathIdentifier otherEntityId);

        string Symbol { get; }
        InfixNotation Notation { get; }
        int PrecedenceGroup { get; }
        bool IsGeneric { get; }

        string[] InputSignals { get; }
        string[] OutputSignals { get; }
        string[] Buses { get; }

        IEntity CompileGenericEntity(int inputSignalsCount, int busesCount, int? outputSignalsCount);

        Port InstantiatePort(IList<Signal> inputSignals, IList<Signal> outputSignals);
        Port InstantiatePort(IList<Signal> inputSignals, IList<Signal> outputSignals, IList<Bus> buses);
        Port InstantiatePort(params Signal[] inputSignals);
        Port InstantiatePort(IList<Signal> inputSignals);
        Port InstantiateUnboundPort();
    }
}
