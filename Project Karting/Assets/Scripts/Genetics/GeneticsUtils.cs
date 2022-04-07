using AI.UtilityAI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Genetics
{
    public static class GeneticsUtils
    {

        public static float mutateRate = 0.2f;
        public static float mutateStrength = 2.0f;
        public static int nbSelected = 5;

        public static List<List<float>> Reproduce(List<List<float>> gen1, List<List<float>> gen2, bool doLerp)
        {
            Assert.AreEqual(gen1.Count, gen2.Count);

            List<List<float>> result = new List<List<float>>();

            for (int i = 0; i < gen1.Count; ++i)
            {
                for (int j = 0; j < gen1[i].Count; j++)
                {
                    var r = Random.value;
                    float value;
                    if (doLerp) value = Mathf.Lerp(gen1[i][j], gen2[i][j], r);
                    else value = r > 0.5f ? gen1[i][j] : gen2[i][j];
                    result[i][j] = value;
                }
            }

            return result;
        }

        public static List<List<float>> Mutate(List<List<float>> gen)
        {
            List<List<float>> result = new List<List<float>>();
            result = gen;

            float randomValue = Random.Range(0.0f, 1.0f);
            if (randomValue < mutateRate)
            {
                int randomIndexI = Mathf.RoundToInt(Random.Range(0, gen.Count));
                int randomIndexJ = Mathf.RoundToInt(Random.Range(0, gen[randomIndexI].Count));
                result[randomIndexI][randomIndexJ] = Random.Range(-1.0f, 1.0f) * mutateStrength;
            }
            return result;
        }

        public static List<List<List<float>>> SelectionByRank(List<List<List<float>>> gen)
        {
            List<List<List<float>>> result = new List<List<List<float>>>();

            for (int i = 0; i < nbSelected; i++)
            {
                result[i] = gen[i];
            }

            return result;
        }
        
        public static List<List<List<float>>> SelectionFortuneWheel(List<List<List<float>>> gen)
        {
            List<List<List<float>>> result = new List<List<List<float>>>();

            float randomValue = Random.Range(0, 1);
            float c = 0;

            while (result.Count < nbSelected)
            {
                for (int i = 0; i < gen.Count; i++)
                {
                    c = (c + Random.Range(0, 1)) / i;
                    if (randomValue <= c) result.Add(gen[i]);
                }
            }
            return result;
        }

        public static List<List<List<float>>> SelectionUniform(List<List<List<float>>> gen)
        {
            List<List<List<float>>> result = new List<List<List<float>>>();

            for (int i = 0; i < nbSelected; i++) result.Add(gen[Mathf.RoundToInt(Random.Range(0, gen.Count - 1))]);

            return result;
        }

       public static void WriteData(UtilityAIGenome gen)
       {
            List<UtilityAIGenome> _data = new List<UtilityAIGenome>();
            _data.Add(gen);

            string json = JsonUtility.ToJson(_data.ToArray());

            System.IO.File.WriteAllText(Application.dataPath + "/Genetics/Genomes/GenomeTest.json",json);
       }
    }
}