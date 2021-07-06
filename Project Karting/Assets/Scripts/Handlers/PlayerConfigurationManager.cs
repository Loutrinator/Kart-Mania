using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerConfigurationManager : MonoBehaviour
{
    
    private List<PlayerConfiguration> playerConfigs;

    [SerializeField] private int MaxPlayer = 1;
    [SerializeField] private ControlTypeDisplay ControlTypeDisplayPrefab;
    public GameObject ControlTypeDisplayParent;
    public bool multiplayer;

    private PlayerInputManager _inputManager;

    public static PlayerConfigurationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Trying to create another singleton");
        }
        else
        {
            Instance = this;
        }
        
        DontDestroyOnLoad(Instance);
    }

    private void Start()
    {
        playerConfigs = new List<PlayerConfiguration>();
        _inputManager = gameObject.GetComponent<PlayerInputManager>();
        
    }

    public void EnableJoining()
    {
        _inputManager.EnableJoining();
    }
    public void DisableJoining()
    {
        _inputManager.DisableJoining();
    }
    public void SetPlayerKart(int index, int kartId)
    {
        playerConfigs[index].KartId = kartId;
    }
    public void ReadyPlayer(int index)
    {
        playerConfigs[index].IsReady = true;
        if (playerConfigs.Count == MaxPlayer && playerConfigs.All(p => p.IsReady == true))
        {
            //Todo : CHARGER LA SUITE
        }
    }

    public void HandlePlayerJoin(PlayerInput pi)
    {
        if (playerConfigs.Count >= 1 && !multiplayer)
        {
            Destroy(pi.gameObject);
            return;
        }
        
        if (!playerConfigs.Any(p => p.PlayerIndex == pi.playerIndex))
        {
            pi.transform.SetParent(transform);
            ControlTypeDisplay inputTypeDisplay =
                Instantiate(ControlTypeDisplayPrefab, ControlTypeDisplayParent.transform);
            if (inputTypeDisplay != null)
            {
                foreach (var device in pi.devices)
                {
                    Debug.Log("Player " + pi.playerIndex + " joined with device : " + device.description.deviceClass);
                    if (device.description.deviceClass.Contains("PS"))
                    {
                        Debug.Log("Input of type playstation");
                        inputTypeDisplay.SetupUI(pi.playerIndex, ControlType.PS);
                    }else if (device.description.deviceClass.Contains("XBox"))
                    {
                        Debug.Log("Input of type XBox");
                        inputTypeDisplay.SetupUI(pi.playerIndex, ControlType.XBOX);
                    }else if (device.description.deviceClass.Contains("Keyboard"))
                    {
                        Debug.Log("Input of type keyboard");
                        inputTypeDisplay.SetupUI(pi.playerIndex, ControlType.KEYBOARD);
                    }else
                    {
                        Debug.Log("Input of type other");
                        inputTypeDisplay.SetupUI(pi.playerIndex, ControlType.OTHER);
                    }
                }
            }
            
            playerConfigs.Add(new PlayerConfiguration(pi));
        }
    }
    
}

public class PlayerConfiguration
{
    public PlayerConfiguration(PlayerInput pi)
    {
        PlayerIndex = pi.playerIndex;
        Input = pi;
    }
    
    public PlayerInput Input { get; set; }
    public int PlayerIndex { get; set; }
    public bool IsReady { get; set; }
    public int KartId { get; set; }
    public int colorId { get; set; }
}