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
using System.IO;
using System.Configuration;

using MathNet.Symbolics.Backend.Mapping;

namespace MathNet.Symbolics
{
    public static class Binder
    {
        private static readonly Mapper _mapper = new Mapper();

        private static readonly IFactory<Signal> _cacheSignal;
        private static readonly IFactory<Signal, IValueStructure> _cacheSignal2;
        private static readonly IFactory<Bus> _cacheBus;
        private static readonly IFactory<Bus, IValueStructure> _cacheBus2;
        private static readonly IFactory<Port, IEntity> _cachePort;
        private static readonly IFactory<Port, IEntity, IEnumerable<Signal>> _cachePort2;
        private static readonly IFactory<IMathSystem> _cacheSystem;

        static Binder()
        {
            LoadBindings();
            _cacheSignal = _mapper.GetFactory<Signal>();
            _cacheSignal2 = _mapper.GetFactory<Signal, IValueStructure>();
            _cacheBus = _mapper.GetFactory<Bus>();
            _cacheBus2 = _mapper.GetFactory<Bus, IValueStructure>();
            _cachePort = _mapper.GetFactory<Port, IEntity>();
            _cachePort2 = _mapper.GetFactory<Port, IEntity, IEnumerable<Signal>>();
            _cacheSystem = _mapper.GetFactory<IMathSystem>();
        }

        #region Loading Bindings
        public static void LoadBindings()
        {
            IList<RawBinding> bindings
                = System.Configuration.ConfigurationManager.GetSection("yttrium/objects") as IList<RawBinding>;
            if(bindings == null)
            {
                FileInfo fi = new FileInfo("yttrium.mapping.config");
                if(fi.Exists)
                {
                    XmlMappingAdapter xma = new XmlMappingAdapter();
                    bindings = xma.Load(fi.FullName);
                }
            }
            if(bindings != null)
                _mapper.ImportBindings(bindings);
        }
        public static void LoadBindings(string sourceFilename)
        {
            XmlMappingAdapter xma = new XmlMappingAdapter();
            IList<RawBinding> bindings = xma.Load(sourceFilename);
            if(bindings != null)
                _mapper.ImportBindings(bindings);
        }
        #endregion

        #region get arbitrary instances
        public static T GetSpecificInstance<T>(MathIdentifier id)
        {
            IFactory<T> factory = _mapper.GetFactory<T>(id);
            return factory.GetInstance();
        }
        public static T GetInstance<T>()
        {
            IFactory<T> factory = _mapper.GetFactory<T>();
            return factory.GetInstance();
        }

        //public static T GetSpecificInstance<T>(MathIdentifier id, params object[] parameters)
        //{
        //    IFactory<T> factory = _mapper.GetFactory<T>(id);
        //    return factory.GetInstance(parameters);
        //}
        //public static T GetInstance<T>(params object[] parameters)
        //{
        //    IFactory<T> factory = _mapper.GetFactory<T>();
        //    return factory.GetInstance(parameters);
        //}

        #region Parametrized

        /*
         * Note: The following list of factories is quite dumb ("WTF"),
         * but it greatly simplifies implementing them and makes it
         * type-safe once you have your factory (compared to the object[]
         * approach where you have to check types and cast/convert manually).
         * 
         * However, let me know if you have a better idea!
         * 
         * [maybe I should check out dependency injection (-> spring.net etc.) ?]
         */

        public static T GetInstance<T, T1>(T1 p1)
        {
            return _mapper.GetFactory<T, T1>().GetInstance(p1);
        }
        public static T GetInstance<T, T1, T2>(T1 p1, T2 p2)
        {
            return _mapper.GetFactory<T, T1, T2>().GetInstance(p1, p2);
        }
        public static T GetInstance<T, T1, T2, T3>(T1 p1, T2 p2, T3 p3)
        {
            return _mapper.GetFactory<T, T1, T2, T3>().GetInstance(p1, p2, p3);
        }
        public static T GetInstance<T, T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4)
        {
            return _mapper.GetFactory<T, T1, T2, T3, T4>().GetInstance(p1, p2, p3, p4);
        }
        public static T GetInstance<T, T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            return _mapper.GetFactory<T, T1, T2, T3, T4, T5>().GetInstance(p1, p2, p3, p4, p5);
        }
        public static T GetInstance<T, T1, T2, T3, T4, T5, T6>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
        {
            return _mapper.GetFactory<T, T1, T2, T3, T4, T5, T6>().GetInstance(p1, p2, p3, p4, p5, p6);
        }
        public static T GetInstance<T, T1, T2, T3, T4, T5, T6, T7>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
        {
            return _mapper.GetFactory<T, T1, T2, T3, T4, T5, T6, T7>().GetInstance(p1, p2, p3, p4, p5, p6, p7);
        }
        public static T GetInstance<T, T1, T2, T3, T4, T5, T6, T7, T8>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
        {
            return _mapper.GetFactory<T, T1, T2, T3, T4, T5, T6, T7, T8>().GetInstance(p1, p2, p3, p4, p5, p6, p7, p8);
        }

        public static T GetSpecificInstance<T, T1>(MathIdentifier id, T1 p1)
        {
            return _mapper.GetFactory<T, T1>(id).GetInstance(p1);
        }
        public static T GetSpecificInstance<T, T1, T2>(MathIdentifier id, T1 p1, T2 p2)
        {
            return _mapper.GetFactory<T, T1, T2>(id).GetInstance(p1, p2);
        }
        public static T GetSpecificInstance<T, T1, T2, T3>(MathIdentifier id, T1 p1, T2 p2, T3 p3)
        {
            return _mapper.GetFactory<T, T1, T2, T3>(id).GetInstance(p1, p2, p3);
        }
        public static T GetSpecificInstance<T, T1, T2, T3, T4>(MathIdentifier id, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            return _mapper.GetFactory<T, T1, T2, T3, T4>(id).GetInstance(p1, p2, p3, p4);
        }
        public static T GetSpecificInstance<T, T1, T2, T3, T4, T5>(MathIdentifier id, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            return _mapper.GetFactory<T, T1, T2, T3, T4, T5>(id).GetInstance(p1, p2, p3, p4, p5);
        }
        public static T GetSpecificInstance<T, T1, T2, T3, T4, T5, T6>(MathIdentifier id, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
        {
            return _mapper.GetFactory<T, T1, T2, T3, T4, T5, T6>(id).GetInstance(p1, p2, p3, p4, p5, p6);
        }
        public static T GetSpecificInstance<T, T1, T2, T3, T4, T5, T6, T7>(MathIdentifier id, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
        {
            return _mapper.GetFactory<T, T1, T2, T3, T4, T5, T6, T7>(id).GetInstance(p1, p2, p3, p4, p5, p6, p7);
        }
        public static T GetSpecificInstance<T, T1, T2, T3, T4, T5, T6, T7, T8>(MathIdentifier id, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
        {
            return _mapper.GetFactory<T, T1, T2, T3, T4, T5, T6, T7, T8>(id).GetInstance(p1, p2, p3, p4, p5, p6, p7, p8);
        }
        #endregion
        #endregion

        #region get special instances
        public static Signal CreateSignal()
        {
            return _cacheSignal.GetInstance();
        }
        public static Signal CreateSignal(IValueStructure value)
        {
            return _cacheSignal2.GetInstance(value);
        }

        public static Bus CreateBus()
        {
            return _cacheBus.GetInstance();
        }
        public static Bus CreateBus(IValueStructure value)
        {
            return _cacheBus2.GetInstance(value);
        }

        public static Port CreatePort(IEntity entity)
        {
            return _cachePort.GetInstance(entity);
        }

        public static Port CreatePort(IEntity entity, IEnumerable<Signal> outputSignals)
        {
            return _cachePort2.GetInstance(entity, outputSignals);
        }

        public static IMathSystem CreateSystem()
        {
            return _cacheSystem.GetInstance();
        }
        #endregion
    }
}
