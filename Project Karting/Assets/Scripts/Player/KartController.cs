using Kart;
using UnityEngine;

namespace Player
{
    public class KartController : MonoBehaviour
    {

        [SerializeField] protected KartBase kart;
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
        
        //TODO: fonction UseItem()
    }
}