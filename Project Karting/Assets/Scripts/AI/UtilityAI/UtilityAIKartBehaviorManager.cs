using Kart;

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
    }
}