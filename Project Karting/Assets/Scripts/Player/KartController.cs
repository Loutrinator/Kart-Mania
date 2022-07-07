using Handlers;
using Items;
using Kart;
using UnityEngine;

namespace Player
{
    public class KartController : MonoBehaviour
    {

        [SerializeField] public KartBase kart;
        
        private PlayerRaceInfo info;

        private void Start()
        {
            info = RaceManager.Instance.GetPlayerRaceInfo(kart.playerIndex);
        }
        protected void Move(Vector2 movement)
        {
            if (kart != null)
            {
                float x = movement[0] > 0.1f ? 1f : movement[0] < -0.1f ? -1f : 0;
                float y = movement[1] > 0.1f ? 1f : movement[1] < -0.1f ? -1f : 0;
                kart.movement = new Vector2(x,y);
            }
        }
        protected void Drift(bool driftEnabled)
        {
            if (kart != null)
            {
                kart.drift = driftEnabled;
            }
        }
        
        
        
        public void OnItemUp()
        {
            ItemObject item = info.Item;
            if (item != null)
            {
                item.OnKeyUp(info);
            }
        }
        
        public void OnItemHold()
        {
            ItemObject item = info.Item;
            if (item != null)
            {
                item.OnKeyHold(info);
            }
        }
        
        public void OnItemDown()
        {
            ItemObject item = info.Item;
            if (item != null)
            {
                item.OnKeyDown(info);
            }
        }
        
        public void OnKlaxonUp()
        {
            kart.DisableKlaxon();
        }
        
        public void OnKlaxonDown()
        {
            kart.EnableKlaxon();
        }
    }
}