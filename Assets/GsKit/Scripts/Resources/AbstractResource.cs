using UnityEngine;

namespace GsKit.Resources
{
    [System.Serializable]
    public abstract class AbstractResource : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The unique identifier for this resource.")]
        private string _resourceID;

        public string ResourceID => _resourceID;
    }
}