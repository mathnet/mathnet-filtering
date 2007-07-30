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
using System.Collections.Generic;
using System.Text;

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Packages.Standard.Structures;
using MathNet.Symbolics.Packages.Standard.Analysis;
using MathNet.Symbolics.Packages.Standard.Algebra;
using MathNet.Symbolics.Packages.Standard.Arithmetics;
using MathNet.Symbolics.Manipulation;
using MathNet.Symbolics.Conversion;

namespace MathNet.Symbolics.Packages.Standard
{
    public static class GeneralizedPolynomial
    {
        #region Classification
        private static bool IsMonomialFactor(Signal signal, Signal[] generalizedVariables)
        {
            if(Array.Exists<Signal>(generalizedVariables, signal.Equals))
                return true;
            if(!Array.Exists<Signal>(generalizedVariables, signal.DependsOn))
                return true;
            if(signal.IsDrivenByPortEntity("Power", "Std"))
            {
                Signal b = signal.DrivenByPort.InputSignals[0];
                Signal e = signal.DrivenByPort.InputSignals[1];
                if(Array.Exists<Signal>(generalizedVariables, b.Equals) && Std.IsAlwaysPositiveInteger(e))
                    return true;
            }
            return false;
        }
        public static bool IsMonomial(Signal signal, Signal[] generalizedVariables)
        {
            if(signal.IsDrivenByPortEntity("Multiply", "Std"))
                return signal.DrivenByPort.InputSignals.TrueForAll(delegate(Signal s) { return IsMonomialFactor(s, generalizedVariables); });
            else
                return IsMonomialFactor(signal, generalizedVariables);
        }
        public static bool IsPolynomial(Signal signal, Signal[] generalizedVariables)
        {
            if(!signal.IsDrivenByPortEntity("Add", "Std"))
                return IsMonomial(signal, generalizedVariables);
            if(Array.Exists<Signal>(generalizedVariables, signal.Equals))
                return true;
            return signal.DrivenByPort.InputSignals.TrueForAll(delegate(Signal s) { return IsMonomial(s, generalizedVariables); });
        }
        #endregion

        #region Variables
        public static Signal[] PolynomialVariables(Signal signal)
        {
            List<Signal> variables = new List<Signal>();
            ISignalSet monomials;
            if(signal.IsDrivenByPortEntity("Add", "Std"))
                monomials = signal.DrivenByPort.InputSignals;
            else
                monomials = new SignalSet(signal);
            for(int i = 0; i < monomials.Count; i++)
            {
                ISignalSet factors;
                if(monomials[i].IsDrivenByPortEntity("Multiply", "Std"))
                    factors = monomials[i].DrivenByPort.InputSignals;
                else
                    factors = new SignalSet(monomials[i]);
                for(int j = 0; j < factors.Count; j++)
                {
                    if(Std.IsAlwaysRational(factors[j]))
                        continue;
                    if(factors[j].IsDrivenByPortEntity("Power", "Std"))
                    {
                        Signal b = factors[j].DrivenByPort.InputSignals[0];
                        if(Std.IsAlwaysPositiveInteger(factors[j].DrivenByPort.InputSignals[1]))
                        {
                            if(!variables.Contains(b))
                                variables.Add(b);
                        }
                        else
                        {
                            if(!variables.Contains(signal))
                                variables.Add(signal);
                        }
                    }
                    else
                        if(!variables.Contains(factors[j]))
                            variables.Add(factors[j]);
                }
            }
            return variables.ToArray();
        }
        #endregion

        #region Degree
        public static IValueStructure MonomialDegree(Signal signal, Signal[] generalizedVariables)
        {
            if(Std.IsConstantAdditiveIdentity(signal))
                return NegativeInfinitySymbol.Instance;
            if(Array.Exists<Signal>(generalizedVariables, signal.Equals))
                return IntegerValue.One;
            if(!Array.Exists<Signal>(generalizedVariables, signal.DependsOn))
                return IntegerValue.Zero;
            ISignalSet factors;
            long deg = 0;
            if(signal.IsDrivenByPortEntity("Multiply", "Std"))
                factors = signal.DrivenByPort.InputSignals;
            else
                factors = new SignalSet(signal);
            for(int i = 0; i < factors.Count; i++)
            {
                if(Array.Exists<Signal>(generalizedVariables, factors[i].Equals))
                    deg++;
                else if(factors[i].IsDrivenByPortEntity("Power", "Std"))
                {
                    Signal b = signal.DrivenByPort.InputSignals[0];
                    Signal e = signal.DrivenByPort.InputSignals[1];
                    IntegerValue v;
                    if(Array.Exists<Signal>(generalizedVariables, b.Equals) && (v = e.Value as IntegerValue) != null && v.Value > 0)
                        deg += v.Value;
                }
            }
            return new IntegerValue(deg);
        }
        public static IValueStructure PolynomialDegree(Signal signal, Signal[] generalizedVariables)
        {
            if(signal == null)
                throw new ArgumentNullException("signal");

            if(signal.IsDrivenByPortEntity("Add", "Std"))
            {
                IntegerValue d = IntegerValue.Zero;
                ISignalSet inputs = signal.DrivenByPort.InputSignals;
                for(int i = 0; i < inputs.Count; i++)
                {
                    IValueStructure f = MonomialDegree(inputs[i], generalizedVariables);
                    if(f is UndefinedSymbol)
                        return f;
                    else if(!(f is NegativeInfinitySymbol))
                        d = d.Max((IntegerValue)f);
                }
                return d;
            }
            else return MonomialDegree(signal, generalizedVariables);
        }
        public static IValueStructure PolynomialTotalDegree(Signal signal)
        {
            Signal[] variables = PolynomialVariables(signal);
            if(variables.Length == 0)
                return UndefinedSymbol.Instance;
            return PolynomialDegree(signal, variables);
        }
        #endregion
    }
}
