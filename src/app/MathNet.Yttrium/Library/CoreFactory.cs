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

using MathNet.Symbolics.Core;

namespace MathNet.Symbolics
{
    internal class CoreFactory :
        IFactory<Signal>,
        IFactory<Signal, IValueStructure>,
        IFactory<Bus>,
        IFactory<Bus, IValueStructure>,
        IFactory<Port, IEntity>,
        IFactory<Port, IEntity, IEnumerable<Signal>>,
        IFactory<IMathSystem>
    {
        private CoreFactory() { }

        // Signal
        Signal IFactory<Signal>.GetInstance()
        {
            return new MathNet.Symbolics.Core.Signal();
        }
        Signal IFactory<Signal, IValueStructure>.GetInstance(IValueStructure value)
        {
            return new MathNet.Symbolics.Core.Signal(value);
        }

        // Bus
        Bus IFactory<Bus>.GetInstance()
        {
            return new MathNet.Symbolics.Core.Bus();
        }
        Bus IFactory<Bus, IValueStructure>.GetInstance(IValueStructure value)
        {
            return new MathNet.Symbolics.Core.Bus(value);
        }

        // Port
        Port IFactory<Port>.GetInstance()
        {
            throw new NotSupportedException("expected parameters: IEntity");
        }
        Port IFactory<Port, IEntity>.GetInstance(IEntity entity)
        {
            return new MathNet.Symbolics.Core.Port(entity);
        }
        Port IFactory<Port, IEntity, IEnumerable<Signal>>.GetInstance(IEntity entity, IEnumerable<Signal> outputSignals)
        {
            return new MathNet.Symbolics.Core.Port(entity, outputSignals);
        }

        // MathSystem
        IMathSystem IFactory<IMathSystem>.GetInstance()
        {
            return new MathNet.Symbolics.Workplace.MathSystem();
        }
    }
}
