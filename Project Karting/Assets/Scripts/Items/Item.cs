using UnityEngine;

namespace Items
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] private Sprite icon;
        public Sprite Icon => icon;
        public string GetName() =>  name;
        public virtual void Use(PlayerRaceInfo info)
        {
            info.ItemIsInUse = false;
            info.Item = null;
        }

        public abstract void OnKeyHold(PlayerRaceInfo info);
        public abstract void OnKeyDown(PlayerRaceInfo info);
        public abstract void OnKeyUp(PlayerRaceInfo info);

    }
}