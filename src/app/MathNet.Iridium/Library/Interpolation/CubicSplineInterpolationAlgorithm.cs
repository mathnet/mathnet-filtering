// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Christoph Ruegg, http://www.cdrnet.net
// Partially based on ideas from Numerical Recipes in C++, Second Edition [2003],
// as well as Handbook of Mathematical Functions [1965].
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

using System;
using MathNet.Numerics;

namespace MathNet.Numerics.Interpolation
{
    //public class CubicSplineInterpolationAlgorithm : IInterpolationAlgorithm
    //{
    //    private SampleList samples;
    //    private double[] factors;
    //    private double dx1, dxn;

    //    /// <param name="dx1">first derivative at leftmost point.</param>
    //    /// <param name="dxn">first derivative at rightmost point.</param>
    //    public CubicSplineInterpolationAlgorithm(double dx1, double dxn)
    //    {
    //        this.dx1 = dx1;
    //        this.dxn = dxn;
    //    }

    //    public void Prepare(SampleList samples)
    //    {
    //        this.samples = samples;

    //        int i,k;
    //        double p,qn,sig,un;

    //        int n = samples.Count;
    //        factors = new double[n];
    //        double[] u = new double[n-1];

    //        if(dx1 > 0.99e33)
    //            factors[0] = u[0] = 0.0;
    //        else
    //        {
    //            factors[0] = -0.5;
    //            u[0] = (3.0/(x[1]-x[0]))*((y[1]-y[0])/(x[1]-x[0])-dx1);
    //        }
    //        for(i=1;i<n-1;i++)
    //        {
    //            sig = (x[i]-x[i-1])/(x[i+1]-x[i-1]);
    //            p = sig*factors[i-1]+2.0;
    //            factors[i] = (sig-1.0)/p;
    //            u[i] = (y[i+1]-y[i])/(x[i+1]-x[i]) - (y[i]-y[i-1])/(x[i]-x[i-1]);
    //            u[i] = (6.0*u[i]/(x[i+1]-x[i-1])-sig*u[i-1])/p;
    //        }
    //        if(dxn > 0.99e33)
    //            qn = un = 0.0;
    //        else
    //        {
    //            qn = 0.5;
    //            un = (3.0/(x[n-1]-x[n-2]))*(dxn-(y[n-1]-y[n-2])/(x[n-1]-x[n-2]));
    //        }
    //        factors[n-1] = (un-qn*u[n-2])/(qn*factors[n-2]+1.0);
    //        for(k=n-2;k>=0;k--)
    //            factors[k] = factors[k]*factors[k+1]+u[k];
    //    }

    //    public double Interpolate(double t)
    //    {
    //        // TODO:  Implementierung von PolynomialInerpolationAlgorithm.Interpolate hinzufügen
    //        return 0;
    //    }

    //    public double Interpolate(double t, out double error)
    //    {
    //        throw new NotSupportedException();
    //    }

    //    public double Extrapolate(double t)
    //    {
    //        return Interpolate(t);
    //    }

    //    public bool SupportErrorEstimation
    //    {
    //        get {return false;}
    //    }
    //}
}
