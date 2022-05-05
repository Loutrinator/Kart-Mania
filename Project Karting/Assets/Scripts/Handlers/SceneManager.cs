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
    [CreateAssetMenu(fileName = "SceneManager", menuName = "ScriptableObjects/Managers/SceneManager")]
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
            TransitionController.Instance.FadeIn();
            TransitionController.Instance.ShowLoading();
            
            PlayerConfigurationManager.Instance.HideJoinUI();
            var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneMap[mode]);

            operation.completed += CallBack;

            void CallBack(AsyncOperation asyncOperation) {
                TransitionController.Instance.FadeOut();
                operation.completed -= CallBack;
            }
        }
        
        public void LoadMainMenu(Action onComplete = null) {
            var operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainMenuScene");
            if (onComplete == null) return;
            operation.completed += CallBack;

            void CallBack(AsyncOperation asyncOperation) {
                onComplete.Invoke();
                operation.completed -= CallBack;
            }
        }
        
        public void LoadCredits() {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("CreditsScene");
        }

        public void QuitGame() {
            
#if UNITY_EDITOR
            Debug.Log("Quitting the app !");
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
