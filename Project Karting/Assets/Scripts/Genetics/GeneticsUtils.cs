using UnityEngine;
using UnityEngine.Assertions;

namespace Genetics
{
    public static class GeneticsUtils
    {

        public static float mutateRate = 0.2f;
        public static float mutateStrength = 2.0f;

        public static float[] Reproduce(float[] gen1, float[] gen2, bool doLerp)
        {
            Assert.AreEqual(gen1.Length, gen2.Length);

            float[] result = new float[gen1.Length];

            for (int i = 0; i < result.Length; ++i)
            {
                var r = Random.value;
                float value;
                if (doLerp) value = Mathf.Lerp(gen1[i], gen2[2], r);
                else value = r > 0.5f ? gen1[i] : gen2[i];
                result[i] = value;
            }

            return result;
        }

        public static float[] Mutate(float[] gen)
        {
            float[] result = new float[gen.Length];
            result = gen;

            float randomValue = Random.Range(0.0f, 1.0f);
            if (randomValue < mutateRate)
            {
                int randomIndex = Mathf.RoundToInt(Random.Range(0, gen.Length));
                result[randomIndex] = Random.Range(-1.0f, 1.0f) * mutateStrength;
            }
            return result;
        }

        public static float[] SelectionByRank(float[] gen, int nbSelected)
        {
            float[] result = new float[nbSelected];

            for(int i = 0; i < nbSelected; i++)
            {
                result[i] = gen[i];
            } 

            return result;
        }
    }
}