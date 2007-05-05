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

using MathNet.Symbolics.Backend;
using MathNet.Symbolics.Backend.Containers;
using MathNet.Symbolics.Library;
using MathNet.Symbolics.Packages.ObjectModel;

namespace MathNet.Symbolics.Packages.Standard
{
    //[PackageManager("Std")]
    //[EntityServer]
    //[ArchitectureServer]
    //[TheoremServer]
    //[StructureServer]
    public class StdPackageManager : IPackageManager
    {
        public StdPackageManager()
        {
        }

        public string Domain
        {
            get { return "Std"; }
        }

        public void Register(ILibrary library)
        {
            RegisterEntities(library);
            RegisterArchitectures(library);
            RegisterTheorems(library);
            RegisterStructures(library);
        }

        public void RegisterEntities(ILibrary library)
        {
            if(library == null)
                throw new ArgumentNullException("library");

            #region Arithmetics
            library.AddEntity(new NaryToOneGenericEntity("+", "Add", "Std", InfixNotation.LeftAssociativeInnerOperator, 60));
            library.AddEntity(new NaryToOneGenericEntity("-", "Subtract", "Std", InfixNotation.LeftAssociativeInnerOperator, 60));
            library.AddEntity(new NaryToOneGenericEntity("*", "Multiply", "Std", InfixNotation.LeftAssociativeInnerOperator, 50));
            library.AddEntity(new NaryToOneGenericEntity("/", "Divide", "Std", InfixNotation.LeftAssociativeInnerOperator, 50));
            library.AddEntity(new SymmetricGenericEntity("-", "Negate", "Std", InfixNotation.PreOperator, 20));
            library.AddEntity(new SymmetricGenericEntity("inv", "Invert", "Std"));
            library.AddEntity(new SymmetricGenericEntity("abs", "Absolute", "Std"));
            library.AddEntity(new SymmetricGenericEntity("exp", "Exponential", "Std"));
            library.AddEntity(new SymmetricGenericEntity("ln", "NaturalLogarithm", "Std"));
            library.AddEntity(new SymmetricGenericEntity("sqr", "Square", "Std"));
            library.AddEntity(new EntityBase("^", "Power", "Std", InfixNotation.RightAssociativeInnerOperator, 19, new string[] { "Power_base", "Power_radix" }, new string[] { "Power_out" }));
            library.AddEntity(new EntityBase("!", "Factorial", "Std", InfixNotation.PostOperator, 45, new string[] { "Factorial_in" }, new string[] { "Factorial_out" }));
            #endregion
            #region Trigonometry
            library.AddEntity(new SymmetricGenericEntity("sin", "Sine", "Std"));
            library.AddEntity(new SymmetricGenericEntity("cos", "Cosine", "Std"));
            library.AddEntity(new SymmetricGenericEntity("tan", "Tangent", "Std"));
            library.AddEntity(new SymmetricGenericEntity("cot", "Cotangent", "Std"));
            library.AddEntity(new SymmetricGenericEntity("sec", "Secant", "Std"));
            library.AddEntity(new SymmetricGenericEntity("csc", "Cosecant", "Std"));
            library.AddEntity(new SymmetricGenericEntity("asin", "InverseSine", "Std"));
            library.AddEntity(new SymmetricGenericEntity("acos", "InverseCosine", "Std"));
            library.AddEntity(new SymmetricGenericEntity("atan", "InverseTangent", "Std"));
            library.AddEntity(new SymmetricGenericEntity("acot", "InverseCotangent", "Std"));
            library.AddEntity(new SymmetricGenericEntity("asec", "InverseSecant", "Std"));
            library.AddEntity(new SymmetricGenericEntity("acsc", "InverseCosecant", "Std"));
            library.AddEntity(new SymmetricGenericEntity("sinh", "HyperbolicSine", "Std"));
            library.AddEntity(new SymmetricGenericEntity("cosh", "HyperbolicCosine", "Std"));
            library.AddEntity(new SymmetricGenericEntity("tanh", "HyperbolicTangent", "Std"));
            library.AddEntity(new SymmetricGenericEntity("coth", "HyperbolicCotangent", "Std"));
            library.AddEntity(new SymmetricGenericEntity("sech", "HyperbolicSecant", "Std"));
            library.AddEntity(new SymmetricGenericEntity("csch", "HyperbolicCosecant", "Std"));
            library.AddEntity(new SymmetricGenericEntity("asinh", "InverseHyperbolicSine", "Std"));
            library.AddEntity(new SymmetricGenericEntity("acosh", "InverseHyperbolicCosine", "Std"));
            library.AddEntity(new SymmetricGenericEntity("atanh", "InverseHyperbolicTangent", "Std"));
            library.AddEntity(new SymmetricGenericEntity("acoth", "InverseHyperbolicCotangent", "Std"));
            library.AddEntity(new SymmetricGenericEntity("asech", "InverseHyperbolicSecant", "Std"));
            library.AddEntity(new SymmetricGenericEntity("acsch", "InverseHyperbolicCosecant", "Std"));
            #endregion
            #region Logic
            library.AddEntity(new NaryToOneGenericEntity("and", "And", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            library.AddEntity(new NaryToOneGenericEntity("nand", "Nand", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            library.AddEntity(new NaryToOneGenericEntity("or", "Or", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            library.AddEntity(new NaryToOneGenericEntity("nor", "Nor", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            library.AddEntity(new NaryToOneGenericEntity("xor", "Xor", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            library.AddEntity(new NaryToOneGenericEntity("xnor", "Xnor", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            library.AddEntity(new NaryToOneGenericEntity("imp", "Imply", "Std", InfixNotation.LeftAssociativeInnerOperator, 250));
            library.AddEntity(new SymmetricGenericEntity("not", "Not", "Std", InfixNotation.PreOperator, 250));
            #endregion
            #region Algebra & Analysis
            library.AddEntity(new NearlySymmetricGenericEntity("diff", "Derive", "Std", 1, 0));
            library.AddEntity(new SymmetricGenericEntity("asimplify", "AutoSimplify", "Std"));
            #endregion
            #region Flow
            library.AddEntity(new SymmetricGenericEntity("<-", "Transport", "Std"));
            library.AddEntity(new NearlySymmetricGenericEntity("$", "Sync", "Std", 1, 0));
            library.AddEntity(new SymmetricGenericEntity("clk", "Clock", "Std"));
            #endregion
        }

        public void RegisterArchitectures(ILibrary library)
        {
            if(library == null)
                throw new ArgumentNullException("library");

            #region Arithmetics
            library.AddArchitecture(new Arithmetics.AdditionArchitectures());
            library.AddArchitecture(new Arithmetics.SubtractionArchitectures());
            library.AddArchitecture(new Arithmetics.MultiplicationArchitectures());
            library.AddArchitecture(new Arithmetics.DivisionArchitectures());
            library.AddArchitecture(new Arithmetics.NegateArchitectures());
            library.AddArchitecture(new Arithmetics.InvertArchitectures());
            library.AddArchitecture(new Arithmetics.AbsoluteArchitectures());
            library.AddArchitecture(new Arithmetics.ExponentialArchitectures());
            library.AddArchitecture(new Arithmetics.NaturalLogarithmArchitectures());
            library.AddArchitecture(new Arithmetics.SquareArchitectures());
            library.AddArchitecture(new Arithmetics.PowerArchitectures());
            #endregion
            #region Trigonometry
            library.AddArchitecture(new Trigonometry.SineArchitectures());
            library.AddArchitecture(new Trigonometry.CosineArchitectures());
            library.AddArchitecture(new Trigonometry.TangentArchitectures());
            library.AddArchitecture(new Trigonometry.CotangentArchitectures());
            library.AddArchitecture(new Trigonometry.SecantArchitectures());
            library.AddArchitecture(new Trigonometry.CosecantArchitectures());
            library.AddArchitecture(new Trigonometry.HyperbolicSineArchitectures());
            library.AddArchitecture(new Trigonometry.HyperbolicCosineArchitectures());
            library.AddArchitecture(new Trigonometry.HyperbolicTangentArchitectures());
            library.AddArchitecture(new Trigonometry.HyperbolicCotangentArchitectures());
            library.AddArchitecture(new Trigonometry.HyperbolicSecantArchitectures());
            library.AddArchitecture(new Trigonometry.HyperbolicCosecantArchitectures());
            #endregion
            #region Logic
            library.AddArchitecture(new Logic.AndArchitectures());
            library.AddArchitecture(new Logic.NandArchitectures());
            library.AddArchitecture(new Logic.OrArchitectures());
            library.AddArchitecture(new Logic.NorArchitectures());
            library.AddArchitecture(new Logic.XorArchitectures());
            library.AddArchitecture(new Logic.XnorArchitectures());
            library.AddArchitecture(new Logic.NotArchitectures());
            #endregion
            #region Algebra & Analysis
            library.AddArchitecture(new Analysis.AlgebraicDerivativeArchitecture());
            library.AddArchitecture(new Algebra.AutoSimplifyArchitecture());
            #endregion
            #region Flow
            library.AddArchitecture(new Flow.TransportArchitecture());
            library.AddArchitecture(new Flow.SyncArchitecture());
            library.AddArchitecture(new Flow.ClockArchitecture());
            #endregion
        }

        public void RegisterTheorems(ILibrary library)
        {
            if(library == null)
                throw new ArgumentNullException("library");

            #region Arithmetics
            Arithmetics.AdditionArchitectures.RegisterTheorems(library);
            Arithmetics.SubtractionArchitectures.RegisterTheorems(library);
            Arithmetics.MultiplicationArchitectures.RegisterTheorems(library);
            Arithmetics.DivisionArchitectures.RegisterTheorems(library);
            Arithmetics.NegateArchitectures.RegisterTheorems(library);
            Arithmetics.InvertArchitectures.RegisterTheorems(library);
            Arithmetics.AbsoluteArchitectures.RegisterTheorems(library);
            Arithmetics.ExponentialArchitectures.RegisterTheorems(library);
            Arithmetics.NaturalLogarithmArchitectures.RegisterTheorems(library);
            Arithmetics.SquareArchitectures.RegisterTheorems(library);
            Arithmetics.PowerArchitectures.RegisterTheorems(library);
            #endregion
            #region Trigonometry
            Trigonometry.SineArchitectures.RegisterTheorems(library);
            Trigonometry.CosineArchitectures.RegisterTheorems(library);
            Trigonometry.TangentArchitectures.RegisterTheorems(library);
            Trigonometry.CotangentArchitectures.RegisterTheorems(library);
            Trigonometry.SecantArchitectures.RegisterTheorems(library);
            Trigonometry.CosecantArchitectures.RegisterTheorems(library);
            Trigonometry.HyperbolicSineArchitectures.RegisterTheorems(library);
            Trigonometry.HyperbolicCosineArchitectures.RegisterTheorems(library);
            Trigonometry.HyperbolicTangentArchitectures.RegisterTheorems(library);
            Trigonometry.HyperbolicCotangentArchitectures.RegisterTheorems(library);
            Trigonometry.HyperbolicSecantArchitectures.RegisterTheorems(library);
            Trigonometry.HyperbolicCosecantArchitectures.RegisterTheorems(library);
            #endregion
            #region Flow
            Flow.TransportArchitecture.RegisterTheorems(library);
            #endregion
            #region Other
            Algebra.AutoSimplifyArchitecture.RegisterTheorems(library);
            #endregion
        }

        public void RegisterStructures(ILibrary library)
        {
            if(library == null)
                throw new ArgumentNullException("library");

            Packages.Standard.Structures.IntegerValue.Register(library);
            Packages.Standard.Structures.RationalValue.Register(library);
            Packages.Standard.Structures.RealValue.Register(library);
            Packages.Standard.Structures.ComplexValue.Register(library);

            Packages.Standard.Structures.LiteralValue.Register(library);
            Packages.Standard.Structures.LogicValue.Register(library);
            Packages.Standard.Structures.ToggleValue.Register(library);

            Packages.Standard.Structures.UndefinedSymbol.Register(library);
            Packages.Standard.Structures.NegativeInfinitySymbol.Register(library);
            Packages.Standard.Structures.PositiveInfinitySymbol.Register(library);
            Packages.Standard.Structures.ComplexInfinitySymbol.Register(library);

            Packages.Standard.Structures.VectorValue<Packages.Standard.Structures.RealValue>.Register(library);
            Packages.Standard.Structures.VectorValue<Packages.Standard.Structures.ComplexValue>.Register(library);
        }
    }
}
