#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityGCC
{
    [CustomEditor(typeof(GCCSheet))]
    public class GCCSheetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GCCSheet sheet = (GCCSheet)target;
            
            EditorGUILayout.LabelField("组件配置", EditorStyles.boldLabel);
            
            // 显示组件列表
            for (int i = 0; i < sheet.components.Count; i++)
            {
                DrawComponentData(sheet.components[i], i);
            }
            
            // 添加新组件按钮
            if (GUILayout.Button("添加组件"))
            {
                sheet.components.Add(new ComponentData());
            }
            
            EditorGUILayout.Space();
            
            // 显示能力列表（保持原样）
            EditorGUILayout.LabelField("能力脚本", EditorStyles.boldLabel);
            SerializedProperty capabilitiesProp = serializedObject.FindProperty("capabilities");
            EditorGUILayout.PropertyField(capabilitiesProp, true);
            
            serializedObject.ApplyModifiedProperties();
        }
        
        void DrawComponentData(ComponentData componentData, int index)
        {
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.BeginHorizontal();
            
            // MonoScript字段
            MonoScript newScript = (MonoScript)EditorGUILayout.ObjectField(
                $"组件 {index}", 
                componentData.script, 
                typeof(MonoScript), 
                false);
            
            // 删除按钮
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                ((GCCSheet)target).components.RemoveAt(index);
                return;
            }
            
            EditorGUILayout.EndHorizontal();
            
            // 如果脚本改变了，更新属性列表
            if (newScript != componentData.script)
            {
                componentData.script = newScript;
                UpdateComponentProperties(componentData);
            }
            
            // 显示组件名称
            if (componentData.script != null)
            {
                componentData.componentName = componentData.script.GetClass()?.Name ?? "Unknown";
                EditorGUILayout.LabelField("类型: " + componentData.componentName);
                
                // 显示可编辑的属性
                if (componentData.properties.Count > 0)
                {
                    EditorGUILayout.LabelField("属性:", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    
                    foreach (var property in componentData.properties)
                    {
                        DrawProperty(property);
                    }
                    
                    EditorGUI.indentLevel--;
                }
            }
            
            EditorGUILayout.EndVertical();
        }
        
        void UpdateComponentProperties(ComponentData componentData)
        {
            componentData.properties.Clear();
            
            if (componentData.script == null) return;
            
            Type componentType = componentData.script.GetClass();
            if (componentType == null) return;
            
            // 获取所有可序列化的字段
            FieldInfo[] fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (FieldInfo field in fields)
            {
                // 检查是否是Unity可序列化的字段
                if (IsSerializableField(field))
                {
                    SerializableProperty prop = new SerializableProperty
                    {
                        fieldName = field.Name,
                        fieldType = field.FieldType.Name
                    };
                    
                    // 设置默认值
                    SetDefaultValue(prop, field.FieldType);
                    
                    componentData.properties.Add(prop);
                }
            }
        }
        
        bool IsSerializableField(FieldInfo field)
        {
            // Unity序列化规则
            if (field.IsStatic || field.IsInitOnly) return false;
            
            // 检查是否有SerializeField特性
            if (field.GetCustomAttribute<SerializeField>() != null) return true;
            
            // 公共字段且是Unity支持的类型
            if (field.IsPublic && IsSupportedType(field.FieldType)) return true;
            
            return false;
        }
        
        bool IsSupportedType(Type type)
        {
            return type == typeof(int) || 
                   type == typeof(float) || 
                   type == typeof(bool) || 
                   type == typeof(string) ||
                   type == typeof(Vector3) ||
                   type == typeof(Color) ||
                   type.IsEnum;
        }
        
        void SetDefaultValue(SerializableProperty prop, Type fieldType)
        {
            if (fieldType == typeof(int)) prop.intValue = 0;
            else if (fieldType == typeof(float)) prop.floatValue = 0f;
            else if (fieldType == typeof(bool)) prop.boolValue = false;
            else if (fieldType == typeof(string)) prop.stringValue = "";
            else if (fieldType == typeof(Vector3)) prop.vector3Value = Vector3.zero;
            else if (fieldType == typeof(Color)) prop.colorValue = Color.white;
        }
        
        void DrawProperty(SerializableProperty property)
        {
            switch (property.fieldType)
            {
                case "Int32":
                    property.intValue = EditorGUILayout.IntField(property.fieldName, property.intValue);
                    break;
                case "Single":
                    property.floatValue = EditorGUILayout.FloatField(property.fieldName, property.floatValue);
                    break;
                case "Boolean":
                    property.boolValue = EditorGUILayout.Toggle(property.fieldName, property.boolValue);
                    break;
                case "String":
                    property.stringValue = EditorGUILayout.TextField(property.fieldName, property.stringValue);
                    break;
                case "Vector3":
                    property.vector3Value = EditorGUILayout.Vector3Field(property.fieldName, property.vector3Value);
                    break;
                case "Color":
                    property.colorValue = EditorGUILayout.ColorField(property.fieldName, property.colorValue);
                    break;
                default:
                    EditorGUILayout.LabelField(property.fieldName, $"不支持的类型: {property.fieldType}");
                    break;
            }
        }
    }
}
#endif