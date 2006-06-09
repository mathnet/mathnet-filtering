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

namespace MathNet.Symbolics.Backend.ValueConversion
{
    public class StructureTable
    {
        private struct StructureType
        {
            public StructureType(MathIdentifier id, ConversionRouter router, Type type)
            {
                this.Type = type;
                this.Identifier = id;
                this.Router = router;
            }
            public Type Type;
            public MathIdentifier Identifier;
            public ConversionRouter Router;
        }

        private IdentifierDictionary<StructureType> _table;

        public StructureTable()
        {
            _table = new IdentifierDictionary<StructureType>(4, 16);
        }

        public void AddStructure(MathIdentifier structureId, ConversionRouter router, Type type)
        {
            if(!_table.ContainsId(structureId))
                _table.Add(structureId, new StructureType(structureId, router, type));
        }

        public ConversionRouter LookupRouter(MathIdentifier structureId)
        {
            return _table.GetValue(structureId).Router;
        }
        public ConversionRouter LookupRouter(ValueStructure value)
        {
            return _table.GetValue(value.StructureId).Router;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach(StructureType item in _table.SelectAll())
                sb.AppendLine(item.Router.ToString());
            return sb.ToString();
        }
    }
}
