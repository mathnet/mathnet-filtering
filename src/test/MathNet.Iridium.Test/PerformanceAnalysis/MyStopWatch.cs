/*
 * Original from http://www.jpboodhoo.com/blog/QuickAndDirtyTiming.aspx
 * 
 * Modified to allow timing of functions as well as procedures by
 * Kevin Whitefoot <kwhitefoot@hotmail.com>
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace PerformanceAnalysis
{

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