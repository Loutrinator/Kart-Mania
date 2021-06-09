using System;
using System.Collections.Generic;

namespace Game {
   public enum GameMode {
      TimeTrial, Championship, Versus, Editor
   }

   [Serializable]
   public struct GameConfig
   {
      public GameMode mode;
      public List<PlayerConfig> players;
      public List<Race> races;
   }
}