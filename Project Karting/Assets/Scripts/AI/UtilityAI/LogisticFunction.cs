using System;

namespace AI.UtilityAI
{
    public class LogisticFunction : EvaluationFunction
    {
        public override float Evaluate(float value)
        {
            float K = 1;
            float r = 11.4f;
            float a = 300f;
            return (float) (K/(1 + a * Math.Pow(Math.E,-r*value)));
        }
    }
}