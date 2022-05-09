using UnityEngine;

namespace MainMenu {
    public class MainMenuCamera : MonoBehaviour
    {
        public Animator TV;
        public void SwitchTV()
        {
            TV.SetTrigger("switch");
        }
    }
}
