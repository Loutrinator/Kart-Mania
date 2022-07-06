using System;
using System.Collections.Generic;
using AI.UtilityAI;

namespace Game {
    public static class RaceUtils {
        public static List<PlayerRaceInfo> GetRankingUtilityAI(IEnumerable<PlayerRaceInfo> levelKarts, float raceLength) {
            var karts = new List<PlayerRaceInfo>();
            foreach (var kart in levelKarts) {
                karts.Add(kart);
            }

            karts.Sort((kart1, kart2) =>
                Math.Sign(kart2.getDistanceTraveled(raceLength) - kart1.getDistanceTraveled(raceLength)));

            return karts;
        }
    }
}