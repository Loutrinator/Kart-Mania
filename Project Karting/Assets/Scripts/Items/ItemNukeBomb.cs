using System.Security.Permissions;
using Kart;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Items
{
    
    [CreateAssetMenu(fileName="NukeBomb",menuName="ScriptableObject/Items/NukeBomb",order=0)]
    public class ItemNukeBomb : Item
    {
        [SerializeField] private NukeBomb prefab;
        public Vector3 spawnPoint = new Vector3(550,600,150);
        public override void OnKeyDown(PlayerRaceInfo info)
        {
            Debug.Log("OnKeyDown");
            Transform transform = info.kart.transform;
            NukeBomb bomb = Instantiate(prefab, spawnPoint, Quaternion.identity, transform).GetComponent<NukeBomb>();
            bomb.target = new GameObject("Target").transform;
            Use(info); 
        }
    }
}