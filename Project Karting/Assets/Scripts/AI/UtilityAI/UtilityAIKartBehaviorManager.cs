using Kart;
using Handlers;
using SplineEditor.Runtime;
using UnityEngine;

namespace AI.UtilityAI
{
    public class UtilityAIKartBehaviorManager
    {
        private float curvatureOffset = 35f;
        private float distCurve = 20f;
        private static UtilityAIKartBehaviorManager instance;
        private GameManager manager;
        public static UtilityAIKartBehaviorManager Instance
        {
            get
            {
                if (instance == null) instance = new UtilityAIKartBehaviorManager();
                return instance;
            }
        }


        private static float startTime = Time.time;
        
        public float GetValue(EvaluationDataEnum dataType, KartBase kart)
        {
            float value = 0;
            switch (dataType)
            {
                case EvaluationDataEnum.alignedToTheRoad:
                    value = AlignedToRoad(kart);
                    break;
                case EvaluationDataEnum.curvatureOfTheRoad:
                    value = CurvatureOfRoadFunction(kart);
                    break;
                case EvaluationDataEnum.constant:
                    value = Constant(kart);
                    break;
                
                case EvaluationDataEnum.speed:
                    value = SpeedFunction(kart);
                    break;
                case EvaluationDataEnum.sineNormalized:
                    value = SineNormalized(kart);
                    break;
                
            }

            return value;
        }

        public float AlignedToRoad(KartBase kart)
        {
            if (kart.closestBezierPos != null)
            {
                return Vector3.Dot(kart.transform.right, kart.closestBezierPos.Tangent);
            }
            return 0;
        }

        public float SpeedFunction(KartBase kart)
        {
            var worldSpeed = kart.transform.position + kart.currentVelocity;
            var localSpeed = kart.transform.worldToLocalMatrix * worldSpeed;

            return localSpeed.z;
        }

        public float CurvatureOfRoadFunction(KartBase kart)
        {
            if (kart.closestBezierPos != null)
            {
                float distance = kart.closestBezierPos.BezierDistance;
                var kartPosOnCurve = AIManager.Instance.circuit.bezierSpline.GetBezierPos(distance);

                var nextPos1 = AIManager.Instance.circuit.bezierSpline.GetBezierPos(distance + distCurve);
                var nextPos2 = AIManager.Instance.circuit.bezierSpline.GetBezierPos(distance + distCurve + curvatureOffset);
                //var nextPos3 = AIManager.Instance.circuit.bezierSpline.GetBezierPos(distance + distCurve + 2*curvatureOffset);

                float dot1 = Vector3.Dot(nextPos1.Normal, nextPos2.Tangent);
                //float dot2 = Vector3.Dot(nextPos2.Normal, nextPos3.Tangent);
                return dot1;
                //return (dot1 + dot2)/2f;
            }
            return 0;
        }

        public Vector3 ClosestPointInCurvature(KartBase kart)
        {
            Vector3 pointCurvature = Vector3.zero;
            if (kart.closestBezierPos != null)
            {
                float distance = kart.closestBezierPos.BezierDistance;
                var nextPos = AIManager.Instance.circuit.bezierSpline.GetBezierPos(distance + distCurve);
                int dir = Mathf.RoundToInt(CurvatureOfRoadFunction(kart));

                float roadWith = AIManager.Instance.circuit.bezierMeshExtrusion.roadWidth;
                pointCurvature = kart.closestBezierPos.GlobalOrigin + dir * nextPos.Normal * roadWith;
            }

            return pointCurvature;
        }

        public float DistanceToCenterOfRoadFunction(KartBase kart) {
            if (kart.closestBezierPos != null)
            {
                float roadSize = AIManager.Instance.circuit.bezierMeshExtrusion.roadWidth;
                float dotProduct = Vector3.Dot(kart.transform.position - kart.closestBezierPos.GlobalOrigin,
                    kart.closestBezierPos.Normal);
                float direction = Mathf.Sign(dotProduct); 
                float distCenter = Vector3.Distance(kart.closestBezierPos.GlobalOrigin, kart.transform.position);
                return direction * distCenter / roadSize;
            }

            return 0;
        }
        
        
        
        public float Constant(KartBase kart)
        {
            return 1;
        }

        public float SineNormalized(KartBase kart)
        {
            return Mathf.Sin(Time.time - startTime) / 2 + 0.5f;
        }

        public void OnDrawGizmos(KartBase kart)
        {
            Gizmos.color = Color.red;
            
            float distance = kart.closestBezierPos.BezierDistance;
            var nextPos1 = AIManager.Instance.circuit.bezierSpline.GetBezierPos(distance + distCurve);
            var nextPos2 = AIManager.Instance.circuit.bezierSpline.GetBezierPos(distance + distCurve + curvatureOffset);
            Gizmos.DrawSphere(nextPos1.GlobalOrigin,1f);
            Gizmos.DrawSphere(nextPos2.GlobalOrigin,1f);
        }
    }
}