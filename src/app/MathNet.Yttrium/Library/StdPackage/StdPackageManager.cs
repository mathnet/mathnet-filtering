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
using MathNet.Symbolics.Backend.Discovery;
using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Backend.Templates;
using MathNet.Symbolics.Backend.ValueConversion;

namespace MathNet.Symbolics.StdPackage
{
    [PackageManager("Std")]
    [EntityServer]
    [ArchitectureServer]
    [TheoremServer]
    [StructureServer]
    public class StdPackageManager : IPackageManager, IArchitectureServer, IEntityServer, ITheoremServer, IStructureServer
    {
        private readonly Context context;

        public StdPackageManager(Context context)
        {
            this.context = context;
        }

        public Context Context
        {
            get { return context; }
        }

        #region IPackageManager Member
        public string Domain
        {
            get { return "Std"; }
        }

        public IEntityServer Entities
        {
            get { return this; }
        }

        public IArchitectureServer Architectures
        {
            get { return this; }
        }

        public ITheoremServer Theorems
        {
            get { return this; }
        }

        public IStructureServer Structures
        {
            get { return this; }
        }
        #endregion

        #region IEntityServer Member
        public void AppendEntities(EntityTable table)
        {
            #region Arithmetics
            table.Add(new NaryToOneGenericEntity("+", "Add", "Std", InfixNotation.LeftAssociativeInnerOperator, 60));
            table.Add(new NaryToOneGenericEntity("-", "Subtract", "Std", InfixNotation.LeftAssociativeInnerOperator, 60));
            table.Add(new NaryToOneGenericEntity("*", "Multiply", "Std", InfixNotation.LeftAssociativeInnerOperator, 50));
            table.Add(new NaryToOneGenericEntity("/", "Divide", "Std", InfixNotation.LeftAssociativeInnerOperator, 50));
            table.Add(new SymmetricGenericEntity("-", "Negate", "Std", InfixNotation.PreOperator, 20));
            table.Add(new SymmetricGenericEntity("inv", "Invert", "Std"));
            table.Add(new SymmetricGenericEntity("abs", "Absolute", "Std"));
            table.Add(new SymmetricGenericEntity("exp", "Exponential", "Std"));
            table.Add(new SymmetricGenericEntity("ln", "NaturalLogarithm", "Std"));
            table.Add(new SymmetricGenericEntity("sqr", "Square", "Std"));
            table.Add(new Entity("^", "Power", "Std", InfixNotation.RightAssociativeInnerOperator, 19, new string[] { "Power_base", "Power_radix" }, new string[] { "Power_out" }));
            table.Add(new Entity("!", "Factorial", "Std", InfixNotation.PostOperator, 45, new string[] { "Factorial_in" }, new string[] { "Factorial_out" }));
            #endregion
            #region Trigonometry
            table.Add(new SymmetricGenericEntity("sin", "Sine", "Std"));
            table.Add(new SymmetricGenericEntity("cos", "Cosine", "Std"));
            table.Add(new SymmetricGenericEntity("tan", "Tangent", "Std"));
            table.Add(new SymmetricGenericEntity("cot", "Cotangent", "Std"));
            table.Add(new SymmetricGenericEntity("sec", "Secant", "Std"));
            table.Add(new SymmetricGenericEntity("csc", "Cosecant", "Std"));
            table.Add(new SymmetricGenericEntity("asin", "InverseSine", "Std"));
            table.Add(new SymmetricGenericEntity("acos", "InverseCosine", "Std"));
            table.Add(new SymmetricGenericEntity("atan", "InverseTangent", "Std"));
            table.Add(new SymmetricGenericEntity("acot", "InverseCotangent", "Std"));
            table.Add(new SymmetricGenericEntity("asec", "InverseSecant", "Std"));
            table.Add(new SymmetricGenericEntity("acsc", "InverseCosecant", "Std"));
            table.Add(new SymmetricGenericEntity("sinh", "HyperbolicSine", "Std"));
            table.Add(new SymmetricGenericEntity("cosh", "HyperbolicCosine", "Std"));
            table.Add(new SymmetricGenericEntity("tanh", "HyperbolicTangent", "Std"));
            table.Add(new SymmetricGenericEntity("coth", "HyperbolicCotangent", "Std"));
            table.Add(new SymmetricGenericEntity("sech", "HyperbolicSecant", "Std"));
            table.Add(new SymmetricGenericEntity("csch", "HyperbolicCosecant", "Std"));
            table.Add(new SymmetricGenericEntity("asinh", "InverseHyperbolicSine", "Std"));
            table.Add(new SymmetricGenericEntity("acosh", "InverseHyperbolicCosine", "Std"));
            table.Add(new SymmetricGenericEntity("atanh", "InverseHyperbolicTangent", "Std"));
            table.Add(new SymmetricGenericEntity("acoth", "InverseHyperbolicCotangent", "Std"));
            table.Add(new SymmetricGenericEntity("asech", "InverseHyperbolicSecant", "Std"));
            table.Add(new SymmetricGenericEntity("acsch", "InverseHyperbolicCosecant", "Std"));
            #endregion
            #region Logic
            table.Add(new NaryToOneGenericEntity("and", "And", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            table.Add(new NaryToOneGenericEntity("nand", "Nand", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            table.Add(new NaryToOneGenericEntity("or", "Or", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            table.Add(new NaryToOneGenericEntity("nor", "Nor", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            table.Add(new NaryToOneGenericEntity("xor", "Xor", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            table.Add(new NaryToOneGenericEntity("xnor", "Xnor", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            table.Add(new NaryToOneGenericEntity("imp", "Imply", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            table.Add(new SymmetricGenericEntity("not", "Not", "Std", InfixNotation.PreOperator, 250));
            #endregion
            #region Algebra & Analysis
            table.Add(new NearlySymmetricGenericEntity("diff", "Derive", "Std", 1, 0));
            table.Add(new SymmetricGenericEntity("asimplify", "AutoSimplify", "Std"));
            #endregion
            #region Flow
            table.Add(new SymmetricGenericEntity("<-", "Transport", "Std"));
            table.Add(new NearlySymmetricGenericEntity("$", "Sync", "Std", 1, 0));
            table.Add(new SymmetricGenericEntity("clk", "Clock", "Std"));
            #endregion
        }
        #endregion

        #region IArchitectureServer Member
        public void AppendArchitectures(ArchitectureTable table)
        {
            #region Arithmetics
            table.AddArchitectureBuilder(new Arithmetics.AdditionArchitectures());
            table.AddArchitectureBuilder(new Arithmetics.SubtractionArchitectures());
            table.AddArchitectureBuilder(new Arithmetics.MultiplicationArchitectures());
            table.AddArchitectureBuilder(new Arithmetics.DivisionArchitectures());
            table.AddArchitectureBuilder(new Arithmetics.NegateArchitectures());
            table.AddArchitectureBuilder(new Arithmetics.InvertArchitectures());
            table.AddArchitectureBuilder(new Arithmetics.AbsoluteArchitectures());
            table.AddArchitectureBuilder(new Arithmetics.ExponentialArchitectures());
            table.AddArchitectureBuilder(new Arithmetics.NaturalLogarithmArchitectures());
            table.AddArchitectureBuilder(new Arithmetics.SquareArchitectures());
            table.AddArchitectureBuilder(new Arithmetics.PowerArchitectures());
            #endregion
            #region Trigonometry
            table.AddArchitectureBuilder(new Trigonometry.SineArchitectures());
            table.AddArchitectureBuilder(new Trigonometry.CosineArchitectures());
            table.AddArchitectureBuilder(new Trigonometry.TangentArchitectures());
            table.AddArchitectureBuilder(new Trigonometry.CotangentArchitectures());
            table.AddArchitectureBuilder(new Trigonometry.SecantArchitectures());
            table.AddArchitectureBuilder(new Trigonometry.CosecantArchitectures());
            table.AddArchitectureBuilder(new Trigonometry.HyperbolicSineArchitectures());
            table.AddArchitectureBuilder(new Trigonometry.HyperbolicCosineArchitectures());
            table.AddArchitectureBuilder(new Trigonometry.HyperbolicTangentArchitectures());
            table.AddArchitectureBuilder(new Trigonometry.HyperbolicCotangentArchitectures());
            table.AddArchitectureBuilder(new Trigonometry.HyperbolicSecantArchitectures());
            table.AddArchitectureBuilder(new Trigonometry.HyperbolicCosecantArchitectures());
            #endregion
            #region Logic
            table.AddArchitectureBuilder(new Logic.AndArchitectures());
            table.AddArchitectureBuilder(new Logic.NandArchitectures());
            table.AddArchitectureBuilder(new Logic.OrArchitectures());
            table.AddArchitectureBuilder(new Logic.NorArchitectures());
            table.AddArchitectureBuilder(new Logic.XorArchitectures());
            table.AddArchitectureBuilder(new Logic.XnorArchitectures());
            table.AddArchitectureBuilder(new Logic.NotArchitectures());
            #endregion
            #region Algebra & Analysis
            table.AddArchitectureBuilder(new Analysis.AlgebraicDerivativeArchitecture());
            table.AddArchitectureBuilder(new Algebra.AutoSimplifyArchitecture());
            #endregion
            #region Flow
            table.AddArchitectureBuilder(new Flow.TransportArchitecture());
            table.AddArchitectureBuilder(new Flow.SyncArchitecture());
            table.AddArchitectureBuilder(new Flow.ClockArchitecture());
            #endregion
        }
        #endregion

        #region ITheoremServer Members
        public void AppendTheorems(TheoremTable table)
        {
            #region Arithmetics
            table.AddTheorem(Arithmetics.AdditionArchitectures.BuildTheorems(context));
            table.AddTheorem(Arithmetics.SubtractionArchitectures.BuildTheorems(context));
            table.AddTheorem(Arithmetics.MultiplicationArchitectures.BuildTheorems(context));
            table.AddTheorem(Arithmetics.DivisionArchitectures.BuildTheorems(context));
            table.AddTheorem(Arithmetics.NegateArchitectures.BuildTheorems(context));
            table.AddTheorem(Arithmetics.InvertArchitectures.BuildTheorems(context));
            table.AddTheorem(Arithmetics.AbsoluteArchitectures.BuildTheorems(context));
            table.AddTheorem(Arithmetics.ExponentialArchitectures.BuildTheorems(context));
            table.AddTheorem(Arithmetics.NaturalLogarithmArchitectures.BuildTheorems(context));
            table.AddTheorem(Arithmetics.SquareArchitectures.BuildTheorems(context));
            table.AddTheorem(Arithmetics.PowerArchitectures.BuildTheorems(context));
            #endregion
            #region Trigonometry
            table.AddTheorem(Trigonometry.SineArchitectures.BuildTheorems(context));
            table.AddTheorem(Trigonometry.CosineArchitectures.BuildTheorems(context));
            table.AddTheorem(Trigonometry.TangentArchitectures.BuildTheorems(context));
            table.AddTheorem(Trigonometry.CotangentArchitectures.BuildTheorems(context));
            table.AddTheorem(Trigonometry.SecantArchitectures.BuildTheorems(context));
            table.AddTheorem(Trigonometry.CosecantArchitectures.BuildTheorems(context));
            table.AddTheorem(Trigonometry.HyperbolicSineArchitectures.BuildTheorems(context));
            table.AddTheorem(Trigonometry.HyperbolicCosineArchitectures.BuildTheorems(context));
            table.AddTheorem(Trigonometry.HyperbolicTangentArchitectures.BuildTheorems(context));
            table.AddTheorem(Trigonometry.HyperbolicCotangentArchitectures.BuildTheorems(context));
            table.AddTheorem(Trigonometry.HyperbolicSecantArchitectures.BuildTheorems(context));
            table.AddTheorem(Trigonometry.HyperbolicCosecantArchitectures.BuildTheorems(context));
            #endregion
            #region Flow
            table.AddTheorem(Flow.TransportArchitecture.BuildTheorems(context));
            #endregion
            #region Other
            table.AddTheorem(GenericMathOpArchitecture.BuildTheorems(context));
            #endregion
        }
        #endregion

        #region IStructureServer Members
        public void AppendStructures(StructureTable table)
        {
            StdPackage.Structures.IntegerValue.PublishConversionRouteNeighbors(table);
            StdPackage.Structures.RationalValue.PublishConversionRouteNeighbors(table);
            StdPackage.Structures.RealValue.PublishConversionRouteNeighbors(table);
            StdPackage.Structures.ComplexValue.PublishConversionRouteNeighbors(table);

            StdPackage.Structures.LiteralValue.PublishConversionRouteNeighbors(table);
            StdPackage.Structures.LogicValue.PublishConversionRouteNeighbors(table);
            StdPackage.Structures.ToggleValue.PublishConversionRouteNeighbors(table);

            StdPackage.Structures.UndefinedSymbol.PublishConversionRouteNeighbors(table);
            StdPackage.Structures.NegativeInfinitySymbol.PublishConversionRouteNeighbors(table);
            StdPackage.Structures.PositiveInfinitySymbol.PublishConversionRouteNeighbors(table);
            StdPackage.Structures.ComplexInfinitySymbol.PublishConversionRouteNeighbors(table);
        }
        #endregion
    }
}
