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
using MathNet.Symbolics.Backend.Containers;

namespace MathNet.Symbolics.Workplace
{
    public class MathFunction
    {
        private readonly Context context;
        private MathSystem system;
        private Entity entity;

        public MathFunction(Context context, Entity entity, MathSystem system)
        {
            this.context = context;
            this.entity = entity;
            this.system = system;
        }
        public MathFunction(Context context, Entity entity)
        {
            this.context = context;
            this.entity = entity;
        }
        public MathFunction(Context context, MathSystem system)
        {
            this.context = context;
            this.system = system;
        }
        public MathFunction(Context context, Signal outputSignal, params Signal[] inputSignals)
        {
            this.context = context;
            this.system = new MathSystem(context);
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

        public Entity Entity
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
            return context.Builder.Functions(entity, arguments);
        }

        public ValueStructure[] Evaluate(params ValueStructure[] inputs)
        {
            BuildSystem();
            return system.Evaluate(inputs);
        }

        public double[] Evaluate(params double[] inputs)
        {
            BuildSystem();
            return system.Evaluate(inputs);
        }

        public Entity BuildEntity()
        {
            if(!HasEntity)
                entity = system.PublishToLibraryAnonymous();
            return entity;
        }

        public MathSystem BuildSystem()
        {
            if(!HasSystem)
            {
                Entity localEntity = entity;
                if(entity.IsGeneric)
                    localEntity = entity.CompileGenericEntity(1,0);
                Signal[] inputs = new Signal[localEntity.InputSignals.Length];
                ReadOnlySignalSet outputs = context.Builder.Functions(localEntity, inputs);
                system = new MathSystem(context);
                system.AddSignalTreeRange(outputs, true, true);
            }
            return system;
        }
    }
}
