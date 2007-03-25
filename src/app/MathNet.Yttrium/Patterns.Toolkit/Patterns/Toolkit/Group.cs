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
using System.Collections.ObjectModel;
using System.Text;

using MathNet.Numerics;

namespace MathNet.Symbolics.Patterns.Toolkit
{
    public class Group : Collection<Tuple<Signal, Port>>
    {
        private string _label;
        public Group(string label)
            : base()
        {
            _label = label;
        }

        public string Label
        {
            get { return _label; }
        }

        public void AddRange(IEnumerable<Tuple<Signal, Port>> range)
        {
            foreach(Tuple<Signal, Port> item in range)
                Add(item);
        }
    }
}
