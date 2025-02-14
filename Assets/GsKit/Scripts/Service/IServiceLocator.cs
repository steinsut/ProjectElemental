using System;

namespace GsKit.Service
{
    public interface IServiceLocator
    {
        void RegisterService(IService service);

        IService GetService(Type type);

        T GetService<T>() where T : IService;

        void RemoveService(Type type);

        void RemoveService<T>() where T : IService;

        bool IsServiceRegistered(Type type);

        bool IsServiceRegistered<T>() where T : IService;
    }
}