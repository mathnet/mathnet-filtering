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
    /// <summary>symbol: positive real infinity</summary>
    public sealed class PositiveInfinitySymbol : ValueStructure, IEquatable<PositiveInfinitySymbol>, IComparable<PositiveInfinitySymbol>
    {
        private static readonly MathIdentifier _structureId = new MathIdentifier("PositiveInfinity", "Std");
        private PositiveInfinitySymbol() { }

        #region Conversion & Conversion Routing
        private static readonly ConversionRouter _router = new ConversionRouter(_structureId);
        public static ConversionRouter Converter
        {
            get { return _router; }
        }
        public static void PublishConversionRouteNeighbors(StructureTable table)
        {
            if(table == null) throw new ArgumentNullException("table");
            table.AddStructure(_structureId, _router, typeof(PositiveInfinitySymbol));
        }
        public static PositiveInfinitySymbol ConvertFrom(ValueStructure value)
        {
            return _instance;
        }
        #endregion

        #region Singleton
        private static PositiveInfinitySymbol _instance;
        public static PositiveInfinitySymbol Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new PositiveInfinitySymbol();
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
                ret.Label = "PositiveInfinity";
                ret.Properties.AddProperty(Properties.ConstantSignalProperty.Instance);
                context.SingletonSignals.Add(id, ret);
                return ret;
            }
        }

        public override bool Equals(ValueStructure other)
        {
            return other is PositiveInfinitySymbol;
        }
        public bool Equals(PositiveInfinitySymbol other)
        {
            return true;
        }
        public int CompareTo(PositiveInfinitySymbol other)
        {
            return 0;
        }

        #region Serialization
        protected override void InnerSerialize(System.Xml.XmlWriter writer)
        {
        }
        private static PositiveInfinitySymbol InnerDeserialize(Context context, System.Xml.XmlReader reader)
        {
            return PositiveInfinitySymbol._instance;
        }
        #endregion
    }
}
