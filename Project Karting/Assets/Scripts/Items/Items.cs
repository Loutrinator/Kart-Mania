using UnityEngine;

namespace Items
{
    public abstract class Item : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;

        public GameObject Prefab => prefab;

        public string GetName()
        {
            return gameObject.name;
        }

        public abstract void Use();

        public virtual void onKeyHold(){return;}
        public virtual void onKeyDown(){return;}
        public virtual void onKeyUp(){return;}
    }
}