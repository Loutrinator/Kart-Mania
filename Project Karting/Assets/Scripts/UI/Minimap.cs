using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Minimap : MonoBehaviour
    {
        public Race race;
        public LineRenderer lineRenderer;

        public float roadWidth = 5;
        public int textureQuality = 1024;

        public RawImage rawImage;
        public Camera cam;

        private Dictionary<GameObject, GameObject> _objectPreviews;
        
        [ContextMenu("Create line")]
        public void DrawMinimap()
        {
            _objectPreviews = new Dictionary<GameObject, GameObject>();

            MinMax minMax = new MinMax();
            
            var frames = race.road.bezierSpline.RotationMinimisingFrames;
            Vector3[] positions = new Vector3[frames.Count];
            lineRenderer.positionCount = frames.Count;

            Transform lineT = lineRenderer.transform;
            float minX = float.MaxValue, maxX = float.MinValue, minZ = float.MaxValue, 
                maxZ = float.MinValue, maxY = float.MinValue;
            Vector3 topPoint = Vector3.zero;
            for (var index = 0; index < frames.Count; index++)
            {
                Vector3 pos = frames[index].LocalOrigin;
                minMax.AddValue(pos.y);
                positions[index] = pos;
                pos = lineT.TransformPoint(pos);
                if (pos.x > maxX) maxX = pos.x;
                if (pos.x < minX) minX = pos.x;
                if (pos.z > maxZ) maxZ = pos.z;
                if (pos.z < minZ) minZ = pos.z;
                if (pos.y > maxY)
                {
                    maxY = pos.y;
                    topPoint = pos;
                }
            }

            lineRenderer.widthMultiplier = roadWidth;
            lineRenderer.SetPositions(positions);
            if (Application.isPlaying)
            {
                lineRenderer.material.SetFloat("_min", minMax.Min);
                lineRenderer.material.SetFloat("_max", minMax.Max);
            } else {
                lineRenderer.sharedMaterial.SetFloat("_min", minMax.Min);
                lineRenderer.sharedMaterial.SetFloat("_max", minMax.Max);
            }

            RenderTexture texture = new RenderTexture(textureQuality, textureQuality, 100);
            cam.targetTexture = texture;
            cam.orthographicSize = Mathf.Max(maxX - minX, maxZ - minZ)/2 + 100;
            
            cam.transform.position = new Vector3((maxX + minX) / 2, topPoint.y + roadWidth * 2, (maxZ + minZ) / 2);
            bool notAligned = true;
            while (notAligned)
            {
                notAligned = false;
                foreach (var position in positions)
                {
                    Vector3 globalPos = lineT.TransformPoint(position);
                    Vector2 viewportPoint = cam.WorldToViewportPoint(globalPos);
                    if (viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
                    {
                        ++topPoint.y;
                        cam.transform.position = new Vector3((maxX + minX) / 2, topPoint.y + roadWidth, (maxZ + minZ) / 2);
                        notAligned = true;
                        break;
                    }
                }
            }
            cam.transform.position = new Vector3((maxX + minX) / 2, topPoint.y + roadWidth * 2, (maxZ + minZ) / 2);
            rawImage.texture = texture;
        }

        public void AddVisualObject(GameObject obj, GameObject prefabPreview, Color color)
        {
            GameObject preview = Instantiate(prefabPreview);
            preview.GetComponentInChildren<MeshRenderer>().material.SetColor("Color_input", color);
            _objectPreviews.Add(obj, preview);
        }

        public void UpdateMinimap()
        {
            foreach (var objectPreview in _objectPreviews)
            {
                objectPreview.Value.transform.position = race.road.bezierSpline.transform.InverseTransformPoint(lineRenderer.transform.TransformPoint(objectPreview.Key.transform.position));
            }
        }

        public void SetPosition(int nbPlayerRacing)
        {
            /*
            RectTransform rt = rawImage.rectTransform;
            if (nbPlayerRacing == 1)
            {
                rt.anchorMin = new Vector2(1,1);
                rt.anchorMax = new Vector2(1,1);
                rt.pivot = new Vector2(1, 1);
                rt.localPosition = new Vector2(10, 10);
            }
            else
            {
                rt.anchorMin = new Vector2(0.5f,0.5f);
                rt.anchorMax = new Vector2(0.5f,0.5f);
                rt.anchoredPosition = new Vector2(0, 0);
                rt.pivot = new Vector2(0, 0);
                rt.localPosition = new Vector2(0, 0);
            }*/
        }
    }
}
