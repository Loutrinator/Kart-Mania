using System;
using System.Collections.Generic;
using Items;
using Kart;
using Player;
using UnityEngine;

public class PlayerRaceInfo
{
    //Kart
    public int playerId; //on sait jamaispublic int lap;
    public KartBase kart;
    //public KartController controller;
    public CameraFollowPlayer camera;
    
    //Items
    public ItemObject Item;
    public bool hasItem => Item != null;
    public bool itemIsInUse;
    
    //Race laps and checkpoints
    public int lap;
    public List<float> lapsTime;
    public int currentCheckpoint;
    public float bestLapTime;
    public float previousLapTime;
    public float currentLapStartTime;
    public float spawnDistance;
 
    //TODO: Pense un jour a gérer le classement en course

    public PlayerRaceInfo(KartBase k, int id)
    {
        bestLapTime = float.MaxValue;
        previousLapTime = float.MaxValue;
        kart = k;
        playerId = id;
        lap = 1;
        currentCheckpoint = 0;
        currentLapStartTime = 0f;
        kart.GetPlayerID += () => playerId;
        lapsTime = new List<float>();
    }

    public float getDistanceTraveled(float roadLength)
    {
        //Debug.Log("TRAVELED : dist " + kart.closestBezierPos.BezierDistance + " lap " + lap + " roadLength " + " spawnDistance " + spawnDistance);
        float res = kart.closestBezierPos.BezierDistance + lap * roadLength - spawnDistance;
        //Debug.Log("TRAVELED RES :  " + res);
        return res;
    }
    
}