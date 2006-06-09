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

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.ValueConversion;

namespace MathNet.Symbolics.StdPackage.Structures
{
    /// <summary>symbol: complex infinity</summary>
    public sealed class ComplexInfinitySymbol : ValueStructure, IEquatable<ComplexInfinitySymbol>, IComparable<ComplexInfinitySymbol>
    {
        private static readonly MathIdentifier _structureId = new MathIdentifier("ComplexInfinity", "Std");
        private ComplexInfinitySymbol() { }

        #region Conversion & Conversion Routing
        private static readonly ConversionRouter _router = new ConversionRouter(_structureId);
        public static ConversionRouter Converter
        {
            get { return _router; }
        }
        public static void PublishConversionRouteNeighbors(StructureTable table)
        {
            if(table == null) throw new ArgumentNullException("table");
            table.AddStructure(_structureId, _router, typeof(ComplexInfinitySymbol));
            _router.AddSourceNeighbor(NegativeInfinitySymbol.Converter, false, LocalConvertFrom);
            _router.AddSourceNeighbor(PositiveInfinitySymbol.Converter, false, LocalConvertFrom);
        }
        private static ValueStructure LocalConvertFrom(ValueStructure value)
        {
            return _instance;
        }
        public static ComplexInfinitySymbol ConvertFrom(ValueStructure value)
        {
            return _instance;
        }
        #endregion

        #region Singleton
        private static ComplexInfinitySymbol _instance;
        public static ComplexInfinitySymbol Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new ComplexInfinitySymbol();
                return _instance;
            }
        }
        #endregion

        public static MathIdentifier StructureIdentifier
        {
            get { return _structureId; }
        }
        public override MathIdentifier StructureId
        {
            get { return _structureId; }
        }

        public static Signal Constant(Context context)
        {
            MathIdentifier id = _structureId.DerivePostfix("Constant");
            Signal ret;
            if(context.SingletonSignals.TryGetValue(id, out ret))
                return ret;
            else
            {
                ret = new Signal(context, Instance);
                ret.Label = "ComplexInfinity";
                ret.Properties.AddProperty(Properties.ConstantSignalProperty.Instance);
                context.SingletonSignals.Add(id, ret);
                return ret;
            }
        }

        public override bool Equals(ValueStructure other)
        {
            return other is ComplexInfinitySymbol;
        }
        public bool Equals(ComplexInfinitySymbol other)
        {
            return true;
        }
        public int CompareTo(ComplexInfinitySymbol other)
        {
            return 0;
        }

        #region Serialization
        protected override void InnerSerialize(System.Xml.XmlWriter writer)
        {
        }
        private static ComplexInfinitySymbol InnerDeserialize(Context context, System.Xml.XmlReader reader)
        {
            return ComplexInfinitySymbol._instance;
        }
        #endregion
    }
}
