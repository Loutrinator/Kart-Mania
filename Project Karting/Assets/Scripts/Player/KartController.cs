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
            Debug.Log("ON ITEM UP");
            ItemObject item = info.Item;
            if (item != null)
            {
                item.OnKeyUp(info);
            }
            else
            {
                Debug.Log("No item");
            }
        }
        
        public void OnItemHold()
        {
            Debug.Log("ON ITEM HOLD");
            ItemObject item = info.Item;
            if (item != null)
            {
                item.OnKeyHold(info);
            }
            else
            {
                Debug.Log("No item");
            }
        }
        
        public void OnItemDown()
        {
            Debug.Log("ON ITEM DOWN");
            ItemObject item = info.Item;
            if (item != null)
            {
                item.OnKeyDown(info);
            }
            else
            {
                Debug.Log("No item");
            }
        }
    }
}