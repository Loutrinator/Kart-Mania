using System;
using System.Collections.Generic;
using UnityEngine;

namespace Handlers
{
    public class LapManager : MonoBehaviour
    {
        
        public static LapManager Instance { get; private set; }

        public List<Action<int>> OnNewLap = new List<Action<int>>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void CheckpointPassed(int checkpointId, int playerId) {
            PlayerRaceInfo player = RaceManager.Instance.playersInfo[playerId];

            //permet de vérifier si premièrement le checkpoint est valide et si il est après le checkpoint actuel
            if (checkpointId < RaceManager.Instance.currentRace.checkpointAmount) {
                if (checkpointId - player.currentCheckpoint == 1) {//si le checkpoint est apres le notre
                    RaceManager.Instance.playersInfo[playerId].currentCheckpoint = checkpointId;
                }
                else if (player.currentCheckpoint == RaceManager.Instance.currentRace.checkpointAmount - 1 && checkpointId == 0) {
                    RaceManager.Instance.playersInfo[playerId].currentCheckpoint = checkpointId;
                    NewLap(playerId);
                }
            }

            //si le checkpoint validé est le dernier de la liste
        }

        private void NewLap(int playerId) {
            //on calcule le temps du lap
            RaceManager.Instance.playersInfo[playerId].previousLapTime = Time.time - RaceManager.Instance.playersInfo[playerId].currentLapStartTime;
            RaceManager.Instance.playersInfo[playerId].currentLapStartTime = Time.time;

            float diff = RaceManager.Instance.playersInfo[playerId].previousLapTime - RaceManager.Instance.playersInfo[playerId].bestLapTime;
            RaceManager.Instance.playersInfo[playerId].lap += 1; // doit être appelé ici pour mettre à jour la diff dans la HUD
            RaceManager.Instance.playersInfo[playerId].lapsTime.Add( RaceManager.Instance.playersInfo[playerId].previousLapTime); //doit être appelé ici pour être sur que le previousLapTime est à jour
            
            //TODO : il y a un problème, la liste n'est pas bien conservé car à l'affichage du score
            // board de fin de course, il ne reste que le dernier temps dans la liste
            foreach (var t in  RaceManager.Instance.playersInfo[playerId].lapsTime)
            {
                //Debug.Log("time add " + Utils.DisplayHelper.FloatToTimeString(t));   
            }
            if (RaceManager.Instance.playersInfo[playerId].previousLapTime < RaceManager.Instance.playersInfo[playerId].bestLapTime) {
                RaceManager.Instance.playersInfo[playerId].bestLapTime = RaceManager.Instance.playersInfo[playerId].previousLapTime;
            }

            /*bestTime.text = floatToTimeString(RaceManager.Instance.playersInfo[playerId].bestLapTime);
        currentTime.text = floatToTimeString(RaceManager.Instance.playersInfo[playerId].previousLapTime);
        if (RaceManager.Instance.playersInfo[playerId].lap != 1) {
            timeDiff.text = floatToTimeString(diff);
            if (diff > 0) {
                timeDiff.color = Color.red;
            }
            else {
                timeDiff.color = Color.green;
            }
        }*/
            
            // TODO : il serait préférable de terminé le race pour ce playerID
            // mais de finir pour la totalité des participants selon une autre condition
            // quand on sera en écran splitté (cf.  document de game design pour la condition de fin de course)

            foreach (var onNewLapAction in OnNewLap)
            {
                onNewLapAction.Invoke(playerId);
            }
        }
    }
}