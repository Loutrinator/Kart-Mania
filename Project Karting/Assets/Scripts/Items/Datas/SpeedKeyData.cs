using Kart;
using UnityEngine;

namespace Items
{
    
    [CreateAssetMenu(fileName="SpeedKeyData",menuName="ScriptableObjects/Items/SpeedKeyData",order=0)]
    public class SpeedKeyData : ItemData
    {
        [SerializeField] private SpeedKey speedKeyPrefab;
        [SerializeField] private Sprite icon;

        public override Sprite GetIcon()
        {
            return icon;
        }
        public override ItemObject GiveItem(Transform parent)
        {
            KartBase kart = parent.GetComponent<KartBase>();
            //kart.effects.InsertKey();
            SpeedKey speedKey = Instantiate(speedKeyPrefab, parent.position, parent.rotation, parent);
            
            //WoodBox marbleLauncher = Instantiate(prefab,parent.position,parent.rotation,parent);
            //marbleLauncher.ResetItem();
            return speedKey;//marbleLauncher;
        }
    }
}