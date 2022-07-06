using UnityEngine;

namespace Items
{
    
    [CreateAssetMenu(fileName="NukeBombData",menuName="ScriptableObjects/Items/NukeBombData",order=0)]
    public class NukeBombData : ItemData
    {
        [SerializeField] private NukeRemote prefab;
        public Vector3 spawnPoint = new Vector3(550,600,150);
        [SerializeField] private Sprite icon;

        public override Sprite GetIcon()
        {
            return icon;
        }
        public override ItemObject GiveItem(Transform parent)
        {
            NukeRemote nuke = Instantiate(prefab,parent.position,parent.rotation,parent);
            return nuke;
        }
    }
}