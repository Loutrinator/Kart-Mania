using System.Collections.Generic;
using UnityEngine;

public static class TimetrialLoader
{
    private static string key = "timetrial";
    public static GhostRecording GetBestTimeForRace(string race)
    {

        Dictionary<string, GhostRecording> bestTimes = GetBestTimes();
        return bestTimes[race];
    }
    public static void SetBestTimeForRace(string race, GhostRecording ghost)
    {
        Dictionary<string, GhostRecording> bestTimes = GetBestTimes();
        bestTimes[race] = ghost;
        string jsonData = JsonUtility.ToJson(bestTimes);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.Save();
    }
    public static Dictionary<string, GhostRecording> GetBestTimes()
    {
        return JsonUtility.FromJson<Dictionary<string, GhostRecording>>(PlayerPrefs.GetString(key));
    }
}