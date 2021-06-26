using Kart;
using SplineEditor.Runtime;
using UnityEngine;

namespace Handlers
{
    public class KartRespawner : MonoBehaviour
    {
        public float distanceForRespawn = 20f;
        
        private bool _initialized;

        public void Init()
        {
            _initialized = true;
        }
        
        private void Update()
        {
            if (!_initialized) return;
            var karts = GameManager.Instance.karts;
            foreach (var kart in karts)
            {
                BezierUtils.BezierPos bezierPos = kart.closestBezierPos;
                if (Vector3.Distance(bezierPos.GlobalOrigin, kart.transform.position) > distanceForRespawn
                    || !kart.IsGrounded() && kart.currentVelocity.magnitude < 0.1f)
                {
                    Respawn(kart);
                }
            }
        }

        public void Respawn(KartBase kart)
        {
            BezierUtils.BezierPos respawnPos = null;
            if (kart.lastGroundBezierPos != null)
            {
                respawnPos = kart.lastGroundBezierPos;
            }
            else
            {
                respawnPos = GameManager.Instance.currentRace.road.bezierSpline.GetBezierPos(0);
            }
            kart.transform.position = respawnPos.GlobalOrigin;
            kart.transform.rotation = respawnPos.Rotation;

            kart.ResetForces();
        }
    }
}
