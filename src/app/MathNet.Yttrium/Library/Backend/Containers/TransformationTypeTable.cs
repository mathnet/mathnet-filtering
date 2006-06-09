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
using MathNet.Symbolics.Backend.Theorems;
using MathNet.Symbolics.Backend.Traversing;

namespace MathNet.Symbolics.Backend.Containers
{
    public sealed class TransformationTypeTable : IdentifierDictionary<ITransformationTheorem>
    {
        private readonly MathIdentifier _transformationTypeId;
        private ITransformationTheorem _default;

        public TransformationTypeTable(MathIdentifier transformationTypeId)
            : base(4, 16)
        {
            _transformationTypeId = transformationTypeId;
        }

        public MathIdentifier TransformationTypeId
        {
            get { return _transformationTypeId; }
        }

        public ITransformationTheorem Default
        {
            get { return _default; }
            set { _default = value; }
        }

        public bool ContainsTheorem(ITransformationTheorem theorem)
        {
            return ContainsId(theorem.TheoremId);
        }

        public bool TryLookupTheorem<T>(Port matchingPort, out T theorem) where T : ITransformationTheorem
        {
            ITransformationTheorem value;
            if(TryFindValue(delegate(ITransformationTheorem t) { return (t is T) && t.SupportsPort(matchingPort); }, out value))
            {
                theorem = (T)value;
                return true;
            }
            else
            {
                theorem = default(T);
                return false;
            }
        }

        public bool TryLookupTheorem(Port matchingPort, out ITransformationTheorem theorem)
        {
            return TryFindValue(delegate(ITransformationTheorem t) { return t.SupportsPort(matchingPort); }, out theorem);
        }

        public void Add(ITransformationTheorem theorem)
        {
            Add(theorem.TheoremId, theorem);
        }
        public override void Add(MathIdentifier id, ITransformationTheorem value)
        {
            base.Add(id, value);
            if(_default == null)
                _default = value;
        }

        public void Remove(ITransformationTheorem theorem)
        {
            Remove(theorem.TheoremId);
        }
    }
}
