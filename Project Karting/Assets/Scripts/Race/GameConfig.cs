
using System.Collections.Generic;

public enum GameMode{timeTrial, championship, versus, editor}
public struct GameConfig
{
   public GameMode mode;
   public List<PlayerConfig> players;
   public List<Race> races;
}
