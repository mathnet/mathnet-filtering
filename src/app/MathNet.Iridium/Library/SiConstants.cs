#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2007, Christoph Rüegg,  http://christoph.ruegg.name
//						
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published 
// by the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Numerics
{
    /// <summary>
    /// Scientific Constants
    /// </summary>
    /// <seealso cref="Constants"/>
    /// <seealso cref="SiPrefixes"/>
    public static class SiConstants
    {
        /// <summary>Speed of Light in Vacuum: c_0 = 2.99792458e8 [m s^-1] (defined, exact; 2002 CODATA)</summary>
        public const double SpeedOfLight = 2.99792458e8;
        /// <summary>Magnetic Permeability in Vacuum: mu_0 = 4*Pi * 10^-7 [N A^-2 = kg m A^-2 s^-2] (defined, exact; 2002 CODATA)</summary>
        public const double MagneticPermeability = 1.2566370614359172953850573533118011536788677597500e-6;
        /// <summary>Electric Permittivity in Vacuum: epsilon_0 = 1/(mu_0*c_0^2) [F m^-1 = A^2 s^4 kg^-1 m^-3] (defined, exact; 2002 CODATA)</summary>
        public const double ElectricPermittivity = 8.8541878171937079244693661186959426889222899381429e-12;
        /// <summary>Characteristic Impedance of Vacuum: Z_0 = mu_0*c_0 [Ohm = m^2 kg s^-3 A^-2] (defined, exact; 2002 CODATA)</summary>
        public const double CharacteristicImpedanceVacuum = 376.73031346177065546819840042031930826862350835242;

        /// <summary>Newtonian Constant of Gravitation: G = 6.6742e-11 [m^3 kg^-1 s^-2] (2002 CODATA)</summary>
        public const double GravitationalConstant = 6.6742e-11;
        /// <summary>Planck's constant: h = 6.6260693e-34 [J s = m^2 kg s^-1] (2002 CODATA)</summary>
        public const double PlancksConstant = 6.6260693e-34;
        /// <summary>Reduced Planck's constant: h_bar = h / (2*Pi) [J s = m^2 kg s^-1] (2002 CODATA)</summary>
        public const double DiracsConstant = 1.05457168e-34;
        /// <summary>Planck mass: m_p = (h_bar*c_0/G)^(1/2) [kg] (2002 CODATA)</summary>
        public const double PlancksMass = 2.17645e-8;
        /// <summary>Planck temperature: T_p = (h_bar*c_0^5/G)^(1/2)/k [K] (2002 CODATA)</summary>
        public const double PlancksTemperature = 1.41679e32;
        /// <summary>Planck length: l_p = h_bar/(m_p*c_0) [m] (2002 CODATA)</summary>
        public const double PlancksLength = 1.61624e-35;
        /// <summary>Planck time: t_p = l_p/c_0 [s] (2002 CODATA)</summary>
        public const double PlancksTime = 5.39121e-44;

        /// <summary>Elementary Electron Charge: e = 1.60217653e-19 [C = A s] (2002 CODATA)</summary>
        public const double ElementaryCharge = 1.60217653e-19;
        /// <summary>Magnetic Flux Quantum: theta_0 = h/(2*e) [Wb = m^2 kg s^-2 A^-1] (2002 CODATA)</summary>
        public const double MagneticFluxQuantum = 2.06783372e-15;
        /// <summary>Conductance Quantum: G_0 = 2*e^2/h [S = m^-2 kg^-1 s^3 A^2] (2002 CODATA)</summary>
        public const double ConductanceQuantum = 7.748091733e-5;
        
        
        public const double JosephsonConstant = 483597.879e9;
        public const double VonKlitzingConstant = 25812.807449;
        public const double BohrMagneton = 927.400949e-26;
        public const double NuclearMagneton = 5.05078343e-27;
        public const double FineStructureConstant = 7.297352568e-3;
        public const double RydbergConstant = 10973731.568525;
        public const double BohrRadius = 0.5291772108e-10;
        public const double HartreeEnergy = 4.35974417e-18;
        public const double QuantumOfCirculation = 3.636947550e-4;
        public const double FermiCouplingConstant = 1.16639e-5;
        public const double WeakMixingAngle = 0.22215;

        public const double ElectronMass = 9.1093826e-31;
        public const double ElectronMassEnergyEquivalent = 8.1871047e-14;
        public const double ElectronMolarMass = 5.4857990945e-7;
        public const double ComptonWavelength = 2.426310238e-12;
        //public const double ComptonWavelengthOver2Pi = 386.1592678e-15;
        public const double ClassicalElectronRadius = 2.817940325e-15;
        public const double ThomsonCrossSection = 0.665245873e-28;
        public const double ElectronMagneticMoment = -928.476412e-26;
        public const double ElectronGFactor = -2.0023193043718;

        public const double MuonMass = 1.88353140e-28;
        public const double MuonMassEnegryEquivalent = 1.69283360e-11;
        public const double MuonMolarMass = 0.1134289264e-3;
        public const double MuonComptonWavelength = 11.73444105e-15;
        public const double MuonMagneticMoment = -4.49044799e-26;
        public const double MuonGFactor = -2.0023318396;

        public const double TauMass = 3.16777e-27;
        public const double TauMassEnergyEquivalent = 2.84705e-10;
        public const double TauMolarMass = 1.90768e-3;
        public const double TauComptonWavelength = 0.69772e-15;

        public const double ProtonMass = 1.67262171e-27;
        public const double ProtonMassEnergyEquivalent = 1.50327743e-10;
        public const double ProtonMolarMass = 1.00727646688e-3;
        public const double ProtonComptonWavelength = 1.3214098555e-15;
        public const double ProtonMagneticMoment = 1.41060671e-26;
        public const double ProtonGFactor = 5.585694701;
        public const double ShieldedProtonMagneticMoment = 1.41057047e-26;
        public const double ProtonGyromagneticRatio = 2.67522205e8;
        public const double ShieldedProtonGyromagneticMoment = 2.67515333e8;

        public const double NeutronMass = 1.67492728e-27;
        public const double NeutronMassEnegryEquivalent = 1.50534957e-10;
        public const double NeutronMolarMass = 1.00866491560e-3;
        public const double NeutronComptonWavelength = 1.3195909067e-1;
        public const double NeutronMagneticMoment = -0.96623645e-26;
        public const double NeutronGFactor = -3.82608546;
        public const double NeutronGyromagneticRatio = 1.83247183e8;

        public const double DeuteronMass = 3.34358335e-27;
        public const double DeuteronMassEnegryEquivalent = 3.00506285e-10;
        public const double DeuteronMolarMass = 2.01355321270e-3;
        public const double DeuteronMagneticMoment = 0.433073482e-26;

        public const double HelionMass = 5.00641214e-27;
        public const double HelionMassEnegryEquivalent = 4.49953884e-10;
        public const double HelionMolarMass = 3.0149322434e-3;
        //public const double  = ;
        //public const double  = ;
        //public const double  = ;
    }
}
