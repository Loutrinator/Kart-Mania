using Game;
using UnityEngine;

namespace Handlers
{
    public class RaceManager : MonoBehaviour
    {
        
        public PlayerRaceInfo[] playersInfo;
        public Race currentRace;
        public GameState gameState;
        
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
        
        
        public bool RaceHadBegun() {
            return  (gameState == GameState.race);
        }
    }
}