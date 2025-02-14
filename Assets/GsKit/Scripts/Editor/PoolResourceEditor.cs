using GsKit.Pooling;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GsKit.Editor
{
    [CustomEditor(typeof(PoolResource))]
    public class PoolResourceEditor : UnityEditor.Editor
    {
        private SerializedProperty _componentMask;
        private SerializedProperty _prefabResource;
        private SerializedProperty _minimumObjects;
        private SerializedProperty _maximumObjects;

        private void OnEnable()
        {
            _componentMask = serializedObject.FindProperty("_componentMask");
            _prefabResource = serializedObject.FindProperty("_prefabResource");
            _minimumObjects = serializedObject.FindProperty("_minimumObjects");
            _maximumObjects = serializedObject.FindProperty("_maximumObjects");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (_maximumObjects.intValue < _minimumObjects.intValue) _maximumObjects.intValue = _minimumObjects.intValue;
            if (_prefabResource != null) ComponentTypes();

            serializedObject.ApplyModifiedProperties();
        }

        private void ComponentTypes()
        {
            PrefabResource prefabResource = (PrefabResource)_prefabResource.objectReferenceValue;
            if (prefabResource != null)
            {
                GameObject prefab = prefabResource.Prefab;
                List<Component> components = new List<Component>(prefab.GetComponents<Component>());

                List<string> typeNames = new List<string>();
                foreach (Component component in components)
                    typeNames.Add(component.GetType().FullName);

                _componentMask.intValue = EditorGUILayout.MaskField(new GUIContent("Components To Cache", "" +
                                                                                                          "The components that will be initially cached."),
                    _componentMask.intValue,
                    typeNames.ToArray());
            }
        }
    }
}