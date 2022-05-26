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
    public StatDisplay speed;
    public StatDisplay acceleration;
    public StatDisplay maniability;
    public StatDisplay weight;
    public StatDisplay luck;
    [Header("Other")]
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
            speed.SetValue( stats.topSpeed);
            acceleration.SetValue( stats.acceleration);
            maniability.SetValue( stats.maniability);
            weight.SetValue( stats.weight);
            luck.SetValue( stats.luck);
            
        }
    }

    private int getKartId(int direction)
    {
        int position = (currentKartId + direction + 100*availableKarts.Count) % availableKarts.Count;
        return position;
    }

    public void SelectNext()
    {
        SoundManager.Instance.PlayUIClick();
        currentKartId = getKartId(1);
        diaphragm.Open();
    }
    public void SelectPrevious()
    {
        SoundManager.Instance.PlayUIClick();
        currentKartId = getKartId(-1);
        diaphragm.Open();
    }

    public void ChooseKart()
    {
        KartBase kart = selectedKart.kartPrefab;
        PlayerConfigurationManager.Instance.SetPlayerKart(kart);
    }
}
