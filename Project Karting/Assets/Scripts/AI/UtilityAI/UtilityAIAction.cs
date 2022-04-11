using System;
using System.Collections.Generic;
using Kart;
using UnityEngine;

namespace AI.UtilityAI
{
    [Serializable]
    public class UtilityAIAction : AIAction
    {
        [SerializeField] public List<EvaluationFunctionSettings> evaluationFunctions;

        public float getUtility(KartBase kart, List<float> evalCoefficients)
        {
            float sum = 0;
            for (var index = 0; index < evaluationFunctions.Count; index++) {
                var evalSetting = evaluationFunctions[index];
                //sum += evalSetting.coefficient * evalSetting.evaluationCurve.Evaluate(UtilityAIKartBehaviorManager.Instance.GetValue(evalSetting.evaluationData,kart)); //evalSetting.function.GetValue());
                sum += evalCoefficients[index] *
                       evalSetting.evaluationCurve.Evaluate(
                           UtilityAIKartBehaviorManager.Instance.GetValue(evalSetting.evaluationData,
                               kart)); //evalSetting.function.GetValue());
            }

            return sum; //keeping it between 0 and 1 just in case
        }
    }
}