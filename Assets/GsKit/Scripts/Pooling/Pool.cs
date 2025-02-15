using System;
using System.Collections.Generic;
using UnityEngine;
using GsKit.Utils;
using UObject = UnityEngine.Object;

namespace GsKit.Pooling
{
    public class Pool
    {
        private PoolResource _poolResource;
        private Dictionary<Type, int> _typeIndex = new();
        private List<Type> _typesToCache = new();
        private Queue<Guid> _objectGuidQueue = new();
        private Dictionary<Guid, GameObject> _objects = new();
        private Dictionary<Guid, List<Component>> _objectComponents = new();
        private GameObject _parent;

        private int _minObjectCount = 0;
        private int _maxObjectCount = 0;
        private bool _limitObjectCount = false;
        private bool _resetOnReturn = false;

        public class Object
        {
            private Guid _guid;
            private Pool _pool;

            public Object(Pool pool, Guid guid)
            {
                _guid = guid;
                _pool = pool;
            }

            public Pool ObjectPool => _pool;

            public GameObject GetGameObject()
            {
                return _pool.GetGameObject(_guid);
            }

            public T GetComponent<T>() where T : Component
            {
                return (T)_pool.GetComponent(_guid, typeof(T));
            }

            public Component GetComponent(Type type)
            {
                return _pool.GetComponent(_guid, type);
            }

            public IList<Component> GetComponents(params Type[] types)
            {
                return _pool.GetComponents(_guid, types);
            }

            public IList<Component> GetAllCachedComponents()
            {
                return _pool.GetAllCachedComponents(_guid);
            }

            public void ReturnToPool()
            {
                _pool.ReturnObjectToPool(_guid);
            }
        }

        public bool ResetOnReturn
        {
            get => _resetOnReturn;
            set => _resetOnReturn = value;
        }

        public bool LimitObjectCount
        {
            get => _limitObjectCount;
            set
            {
                _limitObjectCount = value;
                if (value && _objects.Count > _maxObjectCount && _objectGuidQueue.Count >= _objects.Count - _maxObjectCount)
                {
                    for (int i = 0; i < _objects.Count - _maxObjectCount; i++)
                    {
                        Guid guid = _objectGuidQueue.Dequeue();

                        UObject.Destroy(_objects[guid]);
                        _objects.Remove(guid);
                        _objectComponents.Remove(guid);
                    }
                }
            }
        }

        public int MinObjectCount
        {
            get => _minObjectCount;
            set
            {
                _minObjectCount = value;
                if (value > _maxObjectCount) _maxObjectCount = value;
            }
        }

        public int MaxObjectCount
        {
            get => _maxObjectCount;
            set
            {
                if (value >= _minObjectCount) _maxObjectCount = value;
            }
        }

        public event EventHandler PoolPopulated;

        public Pool(PoolResource poolResource)
        {
            _poolResource = poolResource;
            Initialize();
        }

        private void Initialize()
        {
            GameObject prefab = _poolResource.PrefabResource.Prefab;

            _parent = new GameObject($"{prefab.name} Pool");
            if(_poolResource.PreserveOnSceneLoad)
            {
                UObject.DontDestroyOnLoad(_parent);
            }

            MinObjectCount = _poolResource.MinimumObjects;
            MaxObjectCount = _poolResource.MaximumObjects;
            LimitObjectCount = _poolResource.LimitObjectCount;
            ResetOnReturn = _poolResource.ResetOnReturn;

            List<Component> components = new List<Component>(prefab.GetComponents<Component>());
            for (int i = 0; i < components.Count; i++)
            {
                Type componentType = components[i].GetType();
                _typeIndex.Add(componentType, i);
            }

            List<char> componentsMaskBits = new List<char>(Convert.ToString(_poolResource.ComponentMask, 2).ToCharArray());
            int length = componentsMaskBits.Count;
            componentsMaskBits.Reverse();

            if (_poolResource.ComponentMask == -1) length = components.Count;

            for (int i = 0; i < length; i++)
            {
                if (componentsMaskBits[i] == '1') _typesToCache.Add(components[i].GetType());
            }
        }

        private void AddNewObjectToPool()
        {
            GameObject obj = UObject.Instantiate(_poolResource.PrefabResource.Prefab, _parent.transform);
            obj.name = _poolResource.PrefabResource.Prefab.name + (_objects.Count + 1);
            Guid guid = Guid.NewGuid();
            _objects.Add(guid, obj);
            List<Component> newComponents = new List<Component>(_typesToCache.Count);
            foreach (Type type in _typeIndex.Keys)
            {
                if (_typesToCache.Contains(type)) newComponents.Add(obj.GetComponent(type));
                else newComponents.Add(null);
            }
            _objectComponents.Add(guid, newComponents);
            _objectGuidQueue.Enqueue(guid);
        }

        public Object GetNextObject()
        {
            Guid guid = Guid.Empty;
            if (_objectGuidQueue.Count != 0) guid = _objectGuidQueue.Dequeue();

            if (_objects.Count >= _maxObjectCount && !_limitObjectCount
                || _objects.Count < _maxObjectCount)
            {
                AddNewObjectToPool();
                guid = _objectGuidQueue.Dequeue();
            }
            if (guid == Guid.Empty) throw new InvalidOperationException($"All objects in queue are in use, and there is no space to create any more.");

            return new Object(this, guid);
        }

        private GameObject GetGameObject(Guid objectGuid)
        {
            if (!_objectGuidQueue.Contains(objectGuid))
            {
                if (_objects.ContainsKey(objectGuid)) return _objects[objectGuid];

                throw new ArgumentException($"No object with with GUID \"{objectGuid}\".");
            }
            throw new ArgumentException($"Object with GUID \"${objectGuid}\" is in queue.");
        }

        private Component GetComponent(Guid objectGuid, Type type)
        {
            if (!_objectGuidQueue.Contains(objectGuid))
            {
                if (_objects.ContainsKey(objectGuid))
                {
                    if (_typeIndex.ContainsKey(type)) return _objectComponents[objectGuid][_typeIndex[type]];
                    return _objects[objectGuid].GetComponent(type);
                }
                throw new ArgumentException($"No object with with GUID \"{objectGuid}\".");
            }
            else
            {
                if (!_objects.ContainsKey(objectGuid)) throw new ArgumentException($"No object with with GUID \"${objectGuid}\".");
                throw new ArgumentException($"Object with GUID \"${objectGuid}\" is in queue.");
            }
        }

        private T GetComponent<T>(Guid objectGuid) where T : Component
        {
            return (T)GetComponent(objectGuid, typeof(T));
        }

        private IList<Component> GetComponents(Guid objectGuid, params Type[] types)
        {
            List<Component> components = new List<Component>(types.Length);
            if (!_objectGuidQueue.Contains(objectGuid))
            {
                if (_objects.ContainsKey(objectGuid))
                {
                    foreach (Type type in types)
                    {
                        if (_typeIndex.ContainsKey(type)) components.Add(_objectComponents[objectGuid][_typeIndex[type]]);
                        components.Add(_objects[objectGuid].GetComponent(type));
                    }
                    return components;
                }
                throw new ArgumentException($"No object with with GUID \"{objectGuid}\".");
            }
            else
            {
                if (!_objects.ContainsKey(objectGuid)) throw new ArgumentException($"No object with with GUID \"${objectGuid}\".");
                throw new ArgumentException($"Object with GUID \"${objectGuid}\" is in queue.");
            }
        }

        private IList<Component> GetAllCachedComponents(Guid objectGuid)
        {
            if (!_objectGuidQueue.Contains(objectGuid))
            {
                if (_objects.ContainsKey(objectGuid)) return _objectComponents[objectGuid].AsReadOnly();
                throw new ArgumentException($"No object with with GUID \"{objectGuid}\".");
            }
            else
            {
                if (!_objects.ContainsKey(objectGuid)) throw new ArgumentException($"No object with with GUID \"${objectGuid}\".");
                throw new ArgumentException($"Object with GUID \"${objectGuid}\" is in queue.");
            }
        }

        private void CacheNewType(Type type)
        {
            if (!type.IsSubclassOf(typeof(Component))) throw new ArgumentException($"Type {type} is not a subclass of Component.");
            if (_typesToCache.Contains(type)) throw new ArgumentException($"Type {type} is already cached.");

            if (!_typeIndex.ContainsKey(type)) _typeIndex.Add(type, _typeIndex.Count);

            _typesToCache.Add(type);
            foreach (KeyValuePair<Guid, GameObject> pair in _objects)
            {
                Component component = pair.Value.AddComponent(type);
                _objectComponents[pair.Key].Add(component);
            }
        }

        public void CacheNewType<T>() where T : Component
        {
            CacheNewType(typeof(T));
        }

        public void RemoveTypeFromCache(Type type)
        {
            if (!type.IsSubclassOf(typeof(Component))) throw new ArgumentException($"Type {type} is not a subclass of Component.");
            if (!_typesToCache.Contains(type)) throw new ArgumentException($"Type {type} is not cached.");

            _typesToCache.Remove(type);
            foreach (KeyValuePair<Guid, GameObject> pair in _objects) _objectComponents[pair.Key][_typeIndex[type]] = null;
        }

        public void RemoveTypeFromCache<T>() where T : Component
        {
            RemoveTypeFromCache(typeof(T));
        }

        public IList<Type> GetCachedTypes()
        {
            return _typesToCache.AsReadOnly();
        }

        public void Populate()
        {
            if (_objects.Count < _minObjectCount)
            {
                int objectsToCreate = _minObjectCount - _objects.Count;
                for (int i = 0; i < objectsToCreate; i++) AddNewObjectToPool();
                OnPoolPopulated(EventArgs.Empty);
            }
        }

        protected virtual void OnPoolPopulated(EventArgs e)
        {
            PoolPopulated?.Invoke(this, e);
        }

        public bool IsPopulated()
        {
            return _objects.Count >= _minObjectCount;
        }

        public void Repopulate()
        {
            Clear();
            Populate();
        }

        private void ReturnObjectToPool(Guid objectGuid)
        {
            if (!_objectGuidQueue.Contains(objectGuid))
            {
                if (_objects.ContainsKey(objectGuid))
                {
                    if (_limitObjectCount && _objects.Count >= MaxObjectCount)
                    {
                        DestroyObject(objectGuid);
                    }
                    else
                    {
                        Guid newGuid = Guid.NewGuid();
                        GameObject obj = _objects[objectGuid];
                        obj.SetActive(_poolResource.PrefabResource.Prefab.activeSelf);
                        if (_resetOnReturn)
                        {
                            foreach (Component component in _objectComponents[objectGuid])
                            {
                                if (typeof(IResettable).IsAssignableFrom(component.GetType())) ((IResettable)component).Reset();
                            }
                        }
                        _objectComponents.Add(newGuid, _objectComponents[objectGuid]);
                        _objectComponents.Remove(objectGuid);
                        _objects.Add(newGuid, obj);
                        _objects.Remove(objectGuid);
                        _objectGuidQueue.Enqueue(newGuid);
                    }
                }
                else throw new ArgumentException($"There is no object with with GUID \"{objectGuid}\".");
            }
            else throw new ArgumentException($"Object with GUID \"{objectGuid}\" is in queue.");
        }

        private void DestroyObject(Guid objectGuid)
        {
            if (!_objectGuidQueue.Contains(objectGuid))
            {
                if (_objects.ContainsKey(objectGuid))
                {
                    if (_objects[objectGuid] != null)
                    {
                        UObject.Destroy(_objects[objectGuid]);
                    }
                    _objectComponents.Remove(objectGuid);
                    _objects.Remove(objectGuid);
                }
                else throw new ArgumentException($"There is no object with with GUID \"{objectGuid}\".");
            }
            else throw new ArgumentException($"Object with GUID \"{objectGuid}\" is in queue.");
        }

        public void Clear()
        {
            _objectGuidQueue.Clear();
            _objectComponents.Clear();
            _objects.Clear();
            UObject.Destroy(_parent);
        }

        public void Reinitialize()
        {
            _typeIndex.Clear();
            _typesToCache.Clear();
            Clear();
            Initialize();
        }

        public override string ToString()
        {
            return $"[Pool: ${_poolResource.ResourceID}]";
        }

        ~Pool()
        {
            _typeIndex.Clear();
            _typesToCache.Clear();
            Clear();
        }
    }
}