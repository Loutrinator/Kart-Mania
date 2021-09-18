using System;
using UnityEngine;

namespace AI.UtilityAI
{
    [Serializable]
    public abstract class EvaluationFunction
    {
        [SerializeField] public string name;
        public abstract float GetValue();
    }
}