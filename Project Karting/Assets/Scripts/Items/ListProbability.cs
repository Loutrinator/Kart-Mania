using System;
using System.Collections;
using System.Collections.Generic;

namespace Items
{
    [Serializable]
    public class ListProbability : IEnumerable
    {
        private List<ItemProbability> values;

        public ListProbability()
        {
            values = new List<ItemProbability>();
        }
        public ItemProbability this[int i]
        {
            get { return values[i]; }
            set { values[i] = value; }
        }
        public int Count
        {
            get { return values.Count; }
        }
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