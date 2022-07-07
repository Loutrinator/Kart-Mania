using UnityEngine;

namespace Items
{
    public class SpeedKey : ItemObject
    {
        public Stats boost;
        public float duration;
        public ShakeTransformEventData shakeEffect;

        public override void Use(PlayerRaceInfo info )
        {
            Debug.Log("putaing con");
            info.kart.keyhole.InsertKey(Keyhole.RewindMode.auto,() => KeyCrankedUp(info));
            base.Use(info);
        }

        private void KeyCrankedUp(PlayerRaceInfo info)
        {
            Debug.Log("KeyCrankedUp");
            StatPowerup powerup = new StatPowerup(boost,duration);
            powerup.powerupUsed = () => ExpiredPowerup(info);
            info.kart.effects.ApplyPowerup(powerup, shakeEffect, duration, 4);
        }
        private void ExpiredPowerup(PlayerRaceInfo info)
        {
            info.kart.keyhole.RemoveKey();
            Destroy(gameObject);
        }

        public override void ResetItem()
        {
            throw new System.NotImplementedException();
        }

        public override void OnKeyHold(PlayerRaceInfo info)
        {
            throw new System.NotImplementedException();
        }

        public override void OnKeyDown(PlayerRaceInfo info)
        {
            Use(info);
        }

        public override void OnKeyUp(PlayerRaceInfo info)
        {
            throw new System.NotImplementedException();
        }
    }
}