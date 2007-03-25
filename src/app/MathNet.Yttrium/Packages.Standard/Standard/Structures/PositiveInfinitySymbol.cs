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
using System.Xml;
using System.Collections.Generic;

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Packages.ObjectModel;
using MathNet.Symbolics.Conversion;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Repository;

namespace MathNet.Symbolics.Packages.Standard.Structures
{
    /// <summary>symbol: positive real infinity</summary>
    public sealed class PositiveInfinitySymbol : ValueStructureBase, IEquatable<PositiveInfinitySymbol>, IComparable<PositiveInfinitySymbol>
    {
        private static readonly MathIdentifier _customTypeId = new MathIdentifier("PositiveInfinity", "Std");
        private PositiveInfinitySymbol() { }

        #region Conversion & Conversion Routing
        public static void Register(ILibrary library)
        {
            library.AddCustomDataType<PositiveInfinitySymbol>(Instance);
        }
        public static PositiveInfinitySymbol ConvertFrom(IValueStructure value)
        {
            return Instance;
        }
        public static bool CanConvertLosslessFrom(IValueStructure value)
        {
            return ValueConverter<PositiveInfinitySymbol>.Router.CanConvertLosslessFrom(value);
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

        public static MathIdentifier TypeIdentifier
        {
            get { return _customTypeId; }
        }
        public override MathIdentifier TypeId
        {
            get { return _customTypeId; }
        }

        public static Signal Constant
        {
            get
            {
                MathIdentifier id = _customTypeId.DerivePostfix("Constant");
                Signal ret;
                if(Service<ISignalCache>.Instance.TryGetValue(id, out ret))
                    return ret;
                else
                {
                    ret = Binder.CreateSignal(Instance);
                    ret.Label = "PositiveInfinity";
                    ret.AddConstraint(Properties.ConstantSignalProperty.Instance);
                    Service<ISignalCache>.Instance.Add(id, ret);
                    return ret;
                }
            }
        }

        public override bool Equals(IValueStructure other)
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
        //protected override void InnerSerialize(System.Xml.XmlWriter writer)
        //{
        //}
        //private static PositiveInfinitySymbol InnerDeserialize(IContext context, System.Xml.XmlReader reader)
        //{
        //    return PositiveInfinitySymbol._instance;
        //}
        private static PositiveInfinitySymbol Deserialize(XmlReader reader, IDictionary<Guid, Signal> signals, IDictionary<Guid, Bus> buses)
        {
            return PositiveInfinitySymbol._instance;
        }
        #endregion
    }
}
