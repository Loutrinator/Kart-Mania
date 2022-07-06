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
        [HideInInspector, SerializeField] public List<ItemData> items;
        [HideInInspector, SerializeField] public List<ListProbability> itemProbabilities;

        [CanBeNull]
        public ItemData GetRandomItem(int position) {
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