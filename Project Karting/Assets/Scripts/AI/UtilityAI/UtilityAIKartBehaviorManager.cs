using Handlers;
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
                case EvaluationDataEnum.distanceToCenterOfRoad:
                    value = DistanceToCenterOfRoadFunction(kart);
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

        /**
         * Returns 
         */
        public float DistanceToCenterOfRoadFunction(KartBase kart) {
            float roadSize = GameManager.Instance.currentRace.road.bezierMeshExtrusion.roadWidth;
            float distToCenter = Vector3.Distance(kart.closestBezierPos.GlobalOrigin, kart.transform.position);
            return distToCenter / roadSize;
        }
    }
}