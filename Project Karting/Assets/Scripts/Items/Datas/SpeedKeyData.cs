using UnityEngine;

namespace Items
{
    
    [CreateAssetMenu(fileName="SpeedKeyData",menuName="ScriptableObjects/Items/SpeedKeyData",order=0)]
    public class SpeedKeyData : ItemData
    {
        public Stats boost;
        public float duration;
        [SerializeField] private Sprite icon;

        public override Sprite GetIcon()
        {
            return icon;
        }
        public override ItemObject GiveItem(Transform parent)
        {
            //WoodBox marbleLauncher = Instantiate(prefab,parent.position,parent.rotation,parent);
            //marbleLauncher.ResetItem();
            return null;//marbleLauncher;
        }
        /*public override void Use(PlayerRaceInfo info )
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
            
        }*/
    }
}