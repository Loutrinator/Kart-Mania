using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Newtonsoft.Json;
using AIGenome = System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.List<float>>>;
using Random = UnityEngine.Random;

namespace Genetics {
    public static class GeneticsUtils {
        public static float mutateRate = 0.1f;
        public static float mutateStrength = 0.1f;
        public static int nbSelected = 3;
        public static float startRandomRange = 0.1f;

        public static AIGenome Reproduce(AIGenome gen1, AIGenome gen2, bool doLerp) {
            Assert.AreEqual(gen1.Count, gen2.Count);

            var result = new AIGenome();
            for (int i = 0; i < gen1.Count; ++i) // actiongroup
            {
                result.Add(new List<List<float>>());
                for (int j = 0; j < gen1[i].Count; ++j) // action
                {
                    result[i].Add(new List<float>());
                    for (int k = 0; k < gen1[i][j].Count; ++k) {
                        var r = Random.value;
                        float value;
                        if (doLerp) value = Mathf.Lerp(gen1[i][j][k], gen2[i][j][k], r);
                        else value = r > 0.5f ? gen1[i][j][k] : gen2[i][j][k];
                        result[i][j].Add(value);
                    }
                }
            }

            return result;
        }
        
        public static List<AIGenome> GenerateChildren(List<AIGenome> sortedKarts, int nbToGenerate, int newParentCount) {
            var children = new List<AIGenome>(nbToGenerate);
            var newParentGenomes = SelectionByRank(sortedKarts, newParentCount);

            for (int i = 0; i < nbToGenerate; ++i) {
                var parent1 = newParentGenomes[Random.Range(0, newParentCount)];
                var parentsWithoutFirst = newParentGenomes.Where(p => p != parent1).ToArray();
                var parent2 = parentsWithoutFirst[Random.Range(0, parentsWithoutFirst.Length)];
                children.Add(Reproduce(parent1, parent2, true).Mutate());
            }

            return children;
        }

        public static AIGenome Mutate(this AIGenome gen) {
            var result = gen;

            float randomValue = Random.Range(0.0f, 1.0f);
            if (randomValue < mutateRate) {
                int randomIndexI = Mathf.RoundToInt(Random.Range(0, gen.Count));
                int randomIndexJ = Mathf.RoundToInt(Random.Range(0, gen[randomIndexI].Count));
                int randomIndexK = Mathf.RoundToInt(Random.Range(0, gen[randomIndexI][randomIndexJ].Count));
                result[randomIndexI][randomIndexJ][randomIndexK] += Random.Range(-1.0f, 1.0f) * mutateStrength;
            }

            return result;
        }

        public static AIGenome RandomizeGenome(this AIGenome gen) {
            var result = new AIGenome();

            for (int i = 0; i < gen.Count; ++i) {
                result.Add(new List<List<float>>());
                for (int j = 0; j < gen[i].Count; ++j) {
                    result[i].Add(new List<float>());
                    for (int k = 0; k < gen[i][j].Count; ++k) {
                        result[i][j].Add(gen[i][j][k] + Random.Range(-startRandomRange, startRandomRange) * 0.5f);
                    }
                }
            }
            return result;
        }

        public static List<AIGenome> SelectionByRank(List<AIGenome> gen, int parentCount) {
            return gen.GetRange(0, parentCount);
        }

        public static AIGenome SelectionFortuneWheel(AIGenome gen) {
            AIGenome result = new AIGenome();

            float randomValue = Random.Range(0, 1);
            float c = 0;

            while (result.Count < nbSelected) {
                for (int i = 0; i < gen.Count; i++) {
                    c = (c + Random.Range(0, 1)) / i;
                    if (randomValue <= c) result.Add(gen[i]);
                }
            }

            return result;
        }

        public static AIGenome SelectionUniform(AIGenome gen) {
            AIGenome result = new AIGenome();

            for (int i = 0; i < nbSelected; i++) result.Add(gen[Mathf.RoundToInt(Random.Range(0, gen.Count - 1))]);

            return result;
        }

        public static void WriteData(AIGenome gen, string fileName) {
            string json = JsonConvert.SerializeObject(gen);
            string path = "Assets/_Genomes/" + fileName;
            File.WriteAllText(path, json);
        }

        public static AIGenome GetDataFromFile(string fileName) {
            string s = File.ReadAllText("Assets/_Genomes/" + fileName);
            return JsonConvert.DeserializeObject<AIGenome>(s);
        }

        public static string GetString(this AIGenome genome) {
            return JsonConvert.SerializeObject(genome);
        }
    }
}