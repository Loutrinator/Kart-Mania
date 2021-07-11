using System;
using System.Collections.Generic;
using Game;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

namespace Handlers
{
    [CreateAssetMenu(fileName = "SceneManager", menuName = "ScriptableObject/SceneManager")]
    public class SceneManager : ScriptableObject {
        #region Singleton
        public static SceneManager instance;
        
        private void OnEnable()
        {
            if (instance != null)
                throw new UnityException(typeof(SceneManager) + " is already instantiated");
            instance = this;
        }

        private void OnDisable()
        {
            instance = null;
        }
        #endregion
        
#if UNITY_EDITOR
        [Serializable]
        public class SceneMapInspector {
            public List<SceneAsset> scenes;
            public List<GameMode> gameModes;
        }

        public SceneMapInspector sceneMapInspector;

        private void OnValidate() {
            sceneMap = new SerializedDictionary<GameMode, string>();
            sceneMap.Clear();
            for (int i = 0; i < sceneMapInspector.scenes.Count; ++i) {
                sceneMap.Add(sceneMapInspector.gameModes[i], sceneMapInspector.scenes[i].name);
            }
        }
#endif
        [SerializeField] private SerializedDictionary<GameMode, string> sceneMap;

        public void LoadGameMode(GameMode mode) {
            PlayerConfigurationManager.Instance.HideJoinUI();
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneMap[mode]);
        }
        public void LoadMainMenu() {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
        public void LoadCredits() {
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }
    }
}
