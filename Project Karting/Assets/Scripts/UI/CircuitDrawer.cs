using Game;
using UnityEngine;

namespace UI
{
    public class CircuitDrawer : MonoBehaviour
    {
        public Race race;
        public LineRenderer lineRenderer;

        public float roadWidth = 5;
        
        [ContextMenu("Create line")]
        private void OnValidate()
        {
            var frames = race.road.bezierSpline.RotationMinimisingFrames;
            Vector3[] positions = new Vector3[frames.Count];
            lineRenderer.positionCount = frames.Count;
            for (var index = 0; index < frames.Count; index++)
            {
                Vector3 pos = frames[index].GlobalOrigin;
                positions[index] = pos;
            }

            lineRenderer.widthMultiplier = roadWidth;
            lineRenderer.SetPositions(positions);
        }
    }
}
