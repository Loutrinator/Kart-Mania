using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    public class TransformUtils : MonoBehaviour
    {
        private struct TransformData
        {
            public Transform transform;
            public Vector3 position;
            public Quaternion rotation;
            public Matrix4x4 localToWorldMatrix;
        }

        private static List<TransformData> SaveTransformData(Transform t)
        {
            int childCount = t.childCount;
            List<TransformData> childsData = new List<TransformData>();
            for (int i = 0; i < childCount; i++)
            {
                Transform child = t.GetChild(i);
                childsData.Add(new TransformData()
                {
                    transform = child, position = child.position, rotation = child.rotation,
                    localToWorldMatrix = child.localToWorldMatrix
                });
                Undo.RecordObject(child, "");
            }
            Undo.RecordObject(t, "");

            return childsData;
        }

        private static void LoadChildsTransformData(List<TransformData> childsData, Transform parent)
        {
            foreach (var childData in childsData)
            {
                Transform child = childData.transform;
                child.position = childData.position;
                child.rotation = childData.rotation;

                var localRotationMatrix = Matrix4x4.Rotate(child.localRotation);
                var localTransformMatrix = parent.worldToLocalMatrix * childData.localToWorldMatrix;
                float scaleX = Vector3.Dot(
                    new Vector3(localRotationMatrix.m00, localRotationMatrix.m10, localRotationMatrix.m20),
                    new Vector3(localTransformMatrix.m00, localTransformMatrix.m10, localTransformMatrix.m20));
                float scaleY = Vector3.Dot(
                    new Vector3(localRotationMatrix.m01, localRotationMatrix.m11, localRotationMatrix.m21),
                    new Vector3(localTransformMatrix.m01, localTransformMatrix.m11, localTransformMatrix.m21));
                float scaleZ = Vector3.Dot(
                    new Vector3(localRotationMatrix.m02, localRotationMatrix.m12, localRotationMatrix.m22),
                    new Vector3(localTransformMatrix.m02, localTransformMatrix.m12, localTransformMatrix.m22));


                child.localScale = new Vector3(scaleX, scaleY, scaleZ);
            }
        }

        [MenuItem("CONTEXT/Transform/Reset position and update childs", false, 150)]
        private static void ResetPosUpdateChilds()
        {
            Transform[] targets = GetSelection();

            Undo.SetCurrentGroupName("Reset position and update childs");
            foreach (var target in targets)
            {
                var childs = SaveTransformData(target);

                target.localPosition = Vector3.zero;

                LoadChildsTransformData(childs, target);

                EditorUtility.SetDirty(target);
            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        [MenuItem("CONTEXT/Transform/Reset rotation and update childs", false, 151)]
        private static void ResetRotUpdateChilds()
        {
            Transform[] targets = GetSelection();

            Undo.SetCurrentGroupName("Reset rotation and update childs");
            foreach (var target in targets)
            {
                var childs = SaveTransformData(target);

                target.localRotation = Quaternion.identity;

                LoadChildsTransformData(childs, target);

                EditorUtility.SetDirty(target);
            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        [MenuItem("CONTEXT/Transform/Reset scale and update childs", false, 152)]
        private static void ResetScaleUpdateChilds()
        {
            Transform[] targets = GetSelection();
            
            Undo.SetCurrentGroupName("Reset scale and update childs");
            foreach (var target in targets)
            {
                var childs = SaveTransformData(target);

                target.localScale = Vector3.one;

                LoadChildsTransformData(childs, target);

                EditorUtility.SetDirty(target);
            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        [MenuItem("CONTEXT/Transform/Reset transform and update childs", false, 153)]
        private static void ResetTransformUpdateChilds()
        {
            Transform[] targets = GetSelection();

            Undo.SetCurrentGroupName("Reset transform and update childs");    // create undo group
            foreach (var target in targets)
            {
                var childs = SaveTransformData(target); // undo.record here

                target.localPosition = Vector3.zero;
                target.localRotation = Quaternion.identity;
                target.localScale = Vector3.one;
                LoadChildsTransformData(childs, target);

                EditorUtility.SetDirty(target);
            }

            Undo.CollapseUndoOperations(Undo.GetCurrentGroup()); // register everything into group
        }
        
        [MenuItem("CONTEXT/Transform/Set pivot point to center of object", false, 164)] // priority 11 more than previous to have separator line
        private static void CenterPivotPoint()
        {
            Transform[] targets = GetSelection();

            Undo.SetCurrentGroupName("Set pivot point to center of object");
            foreach (var target in targets)
            {
                Vector3 targetPos = target.position;
                // compute center pos

                if (target.childCount == 0) continue;
                Renderer[] rends = target.GetComponentsInChildren<Renderer>(true);
                Bounds bounds = rends[0].bounds;
                foreach (Renderer rend in rends)
                {
                    bounds.Encapsulate(rend.bounds);
                }

                Vector3 offset = bounds.center - targetPos;
                
                Undo.RecordObject(target, "");
                target.position += offset;
                
                foreach (Transform child in target)
                {
                    if (child == target) continue;
                
                    Undo.RecordObject(child, "");
                    child.position -= offset;
                }
                
                EditorUtility.SetDirty(target);
            }
            
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        }

        private static Transform[] GetSelection()
        {
            if (ActiveEditorTracker.sharedTracker.isLocked)
            {
                var targets = ActiveEditorTracker.sharedTracker.activeEditors[0].targets;
                List<Transform> result = new List<Transform>();
                foreach (var obj in targets)
                {
                    result.Add(((GameObject) obj).transform);
                }

                return result.ToArray();
            }

            return Selection.transforms;
        }
    }
}