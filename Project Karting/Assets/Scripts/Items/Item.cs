using System;
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
            info.itemIsUsing = false;
            info.Item = null;
        }
        public virtual void OnKeyHold(PlayerRaceInfo info){return;}
        public virtual void OnKeyDown(PlayerRaceInfo info){return;}
        public virtual void OnKeyUp(PlayerRaceInfo info){return;}
        
    }
}