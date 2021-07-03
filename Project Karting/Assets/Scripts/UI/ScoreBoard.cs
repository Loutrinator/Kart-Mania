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

        //clean
        foreach (Transform child in currentLapTimesContainer.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        bestLapTime.text = Utils.DisplayHelper.floatToTimeString(_info.bestLapTime);
        
        // TODO : faire la même chose avec les autres lap time ( il faut créer un tableau de lap time dans PlayerRaceInfo )
    }

}