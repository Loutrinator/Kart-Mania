using UnityEngine;
using Kart;
namespace Items
{
    public abstract class Item : ScriptableObject
    {
        [HideInInspector] public KartBase owner;
        public string GetName()
        {
            return typeof(object).ToString();
        }

        abstract public void Use();
    }
}