
using System;
using System.Collections.Generic;

public enum GameMode{timeTrial, championship, versus, editor}
[Serializable]
public struct GameConfig
{
   public GameMode mode;
   public List<PlayerConfig> players;
   public List<Race> races;
}
