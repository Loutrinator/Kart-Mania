using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Items {
    [CreateAssetMenu(fileName = "PaperPlaneData", menuName = "ScriptableObjects/Items/PaperPlaneData")]
    public class PaperPlaneData : ItemData {
        [SerializeField] private PaperPlane paperPlanePrefab;
        [SerializeField] private Sprite icon;
        
        public override Sprite GetIcon() {
            return icon;
        }

        public override ItemObject GiveItem(Transform parent) {
            PaperPlane plane = Instantiate(paperPlanePrefab, parent.position, parent.rotation, parent);
            plane.ResetItem();
            return plane;
        }
    }
}