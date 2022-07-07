using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Items
{
    [CreateAssetMenu(fileName = "PlungerData", menuName = "ScriptableObjects/Items/PlungerData", order = 0)]
    public class PlungerData : ItemData
    {
        [SerializeField] private Plunger plungerPrefab;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector3 offset;

        public override Sprite GetIcon()
        {
            return icon;
        }

        public override ItemObject GiveItem(Transform parent)
        {
            var plunger = Instantiate(plungerPrefab, parent.position, parent.rotation, parent);
            plunger.transform.rotation *= Quaternion.AngleAxis(180, plunger.transform.up);
            plunger.transform.localPosition += offset;
            return plunger;
        }
    }
}