using Kart;
using SplineEditor.Runtime;
using UI;
using UnityEngine;

namespace Handlers
{
    public class KartRespawner : MonoBehaviour
    {
        
        private bool _initialized;

        public void Init()
        {
            _initialized = true;
        }
        
        private void Update()
        {
            if (!_initialized) return;
            var karts = GameManager.Instance.karts;
            foreach (var kart in karts) {
                if (!kart) continue;
                BezierUtils.BezierPos bezierPos = kart.closestBezierPos;
                if (Vector3.Distance(bezierPos.GlobalOrigin, kart.transform.position) > KartPhysicsSettings.instance.respawnMinDistance)
                {
                    Respawn(kart);
                }
            }
        }

        public void Respawn(KartBase kart)
        {
            ScreenEffects.BlackFade(() => {
                BezierUtils.BezierPos respawnPos;
                if (kart.lastGroundBezierPos != null)
                {
                    respawnPos = kart.lastGroundBezierPos;
                }
                else
                {
                    respawnPos = RaceManager.Instance.currentRace.road.bezierSpline.GetBezierPos(0);
                }
                kart.transform.position = respawnPos.GlobalOrigin + respawnPos.LocalUp * KartPhysicsSettings.instance.respawnHeight;
            
                kart.transform.rotation = respawnPos.Rotation;

                kart.ResetForces();
                kart.ResetMovements();
            });
        }
    }
}
