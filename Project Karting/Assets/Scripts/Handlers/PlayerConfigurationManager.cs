using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handlers;
using Kart;
using MainMenu;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

[RequireComponent(typeof(PlayerInputManager))]
public class PlayerConfigurationManager : MonoBehaviour
{
    
    [HideInInspector] public bool multiplayer;
    
    [SerializeField] private int MaxPlayer = 1;
    [SerializeField] private PlayerDisplay playerDisplay;
    [SerializeField] private ControlTypeDisplay ControlTypeDisplayPrefab;
    

    private PlayerInputManager _inputManager;
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
        _inputManager = gameObject.GetComponent<PlayerInputManager>();
        playerDisplay.ShowJoinMessage();
        ControlTypeDisplays = new List<ControlTypeDisplay>();

    }

    public void EnableJoining()
    {
        _inputManager.EnableJoining();
        playerDisplay.gameObject.SetActive(true);
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
        LevelManager.instance.gameConfig.players[currentPlayerIndex].Name = "Player " + (currentPlayerIndex+1);
        LevelManager.instance.gameConfig.players[currentPlayerIndex].KartPrefab = kart;
        ControlTypeDisplays[currentPlayerIndex].PlayerIsReady();
        ReadyPlayer(currentPlayerIndex);
    }
    public void ReadyPlayer(int index)
    {
        Debug.Log("ReadyPlayer " + currentPlayerIndex);
        LevelManager.instance.gameConfig.players[index].IsReady = true;
        if (LevelManager.instance.gameConfig.players.All(p => p.IsReady == true))
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
        if (LevelManager.instance.gameConfig.players.Count >= 1 && !multiplayer)
        {
            Destroy(pi.gameObject);
            return;
        }
        
        if (!LevelManager.instance.gameConfig.players.Any(p => p.PlayerIndex == pi.playerIndex))
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
                
                if (device is XInputController)
                {
                    Debug.Log("Input of type XBox");
                    inputTypeDisplay.SetupUI(pi.playerIndex, ControlType.XBOX);
                }
                else if (device is DualShockGamepad)
                {
                    Debug.Log("Input of type playstation");
                    inputTypeDisplay.SetupUI(pi.playerIndex, ControlType.PS);
                }
                else if (device.description.deviceClass.Contains("Keyboard"))
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

            PlayerConfiguration pg = new PlayerConfiguration(pi);
            pg.Color = UISettings.instance.colors[pi.playerIndex];
            LevelManager.instance.gameConfig.players.Add(pg);
            int playerMaxCount = multiplayer ? MaxPlayer : 1;
            if (LevelManager.instance.gameConfig.players.Count >= playerMaxCount)
            {
                playerDisplay.HideMessage();
            }
        }
    }

    public void HideJoinUI()
    {
        playerDisplay.gameObject.SetActive(false);
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
    public KartBase KartPrefab { get; set; }
    public Color Color { get; set; }
}
