using GsKit.Resources;
using GsKit.Service;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GsKit.Pooling
{
    public class PoolService : MonoBehaviour, IService
    {
        private Dictionary<string, Pool> _pools = new();
        private ResourceService _resourceService;

        public event EventHandler<Pool> PoolCreated;

        public event EventHandler<Pool> PoolRemoved;

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService(this);
        }

        private void Start()
        {
            _resourceService = ServiceLocator.Instance.GetService<ResourceService>();
            _resourceService.ResourceLoaded += OnResourceLoad;
        }

        private void OnResourceLoad(object sender, AbstractResource res)
        {
            if (typeof(PoolResource).IsInstanceOfType(res))
            {
                PoolResource poolRes = (PoolResource)res;
                Pool pool = new Pool(poolRes);
                _pools.Add(poolRes.ResourceID, pool);
                OnPoolCreated(pool);
                if (poolRes.PopulateOnLoad) pool.Populate();
            }
        }

        protected virtual void OnPoolCreated(Pool pool)
        {
            PoolCreated?.Invoke(this, pool);
        }

        public Pool GetPool(string poolId)
        {
            if (!_pools.ContainsKey(poolId)) throw new ArgumentException($"There is no pool with ID: \"{poolId}\"");
            return _pools[poolId];
        }

        public bool HasPool(string poolId)
        {
            return _pools.ContainsKey(poolId);
        }

        public void RemovePool(string poolId)
        {
            if (!_pools.ContainsKey(poolId)) throw new ArgumentException($"There is no pool with ID: \"{poolId}\"");
            Pool pool = _pools[poolId];
            _pools.Remove(poolId);
        }

        protected virtual void OnPoolRemoved(Pool pool)
        {
            PoolRemoved?.Invoke(this, pool);
        }

        public void Clear()
        {
            foreach (Pool pool in _pools.Values) pool.Clear();
            _pools.Clear();
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}