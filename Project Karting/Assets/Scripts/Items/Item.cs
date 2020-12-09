using UnityEngine;

namespace Items
{
    public abstract class Item : ScriptableObject
    {
        public string GetName() =>  name;
        public abstract void Use();
        public virtual void OnKeyHold(PlayerRaceInfo info){return;}
        public virtual void OnKeyDown(PlayerRaceInfo info){return;}
        public virtual void OnKeyUp(PlayerRaceInfo info){return;}
    }
}