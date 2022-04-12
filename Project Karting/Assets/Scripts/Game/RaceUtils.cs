using System;
using System.Collections.Generic;
using AI.UtilityAI;

namespace Game {
    public static class RaceUtils {
        public static List<UtilityAIController> GetRankingUtilityAI(IEnumerable<UtilityAIController> levelKarts) {
            var karts = new List<UtilityAIController>();
            foreach (var kart in levelKarts) {
                karts.Add(kart);
            }

            karts.Sort((kart1, kart2) =>
                Math.Sign(kart2.kart.closestBezierPos.BezierDistance - kart1.kart.closestBezierPos.BezierDistance));

            return karts;
        }
    }
}