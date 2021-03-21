using UnityEngine;
using Kart;

public enum PlayerType
{
    player,
    IA
}

public struct PlayerConfig
{
    public string name;
    public KartBase kartPrefab;
    public PlayerType type;
}
