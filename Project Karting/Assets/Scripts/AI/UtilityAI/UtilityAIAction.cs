using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.UtilityAI
{
    [Serializable]
    public class UtilityAIAction
    {
        [SerializeField] public string actionName = "Action";
        [SerializeField] public List<EvaluationFunctionSettings> evaluationFunctions;

        public float getUtility()
        {
            float coeffSum = 0;
            float sum = 0;
            foreach (var evalSetting in evaluationFunctions)
            {
                coeffSum += evalSetting.coefficient;
                sum += evalSetting.coefficient * evalSetting.evaluationCurve.Evaluate(evalSetting.function.GetValue());
            }

            return Math.Min(1,Math.Max(0,sum / coeffSum)); //keeping it between 0 and 1 just in case
        }
    }
}