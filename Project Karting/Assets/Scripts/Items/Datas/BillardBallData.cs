using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "BillardBallData", menuName = "ScriptableObjects/Items/BillardBallData", order = 0)]
    public class BillardBallData : ItemData
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private BillardBallObject billardBallPrefab;

        public override Sprite GetIcon()
        {
            return icon;
        }
        public override ItemObject GiveItem(Transform parent)
        {
            var billardball = Instantiate(billardBallPrefab, parent.position, parent.rotation, parent);
            billardball.transform.position = parent.position + (parent.forward * -6) + (parent.up * 3);
            return billardball;
        }
    }
}