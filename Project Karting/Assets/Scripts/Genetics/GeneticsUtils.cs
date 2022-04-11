using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using Newtonsoft.Json;
using UnityEditor;
using AIGenome = System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.List<float>>>;
using Random = UnityEngine.Random;

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

       public static void WriteData(AIGenome gen, string fileName)
       {
            string json = JsonConvert.SerializeObject(gen);
            string path = "Assets/_Genomes/" + fileName;
            File.WriteAllText(path, json);
       }

        public static AIGenome GetDataFromFile(string fileName)
        {
            string s = File.ReadAllText("Assets/_Genomes/" + fileName);
            return JsonConvert.DeserializeObject<AIGenome>(s);
        }
    }
}