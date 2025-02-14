using UnityEngine;

namespace GsKit.Resources
{
    public class ConstantField<T> : AbstractResource
    {
        [SerializeField]
        private T _value;

        public T Value => _value;
    }
}