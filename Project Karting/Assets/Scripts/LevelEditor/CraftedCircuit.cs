using System;
using System.IO;
using SplineEditor.Runtime;
using UnityEditor;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace LevelEditor {
    [Serializable]
    public struct CircuitData {
        public string circuitName;
        public Sprite image;
        public BezierCircuit bezierCircuit;
    }
    public class CraftedCircuit : MonoBehaviour {
        public CircuitData circuitData;

        public void Save() {
            string path = FileUtils.craftedLevelsPath + circuitData.circuitName + FileUtils.craftedLevelsExtension;
            Debug.Log(path);
        }

        [ContextMenu("Save File")]
        public void SaveFile() {
            var filePath = AssetDatabase.GetAssetPath(file);
            var json = JsonUtility.ToJson(circuitData);
            File.WriteAllText(filePath, json);
        }

        public void Load() {
            
        }

        public Object file;
        [ContextMenu("Load File")]
        public void LoadFile() {
            var filePath = AssetDatabase.GetAssetPath(file);
            string fileContent = File.ReadAllText(filePath);
            circuitData = JsonUtility.FromJson<CircuitData>(fileContent);
            Init();
        }

        private void Init() {
            
        }
    }
}
