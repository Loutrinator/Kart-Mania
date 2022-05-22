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
            Debug.Log("MarbleLauncher : OnKeyHold");
            throw new System.NotImplementedException();
        }

        public override void OnKeyDown(PlayerRaceInfo info)
        {
            Debug.Log("MarbleLauncher : OnKeyDown");
            Transform parent = info.kart.transform;
            marbleLauncher = Instantiate(marbleLauncherPrefab,parent.position, parent.rotation, parent);
            marbleLauncher.name = "Marble Launcher";
            marbleLauncher.StretchRubber();
        }

        public override void OnKeyUp(PlayerRaceInfo info)
        {
            Debug.Log("MarbleLauncher : OnKeyUp");
            marbleLauncher.ShootMarble();
        }
    }
}