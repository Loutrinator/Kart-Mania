using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Handlers {
    public class MasterLoader : MonoBehaviour {
        private IEnumerator Start() {
            while (!SplashScreen.isFinished) {
                yield return null;
            }
            
            TransitionController.Instance.InNoFade();
            TransitionController.Instance.ShowLoading();
            
            // initialisations
            LevelManager.instance.Init();
            
            SceneManager.instance.LoadMainMenu(() => TransitionController.Instance.FadeOut());
        }
    }
}
