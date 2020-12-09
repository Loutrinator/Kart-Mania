using System.Security.Permissions;
using Kart;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Items
{
    
    [CreateAssetMenu(fileName="SpeedKey",menuName="ScriptableObject/SpeedKey",order=0)]
    public class ItemSpeedKey : Item
    {
        public Stats boost;
        public float duration;
        public override void Use(PlayerRaceInfo info )
        {
            info.kart.keyhole.InsertKey(() => KeyCrankedUp(info));
            base.Use(info);
        }

        private void KeyCrankedUp(PlayerRaceInfo info)
        {
            StatPowerup powerup = new StatPowerup(boost,duration);
            powerup.powerupUsed = () => ExpiredPowerup(info);
            info.kart.addPowerup(powerup);
        }
        private void ExpiredPowerup(PlayerRaceInfo info)
        {
            info.kart.keyhole.RemoveKey();
        }

        public override void OnKeyDown(PlayerRaceInfo info)
        {
            Use(info);
        }
    }
}