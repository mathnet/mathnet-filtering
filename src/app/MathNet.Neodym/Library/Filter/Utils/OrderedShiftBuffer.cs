#region Math.NET Neodym (LGPL) by Christoph Ruegg
// Math.NET Neodym, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2008, Christoph Rüegg,  http://christoph.ruegg.name
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

namespace MathNet.SignalProcessing.Filter.Utils
{
    /// <summary>
    /// A fixed-sized sorted buffer that behaves like a shift register,
    /// hence the item added first is also removed first.
    /// </summary>
    public class OrderedShiftBuffer
    {
        private readonly int _len, _mid;
        private bool _initialized;
        private LinkedList<double> _ordered;
        private LinkedList<LinkedListNode<double>> _shift;

        public OrderedShiftBuffer(int length)
        {
            _len = length;
            _mid = length >> 1; // 4 items -> 3rd item; 5 items -> 3rd item; 6 items -> 4th item etc.
            _initialized = false;
            _ordered = new LinkedList<double>();
            _shift = new LinkedList<LinkedListNode<double>>();
        }

        public int ActualCount
        {
            get { return _shift.Count; }
        }

        public bool IsInitialized
        {
            get { return _initialized; }
        }

        public int InitializedCount
        {
            get { return _len; }
        }

        public void Append(double value)
        {
            LinkedListNode<double> node = new LinkedListNode<double>(value);
            _shift.AddFirst(node);
            if(_initialized)
            {
                _ordered.Remove(_shift.Last.Value);
                _shift.RemoveLast();
            }
            else if(_shift.Count == _len)
                _initialized = true;

            LinkedListNode<double> next = _ordered.First;
            while(next != null)
            {
                if(value > next.Value)
                {
                    next = next.Next;
                    continue;
                }

                _ordered.AddBefore(next, node);
                return;
            }

            _ordered.AddLast(node);
        }

        public void Clear()
        {
            _initialized = false;
            _shift.Clear();
            _ordered.Clear();
        }

        public double Median
        {
            get
            {
                int mid = _initialized ? _mid : (_ordered.Count >> 1);
                LinkedListNode<double> next = _ordered.First;
                for(int i = 0; i < mid; i++)
                    next = next.Next;
                return next.Value;
            }
        }

        public IEnumerable<double> ByValueOrder
        {
            get
            {
                LinkedListNode<double> item = _ordered.First;
                while(item != null)
                {
                    yield return item.Value;
                    item = item.Next;
                }
            }
        }

        public IEnumerable<double> ByInsertOrder
        {
            get
            {
                LinkedListNode<LinkedListNode<double>> item = _shift.First;
                while(item != null)
                {
                    yield return item.Value.Value;
                    item = item.Next;
                }
            }
        }
    }
}
