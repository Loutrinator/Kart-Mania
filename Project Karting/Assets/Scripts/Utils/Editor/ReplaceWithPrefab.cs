using UnityEditor;
using UnityEngine;

namespace Utils.Editor {
    public class ReplaceWithPrefab : EditorWindow {
        public GameObject prefab;
        
        [MenuItem("Tools/Replace With Prefab")]
        public static void ShowWindow() {
            ReplaceWithPrefab window = GetWindow<ReplaceWithPrefab>();
            window.titleContent.text = "Replace with prefab";
        }

        private void OnGUI() {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Prefab", GUILayout.Width(100));
            prefab = (GameObject) EditorGUILayout.ObjectField(prefab, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Replace selected objects with prefab")) {
                Transform[] transforms = Selection.transforms;
                Undo.SetCurrentGroupName("Replace with prefab");
                for (var index = 0; index < transforms.Length; index++) {
                    var transform = transforms[index];
                    var t = ((GameObject) UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform.parent)).transform;
                    t.localPosition = transform.localPosition;
                    t.localRotation = transform.localRotation;
                    t.localScale = transform.localScale;
                    Undo.RegisterCreatedObjectUndo(t.gameObject, "");
                    Undo.DestroyObjectImmediate(transform.gameObject);
                }
                Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            }
        }
    }
}
