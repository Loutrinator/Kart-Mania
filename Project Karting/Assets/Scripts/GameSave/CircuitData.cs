using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace GameSave
{
    public class CircuitData
    {
        private readonly Race _circuit;

        private class RaceScore
        {
            private readonly List<float> _lapTimes;
            private readonly string _circuitName;
            private readonly int _rank;
            private string DataKey => "Score_" + _circuitName + "_" + _rank;

            public RaceScore(int laps, string circuitName, int rank){
                _lapTimes = new List<float>(laps);
                _circuitName = circuitName;
                _rank = rank;
                Load();
            }

            public void RegisterLapTime(int lap, float time)
            {
                if (lap > _lapTimes.Count - 1)
                {
                    throw new IndexOutOfRangeException("Lap out of range : " + lap + " ; max : " +
                                                       (_lapTimes.Count - 1));
                }
                _lapTimes[lap] = time;
            }

            private void Load()
            {
                int laps = _lapTimes.Count;
                for (int i = 0; i < laps; ++i)
                {
                    _lapTimes[i] = PlayerPrefs.GetFloat(DataKey);
                }
            }

            public void Save()
            {
                int laps = _lapTimes.Count;
                for (int i = 0; i < laps; ++i)
                {
                    PlayerPrefs.SetFloat(DataKey, _lapTimes[i]);
                }
            }
        }

        private List<RaceScore> _raceScores;
        
        public CircuitData(Race circuit)
        {
            _circuit = circuit;
            LoadData();
        }

        private void LoadData()
        {
            int scoreCount = PlayerPrefs.GetInt("ScoreCount_" + _circuit.circuitName, 0);
            _raceScores = new List<RaceScore>(scoreCount);
            for (int i = 0; i < _raceScores.Count; ++i)
            {
                _raceScores[i] = new RaceScore(_circuit.laps, _circuit.circuitName, i+1);
            }
        }
        
        private void SaveData()
        {
            int scoreCount = _raceScores.Count;
            foreach (var raceScore in _raceScores)
            {
                raceScore.Save();
            }
            PlayerPrefs.SetInt("ScoreCount_" + _circuit.circuitName, scoreCount);
        }
    }
}
