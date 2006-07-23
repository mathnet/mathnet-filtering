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
using System.Xml;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics;
using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.ValueConversion;
using MathNet.Symbolics.StdPackage.Trigonometry;

namespace MathNet.Symbolics.StdPackage.Structures
{
    /// <summary>vectors with arbitrary components</summary>
    public class VectorValue<TScalar> : ValueStructure, IEquatable<VectorValue<TScalar>>, IAlgebraicVectorSpace<VectorValue<TScalar>,TScalar>
        where TScalar : ValueStructure, IAlgebraicField<TScalar>
    {
        private static readonly MathIdentifier _structureId = new MathIdentifier("Vector", "Std").DerivePrefix(typeof(TScalar).Name);
        private readonly TScalar[] _dataValue;

        #region Conversion & Conversion Routing
        private static readonly ConversionRouter _router = new ConversionRouter(_structureId);
        public static ConversionRouter Converter
        {
            get { return _router; }
        }
        public static void PublishConversionRouteNeighbors(StructureTable table)
        {
            if(table == null) throw new ArgumentNullException("table");
            table.AddStructure(_structureId, _router, typeof(VectorValue<TScalar>));
        }
        public static VectorValue<TScalar> ConvertFrom(ValueStructure value)
        {
            return (VectorValue<TScalar>)_router.ConvertFrom(value);
        }
        #endregion

        #region Constructors
        public VectorValue()
        {
            _dataValue = new TScalar[0];
        }
        public VectorValue(TScalar[] value)
        {
            _dataValue = value;
        }
        public VectorValue(IList<TScalar> values)
        {
            _dataValue = new TScalar[values.Count];
            values.CopyTo(_dataValue,0);
        }
        #endregion

        public TScalar[] Value
        {
            get { return _dataValue; }
        }
        public TScalar this[int index]
        {
            get { return _dataValue[index]; }
        }
        public int Count
        {
            get { return _dataValue.Length; }
        }
        //public double Norm
        //{
        //    get { return _dataValue.Modulus; }
        //}
        //public double NormSquared
        //{
        //    get { return _dataValue.ModulusSquared; }
        //}
        //public bool IsZero
        //{
        //    get { return _dataValue.IsZero; }
        //}

        public static MathIdentifier StructureIdentifier
        {
            get { return _structureId; }
        }
        public override MathIdentifier StructureId
        {
            get { return _structureId; }
        }

        #region Direct Function Implementations
        #region Arithmetic Function
        public static VectorValue<TScalar> Add(VectorValue<TScalar> summand1, VectorValue<TScalar> summand2) { return summand1.Add(summand2); }
        public VectorValue<TScalar> Add(VectorValue<TScalar> summand)
        {
            int max = Math.Max(Count, summand.Count);
            int min = Math.Min(Count, summand.Count);
            TScalar[] otherValue = summand._dataValue;
            TScalar[] value = new TScalar[max];

            if(Count > summand.Count)
                Array.Copy(_dataValue,min,value,min,max-min);
            else
                Array.Copy(otherValue, min, value, min, max - min);
            for(int i = 0; i < min; i++)
                value[i] = _dataValue[i].Add(otherValue[i]);

            return new VectorValue<TScalar>(value);
        }
        public static VectorValue<TScalar> Subtract(VectorValue<TScalar> minuend, VectorValue<TScalar> subtrahend) { return minuend.Subtract(subtrahend); }
        public VectorValue<TScalar> Subtract(VectorValue<TScalar> subtrahend)
        {
            int max = Math.Max(Count, subtrahend.Count);
            int min = Math.Min(Count, subtrahend.Count);
            TScalar[] otherValue = subtrahend._dataValue;
            TScalar[] value = new TScalar[max];

            if(Count > subtrahend.Count)
                Array.Copy(_dataValue, min, value, min, max - min);
            else
                for(int i = min; i < max; i++)
                    value[i] = otherValue[i].Negate();
            for(int i = 0; i < min; i++)
                value[i] = _dataValue[i].Subtract(otherValue[i]);

            return new VectorValue<TScalar>(value);
        }
        public static VectorValue<TScalar> Negate(VectorValue<TScalar> subtrahend) { return subtrahend.Negate(); }
        public VectorValue<TScalar> Negate()
        {
            TScalar[] value = new TScalar[_dataValue.Length];
            
            for(int i = 0; i < _dataValue.Length; i++)
                value[i] = _dataValue[i].Negate();

            return new VectorValue<TScalar>(value);
        }
        //public static TScalar Norm(VectorValue<TScalar> value) { return value.Norm(); }
        //public TScalar Norm()
        //{
        //    if(Count == 0)
        //        return ???;

        //    ...

        //    return new RealValue(_dataValue.Modulus);
        //}
        public VectorValue<TScalar> Scale(TScalar scalar)
        {
            TScalar[] value = new TScalar[_dataValue.Length];

            for(int i = 0; i < _dataValue.Length; i++)
                value[i] = _dataValue[i].Scale(scalar);

            return new VectorValue<TScalar>(value);
        }
        #endregion
        #endregion

        #region Constants
        public static Signal Constant(Context context, TScalar[] value)
        {
            return Constant(context, value, value.ToString());
        }
        public static Signal Constant(Context context, TScalar[] value, string name)
        {
            Signal s = new Signal(context, new VectorValue<TScalar>(value));
            s.Label = name + "_Constant";
            s.AddConstraint(Properties.ConstantSignalProperty.Instance);
            return s;
        }

        public static VectorValue<TScalar> Zero
        {
            get { return new VectorValue<TScalar>(); } //TODO: Think about Singleton like design...
        }

        public static VectorValue<TScalar> AdditiveIdentity
        {
            get { return new VectorValue<TScalar>(); } //TODO: Think about Singleton like design...
        }
        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            for(int i=0;i<_dataValue.Length;i++)
            {
                if(i>0)
                    sb.Append(',');
                sb.Append(_dataValue[i].ToString());
            }
            sb.Append(']');
            return sb.ToString();
        }

        public override bool Equals(ValueStructure other)
        {
            VectorValue<TScalar> vectorValue = other as VectorValue<TScalar>;
            if(vectorValue != null)
                return Equals(vectorValue);

            return other == this;
        }
        public bool Equals(VectorValue<TScalar> other)
        {
            return other != null && _dataValue.Equals(other._dataValue);
        }
        public override int GetHashCode()
        {
            return _dataValue.GetHashCode();
        }

        #region IAlgebraicMonoid Members
        public bool IsAdditiveIdentity
        {
            get
            {
                for(int i = 0; i < _dataValue.Length; i++)
                    if(!_dataValue[i].IsAdditiveIdentity)
                        return false;
                return true;
            }
        }
        ValueStructure IAlgebraicMonoid.AdditiveIdentity
        {
            get { return AdditiveIdentity; }
        }
        #endregion

        #region Serialization
        protected override void InnerSerialize(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("Count", _dataValue.Length.ToString());
            for(int i = 0; i < _dataValue.Length; i++)
                ValueStructure.Serialize(writer,_dataValue[i]);
        }
        private static VectorValue<TScalar> InnerDeserialize(Context context, System.Xml.XmlReader reader)
        {
            int cnt = int.Parse(reader.ReadElementString("Count"));
            TScalar[] values = new TScalar[cnt];
            for(int i = 0; i < cnt; i++)
                values[i] = (TScalar)ValueStructure.Deserialize(context, reader);
            return new VectorValue<TScalar>(values);
        }
        #endregion
    }
}
