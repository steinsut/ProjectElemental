using System;
using UnityEngine;

namespace GsKit.Pooling
{
    [CreateAssetMenu(fileName = "Prefab Resource", menuName = "GsKit/Prefab/Prefab Resource")]
    [Serializable]
    public class PrefabResource : Resources.AbstractResource
    {
        [SerializeField]
        [Tooltip("The prefab.")]
        private GameObject _prefab;

        public GameObject Prefab => _prefab;
    }
}