using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDTimeTrialController : MonoBehaviour
{
    [Header("Front")] [SerializeField] private TextMeshProUGUI bestTime = null;
    [SerializeField] private TextMeshProUGUI currentTime = null;
    [SerializeField] private TextMeshProUGUI timeDiff = null;
    [SerializeField] private TextMeshProUGUI lap = null;
    [SerializeField] private Image iconPlaceholder = null;

    private PlayerRaceInfo _info;
    private int _id = 0;
    private static int _nbInstances = 0;

    private void Start()
    {
        _id = _nbInstances++;
        _info = GameManager.Instance.getPlayerRaceInfo(_id);
        bestTime.text = "";
        currentTime.text = "00:00:00";
        timeDiff.text = "";
        lap.text = "0/" + GameManager.Instance.nbLap;
        _info.onBestLapTimeChange += () => bestTime.text = Utils.DisplayHelper.floatToTimeString(_info.bestLapTime);
        _info.onNewLap += () =>
        {
            // Update time diff each new lap
            lap.text = _info.lap + " / " + GameManager.Instance.nbLap;
            if (_info.lap < 3) return;
            float diff = _info.previousLapTime - _info.bestLapTime;
            timeDiff.text = Utils.DisplayHelper.floatToTimeString(diff);
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
    }

    private void Update()
    {
        if (GameManager.Instance.raceHasBegan())
        {
            currentTime.text = Utils.DisplayHelper.floatToTimeString(Time.time - _info.currentLapStartTime);
        }
    }
}