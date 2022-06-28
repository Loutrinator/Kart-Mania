using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [Serializable]
    public class ListProbability
    {
        [SerializeField] private List<ItemProbability> values;

        public ListProbability()
        {
            values = new List<ItemProbability>();
        }
        public ItemProbability this[int i]
        {
            get => values[i];
            set => values[i] = value;
        }
        public int Count => values.Count;

        public void Add(ItemProbability item)
        {
            values.Add(item);
        }
        public void RemoveAt(int i)
        {
            values.RemoveAt(i);
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}