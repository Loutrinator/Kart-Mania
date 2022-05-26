using UnityEngine;

namespace Items
{
    
    [CreateAssetMenu(fileName="NukeBombData",menuName="ScriptableObjects/Items/NukeBombData",order=0)]
    public class NukeBombData : ItemData
    {
        [SerializeField] private NukeBomb prefab;
        public Vector3 spawnPoint = new Vector3(550,600,150);
        [SerializeField] private Sprite icon;

        public override Sprite GetIcon()
        {
            return icon;
        }
        public override ItemObject GiveItem(Transform parent)
        {
            NukeBomb nuke = Instantiate(prefab,parent.position,parent.rotation,parent);
            nuke.ResetItem();
            return nuke;
        }
    }
}