using Kart;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Items
{
    
    [CreateAssetMenu(fileName="SpeedKey",menuName="ScriptableObject/SpeedKey",order=0)]
    public class SpeedKey : ItemAntoineVersion
    {
        public StatPowerup powerup;
        private bool activated;
        public override void Use()
        {
            owner.keyhole.InsertKey(KeyCrankedUp);
        }

        private void KeyCrankedUp()
        {
            powerup.powerupUsed = ExpiredPowerup;
            owner.addPowerup(powerup);
        }
        private void ExpiredPowerup()
        {
            owner.keyhole.RemoveKey();
        }
    }
}