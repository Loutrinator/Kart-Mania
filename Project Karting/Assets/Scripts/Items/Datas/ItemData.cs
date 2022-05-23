using UnityEngine;

namespace Items
{
    public abstract class ItemData : ScriptableObject
    {
        public string GetName() =>  name;
        public abstract Sprite GetIcon();
        public abstract ItemObject GiveItem(Transform parent);
    }
}