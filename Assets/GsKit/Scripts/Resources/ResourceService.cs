using GsKit.Extensions;
using GsKit.Service;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GsKit.Resources
{
    public class ResourceService : MonoBehaviour, IService
    {
        private Dictionary<string, IList<string>> _resourceGroups = new();
        private Dictionary<string, AsyncOperationHandle> _handles = new();
        private Dictionary<string, AbstractResource> _resources = new();
        private Dictionary<string, bool> _isLoaded = new();

        public event EventHandler<AbstractResource> ResourceLoaded;

        public event EventHandler<ResourceGroupEventArgs> ResourceGroupLoaded;

        public event EventHandler<string> ResourceGroupLoadFailed;

        public event EventHandler<ResourceGroupEventArgs> ResourceGroupUnloaded;

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService(this);
            Clear();
        }

        public AsyncOperationHandle<IList<AbstractResource>> LoadResourceGroup(string groupName)
        {
            if (_resourceGroups.ContainsKey(groupName)) throw new InvalidOperationException(groupName + "is already loaded!");
            this.LogInfo($"Loading resource group \"{groupName}\".");

            _isLoaded.Add(groupName, false);

            List<string> ids = new List<string>();
            _resourceGroups.Add(groupName, ids);

            AsyncOperationHandle<IList<AbstractResource>> handle = Addressables.LoadAssetsAsync<AbstractResource>(groupName, (resource) =>
            {
                if (string.IsNullOrEmpty(resource.ResourceID)) throw new ArgumentException($"Resource has no id. Unity object name: {resource.name}");
                if (!_resources.ContainsKey(resource.ResourceID))
                {
                    _resources.Add(resource.ResourceID, resource);
                    ids.Add(resource.ResourceID);

                    OnResourceLoaded(resource);
                }
                else
                {
                    throw new ArgumentException("Duplicate resource ID found: " +
                                                $"\"{resource.ResourceID}\" " +
                                                $"between:\n{resource}\n{_resources[resource.ResourceID]}");
                }
            });
            _handles.Add(groupName, handle);
            handle.Completed += (AsyncOperationHandle<IList<AbstractResource>> handle) =>
            {
                _isLoaded[groupName] = true;
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    this.LogSuccess($"Successfully loaded group \"{groupName}\".");
                    if (ResourceGroupLoaded != null)
                    {
                        ResourceGroupEventArgs args = new ResourceGroupEventArgs
                        {
                            ResourceGroupName = groupName
                        };

                        Dictionary<string, AbstractResource> res = new Dictionary<string, AbstractResource>();
                        foreach (string id in _resourceGroups[groupName]) res.Add(id, _resources[id]);
                        args.Resources = res;

                        OnResourceGroupLoaded(args);
                    }
                }
                else if (handle.Status == AsyncOperationStatus.Failed)
                {
                    this.LogError($"Loading failed with exception: {handle.OperationException.Message}");

                    foreach (string id in _resourceGroups[groupName]) _resources.Remove(id);
                    _handles.Remove(groupName);
                    _resourceGroups.Remove(groupName);
                    _isLoaded.Remove(groupName);
                    Addressables.Release(handle);

                    OnResourceGroupLoadFailed(groupName);
                }
            };
            return handle;
        }

        public void UnloadResourceGroup(string groupName)
        {
            if (!_resourceGroups.ContainsKey(groupName))
                throw new ArgumentException($"No group with name \"{groupName}\" is loaded.");
            this.LogInfo($"Unloading resource group \"{groupName}\".");
            foreach (string id in _resourceGroups[groupName])
                _resources.Remove(id);
            if (ResourceGroupUnloaded != null)
            {
                ResourceGroupEventArgs args = new ResourceGroupEventArgs();
                args.ResourceGroupName = groupName;
                Dictionary<string, AbstractResource> res = new Dictionary<string, AbstractResource>();
                foreach (string id in _resourceGroups[groupName])
                    res.Add(id, _resources[id]);
                args.Resources = res;
                OnResourceGroupUnloaded(args);
            }
            _resourceGroups.Remove(groupName);
            AsyncOperationHandle handle = _handles[groupName];
            _handles.Remove(groupName);
            _isLoaded.Remove(groupName);
            Addressables.Release(handle);
        }

        protected virtual void OnResourceLoaded(AbstractResource resource)
        {
            ResourceLoaded?.Invoke(this, resource);
        }

        protected virtual void OnResourceGroupLoaded(ResourceGroupEventArgs e)
        {
            ResourceGroupLoaded?.Invoke(this, e);
        }

        protected virtual void OnResourceGroupLoadFailed(string groupName)
        {
            ResourceGroupLoadFailed?.Invoke(this, groupName);
        }

        protected virtual void OnResourceGroupUnloaded(ResourceGroupEventArgs e)
        {
            ResourceGroupUnloaded?.Invoke(this, e);
        }

        public IList<string> GetAllResourceGroupNames()
        {
            return new List<string>(_resourceGroups.Keys);
        }

        public bool HasResourceGroup(string groupName)
        {
            return _resourceGroups.ContainsKey(groupName);
        }

        public bool IsResourceGroupLoaded(string groupName)
        {
            if (_isLoaded.ContainsKey(groupName)) throw new ArgumentException($"No group with name \"{groupName}\" was found in the loading list.");

            return _isLoaded[groupName];
        }

        public bool AreAllGroupsLoaded()
        {
            return _isLoaded.ContainsValue(false);
        }

        public bool HasResource(string resourceId)
        {
            return _resources.ContainsKey(resourceId);
        }

        public AbstractResource GetResource(string resourceId)
        {
            if (!_resources.ContainsKey(resourceId)) throw new ArgumentException($"There is no resource with ID \"{resourceId}\".");

            return _resources[resourceId];
        }

        public T GetResource<T>(string resourceId) where T : AbstractResource
        {
            if (!_resources.ContainsKey(resourceId)) throw new ArgumentException($"There is no resource with ID \"{resourceId}\".");

            AbstractResource resource = _resources[resourceId];
            if (!typeof(T).Equals(resource.GetType())) throw new InvalidCastException($"\"{resource.ResourceID}\" is not of type \r{typeof(T)}\r.");

            return (T)resource;
        }

        public IList<AbstractResource> GetResources(params string[] resourceIds)
        {
            List<AbstractResource> res = new List<AbstractResource>(resourceIds.Length);
            foreach (string id in resourceIds)
            {
                if (!_resources.ContainsKey(id)) throw new ArgumentException($"There is no resource with ID \"{id}\".");
                res.Add(_resources[id]);
            }
            return res;
        }

        public IList<T> GetResources<T>(params string[] resourceIds) where T : AbstractResource
        {
            List<T> res = new List<T>(resourceIds.Length);
            foreach (string id in resourceIds)
            {
                if (!_resources.ContainsKey(id)) throw new ArgumentException($"There is no resource with ID \"{id}\".");
                AbstractResource resource = _resources[id];

                if (!resource.GetType().Equals(typeof(T))) throw new InvalidCastException($"\"{resource.ResourceID}\" is not of type: {typeof(T)}");
            }
            return res;
        }

        public IList<AbstractResource> GetResourcesFromGroup(string groupName)
        {
            if (!_resources.ContainsKey(groupName)) throw new ArgumentException($"There is no group with ID \"{groupName}\".");

            List<AbstractResource> res = new List<AbstractResource>(_resourceGroups[groupName].Count);
            foreach (string id in _resourceGroups[groupName]) res.Add(_resources[id]);

            return res;
        }

        public IList<T> GetResourcesFromGroup<T>(string groupName) where T : AbstractResource
        {
            if (!_resources.ContainsKey(groupName))
                throw new ArgumentException($"There is no group with ID \"{groupName}\".");
            List<T> res = new List<T>();
            foreach (string id in _resourceGroups[groupName])
            {
                AbstractResource resource = _resources[id];
                if (resource.GetType().Equals(typeof(T)))
                    res.Add((T)resource);
            }
            return res;
        }

        public IList<AbstractResource> GetAllResources()
        {
            return new List<AbstractResource>(_resources.Values);
        }

        public IList<T> GetAllResourcesOfType<T>() where T : AbstractResource
        {
            List<T> res = new List<T>();
            foreach (AbstractResource resource in _resources.Values)
            {
                if (resource.GetType().Equals(typeof(T))) res.Add((T)resource);
            }
            return res;
        }

        public void Clear()
        {
            _resources.Clear();
            foreach (AsyncOperationHandle handle in _handles.Values) Addressables.Release(handle);
            _handles.Clear();
            _resourceGroups.Clear();
            _isLoaded.Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}