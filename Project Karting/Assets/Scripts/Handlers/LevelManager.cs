using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Handlers {
    [CreateAssetMenu(fileName = "LevelManager", menuName = "ScriptableObjects/Managers/LevelManager")]
    public class LevelManager : ScriptableObject
    {
        #region Singleton
        public static LevelManager instance;
        
        private void OnEnable()
        {
            if (instance != null)
                throw new UnityException(typeof(LevelManager) + " is already instantiated");
            instance = this;
        }

        private void OnDisable()
        {
            instance = null;
        }
        #endregion

        [HideInInspector] public GameConfig gameConfig;

        public void Init() {
            gameConfig = new GameConfig {
                players = new List<PlayerConfiguration>(), 
                races = new List<Race>()
            };
        }
        
        public Race InitLevel() {
            Race currentRace = Instantiate(gameConfig.races[0]);
            currentRace.Init();
            return currentRace;
        }

        public void OnRaceQuit() {
            OnRaceQuit(null);
        }
        
        public void OnRaceQuit(PlayerConfiguration configuration) {
            Init();
            if(configuration != null)
                gameConfig.players.Add(configuration);
        }
    }
}
