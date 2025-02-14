using UnityEngine;

namespace GsKit.Resources
{
    [System.Serializable]
    public class VariableField<T> : AbstractResource
    {
        [SerializeField]
        public T Value;
    }
}