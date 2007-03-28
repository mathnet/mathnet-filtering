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

using MathNet.Symbolics.Mediator;

namespace MathNet.Symbolics
{
    internal class MediatorFactory :
        IFactory<IMediator>,
        IFactory<ISystemMediator>,
        IFactory<ISystemMediator, IMathSystem>
    {
        //private static IMediator _mediator = new Mediator.Mediator();

        IMediator IFactory<IMediator>.GetInstance()
        {
            return Mediator.Mediator.Instance;
            //return _mediator;
        }

        public ISystemMediator GetInstance()
        {
            return new SystemMediator();
        }

        public ISystemMediator GetInstance(IMathSystem p1)
        {
            ISystemMediatorSource source = p1 as ISystemMediatorSource;
            if(source == null)
            {
                SystemMediator sm = new SystemMediator();
                sm.SubscribeSystem(p1);
                return sm;
            }
            if(source.HasSystemMediator)
                return source.SystemMediator;
            else
            {
                SystemMediator sm = new SystemMediator();
                source.SystemMediator = sm;
                sm.SubscribeSystem(p1); // probably redundant, but doesn't matter (subscribe is robust).
                return sm;
            }
        }
    }
}
