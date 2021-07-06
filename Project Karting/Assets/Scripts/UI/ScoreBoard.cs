using Handlers;
using TMPro;
using UnityEngine;

namespace UI {
    public class ScoreBoard : MonoBehaviour
    {
        [Header("Front")]
        [SerializeField] private GameObject currentLapTimesContainer;
        [SerializeField] private GameObject bestLapTimesContainer;
        [SerializeField] private TextMeshProUGUI totalTimeText;
        [SerializeField] private TextMeshProUGUI bestTotalTimeText;

        private PlayerRaceInfo _info;
        private int _id;

        public void SetId(int playerId)
        {
            _id = playerId;
            _info = GameManager.Instance.GetPlayerRaceInfo(_id);

        }
        private void Start()
        {
            // because it use the example in prefab
            TextMeshProUGUI bestLapTime = Instantiate(currentLapTimesContainer.transform.GetComponentInChildren<TextMeshProUGUI>(), currentLapTimesContainer.transform);
            bestLapTime.text = Utils.DisplayHelper.FloatToTimeString(_info.bestLapTime);

            //clean
            foreach (Transform child in currentLapTimesContainer.transform)
            {
                Destroy(child.gameObject);
            }
            Debug.Log("cb de temps dans la liste de _info ? : "+ _info.lapsTime.Count);

            float totalTime = 0;
            for (var index = 0; index < _info.lapsTime.Count; index++) {
                float time = _info.lapsTime[index];
                TextMeshProUGUI timeText = Instantiate(bestLapTime, currentLapTimesContainer.transform);
                timeText.enabled = true;
                timeText.text = GetLapName(index + 1) + Utils.DisplayHelper.FloatToTimeString(time);
                totalTime += time;
            }

            totalTimeText.text = "Total Time  " + Utils.DisplayHelper.FloatToTimeString(totalTime);

            bestLapTime.fontMaterial.color = Color.green;
        }

        private string GetLapName(int lap) {
            string result = lap.ToString();
            result += lap switch {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            };
            result += " - ";
            return result;
        }
    }
}