using System;

namespace AI.UtilityAI
{
    public class LinearFunction : EvaluationFunction
    {
        public float Evaluate(float value)
        {
            return Math.Max(0, Math.Min(1, value));
        }

        public override float GetValue()
        {
            throw new NotImplementedException();
        }
    }
}