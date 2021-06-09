using System.Collections.Generic;
using System.Linq;
using SplineEditor.Runtime;
using UnityEngine;

namespace Road.RoadMesh
{
    public class RoadBorder : MonoBehaviour
    {
        public enum Side {
            Left, Right
        }

        public MeshFilter meshFilter;
        public bool generateCollider;
        public BezierPath bezierPath;
        public Side roadSide;
        public float roadWidth = 1;
        public float roadThickness = 1;
        public float distanceStart;
        public float distanceEnd = 10;
        
        [SerializeField, HideInInspector] private MeshCollider meshCollider;

        private Vector3 Offset(BezierUtils.BezierPos pos) {
            return (roadSide == Side.Left
                ? pos.Normal * (bezierPath.bezierMeshExtrusion.roadWidth - roadWidth)
                : -pos.Normal * (bezierPath.bezierMeshExtrusion.roadWidth - roadWidth))
                + pos.LocalUp * roadThickness;
        }

        private void OnValidate() {
            UpdateMesh();
        }

        [ContextMenu("Update Mesh")]
        public void UpdateMesh() {
            if (bezierPath == null) return;
            Mesh mesh = new Mesh();
            
            List<BezierUtils.BezierPos> vectorFrames = new List<BezierUtils.BezierPos>(
                bezierPath.bezierSpline.RotationMinimisingFrames.Where(
                    bezierPos => bezierPos.BezierDistance >= distanceStart && bezierPos.BezierDistance <= distanceEnd));
            int arrayLen = vectorFrames.Count;
            
            var vertices = new Vector3[arrayLen * 2 * 4 + 8];    // 2 vertices per bezier vertex * (2 faces + 2 sides) + 2 extremities 
            var normals = new Vector3[arrayLen * 2 * 4 + 8];
            var triangles = new int[(arrayLen * 6 - 6) * 4 + 2 * 6];    // 2 faces + 2 sides + 2 extremities

            int indexUp = 0;
            int indexBottom = arrayLen * 2;
            int indexLeftSide = arrayLen * 2 * 2;
            int indexRightSide = arrayLen * 2 * 3;
            int indexStartFace = arrayLen * 2 * 4;
            int indexEndFace = arrayLen * 2 * 4 + 4;
            int indexTriangles = 0;
            
            for (int i = 0; i < arrayLen; ++i) {
                var bezierCenter = vectorFrames[i].LocalOrigin + Offset(vectorFrames[i]);
                var normal = vectorFrames[i].Normal;
                var rotAxis = vectorFrames[i].LocalUp;
                
                // up face
                vertices[indexUp] = bezierCenter + normal * roadWidth;
                vertices[indexUp + 1] = bezierCenter - normal * roadWidth;
                normals[indexUp] = normals[indexUp + 1] = rotAxis;
                
                // bottom face
                vertices[indexBottom] = vertices[indexUp] - rotAxis * roadThickness;
                vertices[indexBottom + 1] = vertices[indexUp + 1] - rotAxis * roadThickness;
                normals[indexBottom] = normals[indexBottom + 1] = -rotAxis;
                
                // left side
                vertices[indexLeftSide] = vertices[indexUp + 1];
                vertices[indexLeftSide + 1] = vertices[indexBottom + 1];
                normals[indexLeftSide] = normals[indexLeftSide + 1] = -normal;
                
                // right side
                vertices[indexRightSide] = vertices[indexUp];
                vertices[indexRightSide + 1] = vertices[indexBottom];
                normals[indexRightSide] = normals[indexRightSide + 1] = normal;

                if (indexUp > 1) {
                    // up face
                    triangles[indexTriangles++] = indexUp + 1;
                    triangles[indexTriangles++] = indexUp;
                    triangles[indexTriangles++] = indexUp - 1;
                    
                    triangles[indexTriangles++] = indexUp - 1;
                    triangles[indexTriangles++] = indexUp;
                    triangles[indexTriangles++] = indexUp - 2;
                    
                    // bottom face
                    triangles[indexTriangles++] = indexBottom - 1;
                    triangles[indexTriangles++] = indexBottom;
                    triangles[indexTriangles++] = indexBottom + 1;
                    
                    triangles[indexTriangles++] = indexBottom - 1;
                    triangles[indexTriangles++] = indexBottom - 2;
                    triangles[indexTriangles++] = indexBottom;
                    
                    // side left face
                    triangles[indexTriangles++] = indexLeftSide;
                    triangles[indexTriangles++] = indexLeftSide - 2;
                    triangles[indexTriangles++] = indexLeftSide + 1;

                    triangles[indexTriangles++] = indexLeftSide + 1;
                    triangles[indexTriangles++] = indexLeftSide - 2;
                    triangles[indexTriangles++] = indexLeftSide - 1;
                    
                    // side right face
                    triangles[indexTriangles++] = indexRightSide - 2;
                    triangles[indexTriangles++] = indexRightSide;
                    triangles[indexTriangles++] = indexRightSide + 1;

                    triangles[indexTriangles++] = indexRightSide + 1;
                    triangles[indexTriangles++] = indexRightSide - 1;
                    triangles[indexTriangles++] = indexRightSide - 2;
                }
                indexUp += 2;
                indexBottom += 2;
                indexLeftSide += 2;
                indexRightSide += 2;
            }
            // start face
            vertices[indexStartFace] = vertices[0];
            vertices[indexStartFace + 1] = vertices[1];
            vertices[indexStartFace + 2] = vertices[arrayLen * 2];
            vertices[indexStartFace + 3] = vertices[arrayLen * 2 + 1];
            normals[indexStartFace] = normals[indexStartFace + 1] = normals[indexStartFace + 2]
                = normals[indexStartFace + 3] = -vectorFrames[0].Tangent;

            triangles[indexTriangles++] = indexStartFace + 1;
            triangles[indexTriangles++] = indexStartFace;
            triangles[indexTriangles++] = indexStartFace + 2;
            
            triangles[indexTriangles++] = indexStartFace + 2;
            triangles[indexTriangles++] = indexStartFace + 3;
            triangles[indexTriangles++] = indexStartFace + 1;
            
            // end face
            vertices[indexEndFace] = vertices[arrayLen * 2 - 1];
            vertices[indexEndFace + 1] = vertices[arrayLen * 2 - 2];
            vertices[indexEndFace + 2] = vertices[arrayLen * 2 * 2 - 1];
            vertices[indexEndFace + 3] = vertices[arrayLen * 2 * 2 - 2];
            normals[indexEndFace] = normals[indexEndFace + 1] = normals[indexEndFace + 2]
                = normals[indexEndFace + 3] = -vectorFrames[0].Tangent;

            triangles[indexTriangles++] = indexEndFace + 1;
            triangles[indexTriangles++] = indexEndFace;
            triangles[indexTriangles++] = indexEndFace + 2;
            
            triangles[indexTriangles++] = indexEndFace + 2;
            triangles[indexTriangles++] = indexEndFace + 3;
            triangles[indexTriangles] = indexEndFace + 1;

            // finally set mesh
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = triangles;
            meshFilter.mesh = mesh;

            if (generateCollider)
            {
                if (!meshCollider)
                {
                    meshCollider = meshFilter.gameObject.GetComponent<MeshCollider>();
                    if(!meshCollider) meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                }

                meshCollider.sharedMesh = mesh;
            }
        }
    }
}
