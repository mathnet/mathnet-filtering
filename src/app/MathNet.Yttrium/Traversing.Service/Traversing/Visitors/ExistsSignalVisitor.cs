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

namespace MathNet.Symbolics.Traversing.Visitors
{
    public class ExistsSignalVisitor : AbstractScanVisitor, IExistsSignalVisitor
    {
        private Predicate<Signal> _match;
        private Signal _foundSignal;
        private Port _foundSignalTarget;

        public ExistsSignalVisitor(Predicate<Signal> match)
        {
            _match = match;
        }

        public override IScanStrategy DefaultStrategy
        {
            get { return Strategies.AllSignalsStrategy.Instance; }
        }

        public void Reset()
        {
            _foundSignal = null;
            _foundSignalTarget = null;
        }

        public bool Exists
        {
            get { return _foundSignal != null; }
        }

        public Signal FoundSignal
        {
            get { return _foundSignal; }
        }

        public Port FoundSignalTarget
        {
            get { return _foundSignalTarget; }
        }

        public override bool EnterSignal(Signal signal, Port parent, bool again, bool root)
        {
            if(again)
                return false;
            if(_match(signal))
            {
                _foundSignal = signal;
                _foundSignalTarget = parent;
                return false;
            }
            return true;
        }

        public override bool LeaveSignal(Signal signal, Port parent, bool again, bool root)
        {
            return _foundSignal == null;
        }
    }
}
