using Handlers;
using Kart;
using UnityEngine;

namespace Game {
    public class Checkpoint : MonoBehaviour
    {
        public int checkpointId;
        
        private void OnTriggerEnter(Collider other)
        {
            KartBase kart = other.gameObject.GetComponentInParent<KartBase>();
            if (kart != null)
            {
                GameManager.Instance.CheckpointPassed(checkpointId,kart.GetPlayerID.Invoke());
            }
        }
    }
}
