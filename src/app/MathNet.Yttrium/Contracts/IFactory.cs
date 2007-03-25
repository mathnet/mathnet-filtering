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

namespace MathNet.Symbolics
{
    public interface IFactory<T>
    {
        T GetInstance();
        //T GetInstance(params object[] parameters);
    }

    /*
     * Note: The following list of factories is quite dumb ("WTF"),
     * but it greatly simplifies implementing them (compared
     * to the object[] approach where you have to check types
     * and cast/convert manually). However, let me know if
     * you have a better idea! [maybe I should check out
     * dependency injection (-> spring.net etc.) ?]
     */

    public interface IFactory<T, T1> : IFactory<T>
    {
        T GetInstance(T1 p1);
    }
    public interface IFactory<T, T1, T2> : IFactory<T>
    {
        T GetInstance(T1 p1, T2 p2);
    }
    public interface IFactory<T, T1, T2, T3> : IFactory<T>
    {
        T GetInstance(T1 p1, T2 p2, T3 p3);
    }
    public interface IFactory<T, T1, T2, T3, T4> : IFactory<T>
    {
        T GetInstance(T1 p1, T2 p2, T3 p3, T4 p4);
    }
    public interface IFactory<T, T1, T2, T3, T4, T5> : IFactory<T>
    {
        T GetInstance(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);
    }
    public interface IFactory<T, T1, T2, T3, T4, T5, T6> : IFactory<T>
    {
        T GetInstance(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);
    }
    public interface IFactory<T, T1, T2, T3, T4, T5, T6, T7> : IFactory<T>
    {
        T GetInstance(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7);
    }
    public interface IFactory<T, T1, T2, T3, T4, T5, T6, T7, T8> : IFactory<T>
    {
        T GetInstance(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8);
    }
}
