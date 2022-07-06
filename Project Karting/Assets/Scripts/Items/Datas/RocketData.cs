using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Items {
    [CreateAssetMenu(fileName = "RocketData", menuName = "ScriptableObjects/Items/RocketData")]
    public class RocketData : ItemData {
        [SerializeField] private Rocket rocketPrefab;
        [SerializeField] private Sprite icon;
        
        public override Sprite GetIcon() {
            return icon;
        }

        public override ItemObject GiveItem(Transform parent) {
            Rocket rocket = Instantiate(rocketPrefab, parent.position, parent.rotation, parent);
            rocket.ResetItem();
            return rocket;
        }
    }
}