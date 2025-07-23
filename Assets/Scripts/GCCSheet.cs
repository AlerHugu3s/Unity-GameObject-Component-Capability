using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityGCC.Capabilities;
using UnityGCC.Components;

namespace UnityGCC
{
    [System.Serializable]
    public class ComponentData
    {
        public MonoScript script;
        public string componentName;
        public List<SerializableProperty> properties = new List<SerializableProperty>();
    }
    
    [System.Serializable]
    public class SerializableProperty
    {
        public string fieldName;
        public string fieldType;
        public string stringValue;
        public int intValue;
        public float floatValue;
        public bool boolValue;
        public Vector3 vector3Value;
        public Color colorValue;
        // 可以根据需要添加更多类型
    }

    [CreateAssetMenu(fileName = "GCCSheet", menuName = "GCC/GCCSheet", order = 1)]
    public class GCCSheet : ScriptableObject
    {
        public List<ComponentData> components = new List<ComponentData>();
        public List<MonoScript> capabilities = new List<MonoScript>();
    }
}