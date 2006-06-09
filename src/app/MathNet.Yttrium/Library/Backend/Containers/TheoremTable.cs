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
using System.Collections.ObjectModel;

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Theorems;

namespace MathNet.Symbolics.Backend.Containers
{
    public sealed class TheoremTable
    {
        // TODO: consider a more usable/standardized interface. inherit?

        private readonly IdentifierDictionary<ITheorem> _theorems;
        private readonly IdentifierDictionary<TransformationTypeTable> _transformations;
        private readonly IdentifierDictionary<PropertyProviderTable> _properties;

        public TheoremTable()
        {
            _theorems = new IdentifierDictionary<ITheorem>(4, 32);
            _transformations = new IdentifierDictionary<TransformationTypeTable>(4, 16);
            _properties = new IdentifierDictionary<PropertyProviderTable>(4, 8);
        }

        public bool ContainsTheorem(MathIdentifier theoremId)
        {
            return _theorems.ContainsId(theoremId);
        }

        public bool ContainsTransformationType(MathIdentifier transformationType)
        {
            return _transformations.ContainsId(transformationType);
        }

        public bool ContainsPropertyProvider(MathIdentifier propertyType)
        {
            return _properties.ContainsId(propertyType);
        }


        public ITheorem LookupTheorem(MathIdentifier theoremId)
        {
            return _theorems.GetValue(theoremId);
        }

        public TransformationTypeTable LookupTransformationType(MathIdentifier transformationType)
        {
            return _transformations.GetValue(transformationType);
        }

        public PropertyProviderTable LookupPropertyProvider(MathIdentifier propertyType)
        {
            return _properties.GetValue(propertyType);
        }


        public bool TryLookupTheorem(MathIdentifier theoremId, out ITheorem value)
        {
            return _theorems.TryGetValue(theoremId, out value);
        }

        public bool TryLookupTransformationType(MathIdentifier transformationType, out TransformationTypeTable value)
        {
            return _transformations.TryGetValue(transformationType, out value);
        }

        public bool TryLookupPropertyProvider(MathIdentifier propertyType, out PropertyProviderTable value)
        {
            return _properties.TryGetValue(propertyType, out value);
        }

        public bool TryLookupTheorem<T>(MathIdentifier theoremId, out T value) where T : ITheorem
        {
            ITheorem t;
            if(_theorems.TryGetValue(theoremId, out t))
            {
                if(t is T)
                {
                    value = (T)t;
                    return true;
                }
            }
            value = default(T);
            return false;
        }

        private TransformationTypeTable AddTransformationType(MathIdentifier transformationType)
        {
            TransformationTypeTable value;
            if(!_transformations.TryGetValue(transformationType, out value))
            {
                value = new TransformationTypeTable(transformationType);
                _transformations.Add(transformationType, value);
            }
            return value;
        }

        private PropertyProviderTable AddPropertyProvider(MathIdentifier propertyType)
        {
            PropertyProviderTable value;
            if(!_properties.TryGetValue(propertyType, out value))
            {
                value = new PropertyProviderTable(propertyType);
                _properties.Add(propertyType, value);
            }
            return value;
        }

        public void AddTheorem(ITheorem theorem)
        {
            _theorems.Add(theorem.TheoremId, theorem);
            ITransformationTheorem transTheorem = theorem as ITransformationTheorem;
            if(transTheorem != null)
            {
                TransformationTypeTable table = AddTransformationType(transTheorem.TransformationTypeId);
                table.Add(transTheorem);
            }
            IPropagationTheorem propTheorem = theorem as IPropagationTheorem;
            if(propTheorem != null)
            {
                PropertyProviderTable table = AddPropertyProvider(propTheorem.PropertyTypeId);
                table.AddTheorem(propTheorem);
            }
        }
        public void AddTheorem(IEnumerable<ITheorem> theorems)
        {
            foreach(ITheorem theorem in theorems)
                AddTheorem(theorem);
        }

        public void AddTransformationTheorem(ITransformationTheorem theorem)
        {
            _theorems.Add(theorem.TheoremId, theorem);
            AddTransformationType(theorem.TransformationTypeId).Add(theorem);
        }
        public void AddTransformationTheorem(IEnumerable<ITransformationTheorem> theorems)
        {
            foreach(ITransformationTheorem theorem in theorems)
                AddTransformationTheorem(theorem);
        }

        public void AddPropagationTheorem(IPropagationTheorem theorem)
        {
            _theorems.Add(theorem.TheoremId, theorem);
            AddPropertyProvider(theorem.PropertyTypeId).AddTheorem(theorem);
        }
        public void AddTransformationTheorem(IEnumerable<IPropagationTheorem> theorems)
        {
            foreach(IPropagationTheorem theorem in theorems)
                AddPropagationTheorem(theorem);
        }
    }
}
