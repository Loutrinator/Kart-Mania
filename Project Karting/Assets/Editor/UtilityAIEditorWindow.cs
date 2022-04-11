using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using AI.UtilityAI;
using Editor;
using Genetics;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class UtilityAIEditorWindow : ExtendedEditorWindow
{
    private static UtilityAIEditorWindow window;
    private UtilityAIAsset target;
    private SerializedObject targetSO;
    private SerializedProperty currentAction;
    private GUIStyle headerStyle;
    public static void Open(UtilityAIAsset utilityAIAsset)
    {
        window = GetWindow<UtilityAIEditorWindow>("UtilityAI Editor");
        window.serializedObject = new SerializedObject(utilityAIAsset);
        window.targetSO = window.serializedObject;
        window.target = utilityAIAsset;
    }

    private void OnEnable()
    {
        headerStyle = new GUIStyle();
        headerStyle.fontSize = 18;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white;
    }

    private void OnGUI()
    {
        
        if (window != null)
        {
            if (window.serializedObject != null)
            {
                targetSO.Update();        
                
                EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
                
                //Begin Groups
                EditorGUILayout.BeginVertical();
                
               /*GUIStyle titleStyle = new GUIStyle();
                titleStyle.fontSize = 24;
                titleStyle.fontStyle = FontStyle.Bold;
                titleStyle.normal.textColor = Color.white;
                titleStyle.padding.left = 10;
                titleStyle.padding.top = 10;
                
                GUILayout.Label("Utility AI Editor",titleStyle,GUILayout.MinHeight(30));*/
                EditorGUILayout.Separator();
                
                EditorGUILayout.BeginVertical("box");
                
                DrawGroupSelector();
                DrawGroupContent();
                
                EditorGUILayout.EndVertical();
                //End group 
                
                EditorGUILayout.EndVertical();

                //End Layout
                Apply();
                
            }
            else
            {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
                GUILayout.Label("Please open a UtilityAIAsset.");
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private void DrawGroupSelector()
    {
        
        
        EditorGUI.BeginChangeCheck();
        //Begin Groups first line
        EditorGUILayout.BeginHorizontal();
        
        GUILayout.Label("Action groups",headerStyle,GUILayout.MinHeight(30),GUILayout.MaxWidth(150));
        //Begin Tabs
        EditorGUILayout.BeginHorizontal();
        currentProperty = serializedObject.FindProperty("actionGroups");
        List<string> groupNames = new List<string>();
        foreach (var actionGroup in target.actionGroups)
        {
            groupNames.Add(actionGroup.groupName);   
        }
        target.currentTab = GUILayout.Toolbar(target.currentTab,groupNames.ToArray());
        EditorGUILayout.EndHorizontal();
        //End Tabs
                
        //Begin group + - buttons
        EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(50));
        if (GUILayout.Button("+", GUILayout.MaxHeight(20), GUILayout.MaxWidth(20)))
        {
            UtilityAIActionGroup newGroup = new UtilityAIActionGroup();
            target.actionGroups.Add(newGroup);
            target.currentTab = target.actionGroups.Count - 1;
            Debug.Log("+");
        }
        if (GUILayout.Button("-", GUILayout.MaxHeight(20), GUILayout.MaxWidth(20)))
        {
            target.actionGroups.RemoveAt(target.currentTab);
            target.currentTab -= 1;
            Debug.Log("-");
        }
        EditorGUILayout.EndHorizontal();
        //End group + - buttons
                
        EditorGUILayout.EndHorizontal();
        //End group first line
        
        if (EditorGUI.EndChangeCheck())
        {
            targetSO.ApplyModifiedProperties();
            GUI.FocusControl(null);
            currentAction = null;
        }
    }
    private void DrawGroupContent()
    {
        
        EditorGUILayout.Separator();
        
        SerializedProperty actionGroups = targetSO.FindProperty("actionGroups");//FindPropertyRelative("actionGroups");
        IEnumerator groupEnumerator = actionGroups.GetEnumerator();
        int enumarationCount = 0;
        do
        {

            groupEnumerator.MoveNext();
            enumarationCount++;
        } while (enumarationCount <= target.currentTab);

        SerializedProperty currentGroup = (SerializedProperty)groupEnumerator.Current;
        SerializedProperty groupName = currentGroup.FindPropertyRelative("groupName");
        SerializedProperty actions = currentGroup.FindPropertyRelative("actions");
        EditorGUILayout.PropertyField(groupName);
        EditorGUILayout.Separator();

        EditorGUILayout.EndVertical();//CLOTURE DU BOX OUVERT DANS ON GUI
        
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        DrawSidebar(actions);
        DrawSelectedPropertyPanel();
        EditorGUILayout.EndHorizontal();

    }
    void DrawSidebar(SerializedProperty actions)
    {
        
        EditorGUI.BeginChangeCheck();
        GUIStyle notSelectedStyle = new GUIStyle();
        notSelectedStyle.fontSize = 11;
        notSelectedStyle.normal.textColor = new Color(0.8f,0.8f,0.8f);
        GUIStyle selectedStyle = new GUIStyle();
        selectedStyle.fontSize = 15;
        selectedStyle.fontStyle = FontStyle.Bold;
        selectedStyle.normal.textColor = Color.white;
        selectedStyle.padding.left = 5;


        EditorGUILayout.BeginVertical("box",GUILayout.MaxWidth(150));
        GUILayout.Label("Actions",headerStyle,GUILayout.MinHeight(30));

        EditorGUILayout.BeginVertical(GUILayout.ExpandHeight(true));
        int tempIndex = 0;
        IEnumerator propEnumerator = actions.GetEnumerator();
        while (propEnumerator.MoveNext())
        {
            SerializedProperty current = (SerializedProperty)propEnumerator.Current;
            name = current.FindPropertyRelative("actionName").stringValue;
            if (string.IsNullOrEmpty(name))
            {
                name = "New Action";
            }
            GUIStyle buttonStyle = sidebarIndex == tempIndex ? selectedStyle : notSelectedStyle;
            float height = sidebarIndex == tempIndex ? 24 : 20;
            if (GUILayout.Button(name,buttonStyle,GUILayout.MaxHeight(height)))
            {
                Debug.Log("Sidebar UtilityAIAction button pressed.");
                currentAction = current;
                sidebarIndex = tempIndex;
                Debug.Log("tempIndex : " + tempIndex + " sidebarIndex : "+ sidebarIndex);
            }
            tempIndex++;
        }

        if (!string.IsNullOrEmpty(selectedPropertyPath))
        {
            selectedProperty = serializedObject.FindProperty(selectedPropertyPath);
        }
        EditorGUILayout.EndVertical();

        
        EditorGUILayout.BeginHorizontal(GUILayout.MaxHeight(20));
        if (GUILayout.Button("+", GUILayout.MaxHeight(20), GUILayout.MaxWidth(20)))
        {
            actions.InsertArrayElementAtIndex(actions.arraySize);
            Debug.Log("+");
        }
        if (GUILayout.Button("-", GUILayout.MaxHeight(20), GUILayout.MaxWidth(20)))
        {
            actions.DeleteArrayElementAtIndex(sidebarIndex);
            Debug.Log("-");
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        
        if (EditorGUI.EndChangeCheck())
        {
            targetSO.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }
        
    }
    void DrawSelectedPropertyPanel()
    {
        EditorGUILayout.BeginVertical("box",GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));

        if (currentAction != null)
        {
            
            EditorGUILayout.PropertyField(currentAction.FindPropertyRelative("actionName"));
            EditorGUILayout.Separator();
            
            
            EditorGUILayout.BeginVertical("box",GUILayout.ExpandWidth(true));
            
            GUILayout.Label("Evaluation functions",headerStyle,GUILayout.MinHeight(30));
            
            SerializedProperty evalFunctionListProperties = currentAction.FindPropertyRelative("evaluationFunctions");
            
            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label("Functions");
        
            if (GUILayout.Button("+", GUILayout.MaxHeight(20), GUILayout.MaxWidth(20)))
            {
                evalFunctionListProperties.InsertArrayElementAtIndex(evalFunctionListProperties.arraySize);
                Debug.Log("+");
            }
            if (GUILayout.Button("-", GUILayout.MaxHeight(20), GUILayout.MaxWidth(20)))
            {
                evalFunctionListProperties.DeleteArrayElementAtIndex(evalFunctionListProperties.arraySize-1);
                Debug.Log("-");
            }
        
            EditorGUILayout.EndHorizontal();



            GUIContent dataLabel = new GUIContent("Data : ");
            
            foreach (SerializedProperty evalFunc in evalFunctionListProperties) {
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 50f;
                EditorGUILayout.PropertyField(evalFunc.FindPropertyRelative("evaluationData"), dataLabel, GUILayout.Width(200));
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUILayout.PropertyField(evalFunc.FindPropertyRelative("coefficient"));
                EditorGUILayout.PropertyField(evalFunc.FindPropertyRelative("evaluationCurve"));
                EditorGUILayout.EndHorizontal();
            }

            
            EditorGUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Generate JSON in ClipBoard")) {
                string fileName = "ai_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".json";
                GeneticsUtils.WriteData(target.GetGenome(), fileName);
                AssetDatabase.Refresh();
            }
        }
        EditorGUILayout.EndVertical();
        /*
        
        //Functions list
       */

    }

}
