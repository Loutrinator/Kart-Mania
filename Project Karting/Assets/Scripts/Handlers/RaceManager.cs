using Game;
using Items;
using UnityEngine;

namespace Handlers
{
    public class RaceManager : MonoBehaviour
    {
        
        public PlayerRaceInfo[] playersInfo;
        public Race currentRace;
        public GameState gameState;
        public ItemManager itemManager;
        
        public static RaceManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public PlayerRaceInfo GetPlayerRaceInfo(int id)
        {
            foreach (var info in playersInfo) {
                if (info.playerId == id) return info;
            }
            return null;
        }
        
        public bool RaceHadBegun() {
            return  (gameState == GameState.Race);
        }
    }
}