using Kart;
using UnityEngine;

namespace AI.UtilityAI
{
    public class UtilityAIKartBehaviorManager
    {
        private static UtilityAIKartBehaviorManager instance;
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
            return kart.currentVelocity.magnitude;
        }

        public float CurvatureOfRoadFunction(KartBase kart)
        {
            return 1;
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