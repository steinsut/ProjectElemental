using GsKit.Extensions;
using System;
using UnityEngine;

namespace GsKit.Singleton
{
    public abstract class AbstractSingletonBehaviour<T> : MonoBehaviour where T : AbstractSingletonBehaviour<T>
    {
        protected static Lazy<T> s_lazyInstance = new(CreateInstance);

        private static T s_instance = null;

        private static T CreateInstance()
        {
            if (s_instance == null)
            {
                GameObject gameObject = new GameObject(typeof(T).Name);
                T component = gameObject.AddComponent<T>();
                return component;
            }
            return s_instance;
        }

        private void Start()
        {
            if (s_instance == null)
            {
                DontDestroyOnLoad(this);
                s_instance = (T)this;
                BehaviourStart();
            }
            else
            {
                this.LogError($"An instance of {typeof(T)} already exists. Destroying.");
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (s_instance == (T)this)
            {
                BehaviourDestroy();
                s_instance = null;
                s_lazyInstance = new Lazy<T>(CreateInstance);
            }
        }

        protected virtual void BehaviourStart()
        {
        }

        protected virtual void BehaviourDestroy()
        {
        }
    }
}