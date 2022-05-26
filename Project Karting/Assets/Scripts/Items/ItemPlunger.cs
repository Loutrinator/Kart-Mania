using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Plunger", menuName = "ScriptableObject/Items/Plunger", order = 0)]
    public class ItemPlunger : Item
    {
        [SerializeField] private Plunger plunger;
        public float offset = 2.0f;

        public override void OnKeyDown(PlayerRaceInfo info)
        {
            var obj = Instantiate(plunger, info.kart.transform);

            obj.transform.position += info.kart.closestBezierPos.LocalUp * offset;
        }

        public override void OnKeyHold(PlayerRaceInfo info)
        {

        }

        public override void OnKeyUp(PlayerRaceInfo info)
        {

        }
    }
}