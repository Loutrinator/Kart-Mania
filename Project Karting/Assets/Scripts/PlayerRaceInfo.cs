using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerRaceInfo
{
    public KartBase kart;
    public int playerId;//on sait jamais
    public int lap;//the current lap
    public int position;//the kart's position in te race (1rst, 2nd etc)
    public int currentCheckpoint;//the previous checkpoint passed
}
