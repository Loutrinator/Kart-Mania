using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Items {
    [Serializable]
    public class ItemProbability {
        [SerializeField] public float probability;
        [SerializeField] public int itemId;
    }

    #region ItemManager

    [CreateAssetMenu(fileName = "ItemManagerPreset", menuName = "ScriptableObjects/Managers/ItemManager", order = 0)]
    public class ItemManager : ScriptableObject {
        [HideInInspector, SerializeField] public int nbItems;
        [HideInInspector, SerializeField] public int nbPositions;
        [HideInInspector, SerializeField] public List<Item> items;
        [HideInInspector, SerializeField] public List<ListProbability> itemProbabilities;


/*
        private void Start()
        {
            //GenerateProbabilities();
        }

        private void GenerateProbabilities()
        {
            itemProba = new List<List<ItemProbability>>();
            nbPositions = GameManager.Instance.nbPlayerRacing;
            //nbItems = items.Count;
            for (int i = 0; i < nbPositions; i++)
            {
                float sumProba = 0f;
                itemProba.Append(new List<ItemProbability>());
                for (int j = 0; j < nbItems; j++)
                {
                    float proba = probabilities[i][j]; //[i][j]
                    if (proba > 0)
                    {
                        sumProba += probabilities[i][j]; //[i][j]

                        if (sumProba > 1f)
                        {
                            //Debug.LogError("SUM OF PROBABILITIES AT POSITION " + i + " IS OVER 1f");
                        }
                        else
                        {
                            ItemProbability currentItemProba = new ItemProbability();
                            currentItemProba.probability = sumProba;

                            itemProba[i].Append(currentItemProba);
                        }
                    }
                }

                //TODO: TRIER LES LISTES D'ITEMPROBABILITY PAR PROBABILITE
            }
        }
*/

        [CanBeNull]
        public Item GetRandomItem(int position) {
            float rnd = Random.value;
            for (int i = 0; i < itemProbabilities[position].Count; i++) {
                ItemProbability proba = itemProbabilities[position][i];
                if (rnd <= proba.probability) {
                    return items[proba.itemId];
                }
            }

            return null;
        }
    }

    #endregion
}