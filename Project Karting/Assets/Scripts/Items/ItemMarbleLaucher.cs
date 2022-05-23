using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName="MarbleLaucher",menuName="ScriptableObjects/Items/MarbleLaucher",order=0)]
    public class ItemMarbleLaucher : Item
    {
        [SerializeField] private MarbleLauncher marbleLauncherPrefab;
        private MarbleLauncher marbleLauncher;
        private bool instanciated;
        
        public override void OnKeyHold(PlayerRaceInfo info)
        {
            Debug.Log("MarbleLauncher : OnKeyHold");
        }

        public override void OnKeyDown(PlayerRaceInfo info)
        {
            Debug.Log("MarbleLauncher : OnKeyDown");
            if (!instanciated)
            {
                Transform parent = info.kart.transform;
                marbleLauncher = Instantiate(marbleLauncherPrefab,parent.position, parent.rotation, parent);
                marbleLauncher.name = "Marble Launcher";
                instanciated = true;
            }
            marbleLauncher.StretchRubber();
        }

        public override void OnKeyUp(PlayerRaceInfo info)
        {
            Debug.Log("MarbleLauncher : OnKeyUp");
            marbleLauncher.ShootMarble();
        }
    }
}