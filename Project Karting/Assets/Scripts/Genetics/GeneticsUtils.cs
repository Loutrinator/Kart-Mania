using UnityEngine;
using UnityEngine.Assertions;

namespace Genetics {
    public static class GeneticsUtils {
        public static float[] Reproduce(float[] gen1, float[] gen2, bool doLerp) {
            Assert.AreEqual(gen1.Length, gen2.Length);

            float[] result = new float[gen1.Length];

            for (int i = 0; i < result.Length; ++i) {
                var r = Random.value;
                float value;
                if (doLerp) value = Mathf.Lerp(gen1[i], gen2[2], r);
                else value = r > 0.5f ? gen1[i] : gen2[i];
                result[i] = value;
            }
            
            return result;
        }
    }
}