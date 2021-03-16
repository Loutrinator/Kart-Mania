using System;
using System.Collections;
using System.Collections.Generic;
using Kart;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class KartSelectionManager : MonoBehaviour
{
    public MenuManager menuManager;
    [Header("UI")]
    public TextMeshProUGUI kartNameDisplay;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI acceleration;
    public TextMeshProUGUI steering;
    public TextMeshProUGUI brake;
    public TextMeshProUGUI okayletzgo;
    [Header("Other")]
    public List<GameObject> kartPreview;
    public List<KartBase> kartPrefabs;
    public Diaphragm diaphragm;
    public Transform carHolder;
    public float spawnHeight = 10f;
    private GameObject selectedKart;
    private KartPreview currentKartInformations;
    
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
        selectedKart = Instantiate(kartPreview[i],diaphragm.gameObject.transform.position + Vector3.up*spawnHeight,Quaternion.Euler(-1.5f,0,-3.2f),carHolder);
        currentKartInformations = selectedKart.GetComponent<KartPreview>();
        diaphragm.currentKart = selectedKart.transform;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (currentKartInformations != null)
        {
            kartNameDisplay.text = currentKartInformations.name;
            speed.text = ((int)currentKartInformations.stats.topSpeed).ToString();
            acceleration.text = ((int)currentKartInformations.stats.acceleration).ToString();
            brake.text = ((int)currentKartInformations.stats.braking).ToString();
            steering.text = ((int)currentKartInformations.stats.steer).ToString();
            okayletzgo.text = currentKartInformations.stats.suspension.ToString();
            
        }
    }

    public void SelectNext()
    {
        currentKartId = (currentKartId + 1 + kartPreview.Count) % kartPreview.Count;
        diaphragm.Open();
    }
    public void SelectPrevious()
    {
        currentKartId = (currentKartId - 1 + kartPreview.Count) % kartPreview.Count;
        diaphragm.Open();
    }

    public void Choosekart()
    {
        GameManager.Instance.kartPrefab = kartPrefabs[currentKartId];
        menuManager.ShowNextScreen();
    }
}
