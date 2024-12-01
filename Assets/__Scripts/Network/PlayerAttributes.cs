using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Manages the player attributes.
/// </summary>
public class PlayerAttributes : Singleton<PlayerAttributes>
{
    private const string PLAYER_PREFS_PLAYER_NAME = "playerName";
    private string defaultPlayerName;
    
    [SerializeField] private List<Color> playerColors;
    
    private string localPlayerName;
    private ushort localPlayerColorId;
    private ulong localClientId;
    
    // Dependencies
    private PlayerDataHandler playerDataHandler;
    private NetworkManager networkManager;

    protected override void Awake()
    {
        base.Awake();
        
        DontDestroyOnLoad(gameObject);

        defaultPlayerName = "PlayerName" + Random.Range(1000, 10000);
        
        localPlayerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME, defaultPlayerName);
        localPlayerColorId = 0;
    }

    private void Start()
    {
        playerDataHandler = PlayerDataHandler.Instance;
        networkManager = NetworkManager.Singleton;
        
        playerDataHandler.OnPlayerDataChanged += OnPlayerDataChanged;
    }

    private void OnPlayerDataChanged()
    {
        PlayerData localPlayerData = playerDataHandler.GetLocalPlayerData();
        
        if (localPlayerData.name != (FixedString128Bytes) "")
            localPlayerName = localPlayerData.name.ToString();
        
        localPlayerColorId = localPlayerData.colorId;
    }

    public void SetLocalClientId(ulong _localClientId)
    {
        localClientId = _localClientId;
    }
    
    private ushort GetNextAvailableColorId(ushort colorId)
    {
        ushort newPlayerColorId = colorId;

        while (true)
        {
            newPlayerColorId++;

            if (newPlayerColorId >= playerColors.Count)
                newPlayerColorId = 0;

            if (!IsColorIdUsed(newPlayerColorId))
                return newPlayerColorId;
        }
    }
    
    private string GenerateAvailablePlayerName(string playerName)
    {
        int counter = 2;
        
        while (true)
        {
            string newPlayerName = playerName + counter;
    
            if (!IsNameUsed(newPlayerName))
                return newPlayerName;

            counter++;
        }
    }

    private bool IsColorIdUsed(ushort colorId, bool includeLocalClient = false)
    {
        return playerDataHandler.GetAllPlayerData()
            .Any(_playerData => _playerData.colorId == colorId &&
                                (_playerData.clientId != networkManager.LocalClientId || includeLocalClient));
    }
    
    private bool IsNameUsed(string playerName)
    {
        return playerDataHandler.GetAllPlayerData()
            .Any(_playerData => _playerData.name == (FixedString128Bytes) playerName && _playerData.clientId != networkManager.LocalClientId);
    }
    
    public string UpdateNameIfNotAvailable(string playerName)
    {
        return !IsNameUsed(playerName) ? playerName : GenerateAvailablePlayerName(playerName);
    }
    
    public ushort UpdateColorIfNotAvailable(ushort colorId)
    {
        return !IsColorIdUsed(colorId) ? colorId : GetNextAvailableColorId(colorId);
    }
    
    public string GetLocalPlayerName()
    {
        if (localPlayerName == "")
            SetLocalPlayerName(defaultPlayerName);
        
        return localPlayerName;
    }
    
    public ushort GetLocalPlayerColorId()
    {
        return localPlayerColorId;
    }
    
    public Color GetColorFromColorId(ushort colorId)
    {
        return playerColors[colorId];
    }

    public void SetLocalPlayerName(string newLocalPlayerName)
    {
        localPlayerName = newLocalPlayerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME, localPlayerName);
    }
}