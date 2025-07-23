using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityGCC;
using UnityGCC.Capabilities;

public class GCCSheetLoader : MonoBehaviour
{
    public GCCSheet Sheet;

    private void Awake()
    {
        if (Sheet != null)
        {
            AddComponentsFromSheet();
        }
    }

    void Start()
    {
        if (Sheet != null)
        {
            CreateCapabilitiesFromSheet();
        }
    }

    void AddComponentsFromSheet()
    {
        foreach (ComponentData componentData in Sheet.components)
        {
            if (componentData.script == null) continue;

            Type componentType = componentData.script.GetClass();

            if (componentType != null && typeof(MonoBehaviour).IsAssignableFrom(componentType))
            {
                // 检查是否已经有这个组件
                Component existingComponent = GetComponent(componentType);
                if (existingComponent == null)
                {
                    // 添加组件
                    Component newComponent = gameObject.AddComponent(componentType);

                    // 应用属性值
                    ApplyPropertiesToComponent(newComponent, componentData.properties);

                    Debug.Log($"[GCC] 添加组件: {componentType.Name}");
                }
                else
                {
                    // 如果组件已存在，仍然可以应用属性值
                    ApplyPropertiesToComponent(existingComponent, componentData.properties);
                    Debug.LogWarning($"[GCC] 组件 {componentType.Name} 已经存在，已应用属性值");
                }
            }
        }
    }

    void ApplyPropertiesToComponent(Component component, List<SerializableProperty> properties)
    {
        Type componentType = component.GetType();

        foreach (SerializableProperty property in properties)
        {
            try
            {
                FieldInfo field = componentType.GetField(property.fieldName,
                    BindingFlags.Public | BindingFlags.Instance);

                if (field != null)
                {
                    object value = GetPropertyValue(property, field.FieldType);
                    field.SetValue(component, value);

                    Debug.Log($"[GCC] 设置 {componentType.Name}.{property.fieldName} = {value}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[GCC] 设置属性失败 {componentType.Name}.{property.fieldName}: {e.Message}");
            }
        }
    }

    object GetPropertyValue(SerializableProperty property, Type targetType)
    {
        if (targetType == typeof(int)) return property.intValue;
        else if (targetType == typeof(float)) return property.floatValue;
        else if (targetType == typeof(bool)) return property.boolValue;
        else if (targetType == typeof(string)) return property.stringValue;
        else if (targetType == typeof(Vector3)) return property.vector3Value;
        else if (targetType == typeof(Color)) return property.colorValue;
        else if (targetType.IsEnum) return Enum.ToObject(targetType, property.intValue);

        return null;
    }
    
    void CreateCapabilitiesFromSheet()
    {
        foreach (MonoScript script in Sheet.capabilities)
        {
            if (script == null) continue;
            
            Type capabilityType = script.GetClass();
            
            if (capabilityType != null && typeof(BaseCapabilities).IsAssignableFrom(capabilityType))
            {
                try
                {
                    BaseCapabilities capability = (BaseCapabilities)Activator.CreateInstance(capabilityType);
                    capability.Owner = gameObject;
                    capability.SetUp();
                    
                    Debug.Log($"[GCC] 创建能力: {capabilityType.Name}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[GCC] 创建能力失败 {capabilityType.Name}: {e.Message}");
                }
            }
        }
    }

    private void OnDestroy()
    {
        var readyRemoves = new List<BaseCapabilities>();
        foreach (var group in CapabilitiesController.Instance.Capabilities.Values)
        foreach (var capability in group)
        {
            if (capability.Owner == gameObject)
            {
                readyRemoves.Add(capability);
            }
        }
        readyRemoves.ForEach(capability => capability.OnOwnerDestroyed());
    }
}
