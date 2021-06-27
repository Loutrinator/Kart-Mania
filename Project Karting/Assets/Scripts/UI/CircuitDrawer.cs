using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CircuitDrawer : MonoBehaviour
    {
        public Race race;
        public LineRenderer lineRenderer;

        public float roadWidth = 5;
        public int textureQuality = 1024;

        public RawImage rawImage;
        public Camera cam;
        
        [ContextMenu("Create line")]
        private void OnValidate()
        {
            var frames = race.road.bezierSpline.RotationMinimisingFrames;
            Vector3[] positions = new Vector3[frames.Count];
            lineRenderer.positionCount = frames.Count;

            float minX = float.MaxValue, maxX = float.MinValue, minZ = float.MaxValue, maxZ = float.MinValue;
            for (var index = 0; index < frames.Count; index++)
            {
                Vector3 pos = frames[index].GlobalOrigin;
                positions[index] = pos;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.x < minX) minX = pos.x;
                if (pos.z > maxZ) maxZ = pos.z;
                if (pos.z < minZ) minZ = pos.z;
            }

            lineRenderer.widthMultiplier = roadWidth;
            lineRenderer.SetPositions(positions);

            RenderTexture texture = new RenderTexture(textureQuality, textureQuality, 100);
            cam.targetTexture = texture;
            cam.orthographicSize = Mathf.Max(maxX - minX, maxZ - minZ)/2 + 100;
            rawImage.texture = texture;
        }
    }
}
