using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Kart;

public class KartSelectionManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI kartNameDisplay;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI acceleration;
    public TextMeshProUGUI steering;
    public TextMeshProUGUI brake;
    public TextMeshProUGUI okayletzgo;
    [Header("Other")]
    public MenuManager menuManager;
    public List<KartPreview> availableKarts;
    public Diaphragm diaphragm;
    public Transform carHolder;
    public float spawnHeight = 10f;
    private KartPreview selectedKart;
    
    private int currentKartId;

    private void Start()
    {
        SpawnKart(0);
    }

    public void FixedUpdate()
    {
        if (selectedKart == null)
        {
            SpawnKart(currentKartId);
        }
    }

    private void SpawnKart(int i)
    {
        selectedKart = Instantiate(availableKarts[i],diaphragm.gameObject.transform.position + Vector3.up*spawnHeight,Quaternion.Euler(-1.5f,0,-3.2f),carHolder);
        diaphragm.currentKart = selectedKart.transform;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (selectedKart != null)
        {
            Stats stats = selectedKart.kartPrefab.vehicleStats;
            kartNameDisplay.text = selectedKart.kartName;
            speed.text = ((int)stats.topSpeed).ToString();
            acceleration.text = ((int)stats.acceleration).ToString();
            brake.text = ((int)stats.braking).ToString();
            steering.text = ((int)stats.steer).ToString();
            okayletzgo.text = stats .suspension.ToString();
            
        }
    }

    public void SelectNext()
    {
        currentKartId = (currentKartId + 1 + availableKarts.Count) % availableKarts.Count;
        diaphragm.Open();
    }
    public void SelectPrevious()
    {
        currentKartId = (currentKartId - 1 + availableKarts.Count) % availableKarts.Count;
        diaphragm.Open();
    }

    public void ChooseKart()
    {
        PlayerConfig playerConfig = new PlayerConfig();
        playerConfig.type = PlayerType.player;
        playerConfig.name = "Player 1";
        playerConfig.kartPrefab = selectedKart.kartPrefab;
        GameManager.Instance.gameConfig.players.Add(playerConfig);
        menuManager.ShowNextScreen();
    }
}
