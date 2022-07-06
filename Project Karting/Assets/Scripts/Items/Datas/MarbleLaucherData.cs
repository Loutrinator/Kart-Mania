using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName="MarbleLaucherData",menuName="ScriptableObjects/Items/MarbleLaucherData",order=0)]
    public class MarbleLaucherData : ItemData
    {
        [SerializeField] private MarbleLauncher marbleLauncherPrefab;
        [SerializeField] private Sprite icon;

        public override Sprite GetIcon()
        {
            return icon;
        }

        public override ItemObject GiveItem(Transform parent)
        {
            MarbleLauncher marbleLauncher = Instantiate(marbleLauncherPrefab,parent.position,parent.rotation,parent);
            marbleLauncher.ResetItem();
            marbleLauncher.ShowItem();
            return marbleLauncher;
        }
    }
}