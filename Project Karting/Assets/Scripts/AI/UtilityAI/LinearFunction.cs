using System;

namespace AI.UtilityAI
{
    public class LinearFunction : EvaluationFunction
    {
        public override float Evaluate(float value)
        {
            return Math.Max(0, Math.Min(1, value));
        }
    }
}