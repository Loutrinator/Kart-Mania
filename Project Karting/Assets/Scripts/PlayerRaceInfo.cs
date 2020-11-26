using Kart;

public struct PlayerRaceInfo
{
    public KartBase kart;
    public int playerId;//on sait jamais
    public int lap;//the current lap
    public int position;//the kart's position in te race (1rst, 2nd etc)
    public int currentCheckpoint;//the previous checkpoint passed
    public float bestLapTime;
    public float previousLapTime;
    public float currentLapStartTime;

    public PlayerRaceInfo(KartBase k, int id)
    {
        bestLapTime = float.MaxValue;
        previousLapTime = float.MaxValue;
        kart = k;
        playerId = id;
        lap = 1;
        position = id;
        currentCheckpoint = 0;
        currentLapStartTime = 0f;
    }
}
