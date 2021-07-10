using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kart;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerConfigurationManager : MonoBehaviour
{
    
    [HideInInspector] public bool multiplayer;
    
    [SerializeField] private int MaxPlayer = 1;
    [SerializeField] private PlayerDisplay playerDisplay;
    [SerializeField] private ControlTypeDisplay ControlTypeDisplayPrefab;
    

    private PlayerInputManager _inputManager;
    private List<PlayerConfiguration> playerConfigs;
    private List<ControlTypeDisplay> ControlTypeDisplays;

    private int currentPlayerIndex;

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
        playerDisplay.ShowJoinMessage();
        ControlTypeDisplays = new List<ControlTypeDisplay>();

    }

    public void EnableJoining()
    {
        _inputManager.EnableJoining();
        playerDisplay.ShowAddPlayerMessage();
    }
    public void DisableJoining()
    {
        _inputManager.DisableJoining();
        playerDisplay.HideMessage();
    }
    public void SetPlayerKart(KartBase kart)
    {
        Debug.Log("SetPlayer " + currentPlayerIndex);
        playerConfigs[currentPlayerIndex].Name = "Player " + (currentPlayerIndex+1);
        playerConfigs[currentPlayerIndex].Kart = kart;
        ControlTypeDisplays[currentPlayerIndex].PlayerIsReady();
        ReadyPlayer(currentPlayerIndex);
    }
    public void ReadyPlayer(int index)
    {
        Debug.Log("ReadyPlayer " + currentPlayerIndex);
        playerConfigs[index].IsReady = true;
        if (playerConfigs.All(p => p.IsReady == true))
        {
            Debug.Log("All players ready !");
            playerDisplay.HideMarker();
            MenuManager.Instance.KartConfigsReady();
        }
        else
        {
            SelectNextPlayer();
        }
    }

    public void SelectNextPlayer()
    {
        currentPlayerIndex++;
        playerDisplay.SelectPlayer(currentPlayerIndex);
    }
    public void BeginSetup()
    {
        playerDisplay.ShowSelectionMarker(currentPlayerIndex);
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
            foreach (var device in pi.devices)
            {
                Debug.Log("Player " + pi.playerIndex + " joined with device : " + device.description.deviceClass + " interface name "  + device.description.interfaceName);
                if (device.description.deviceClass.Contains("Mouse"))
                {
                    Destroy(pi.gameObject);
                    return;
                    
                }
                ControlTypeDisplay inputTypeDisplay =
                    Instantiate(ControlTypeDisplayPrefab, playerDisplay.displayParent.transform);
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
                ControlTypeDisplays.Add(inputTypeDisplay);
            }
            
            playerConfigs.Add(new PlayerConfiguration(pi));
            int playerMaxCount = multiplayer ? MaxPlayer : 1;
            if (playerConfigs.Count >= playerMaxCount)
            {
                playerDisplay.HideMessage();
            }
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
    public string Name { get; set; }
    public bool IsReady { get; set; }
    public KartBase Kart { get; set; }
    public Color Color { get; set; }
}