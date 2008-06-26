#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
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

namespace MathNet.Numerics
{
    internal delegate T OnDemandCompute<T>();

    internal sealed class OnDemandComputation<T>
    {
        OnDemandCompute<T> _compute;
        T _result;
        bool _isValid;

        internal
        OnDemandComputation(
            OnDemandCompute<T> compute
            )
        {
            _compute = compute;
        }

        [System.Diagnostics.DebuggerStepThrough]
        internal
        T
        Compute()
        {
            if(!_isValid)
            {
                ForceRecompute();
            }

            return _result;
        }

        [System.Diagnostics.DebuggerStepThrough]
        internal
        void
        ForceRecompute()
        {
            _result = _compute();
            _isValid = true;
        }

        internal
        void
        Reset()
        {
            _result = default(T);
            _isValid = false;
        }

        internal bool IsValid
        {
            get { return _isValid; }
        }
    }
}
