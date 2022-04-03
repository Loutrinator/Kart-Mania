using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ExtendedEditorWindow : EditorWindow
    {
        protected SerializedObject serializedObject;
        protected SerializedProperty currentProperty;
        protected string selectedPropertyPath;
        protected SerializedProperty selectedProperty;
        protected int sidebarIndex;

        protected void DrawProperties(SerializedProperty prop, bool drawChildren)
        {
            string lastPropPath = string.Empty;
            foreach (SerializedProperty p in prop)
            {
                if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                    EditorGUILayout.EndHorizontal();

                    if (p.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawProperties(p, drawChildren);
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath))
                    {
                        continue;
                    }

                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField(p, drawChildren);
                }
            }
        }
/*
        protected void DrawSidebar(SerializedProperty prop)
        {
            int tempIndex = 0;
            IEnumerator propEnumerator = prop.GetEnumerator();
            do
            {
                SerializedProperty current = (SerializedProperty)propEnumerator.Current;
                if (GUILayout.Button(current.displayName))
                {
                    Debug.Log("Sidebar UtilityAIAction button pressed.");
                    //selectedPropertyPath = current.propertyPath;
                    sidebarIndex = tempIndex;
                    Debug.Log("tempIndex : " + tempIndex + " sidebarIndex : "+ sidebarIndex);
                }
                tempIndex++;
            } while (propEnumerator.MoveNext());

            if (!string.IsNullOrEmpty(selectedPropertyPath))
            {
                selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
            }
        }
*/
        protected void DrawField(string propName, bool relative)
        {
            if(relative && currentProperty != null)
            {
                EditorGUILayout.PropertyField(currentProperty.FindPropertyRelative(propName), true);
            }else if(serializedObject != null)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), true);
            }
        }

        protected void Apply()
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}