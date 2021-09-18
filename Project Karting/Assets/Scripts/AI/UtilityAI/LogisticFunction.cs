using System;

namespace AI.UtilityAI
{
    public class LogisticFunction : EvaluationFunction
    {
        public float Evaluate(float value)
        {
            float K = 1;
            float r = 11.4f;
            float a = 300f;
            return (float) (K/(1 + a * Math.Pow(Math.E,-r*value)));
        }

        public override float GetValue()
        {
            throw new NotImplementedException();
        }
    }
}