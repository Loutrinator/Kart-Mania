using Handlers;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    [Header("Front")] 
    [SerializeField] private GameObject currentLapTimesContainer = null;
    [SerializeField] private GameObject bestLapTimesContainer = null;
    [SerializeField] private TextMeshProUGUI totalTimeText = null;
    [SerializeField] private TextMeshProUGUI bestTotalTimeText = null;

    private PlayerRaceInfo _info;
    private int _id = 0;

    public void setId(int playerId)
    {
        _id = playerId;
        _info = GameManager.Instance.GetPlayerRaceInfo(_id);

    }
    private void Start()
    {

        // because it use the example in prefab
        TextMeshProUGUI bestLapTime = Instantiate(currentLapTimesContainer.transform.GetComponentInChildren<TextMeshProUGUI>(), currentLapTimesContainer.transform);
        bestLapTime.text = Utils.DisplayHelper.floatToTimeString(_info.bestLapTime);

        //clean
        foreach (Transform child in currentLapTimesContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        Debug.Log("cb de temps dans la liste de _info ? : "+ _info.lapsTime.Count);

        foreach (float time in _info.lapsTime)
        {
            TextMeshProUGUI timeText = Instantiate(bestLapTime, currentLapTimesContainer.transform);
            timeText.text = Utils.DisplayHelper.floatToTimeString(time);
            Debug.Log("display time : "+  timeText.text);
        }
        bestLapTime.fontMaterial.color = Color.green;
    }

}