using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Manages the player data.
/// </summary>
public class PlayerDataHandler : SingletonNetwork<PlayerDataHandler>
{
    public event Action OnPlayerDataChanged;
    
    private NetworkList<PlayerData> playerData;
    
    // Dependencies
    private NetworkManager networkManager;

    protected override void Awake()
    {
        base.Awake();
        
        DontDestroyOnLoad(gameObject);
        
        playerData = new NetworkList<PlayerData>();
        playerData.OnListChanged += PlayerData_OnValueChange;
    }

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
    }

    public override void OnDestroy()
    {
        playerData.OnListChanged -= PlayerData_OnValueChange;
    }

    private void PlayerData_OnValueChange(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataChanged?.Invoke();
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData _playerData in playerData)
        {
            if (_playerData.clientId != clientId)
                continue;

            return _playerData;
        }

        return default;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < playerData.Count; i++)
            if (playerData[i].clientId == clientId)
                return i;

        Debug.LogError("PlayerData not found for clientId: " + clientId);
        return -1;
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return playerData[playerIndex];
    }
    
    public PlayerData GetLocalPlayerData()
    {
        return GetPlayerDataFromClientId(networkManager.LocalClientId);
    }
    
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < playerData.Count;
    }
    
    public void UpdatePlayerData(int playerDataIndex, PlayerData newPlayerData)
    {
        playerData[playerDataIndex] = newPlayerData;
    }

    public List<PlayerData> GetAllPlayerData()
    {
        List<PlayerData> playerDataList = new();
        
        foreach (PlayerData _playerData in playerData)
            playerDataList.Add(_playerData);
        
        return playerDataList;
    }

    public void AddPlayerData(ulong clientId, string playerName, ushort playerColorId, string playerId)
    {
        playerData.Add(new PlayerData(clientId, playerName, playerColorId, playerId));
    }
    
    public void RemovePlayerData(ulong clientId)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(clientId);
        playerData.RemoveAt(playerDataIndex);
    }
    
    private void PrintPlayerData()
    {
        Debug.Log("----- PlayerData -----");

        foreach (PlayerData _playerData in playerData)
            Debug.Log(_playerData.ToString(), this);

        Debug.Log("----------------------");
    }
}
