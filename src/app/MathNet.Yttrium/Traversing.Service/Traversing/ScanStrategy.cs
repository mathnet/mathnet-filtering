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

namespace MathNet.Symbolics.Traversing
{
    [System.Diagnostics.DebuggerStepThrough]
    public abstract class ScanStrategy : IScanStrategy
    {
        private readonly ConcurrencyMode _concurrency;
        private WeakReference _nextInstance;
        private int _activeScans;

        public enum ConcurrencyMode
        {
            /// <summary>supports parallel execution with the same shared instance.</summary>
            ParallelStateless,
            /// <summary>supports parallel execution with different instances.</summary>
            ParallelStateful,
            /// <summary>does not support parallel execution in any way.</summary>
            MutualExclusive
        }

        protected ScanStrategy(ConcurrencyMode concurrency)
        {
            _concurrency = concurrency;
            _nextInstance = new WeakReference(null, false);
        }

        public bool IsActive
        {
            get { return _activeScans != 0; }
        }
        public ConcurrencyMode Concurrency
        {
            get { return _concurrency; }
        }

        public void Traverse(Signal rootSignal, IScanVisitor visitor, bool ignoreHold)
        {
            //lock(??) {
            ScanStrategy strat = ProvideExecutableInstance();
            strat._activeScans++; //}
            try { strat.DoTraverse(rootSignal, visitor, ignoreHold); }
            finally { strat._activeScans--; }
        }
        public void Traverse(Port rootPort, IScanVisitor visitor, bool ignoreHold)
        {
            //lock(??) {
            ScanStrategy strat = ProvideExecutableInstance();
            strat._activeScans++; //}
            try { strat.DoTraverse(rootPort, visitor, ignoreHold); }
            finally { strat._activeScans--; }
        }
        public void Traverse(IEnumerable<Signal> rootSignals, IScanVisitor visitor, bool ignoreHold)
        {
            //lock(??) {
            ScanStrategy strat = ProvideExecutableInstance();
            strat._activeScans++; //}
            try { strat.DoTraverse(rootSignals, visitor, ignoreHold); }
            finally { strat._activeScans--; }
        }

        protected abstract void DoTraverse(Signal rootSignal, IScanVisitor visitor, bool ignoreHold);
        protected abstract void DoTraverse(Port rootPort, IScanVisitor visitor, bool ignoreHold);
        protected abstract void DoTraverse(IEnumerable<Signal> rootSignals, IScanVisitor visitor, bool ignoreHold);

        private ScanStrategy ProvideExecutableInstance()
        {
            if(_activeScans == 0 || _concurrency == ConcurrencyMode.ParallelStateless)
            {
                ReuseStrategy();
                return this;
            }
            else if(_concurrency == ConcurrencyMode.ParallelStateful)
            {
                ScanStrategy ss;
                if(_nextInstance.IsAlive)
                {
                    ss = _nextInstance.Target as ScanStrategy;
                    if(ss != null)
                        return ss.ProvideExecutableInstance();
                }
                ss = Clone();
                ss.ReuseStrategy();
                _nextInstance.Target = ss;
                return ss;
            }
            else //Exclusive
                throw new MathNet.Symbolics.Exceptions.TraversingException("A mutual exclusive system traversing strategy is already in use and may not be executed again before the other finishes its operation.");
        }

        /// <remarks>Should be overridden in all stateful strategies.</remarks>
        protected virtual void ReuseStrategy()
        {
        }

        /// <summary>
        /// Semideep clone, having all shared (reference type) members replaced.
        /// </summary>
        /// <remarks>Should be overridden in parallel-stateful strategies.</remarks>
        protected virtual ScanStrategy Clone()
        {
            return (ScanStrategy)this.MemberwiseClone();
        }
    }
}
