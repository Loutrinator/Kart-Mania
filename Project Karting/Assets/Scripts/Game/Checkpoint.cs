using Handlers;
using Kart;
using UnityEngine;

namespace Game {
    public class Checkpoint : MonoBehaviour
    {
        public int checkpointId;
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Checkpoint touché");
            KartBase kart = other.gameObject.GetComponent<KartBase>();
            if (kart != null)
            {
                Debug.Log("kart pas null");
                GameManager.Instance.checkpointPassed(checkpointId,kart.GetPlayerID.Invoke());
            }
        }
    }
}
