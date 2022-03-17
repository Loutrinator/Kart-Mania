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

        public float getUtility(KartBase kart)
        {
            float coeffSum = 0;
            float sum = 0;
            foreach (var evalSetting in evaluationFunctions)
            {
                coeffSum += evalSetting.coefficient;
                sum += evalSetting.coefficient *
                       evalSetting.evaluationCurve.Evaluate(UtilityAIKartBehaviorManager.Instance.GetValue(evalSetting.evaluationData,kart)); //evalSetting.function.GetValue());
            }

            return Math.Min(1,Math.Max(0,sum / coeffSum)); //keeping it between 0 and 1 just in case
        }
    }
}