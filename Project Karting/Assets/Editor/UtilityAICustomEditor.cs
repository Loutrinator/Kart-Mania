using AI.UtilityAI;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class AssetHandler{
        [OnOpenAsset()]
        public static bool OpenEditor(int instanceId, int line)
        {
            UtilityAIAsset obj = EditorUtility.InstanceIDToObject(instanceId) as UtilityAIAsset;
            if (obj != null)
            {
                UtilityAIEditorWindow.Open(obj);
                return true;
            }
            return false;
        }
    }
    
    [CustomEditor(typeof(UtilityAIAsset))]
    public class UtilityAICustomEditor: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                UtilityAIEditorWindow.Open((UtilityAIAsset)target);
            }
        }
    }
}