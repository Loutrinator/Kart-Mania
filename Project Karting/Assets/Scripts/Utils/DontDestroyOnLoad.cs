using UnityEngine;

namespace Utils {
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    }
}
