using GsKit.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GsKit
{
    namespace Service
    {
        public class ServiceLocator : AbstractSingleton<ServiceLocator>, IServiceLocator
        {
            public event EventHandler<IService> ServiceRegistered;

            public event EventHandler<IService> ServiceRemoved;

            protected Dictionary<Type, IService> _services = new();

            public void RegisterService(IService service)
            {
                if (_services.ContainsKey(service.GetType()))
                    throw new ArgumentException($"A service with type {service.GetType()} is already registered.");
                OnServiceRegistered(service);
                _services.Add(service.GetType(), service);
                Debug.Log($"Service added: {service}");
            }

            protected virtual void OnServiceRegistered(IService service)
            {
                ServiceRegistered?.Invoke(this, service);
            }

            public IService GetService(Type type)
            {
                if (!_services.ContainsKey(type))
                    throw new ArgumentException($"A service with type {type} is not registered.");
                return _services[type];
            }

            public T GetService<T>() where T : IService
            {
                return (T)GetService(typeof(T));
            }

            public void RemoveService(Type type)
            {
                if (!_services.ContainsKey(type))
                    throw new ArgumentException($"A service with type {type} is not registered.");
                OnServiceRemoved(_services[type]);
                _services.Remove(type);
            }

            public void RemoveService<T>() where T : IService
            {
                RemoveService(typeof(T));
            }

            protected virtual void OnServiceRemoved(IService service)
            {
                ServiceRemoved?.Invoke(this, service);
            }

            public bool IsServiceRegistered(Type type)
            {
                return _services.ContainsKey(type);
            }

            public bool IsServiceRegistered<T>() where T : IService
            {
                return _services.ContainsKey(typeof(T));
            }
        }
    }
}