using UnityEngine;

namespace Utils
{
   public static class DisplayHelper
   {
      public static string floatToTimeString(float time)
      {
         string prefix = "";
         if (time< 0) prefix = "-";
         time = Mathf.Abs(time);
         int minutes = (int) time / 60;
         int seconds = (int) time - 60 * minutes;
         int milliseconds = (int) (1000 * (time - minutes * 60 - seconds));
         return prefix + string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds );
      }
   }
}