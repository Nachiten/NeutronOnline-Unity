﻿using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

/// <summary>
/// Manages the player network actions.
/// </summary>
public class PlayerActions : SingletonNetwork<PlayerActions>
{
    public event Action OnTryingToJoinGame;
    public event Action OnFailedToJoinGame;

    // Dependencies
    private NetworkManager networkManager;
    private PlayerDataHandler playerDataHandler;
    private PlayerAttributes playerAttributes;
    private IAuthenticationService authenticationService;
    
    protected override void Awake()
    {
        base.Awake();
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
        playerDataHandler = PlayerDataHandler.Instance;
        playerAttributes = PlayerAttributes.Instance;
        authenticationService = AuthenticationService.Instance;
    }

    public void StartClient()
    {
        OnTryingToJoinGame?.Invoke();
        
        networkManager.OnClientConnectedCallback += OnClientConnectedCallback;
        networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
        
        networkManager.StartClient();
    }

    public void StartHost()
    {
        networkManager.OnClientConnectedCallback += OnClientConnectedCallback;
        networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
        
        networkManager.StartHost();
    }

    public void Disconnect()
    {
        networkManager.Shutdown();
    }
    
    public override void OnDestroy()
    {
        if (networkManager == null)
            return;
        
        networkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    }
    
    private void OnClientConnectedCallback(ulong clientId)
    {
        //Debug.Log("OnClientConnectedCallback: " + clientId + " | IsServer: " + networkManager.IsServer, this);

        // Server receiving a new client
        if (networkManager.IsServer)
        {
            Debug.Log($"Server received a new client: ID: {clientId} | Name: {playerAttributes.GetLocalPlayerName()}", this);
            
            playerDataHandler.AddPlayerData(clientId, "", 0, "");
            
            // Host is connecting to server
            if (clientId == 0)
            {
                SetPlayerIdServerRpc(authenticationService.PlayerId);
                
                SetPlayerNameServerRpc(playerAttributes.GetLocalPlayerName());
                SetPlayerColorServerRpc(playerAttributes.GetLocalPlayerColorId());
            }
        }
        // Client connecting to server
        else
        {
            Debug.Log($"Client connected to server: ID: {clientId} | Name: {playerAttributes.GetLocalPlayerName()} | Auth ID: {authenticationService.PlayerId}", this);
            
            playerAttributes.SetLocalClientId(clientId);
            SetPlayerIdServerRpc(authenticationService.PlayerId);
            
            // Set local player name and color for everyone
            SetPlayerNameServerRpc(playerAttributes.GetLocalPlayerName());
            SetPlayerColorServerRpc(playerAttributes.GetLocalPlayerColorId());
        }
    }
    
    private void OnClientDisconnectCallback(ulong clientId)
    {
        //Debug.Log("OnClientDisconnectCallback: " + clientId + " | IsServer: " + networkManager.IsServer, this);

        // Server removing a client
        if (networkManager.IsServer)
        {
            Debug.Log($"Server removed a client: ID: {clientId} | Name: {playerDataHandler.GetPlayerDataFromClientId(clientId).name}", this);
            
            playerDataHandler.RemovePlayerData(clientId);
        }
        // Client disconnecting from server
        else
        {
            Debug.Log($"Client disconnected from server: ID: {clientId} | Name: {playerDataHandler.GetPlayerDataFromClientId(clientId).name}", this);
            
            // ??? TODO - Review
            OnFailedToJoinGame?.Invoke();
        }
    }

    [Rpc(SendTo.Server)]
    private void SetPlayerNameServerRpc(string playerName, RpcParams rpcParams = default)
    {
        int playerDataIndex = playerDataHandler.GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        PlayerData _playerData = playerDataHandler.GetPlayerDataFromClientId(rpcParams.Receive.SenderClientId);
        
        playerName = playerAttributes.UpdateNameIfNotAvailable(playerName);
        _playerData.name = (FixedString128Bytes) playerName;
        
        playerDataHandler.UpdatePlayerData(playerDataIndex, _playerData);
    }

    [Rpc(SendTo.Server)]
    private void SetPlayerColorServerRpc(ushort colorId, RpcParams rpcParams = default)
    {
        int playerDataIndex = playerDataHandler.GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        PlayerData _playerData = playerDataHandler.GetPlayerDataFromClientId(rpcParams.Receive.SenderClientId);
        
        colorId = playerAttributes.UpdateColorIfNotAvailable(colorId);
        _playerData.colorId = colorId;
        
        playerDataHandler.UpdatePlayerData(playerDataIndex, _playerData);
    }
    
    [Rpc(SendTo.Server)]
    private void SetPlayerIdServerRpc(string playerId, RpcParams rpcParams = default)
    {
        int playerDataIndex = playerDataHandler.GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        PlayerData _playerData = playerDataHandler.GetPlayerDataFromClientId(rpcParams.Receive.SenderClientId);
        
        _playerData.playerId = playerId;
        
        playerDataHandler.UpdatePlayerData(playerDataIndex, _playerData);
    }
}