using UnityEngine;

namespace Items
{
    
    [CreateAssetMenu(fileName="SpeedKey",menuName="ScriptableObjects/Items/SpeedKey",order=0)]
    public class ItemSpeedKey : Item
    {
        public Stats boost;
        public float duration;
        public override void Use(PlayerRaceInfo info )
        {
            info.kart.keyhole.InsertKey(Keyhole.RewindMode.auto,() => KeyCrankedUp(info));
            base.Use(info);
        }


        private void KeyCrankedUp(PlayerRaceInfo info)
        {
            StatPowerup powerup = new StatPowerup(boost,duration);
            powerup.powerupUsed = () => ExpiredPowerup(info);
            info.kart.AddPowerup(powerup);
        }
        private void ExpiredPowerup(PlayerRaceInfo info)
        {
            info.kart.keyhole.RemoveKey();
        }

        public override void OnKeyDown(PlayerRaceInfo info)
        {
            Use(info);
        }
        
        public override void OnKeyHold(PlayerRaceInfo info) {
            
        }

        public override void OnKeyUp(PlayerRaceInfo info) {
            
        }
    }
}