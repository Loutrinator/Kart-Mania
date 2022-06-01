using System.Collections.Generic;
using UnityEngine;

public class GhostRecording
{
    public int count;
    public float timeBetweenRecords;
    public List<Vector3> kartPositions;
    public List<Quaternion> kartRotations;
    public List<Vector2> kartMovements;
    public List<bool> kartDrifts;

    public GhostRecording()
    {
        count = 0;
        timeBetweenRecords = 0;
        kartPositions = new List<Vector3>();
        kartRotations = new List<Quaternion>();
        kartMovements = new List<Vector2>();
        kartDrifts = new List<bool>();
    }
}
