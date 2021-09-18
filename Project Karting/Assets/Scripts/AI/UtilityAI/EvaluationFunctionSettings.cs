using System;
using UnityEngine;

namespace AI.UtilityAI
{
    [Serializable]
    public struct EvaluationFunctionSettings
    {
        [SerializeField] public EvaluationFunction function;
        [SerializeField] public AnimationCurve evaluationCurve;
        [SerializeField] public float coefficient;
        
    }
}