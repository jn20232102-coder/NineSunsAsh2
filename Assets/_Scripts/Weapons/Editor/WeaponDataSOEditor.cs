using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using NineSunsAsh.Weapons;            
using NineSunsAsh.Weapons.Components; 

namespace NineSunsAsh.Weapons.Editor
{
    [CustomEditor(typeof(WeaponDataSO))]
    public class WeaponDataSOEditor : UnityEditor.Editor
    {
        private SerializedProperty componentDataProp;
        private List<Type> dataCompTypes = new List<Type>();

        private void OnEnable()
        {
            // 1. 容错查找：先找小写 componentData，如果没找到再找大写 ComponentData
            componentDataProp = serializedObject.FindProperty("componentData");
            if (componentDataProp == null)
            {
                componentDataProp = serializedObject.FindProperty("ComponentData");
            }

            // 如果还没找到，说明 WeaponDataSO 里的变量名既不是 componentData 也不是 ComponentData
            if (componentDataProp == null)
            {
                Debug.LogError("WeaponDataSOEditor: 找不到名为 'componentData' 或 'ComponentData' 的 List 变量，请检查 WeaponDataSO.cs 中的命名！");
                return;
            }

            // 2. 查找所有子类
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    if (type.IsSubclassOf(typeof(WeaponComponentData)) && !type.IsAbstract)
                    {
                        dataCompTypes.Add(type);
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 绘制默认属性（排除 componentData，防止重复绘制）
            DrawPropertiesExcluding(serializedObject, "m_Script", componentDataProp.name);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"组件列表 ({componentDataProp.arraySize})", EditorStyles.boldLabel);

            // 3. 绘制自定义列表
            for (int i = 0; i < componentDataProp.arraySize; i++)
            {
                SerializedProperty item = componentDataProp.GetArrayElementAtIndex(i);
                
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                EditorGUILayout.BeginHorizontal();
                // 获取标题：如果 value 不为空则显示类型名，否则显示 "Empty"
                string labelName = "Null (Error)";
                if (item.managedReferenceValue != null)
                {
                    // 自动获取类名，例如 "WeaponSpriteData"
                    labelName = item.managedReferenceValue.GetType().Name;
                }
                
                item.isExpanded = EditorGUILayout.Foldout(item.isExpanded, labelName, true);

                if (GUILayout.Button("删除", GUILayout.Width(50)))
                {
                    componentDataProp.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties(); 
                    return; // 删除后立即退出，防止后续绘制报错
                }
                EditorGUILayout.EndHorizontal();

                // 绘制组件内部内容
                if (item.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    
                    // 这里的迭代器逻辑是绘制子属性的标准写法
                    SerializedProperty iterator = item.Copy();
                    SerializedProperty endProperty = iterator.GetEndProperty();
                    
                    if (iterator.NextVisible(true)) 
                    {
                        do
                        {
                            if (SerializedProperty.EqualContents(iterator, endProperty)) break;
                            EditorGUILayout.PropertyField(iterator, true);
                        } 
                        while (iterator.NextVisible(false));
                    }
                    
                    EditorGUI.indentLevel--;
                }
                
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            // 4. 添加按钮
            if (GUILayout.Button("添加组件 (Add Component)", GUILayout.Height(30)))
            {
                GenericMenu menu = new GenericMenu();
                foreach (var type in dataCompTypes)
                {
                    menu.AddItem(new GUIContent(type.Name), false, OnAddComponent, type);
                }
                menu.ShowAsContext();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void OnAddComponent(object typeObj)
        {
            // 在回调中先更新序列化对象，确保数据同步
            serializedObject.Update();
            
            Type type = (Type)typeObj;
            var newDataInstance = Activator.CreateInstance(type);
            
            // 使用 Insert 增加数组长度，比 arraySize++ 更安全
            componentDataProp.InsertArrayElementAtIndex(componentDataProp.arraySize);
            
            var newElement = componentDataProp.GetArrayElementAtIndex(componentDataProp.arraySize - 1);
            newElement.managedReferenceValue = newDataInstance;
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}