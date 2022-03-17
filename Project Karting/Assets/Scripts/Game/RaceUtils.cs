using System;
using System.Collections.Generic;
using Kart;
using UnityEngine;

namespace Game {
    public class RaceUtils : MonoBehaviour {
        public static List<KartBase> GetRanking(List<KartBase> levelKarts) {
            var karts = new List<KartBase>();
            for (int i = levelKarts.Count - 1; i >= 0; --i) {
                karts.Add(levelKarts[i]);
            }

            karts.Sort((kart1, kart2) =>
                Math.Sign(kart2.closestBezierPos.BezierDistance - kart1.closestBezierPos.BezierDistance));

            return karts;
        }
    }
}