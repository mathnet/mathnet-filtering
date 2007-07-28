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

using MathNet.Symbolics.Library;
using MathNet.Symbolics.Patterns.Toolkit;

namespace MathNet.Symbolics.AutoEvaluation
{
    public class AutoEvaluator : IAutoEvaluator
    {
        ILibrary _library;

        public AutoEvaluator()
        {
            _library = Service<ILibrary>.Instance;
        }

        private IAutoEvaluationTheoremProvider<NodeFlag> GetTheoremProvider(NodeFlag flag)
        {
            ITheoremProvider provider;
            if(!_library.TryLookupTheoremType(flag.Friendly.DerivePostfix("FlagTheorem"), out provider))
                return null;
            return provider as IAutoEvaluationTheoremProvider<NodeFlag>;
        }
        private IAutoEvaluationTheoremProvider<NodeProperty> GetTheoremProvider(NodeProperty property)
        {
            ITheoremProvider provider;
            if(!_library.TryLookupTheoremType(property.Friendly.DerivePostfix("PropertyTheorem"), out provider))
                return null;
            return provider as IAutoEvaluationTheoremProvider<NodeProperty>;
        }

        public void AutoEvaluateFlag(Signal signal, NodeFlag flag)
        {
            IAutoEvaluationTheoremProvider<NodeFlag> provider = GetTheoremProvider(flag);
            if(provider == null)
                return;

            IAutoEvaluationTheorem<NodeFlag> theorem;
            GroupCollection groups;
            if(!provider.TryLookupBest(signal, out theorem, out groups))
                return;

            theorem.AutoEvaluate(signal, groups);
        }

        public void AutoEvaluateFlag(Port port, NodeFlag flag)
        {
            IAutoEvaluationTheoremProvider<NodeFlag> provider = GetTheoremProvider(flag);
            if(provider == null)
                return;

            IAutoEvaluationTheorem<NodeFlag> theorem;
            GroupCollection groups;
            if(!provider.TryLookupBest(port, out theorem, out groups))
                return;

            theorem.AutoEvaluate(port, groups);
        }

        public void AutoEvaluateFlag(Bus bus, NodeFlag flag)
        {
            IAutoEvaluationTheoremProvider<NodeFlag> provider = GetTheoremProvider(flag);
            if(provider == null)
                return;

            IAutoEvaluationTheorem<NodeFlag> theorem;
            GroupCollection groups;
            if(!provider.TryLookupBest(bus, out theorem, out groups))
                return;

            theorem.AutoEvaluate(bus, groups);
        }

        public void AutoEvaluateProperty(Signal signal, NodeProperty property)
        {
            IAutoEvaluationTheoremProvider<NodeProperty> provider = GetTheoremProvider(property);
            if(provider == null)
                return;

            IAutoEvaluationTheorem<NodeProperty> theorem;
            GroupCollection groups;
            if(!provider.TryLookupBest(signal, out theorem, out groups))
                return;

            theorem.AutoEvaluate(signal, groups);
        }

        public void AutoEvaluateProperty(Port port, NodeProperty property)
        {
            IAutoEvaluationTheoremProvider<NodeProperty> provider = GetTheoremProvider(property);
            if(provider == null)
                return;

            IAutoEvaluationTheorem<NodeProperty> theorem;
            GroupCollection groups;
            if(!provider.TryLookupBest(port, out theorem, out groups))
                return;

            theorem.AutoEvaluate(port, groups);
        }

        public void AutoEvaluateProperty(Bus bus, NodeProperty property)
        {
            IAutoEvaluationTheoremProvider<NodeProperty> provider = GetTheoremProvider(property);
            if(provider == null)
                return;

            IAutoEvaluationTheorem<NodeProperty> theorem;
            GroupCollection groups;
            if(!provider.TryLookupBest(bus, out theorem, out groups))
                return;

            theorem.AutoEvaluate(bus, groups);
        }
    }
}
