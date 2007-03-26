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

namespace MathNet.Symbolics.Packages.ObjectModel
{
    [Serializable]
    public class NearlySymmetricGenericEntity : EntityBase
    {
        private int additionalInputs, additionalOutputs;

        public NearlySymmetricGenericEntity(string symbol, string label, string domain, int additionalInputs, int additionalOutputs)
            : this(symbol, label, domain, InfixNotation.None, -1, additionalInputs, additionalOutputs) { }
        public NearlySymmetricGenericEntity(string symbol, string label, string domain, InfixNotation notation, int precedence, int additionalInputs, int additionalOutputs)
            : base(symbol, label, domain, notation, precedence, true)
        {
            this.additionalInputs = additionalInputs;
            this.additionalOutputs = additionalOutputs;
        }

        public NearlySymmetricGenericEntity(string symbol, string label, string domain, int additionalInputs, int additionalOutputs, string[] buses)
            : this(symbol, label, domain, InfixNotation.None, -1, additionalInputs, additionalOutputs, buses) { }
        public NearlySymmetricGenericEntity(string symbol, string label, string domain, InfixNotation notation, int precedence, int additionalInputs, int additionalOutputs, string[] buses)
            : base(symbol, label, domain, notation, precedence, true, buses)
        {
            this.additionalInputs = additionalInputs;
            this.additionalOutputs = additionalOutputs;
        }

        //protected override Entity CompileGenericEntity(Signal[] inputSignals, Bus[] buses)
        public override IEntity CompileGenericEntity(int inputSignalsCount, int busesCount, int? outputSignalsCnt)
        {
            int signalCnt = inputSignalsCount - additionalInputs;
            int outputSignalsCount = signalCnt + additionalOutputs;

            if(outputSignalsCnt.HasValue && !outputSignalsCnt.Value.Equals(outputSignalsCount))
                throw new ArgumentException("Unexpected argument value", "outputSignalsCnt");

            string[] newInputLabels = new string[inputSignalsCount];
            string[] newOutputLabels = new string[outputSignalsCount];

            string lbl = EntityId.Label;
            string prefixIn = lbl + "_in_";
            string prefixOut = lbl + "_out_";

            for(int i = 0; i < signalCnt; i++)
            {
                string nr = i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo).PadLeft(3, '0');
                newInputLabels[i] = prefixIn + nr;
                newOutputLabels[i] = prefixOut + nr;
            }

            prefixIn = lbl + "_additionalin_";
            prefixOut = lbl + "_additionalout_";

            for(int i = 0; i < newInputLabels.Length - signalCnt; i++)
            {
                string nr = i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo).PadLeft(3, '0');
                newInputLabels[i + signalCnt] = prefixIn + nr;
            }
            for(int i = 0; i < newOutputLabels.Length - signalCnt; i++)
            {
                string nr = i.ToString(System.Globalization.NumberFormatInfo.InvariantInfo).PadLeft(3, '0');
                newOutputLabels[i + signalCnt] = prefixOut + nr;
            }

            return new EntityBase(Symbol, EntityId, Notation, PrecedenceGroup, newInputLabels, newOutputLabels, Buses);
        }

        //public override bool Equals(Entity other)
        //{
        //    if(!base.Equals(other))
        //        return false;
        //    NearlySymmetricGenericEntity nsge = other as NearlySymmetricGenericEntity;
        //    if(nsge == null)
        //        return false;
        //    return additionalInputs == nsge.additionalInputs && additionalOutputs == nsge.additionalOutputs;
        //}
    }
}
