using Handlers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class HUDTimeTrialController : MonoBehaviour
    {
        [Header("Front")] [SerializeField] private TextMeshProUGUI bestTime;
        [SerializeField] private TextMeshProUGUI currentTime;
        [SerializeField] private TextMeshProUGUI timeDiff;
        [SerializeField] private TextMeshProUGUI lap;
        [SerializeField] private Image iconPlaceholder;

        private PlayerRaceInfo _info;
        private int _id;
        public static int _nbInstances;

        private void Start()
        {
            _id = _nbInstances++;
            _info = GameManager.Instance.GetPlayerRaceInfo(_id);
            bestTime.text = "";
            currentTime.text = "00:00:00";
            timeDiff.text = "";
            lap.text = "0/" + GameManager.Instance.currentRace.laps;
            _info.onBestLapTimeChange += () => bestTime.text = Utils.DisplayHelper.FloatToTimeString(_info.bestLapTime);
            _info.onNewLap += () =>
            {
                // Update time diff each new lap
                lap.text = _info.lap + " / " + GameManager.Instance.currentRace.laps;
                if (_info.lap < 3) return;
                float diff = _info.previousLapTime - _info.bestLapTime;
                timeDiff.text = Utils.DisplayHelper.FloatToTimeString(diff);
                timeDiff.color = diff > 0 ? Color.red : Color.green;
            };
            _info.onItemSet += () =>
            {
                if (_info.Item)
                {
                    iconPlaceholder.sprite = _info.Item.Icon;
                    var tmp = iconPlaceholder.color;
                    tmp.a = 1;
                    iconPlaceholder.color = tmp;
                }
                else
                {
                    var tmp = iconPlaceholder.color;
                    tmp.a = 0;
                    iconPlaceholder.color = tmp;
                    iconPlaceholder.sprite = null;
                }
            };

            _info.onFinishRace -= OnFinishRace;
            _info.onFinishRace += OnFinishRace;
        }

        private void OnFinishRace() {
            Destroy(gameObject);//.SetActive(false));
        }

        private void Update()
        {
            if (GameManager.Instance.RaceHadBegun())
            {
                currentTime.text = Utils.DisplayHelper.FloatToTimeString(Time.time - _info.currentLapStartTime);
            }
            else {
                currentTime.text = Utils.DisplayHelper.FloatToTimeString(0f);
            }
        }

    }
}