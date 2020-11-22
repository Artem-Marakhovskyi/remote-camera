using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using RemoteCameraControl.Logger;
using XLabs.Ioc;

namespace RemoteCameraControl.Android.RemoteCameraControl.Ioc
{
    public class Resolver : IResolver
    {
        private readonly IContainer _container;
        private readonly ILogger _logger;

        public Resolver(
            IContainer container,
            ILogger logger)
        {
            _container = container;
            _logger = logger;
        }

        public T Resolve<T>() where T : class => (T)Resolve(typeof(T));
        public object Resolve(Type type)
        {
            _logger.LogInfo($"Resolving {type.FullName}");
            return _container.Resolve(type);
        }

        public IEnumerable<T> ResolveAll<T>() where T : class => ResolveAll(typeof(T)).Cast<T>();

        public IEnumerable<object> ResolveAll(Type type)
        {
            _logger.LogInfo($"Resolving multiple of type {type.FullName}");
            Type listType = typeof(IEnumerable<>);
            Type[] typeArgs = { type };
            Type typeConstructed = listType.MakeGenericType(typeArgs);

            var dependencies = (IEnumerable<object>)_container.ResolveOptional(typeConstructed);

            return dependencies;
        }

        public bool IsRegistered(Type type) => _container.IsRegistered(type);

        public bool IsRegistered<T>() where T : class => IsRegistered(typeof(T));
    }
}