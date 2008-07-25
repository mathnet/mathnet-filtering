#region Math.NET Iridium (LGPL) by Ruegg, Whitefoot
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
//                          Kevin Whitefoot, kwhitefoot@hotmail.com
//
// Based on http://www.jpboodhoo.com/blog/QuickAndDirtyTiming.aspx
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
using System.Diagnostics;

namespace Iridium.Test {

  public class MyStopwatch : IDisposable {
    public delegate void MethodToTime();
    public delegate object FunctionToTime();

    private readonly MethodToTime methodToTime;
    private int numberOfTimesToInvokeMethod;

    public MyStopwatch(MethodToTime methodToTime)
      : this(methodToTime, 1) {
    }

    public MyStopwatch(MethodToTime methodToTime, int numberOfTimesToInvokeMethod) {
      this.methodToTime = methodToTime;
      this.numberOfTimesToInvokeMethod = numberOfTimesToInvokeMethod;
    }

    public void Dispose() {
      TimeMethodInvocation();

    }

    private void TimeMethodInvocation() {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int i = 0; i < numberOfTimesToInvokeMethod; i++) {
        methodToTime();
      }
      stopwatch.Stop();
      Console.Out.WriteLine(stopwatch.ElapsedMilliseconds);
    }
    public static void Time(MethodToTime methodToTime) {
      Time(methodToTime, 1);
    }

    public static void Time(MethodToTime methodToTime, int numberOfTimesToInvokeMethod) {
      new MyStopwatch(methodToTime, numberOfTimesToInvokeMethod).Dispose();
    }

    public static object Time(FunctionToTime functionToTime) {
      object result = null;
      MethodToTime m = delegate {
        result = functionToTime;
      };
      Time(m);
      return result;
    }



  }
}