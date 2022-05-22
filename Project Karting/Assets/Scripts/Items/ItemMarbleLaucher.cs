using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName="MarbleLaucher",menuName="ScriptableObjects/Items/MarbleLaucher",order=0)]
    public class ItemMarbleLaucher : Item
    {
        [SerializeField] private MarbleLauncher marbleLauncherPrefab;
        private MarbleLauncher marbleLauncher;
        
        public override void OnKeyHold(PlayerRaceInfo info)
        {
            throw new System.NotImplementedException();
        }

        public override void OnKeyDown(PlayerRaceInfo info)
        {
            marbleLauncher = Instantiate(marbleLauncherPrefab, Vector3.zero, Quaternion.identity, info.kart.transform);
            marbleLauncher.name = "Marble Launcher";
            marbleLauncher.StretchRubber();
        }

        public override void OnKeyUp(PlayerRaceInfo info)
        {
            marbleLauncher.ShootMarble();
        }
    }
}