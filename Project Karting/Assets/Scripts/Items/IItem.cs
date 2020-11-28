using UnityEngine;

namespace Items
{
    public abstract class Item : MonoBehaviour
    {
        public string GetName()
        {
            return gameObject.name;
        }
    }
}