using System;

namespace AI.UtilityAI
{
    public class ExponentialFunction : EvaluationFunction
    {
        public float Evaluate(float value)
        {
            float v = (float) (29.7f * Math.Pow(Math.E,(value + 1.64f) * 5.3f));
            return Math.Max(0, Math.Min(1, v));
        }

        public override float GetValue()
        {
            throw new NotImplementedException();
        }
    }
}