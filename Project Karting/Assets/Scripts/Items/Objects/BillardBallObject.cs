using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class BillardBallObject : ItemObject
    {
        private BillardBall billardBall;

        private void Start()
        {
            billardBall = GetComponent<BillardBall>();
        }

        public override void OnKeyDown(PlayerRaceInfo info)
        {
            transform.parent = null;
            billardBall.audioSource.PlayOneShot(billardBall.spawn);
            billardBall.isThrown = true;
            billardBall.rigidBody.isKinematic = false;
            billardBall.rigidBody.constraints = RigidbodyConstraints.None;
        }

        public override void OnKeyHold(PlayerRaceInfo info)
        {
            return;
        }

        public override void OnKeyUp(PlayerRaceInfo info)
        {
            return;
        }

        public override void ResetItem()
        {
            billardBall.isThrown = false;
        }

    }
}