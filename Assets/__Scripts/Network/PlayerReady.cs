using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerReady : SingletonNetwork<PlayerReady>
{
    public event Action<bool> OnEveryPlayerReadyChanged;
    public event Action OnReadyChanged;
    public event Action OnSecondPlayerJoined;
    public event Action OnSecondPlayerLeft;

    private Dictionary<ulong, bool> playersReady;
    private bool everyPlayerReady;
    
    // Dependencies
    private NetworkManager networkManager;
    
    protected override void Awake()
    {
        base.Awake();

        playersReady = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
        
        networkManager.OnClientConnectedCallback += OnClientConnectedCallback;
        networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        
        if (networkManager == null)
            return;
        
        networkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    }

    private void OnClientDisconnectCallback(ulong clientId)
    {
        // If the player that disconnected was the local player, we don't need to update the ready state
        if (clientId == networkManager.LocalClientId)
            return;
        
        OnSecondPlayerLeft?.Invoke();
        
        UpdatePlayerReadyClientRpc();
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        if (IsServer)
            OnSecondPlayerJoined?.Invoke();
        
        UpdatePlayerReadyClientRpc();
    }

    public override void OnNetworkSpawn()
    {
        if (!NetworkManager.Singleton.IsServer)
            GetPlayersReadyServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void GetPlayersReadyServerRpc()
    {
        foreach (ulong clientId in networkManager.ConnectedClientsIds)
        {
            if (playersReady.ContainsKey(clientId))
                SetPlayerReadyClientRpc(playersReady[clientId], clientId);
        }
    }

    private void PrintPlayersReady()
    {
        Debug.Log("----- PLAYERS READY -----");
        foreach (KeyValuePair<ulong, bool> playerReady in playersReady)
        {
            Debug.Log(playerReady.Key + " " + playerReady.Value);
        }
        Debug.Log("-------------------------");
    }
    
    public void SetLocalPlayerReady(bool ready)
    {
        SetPlayerReadyServerRpc(ready);
    }
    
    public bool IsPlayerReady(ulong clientId)
    {
        return playersReady.ContainsKey(clientId) && playersReady[clientId];
    }
    
    public bool IsLocalPlayerReady()
    {
        return IsPlayerReady(networkManager.LocalClientId);
    }
    
    [Rpc(SendTo.Server)]
    private void SetPlayerReadyServerRpc(bool ready, RpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        SetPlayerReadyClientRpc(ready, clientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetPlayerReadyClientRpc(bool ready, ulong clientId)
    {
        playersReady[clientId] = ready;

        OnReadyChanged?.Invoke();

        AllClientsReadyLogic();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdatePlayerReadyClientRpc()
    {
        AllClientsReadyLogic();
    }

    private void AllClientsReadyLogic()
    {
        if (AllClientsReady())
        {
            everyPlayerReady = true;
            OnEveryPlayerReadyChanged?.Invoke(everyPlayerReady);
        }
        else if (everyPlayerReady)
        {
            everyPlayerReady = false;
            OnEveryPlayerReadyChanged?.Invoke(everyPlayerReady);
        }
    }
    
    private bool AllClientsReady()
    {
        if (networkManager == null)
            return false;

        // If there is only one player, reset player ready
        if (networkManager.ConnectedClientsIds.Count < 2)
        {
            if (IsLocalPlayerReady())
                SetLocalPlayerReady(false);
            return false;
        }
        
        return networkManager.ConnectedClientsIds
            .All(clientId => playersReady.ContainsKey(clientId) && playersReady[clientId]);
    }
}