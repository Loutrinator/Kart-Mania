using System.Reflection;
using UnityEngine;

namespace Items
{
    public abstract class ItemObject : MonoBehaviour
    {
        protected bool inUse;
        public virtual void Use(PlayerRaceInfo info)
        {
            info.Item = null;
        }

        public abstract void ResetItem();
        
        public abstract void OnKeyHold(PlayerRaceInfo info);
        public abstract void OnKeyDown(PlayerRaceInfo info);
        public abstract void OnKeyUp(PlayerRaceInfo info);
    }
}