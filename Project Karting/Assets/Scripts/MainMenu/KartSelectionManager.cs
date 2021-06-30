using System.Collections.Generic;
using System.Linq;
using Game;
using Handlers;
using TMPro;
using UnityEngine;
using Kart;
using MainMenu;

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

    private int getKartId(int direction)
    {
        int position = (currentKartId + direction + 100*availableKarts.Count) % availableKarts.Count;
        Debug.Log("Position : " + position + " currentKartId : " + currentKartId + "availableKarts.Count : " + availableKarts.Count);
        return position;
    }

    public void SelectNext()
    {
        currentKartId = getKartId(1);
        diaphragm.Open();
    }
    public void SelectPrevious()
    {
        currentKartId = getKartId(-1);
        diaphragm.Open();
    }

    public void ChooseKart()
    {
        PlayerConfig playerConfig = new PlayerConfig();
        playerConfig.type = PlayerType.Player;
        playerConfig.name = "Player 1";
        playerConfig.kartPrefab = selectedKart.kartPrefab;
        LevelManager.instance.gameConfig.players.Add(playerConfig);
        menuManager.ShowNextScreen();
    }
}
