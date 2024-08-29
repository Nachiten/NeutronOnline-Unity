using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class PlayerReady : SingletonNetwork<PlayerReady>
{
    public static event Action OnEveryPlayerReady;
    public event Action OnReadyChanged;

    private Dictionary<ulong, bool> playersReady;
    
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
        playersReady[clientId] = ready;

        if (!AllClientsReady()) 
            return;
        
        OnEveryPlayerReady?.Invoke();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetPlayerReadyClientRpc(bool ready, ulong clientId)
    {
        playersReady[clientId] = ready;

        OnReadyChanged?.Invoke();
    }
    
    private bool AllClientsReady()
    {
        return networkManager.ConnectedClientsIds
            .All(clientId => playersReady.ContainsKey(clientId) && playersReady[clientId]);
    }
    
    public static void ResetStaticData()
    {
        OnEveryPlayerReady = null;
    }
}