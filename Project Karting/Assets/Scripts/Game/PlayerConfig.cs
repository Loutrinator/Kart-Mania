using System;
using Kart;

namespace Game {
    public enum PlayerType
    {
        Player,
        Ia
    }

    [Serializable]
    public struct PlayerConfig
    {
        public string name;
        public KartBase kartPrefab;
        public PlayerType type;
    }
}