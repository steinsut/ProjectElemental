using System;

namespace GsKit.Singleton
{
    public abstract class AbstractSingleton<T> where T : class
    {
        private static readonly Lazy<T> s_lazyInstance = new(() =>
        {
            return (T)Activator.CreateInstance(typeof(T), true);
        });

        public static T Instance => s_lazyInstance.Value;
    }
}