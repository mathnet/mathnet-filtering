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

using MathNet.Symbolics.Core;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.ValueConversion;

namespace MathNet.Symbolics.StdPackage.Structures
{
    /// <summary>signed integer</summary>
    public class IntegerValue : ValueStructure, IEquatable<IntegerValue>, IComparable<IntegerValue>, IAlgebraicCommutativeRingWithUnity<IntegerValue>
    {
        private static readonly MathIdentifier _structureId = new MathIdentifier("Integer", "Std");
        private readonly long _dataValue; // = 0;

        #region Conversion & Conversion Routing
        private static readonly ConversionRouter _router = new ConversionRouter(_structureId);
        public static ConversionRouter Converter
        {
            get { return _router; }
        }
        public static void PublishConversionRouteNeighbors(StructureTable table)
        {
            if(table == null) throw new ArgumentNullException("table");
            table.AddStructure(_structureId, _router, typeof(IntegerValue));
        }
        public static IntegerValue ConvertFrom(ValueStructure value)
        {
            return (IntegerValue)_router.ConvertFrom(value);
        }
        #endregion

        #region Constructors
        public IntegerValue()
        {
            //this.dataValue = 0;
        }
        public IntegerValue(long value)
        {
            _dataValue = value;
        }
        #endregion

        #region Basic Operation Processes
        public class AddProcess : GenericFunctionProcess
        {
            public AddProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public AddProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action(bool isInit, Signal origin)
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(IntegerValue.AdditiveIdentity);
                else
                {
                    IntegerValue sum = (IntegerValue)Inputs[0].Value;
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        IntegerValue integer = (IntegerValue)Inputs[i].Value;
                        sum = sum.Add(integer);
                    }
                    PublishToOutputs(sum);
                }
            }
        }
        public class SubtractProcess : GenericFunctionProcess
        {
            public SubtractProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public SubtractProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action(bool isInit, Signal origin)
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(IntegerValue.AdditiveIdentity);
                else
                {
                    IntegerValue sum = (IntegerValue)Inputs[0].Value;
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        IntegerValue integer = (IntegerValue)Inputs[i].Value;
                        sum = sum.Subtract(integer);
                    }
                    PublishToOutputs(sum);
                }
            }
        }
        public class MultiplyProcess : GenericFunctionProcess
        {
            public MultiplyProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public MultiplyProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action(bool isInit, Signal origin)
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(IntegerValue.MultiplicativeIdentity);
                else
                {
                    IntegerValue product = (IntegerValue)Inputs[0].Value;
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        IntegerValue integer = (IntegerValue)Inputs[i].Value;
                        product = product.Multiply(integer);
                    }
                    PublishToOutputs(product);
                }
            }
        }
        public class DivideProcess : GenericFunctionProcess
        {
            public DivideProcess(bool[] summandInput, bool[] summandInternal, bool[] outOutput, bool[] outInternal)
                : base(summandInput, summandInternal, outOutput, outInternal) { }
            public DivideProcess(int firstInput, int lastInput, int output)
                : base(firstInput, lastInput, output) { }

            protected override void Action(bool isInit, Signal origin)
            {
                if(Inputs.Length == 0)
                    PublishToOutputs(RationalValue.MultiplicativeIdentity);
                else
                {
                    RationalValue product = new RationalValue((IntegerValue)Inputs[0].Value);
                    for(int i = 1; i < Inputs.Length; i++)
                    {
                        IntegerValue integer = (IntegerValue)Inputs[i].Value;
                        product = product.Divide(integer);
                    }
                    PublishToOutputs(product);
                }
            }
        }
        #endregion

        public long Value
        {
            get { return _dataValue; }
        }

        public static MathIdentifier StructureIdentifier
        {
            get { return _structureId; }
        }
        public override MathIdentifier StructureId
        {
            get { return _structureId; }
        }

        #region Direct Operator Implementations
        public static bool operator <(IntegerValue left, IntegerValue right)
        {
            return left._dataValue < right._dataValue;
        }
        public static bool operator >(IntegerValue left, IntegerValue right)
        {
            return left._dataValue > right._dataValue;
        }
        #endregion

        #region Direct Function Implementations
        #region Arithmetic Operations
        public IntegerValue Add(IntegerValue op)
        {
            return new IntegerValue(_dataValue + op._dataValue);
        }
        public IntegerValue Subtract(IntegerValue op)
        {
            return new IntegerValue(_dataValue - op._dataValue);
        }
        public IntegerValue Negate()
        {
            return new IntegerValue(-_dataValue);
        }
        public IntegerValue Multiply(IntegerValue op)
        {
            return new IntegerValue(_dataValue * op._dataValue);
        }
        public RationalValue Divide(IntegerValue op)
        {
            return new RationalValue(_dataValue, op._dataValue);
        }
        public RationalValue Invert()
        {
            return new RationalValue(1, _dataValue);
        }
        public IntegerValue Power(IntegerValue op)
        {
            return new IntegerValue((long)Math.Round(Math.Pow(_dataValue, op._dataValue)));
        }
        public IntegerValue Absolute()
        {
            return (_dataValue < 0) ? new IntegerValue(-_dataValue) : this;
        }
        public IntegerValue Max(IntegerValue op)
        {
            return (_dataValue >= op._dataValue) ? this : op;
        }
        public IntegerValue Min(IntegerValue op)
        {
            return (_dataValue <= op._dataValue) ? this : op;
        }
        #endregion
        #endregion

        #region Constants
        public static Signal ParseConstant(Context context, string value)
        {
            return Constant(context, long.Parse(value, Context.NumberFormat));
        }
        public static Signal Constant(Context context, long value)
        {
            Signal s = new Signal(context, new IntegerValue(value));
            s.Label = s.Value.ToString() + "_Constant";
            s.Properties.AddProperty(Properties.ConstantSignalProperty.Instance);
            return s;
        }

        public static Signal ConstantZero(Context context)
        {
            MathIdentifier id = new MathIdentifier("IntegerValueConstantZero", "Std");
            Signal ret;
            if(context.SingletonSignals.TryGetValue(id, out ret))
                return ret;
            else
            {
                ret = Constant(context, 0);
                context.SingletonSignals.Add(id, ret);
                return ret;
            }
        }

        public static Signal ConstantOne(Context context)
        {
            MathIdentifier id = new MathIdentifier("IntegerValueConstantOne", "Std");
            Signal ret;
            if(context.SingletonSignals.TryGetValue(id, out ret))
                return ret;
            else
            {
                ret = Constant(context, 1);
                context.SingletonSignals.Add(id, ret);
                return ret;
            }
        }

        public static Signal ConstantTwo(Context context)
        {
            MathIdentifier id = new MathIdentifier("IntegerValueConstantTwo", "Std");
            Signal ret;
            if(context.SingletonSignals.TryGetValue(id, out ret))
                return ret;
            else
            {
                ret = Constant(context, 2);
                context.SingletonSignals.Add(id, ret);
                return ret;
            }
        }

        public static Signal ConstantMinusOne(Context context)
        {
            MathIdentifier id = new MathIdentifier("IntegerValueConstantMinusOne", "Std");
            Signal ret;
            if(context.SingletonSignals.TryGetValue(id, out ret))
                return ret;
            else
            {
                ret = Constant(context, -1);
                context.SingletonSignals.Add(id, ret);
                return ret;
            }
        }

        public static Signal ConstantAdditiveIdentity(Context context)
        {
            return ConstantZero(context);
        }

        public static Signal ConstantMultiplicativeIdentity(Context context)
        {
            return ConstantOne(context);
        }

        public static IntegerValue Zero
        {
            get { return new IntegerValue(0); } //TODO: Think about Singleton like design...
        }

        public static IntegerValue One
        {
            get { return new IntegerValue(1); } //TODO: Think about Singleton like design...
        }

        public static IntegerValue AdditiveIdentity
        {
            get { return new IntegerValue(0); } //TODO: Think about Singleton like design...
        }

        public static IntegerValue MultiplicativeIdentity
        {
            get { return new IntegerValue(1); } //TODO: Think about Singleton like design...
        }
        #endregion

        public double ToDouble()
        {
            return (double)_dataValue;
        }

        public override string ToString()
        {
            return base.ToString() + "(" + _dataValue.ToString(System.Globalization.NumberFormatInfo.InvariantInfo) + ")";
        }

        public override bool Equals(ValueStructure other)
        {
            IntegerValue integerValue = other as IntegerValue;
            if(integerValue != null)
                return Equals(integerValue);

            RationalValue rationalValue = other as RationalValue;
            if(rationalValue != null)
                return rationalValue.Equals(this);

            RealValue realValue = other as RealValue;
            if(realValue != null)
                return realValue.Equals(this);

            return other == this;
        }
        public bool Equals(IntegerValue other)
        {
            return other != null && _dataValue.Equals(other._dataValue);
        }
        public int CompareTo(IntegerValue other)
        {
            return _dataValue.CompareTo(other._dataValue);
        }
        public override int GetHashCode()
        {
            return _dataValue.GetHashCode();
        }

        #region IAlgebraicRingWithUnity Members
        public bool IsMultiplicativeIdentity
        {
            get { return _dataValue == 1; }
        }
        ValueStructure IAlgebraicRingWithUnity.MultiplicativeIdentity
        {
            get { return MultiplicativeIdentity; }
        }
        #endregion
        #region IAlgebraicMonoid Members
        public bool IsAdditiveIdentity
        {
            get { return _dataValue == 0; }
        }
        ValueStructure IAlgebraicMonoid.AdditiveIdentity
        {
            get { return AdditiveIdentity; }
        }
        #endregion

        #region Serialization
        protected override void InnerSerialize(System.Xml.XmlWriter writer)
        {
            writer.WriteString(_dataValue.ToString(Context.NumberFormat));
        }
        private static IntegerValue InnerDeserialize(Context context, System.Xml.XmlReader reader)
        {
            return new IntegerValue(long.Parse(reader.ReadString(), Context.NumberFormat));
        }
        #endregion
    }
}
