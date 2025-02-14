using System;
using UnityEngine;

namespace GsKit
{
    namespace Utils
    {
        [Serializable]
        public class SerializableType : ISerializationCallbackReceiver
        {
            public Type Value = null;

            [SerializeField] private string m_TypeFullName = null;

            public void OnBeforeSerialize()
            {
                if (Value != null)
                    m_TypeFullName = Value.FullName;
                else
                    m_TypeFullName = typeof(Component).FullName;
                Debug.Log("Before: " + m_TypeFullName);
            }

            public void OnAfterDeserialize()
            {
                Debug.Log("After: " + m_TypeFullName);
                if (m_TypeFullName == null)
                    Value = typeof(Component);
                else
                    Value = Type.GetType(m_TypeFullName);
            }
        }
    }
}