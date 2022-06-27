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
            billardball.transform.position = new Vector3(parent.position.x - 6, billardball.transform.position.y + 2, parent.position.z);
            return billardball;
        }
    }
}