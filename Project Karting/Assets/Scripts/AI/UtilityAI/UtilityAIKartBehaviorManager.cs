using Kart;
using Handlers;
using SplineEditor.Runtime;
using UnityEngine;

namespace AI.UtilityAI
{
    public class UtilityAIKartBehaviorManager
    {
        private float curvatureOffset = 2f;
        private float distCurve = 5f;
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
                case EvaluationDataEnum.speed:
                    value = SpeedFunction(kart);
                    break;
                case EvaluationDataEnum.curvatureOfTheRoad:
                    value = CurvatureOfRoadFunction(kart);
                    break;
                case EvaluationDataEnum.constant:
                    value = Constant(kart);
                    break;
                case EvaluationDataEnum.sineNormalized:
                    value = SineNormalized(kart);
                    break;
            }

            return value;
        }

        public float SpeedFunction(KartBase kart)
        {
            var worldSpeed = kart.transform.position + kart.currentVelocity;
            var localSpeed = kart.transform.worldToLocalMatrix * worldSpeed;

            return localSpeed.z;
        }

        public float CurvatureOfRoadFunction(KartBase kart)
        {
            float distance = kart.closestBezierPos.BezierDistance;
            var kartPosOnCurve = GameManager.Instance.currentRace.road.bezierSpline.GetBezierPos(distance);

            var nextPos1 = GameManager.Instance.currentRace.road.bezierSpline.GetBezierPos(distance + distCurve);
            var nextPos2 = GameManager.Instance.currentRace.road.bezierSpline.GetBezierPos(distance + distCurve + curvatureOffset);
            var nextPos3 = GameManager.Instance.currentRace.road.bezierSpline.GetBezierPos(distance + distCurve + 2*curvatureOffset);

            float dot1 = Vector3.Dot(nextPos1.Normal, nextPos2.Tangent);
            float dot2 = Vector3.Dot(nextPos2.Normal, nextPos3.Tangent);

            return (dot1 + dot2)/2f;
        }

        public Vector3 ClosestPointInCurvature(KartBase kart)
        {
            Vector3 pointCurvature = Vector3.zero;

            float distance = kart.closestBezierPos.BezierDistance;
            var nextPos = GameManager.Instance.currentRace.road.bezierSpline.GetBezierPos(distance + distCurve);
            int dir = Mathf.RoundToInt(CurvatureOfRoadFunction(kart));

            float roadWith = GameManager.Instance.currentRace.road.bezierMeshExtrusion.roadWidth;
            pointCurvature = kart.closestBezierPos.GlobalOrigin + dir * nextPos.Normal * roadWith;

            return pointCurvature;
        }

        public float DistanceToCenterOfRoadFunction(KartBase kart) {
            float roadSize = GameManager.Instance.currentRace.road.bezierMeshExtrusion.roadWidth;
            float distCenter = Vector3.Distance(kart.closestBezierPos.GlobalOrigin, kart.transform.position);
            return distCenter / roadSize;
        }
        
        
        
        public float Constant(KartBase kart)
        {
            return 1;
        }

        public float SineNormalized(KartBase kart)
        {
            return Mathf.Sin(Time.time - startTime) / 2 + 0.5f;
        }
    }
}