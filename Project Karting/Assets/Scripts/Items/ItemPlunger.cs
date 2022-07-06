using Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class ItemPlunger : ItemObject
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

        public override void ResetItem()
        {
            throw new System.NotImplementedException();
        }
    }
}