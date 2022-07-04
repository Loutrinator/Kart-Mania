using UnityEngine;

namespace Items {
    [CreateAssetMenu(fileName = "Rocket", menuName = "ScriptableObjects/Items/Rocket")]
    public class ItemRocket : ItemData {
        public override Sprite GetIcon() {
            throw new System.NotImplementedException();
        }

        public override ItemObject GiveItem(Transform parent) {
            throw new System.NotImplementedException();
        }
    }
}