using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that records a 
/// </summary>
public class GhostRecorder : MonoBehaviour
{
    public GhostController ghost;
    public bool isRecording = true;
    public bool isReplaying = false;
    [SerializeField] private float timeBetweenRecords = 0.1f;
    [SerializeField] private float maxTimeRecorded = 20f;//in seconds 

    private float timeElapsed = 0; //time elapsed since previous record
    private float recordedTime = 0; //the total recorded time
    private int replayIndex;

    private GhostRecording recording;

    private void Start()
    {
        ResetRecords();
        if (isReplaying)
        {
            LoadRecording();
        }
    }

    private void ResetRecords()
    {
        recording = new GhostRecording();
        recording.timeBetweenRecords = timeBetweenRecords;
        timeElapsed = 0;
        recordedTime = 0;
        replayIndex = 0;
    }
    
    private void Update()
    {
        recordedTime += Time.deltaTime;
        if (isRecording)
        {
            timeElapsed += Time.deltaTime;
            if (recordedTime < maxTimeRecorded)
            {
                
                if (timeElapsed >= timeBetweenRecords)
                {
                    timeElapsed -= timeBetweenRecords;
                    RecordNewValues();
                }
            }
            else
            {
                StopRecording();
                SaveRecording();
            }
        }
        else if(isReplaying)
        {
            if (replayIndex >= recording.count)
            {
                recordedTime = 0f;
            }
            replayIndex = (int)(recordedTime / recording.timeBetweenRecords);
            float lerp = (recordedTime-replayIndex) / recording.timeBetweenRecords;
            PlayRecordValues(replayIndex, lerp);
        }
    }

    private void StopRecording()
    {
        isRecording = false;
        recording.count = recording.kartPositions.Count;

    }
    private void SaveRecording()
    {
        string recordingJson = JsonUtility.ToJson(recording);
        PlayerPrefs.SetString("ghost",recordingJson);
        PlayerPrefs.Save();
        Debug.Log("Ghost data saved !");
    }
    private void LoadRecording()
    {
        Debug.Log("Loading ghost data");
        string recordingJson = PlayerPrefs.GetString("ghost");
        recording = JsonUtility.FromJson<GhostRecording>(recordingJson);
    }

    private void RecordNewValues()
    {
        recording.kartPositions.Add(ghost.GetKartPosition());
        recording.kartRotations.Add(ghost.GetKartRotation());
        recording.kartMovements.Add(ghost.GetKartMovement()); 
        recording.kartDrifts.Add(ghost.GetKartDrift());
    }
    private void PlayRecordValues(int index, float lerp)
    {
        Debug.Log("index " + index + " lerp " + lerp);
        int secondIndex = index < recording.count - 1 ? index + 1 : index;
        Vector3 pos1 = recording.kartPositions[index];
        Vector3 pos2 = recording.kartPositions[secondIndex];
        //ghost.SetKartPosition(pos1);//
        ghost.SetKartPosition(Vector3.Lerp(pos1,pos2, lerp));
        Quaternion rot1 = recording.kartRotations[index];
        Quaternion rot2 = recording.kartRotations[secondIndex];
        //ghost.SetKartRotation(rot1); //
        ghost.SetKartRotation(Quaternion.Lerp(rot1,rot2,lerp));
    }
}
