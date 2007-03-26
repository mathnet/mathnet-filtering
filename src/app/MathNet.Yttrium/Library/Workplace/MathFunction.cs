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
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Workplace
{
    public class MathFunction
    {
        private MathSystem system;
        private IEntity entity;

        public MathFunction(IEntity entity, MathSystem system)
        {
            this.entity = entity;
            this.system = system;
        }
        public MathFunction(IEntity entity)
        {
            this.entity = entity;
        }
        public MathFunction(MathSystem system)
        {
            this.system = system;
        }
        public MathFunction(Signal outputSignal, params Signal[] inputSignals)
        {
            this.system = new MathSystem();
            system.AddSignalTree(outputSignal, inputSignals, true, true);
        }

        public MathSystem System
        {
            get { return system; }
        }
        public bool HasSystem
        {
            get { return system != null; }
        }

        public IEntity Entity
        {
            get { return entity; }
        }
        public bool HasEntity
        {
            get { return entity != null; }
        }

        public ReadOnlySignalSet Apply(params Signal[] arguments)
        {
            BuildEntity();
            return Service<IBuilder>.Instance.Functions(entity, arguments);
        }

        public IValueStructure[] Evaluate(params IValueStructure[] inputs)
        {
            BuildSystem();
            return system.Evaluate(inputs);
        }

        public double[] Evaluate(params double[] inputs)
        {
            BuildSystem();
            return system.Evaluate(inputs);
        }

        public IEntity BuildEntity()
        {
            if(!HasEntity)
                entity = system.PublishToLibraryAnonymous();
            return entity;
        }

        public MathSystem BuildSystem()
        {
            if(!HasSystem)
            {
                IEntity localEntity = entity;
                if(entity.IsGeneric)
                    localEntity = entity.CompileGenericEntity(1,0,null);
                Signal[] inputs = new Signal[localEntity.InputSignals.Length];
                ReadOnlySignalSet outputs = Service<IBuilder>.Instance.Functions(localEntity, inputs);
                system = new MathSystem();
                system.AddSignalTreeRange(outputs, true, true);
            }
            return system;
        }
    }
}
