using Game;
using Handlers;
using TMPro;
using UnityEngine;

namespace UI {
    public class RaceHUDController : MonoBehaviour
    {
        [Header("Front")] [SerializeField] private TextMeshProUGUI bestTime;
        [SerializeField] private TextMeshProUGUI currentTime;
        [SerializeField] private TextMeshProUGUI timeDiff;
        [SerializeField] private TextMeshProUGUI lap;
        [SerializeField] private TextMeshProUGUI speed;
        public ItemWheel itemWheel;

        private PlayerRaceInfo _info;
        private int _id;
        public static int nbInstances;

        private void Start()
        {
            _id = nbInstances++;
            _info = GameManager.Instance.GetPlayerRaceInfo(_id);
            bestTime.text = "";
            currentTime.text = "00:00:00";
            timeDiff.text = "";
            lap.text = "0/" + RaceManager.Instance.currentRace.laps;
        }

        public void Init(GameMode mode) {
            if(mode == GameMode.TimeTrial)
                itemWheel.gameObject.SetActive(false);
        }

        private void OnFinishRace() {
            
            //Destroy(gameObject);
        }

        private void Update()
        {
            if (RaceManager.Instance.RaceHadBegun())
            {
                currentTime.text = Utils.DisplayHelper.FloatToTimeString(Time.time - _info.currentLapStartTime);
            }
            else {
                currentTime.text = Utils.DisplayHelper.FloatToTimeString(0f);
            }

            speed.text = (int)(_info.kart.rigidBody.velocity.magnitude) + " km/h";
        }

    }
}