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
using System.Reflection;

using MathNet.Symbolics.Exceptions;
using MathNet.Symbolics.Properties;

namespace MathNet.Symbolics.Kernel
{
    internal sealed class Mapper
    {
        private Dictionary<MathIdentifier, object> _factoryById;
        private Dictionary<string, object> _factoryByType;

        internal Mapper()
        {
            _factoryById = new Dictionary<MathIdentifier, object>();
            _factoryByType = new Dictionary<string, object>();
        }

        private object LookupFactory(Type type)
        {
            string typeName = type.FullName;
            object factory;

            if(!_factoryByType.TryGetValue(typeName, out factory))
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingUnknownType, typeName));

            return factory;
        }

        private object LookupFactory(MathIdentifier id)
        {
            object factory;

            if(!_factoryById.TryGetValue(id, out factory))
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingUnknownId, id.ToString()));

            return factory;
        }

        #region Type-Safe Accessors
        public IFactory<T> GetFactory<T>(MathIdentifier id)
        {
            return (IFactory<T>)LookupFactory(id);
        }

        public IFactory<T> GetFactory<T>()
        {
            return (IFactory<T>)LookupFactory(typeof(T));
        }

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

        public IFactory<T, T1> GetFactory<T, T1>()
        {
            IFactory<T, T1> factory = LookupFactory(typeof(T)) as IFactory<T, T1>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2> GetFactory<T, T1, T2>()
        {
            IFactory<T, T1, T2> factory = LookupFactory(typeof(T)) as IFactory<T, T1, T2>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2, T3> GetFactory<T, T1, T2, T3>()
        {
            IFactory<T, T1, T2, T3> factory = LookupFactory(typeof(T)) as IFactory<T, T1, T2, T3>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2, T3, T4> GetFactory<T, T1, T2, T3, T4>()
        {
            IFactory<T, T1, T2, T3, T4> factory = LookupFactory(typeof(T)) as IFactory<T, T1, T2, T3, T4>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2, T3, T4, T5> GetFactory<T, T1, T2, T3, T4, T5>()
        {
            IFactory<T, T1, T2, T3, T4, T5> factory = LookupFactory(typeof(T)) as IFactory<T, T1, T2, T3, T4, T5>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2, T3, T4, T5, T6> GetFactory<T, T1, T2, T3, T4, T5, T6>()
        {
            IFactory<T, T1, T2, T3, T4, T5, T6> factory = LookupFactory(typeof(T)) as IFactory<T, T1, T2, T3, T4, T5, T6>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2, T3, T4, T5, T6, T7> GetFactory<T, T1, T2, T3, T4, T5, T6, T7>()
        {
            IFactory<T, T1, T2, T3, T4, T5, T6, T7> factory = LookupFactory(typeof(T)) as IFactory<T, T1, T2, T3, T4, T5, T6, T7>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2, T3, T4, T5, T6, T7, T8> GetFactory<T, T1, T2, T3, T4, T5, T6, T7, T8>()
        {
            IFactory<T, T1, T2, T3, T4, T5, T6, T7, T8> factory = LookupFactory(typeof(T)) as IFactory<T, T1, T2, T3, T4, T5, T6, T7, T8>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }

        public IFactory<T, T1> GetFactory<T, T1>(MathIdentifier id)
        {
            IFactory<T, T1> factory = LookupFactory(id) as IFactory<T, T1>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2> GetFactory<T, T1, T2>(MathIdentifier id)
        {
            IFactory<T, T1, T2> factory = LookupFactory(id) as IFactory<T, T1, T2>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2, T3> GetFactory<T, T1, T2, T3>(MathIdentifier id)
        {
            IFactory<T, T1, T2, T3> factory = LookupFactory(id) as IFactory<T, T1, T2, T3>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2, T3, T4> GetFactory<T, T1, T2, T3, T4>(MathIdentifier id)
        {
            IFactory<T, T1, T2, T3, T4> factory = LookupFactory(id) as IFactory<T, T1, T2, T3, T4>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2, T3, T4, T5> GetFactory<T, T1, T2, T3, T4, T5>(MathIdentifier id)
        {
            IFactory<T, T1, T2, T3, T4, T5> factory = LookupFactory(id) as IFactory<T, T1, T2, T3, T4, T5>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2, T3, T4, T5, T6> GetFactory<T, T1, T2, T3, T4, T5, T6>(MathIdentifier id)
        {
            IFactory<T, T1, T2, T3, T4, T5, T6> factory = LookupFactory(id) as IFactory<T, T1, T2, T3, T4, T5, T6>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2, T3, T4, T5, T6, T7> GetFactory<T, T1, T2, T3, T4, T5, T6, T7>(MathIdentifier id)
        {
            IFactory<T, T1, T2, T3, T4, T5, T6, T7> factory = LookupFactory(id) as IFactory<T, T1, T2, T3, T4, T5, T6, T7>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        public IFactory<T, T1, T2, T3, T4, T5, T6, T7, T8> GetFactory<T, T1, T2, T3, T4, T5, T6, T7, T8>(MathIdentifier id)
        {
            IFactory<T, T1, T2, T3, T4, T5, T6, T7, T8> factory = LookupFactory(id) as IFactory<T, T1, T2, T3, T4, T5, T6, T7, T8>;
            if(factory == null)
                throw new MicrokernelException(
                    string.Format(Config.UserCulture, Resources.BindingWrongTypeParameters, typeof(T).FullName));
            return factory;
        }
        #endregion
        #endregion

        public void AddBinding<T>(IFactory<T> factory, MathIdentifier id)
        {
            _factoryById[id] = factory;
            _factoryByType[typeof(T).FullName] = factory;
        }
        public void AddBinding(object factory, Type contractType, MathIdentifier id)
        {
            _factoryById[id] = factory;
            _factoryByType[contractType.FullName] = factory;
        }

        internal void ImportBindings(IEnumerable<RawBinding> bindings)
        {
            Dictionary<string, object> factoryCache = new Dictionary<string, object>();

            foreach(RawBinding binding in bindings)
            {
                MathIdentifier id = MathIdentifier.Parse(binding.id);
                Type contractType;
                try
                {
                    contractType = Type.GetType(binding.contractType, true);
                }
                catch(TypeLoadException e)
                {// usually a bad practice to repack exceptions,
                    // but since TypeLoadException isn't that helpful, this will help the users more:
                    throw new MicrokernelException(
                        string.Format(Config.UserCulture, Resources.BindingContractLoadFailed, binding.comment, binding.id, binding.contractType), e);
                }
                object factory;
                if(!factoryCache.TryGetValue(binding.factoryType, out factory))
                {
                    Type factoryType;
                    try
                    {
                        factoryType = Type.GetType(binding.factoryType, true);
                    }
                    catch(TypeLoadException e)
                    {
                        // usually a bad practice to repack exceptions,
                        // but since TypeLoadException isn't that helpful, this will help the users more:
                        throw new MicrokernelException(
                            string.Format(Config.UserCulture, Resources.BindingFactoryLoadFailed, binding.comment, binding.id, binding.factoryType), e);
                    }
                    //if(factoryType == null)
                    //    continue;
                    ConstructorInfo ctor = factoryType.GetConstructor(new Type[] { });
                    if(ctor == null)
                        continue;
                    factory = ctor.Invoke(new object[] { });
                    factoryCache.Add(binding.factoryType, factory);
                }
                AddBinding(factory, contractType, id);
            }
        }
    }
}
