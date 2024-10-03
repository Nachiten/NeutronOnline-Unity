using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LobbyHandler : Singleton<LobbyHandler>
{
    private const int MAX_PLAYER_AMOUNT = 4;

    private const string RELAY_JOIN_CODE_KEY = "RelayJoinCode";
    private const string RELAY_CONNECTION_TYPE = "dtls";
    
    public event Action OnCreateLobbyStarted;
    public event Action OnCreateLobbyFailed;
    public event Action OnJoinStarted;
    public event Action OnJoinWithCodeFailed;
    public event Action OnQuickJoinFailed;
    public event Action<List<Lobby>> OnLobbyListChanged;

    private Lobby joinedLobby;
    
    private const float heartbeatTimerMax = 15f;
    private float heartbeatTimer;
    private const int lobbiesRefreshTimerMax = 3;
    private float lobbiesRefreshTimer;
    
    // Dependencies
    private IAuthenticationService authenticationService;
    private ILobbyService lobbyService;
    private IRelayService relayService;
    private NetworkManager networkManager;
    private PlayerActions playerActions;
    private PlayerReady playerReady;
    
    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);
        
        InitializeUnityAuthentication();
    }

    private void Start()
    { 
        networkManager = NetworkManager.Singleton;
        lobbyService = LobbyService.Instance;
        relayService = RelayService.Instance;
        playerActions = PlayerActions.Instance;
        playerReady = PlayerReady.Instance;
        
        //playerReady.OnEveryPlayerReady += StartGame;
        GameStartTimerUI.OnGameStartTimerFinished += StartGame;
    }

    private void Update()
    {
        HandleHeartbeat();
        HandlePeriodicLobbyListRefresh();
    }

    private void HandleHeartbeat()
    {
        if (!IsLobbyHost()) 
            return;
        
        heartbeatTimer -= Time.deltaTime;

        if (heartbeatTimer > 0) 
            return;
        
        heartbeatTimer = heartbeatTimerMax;
                
        lobbyService.SendHeartbeatPingAsync(joinedLobby.Id);
    }

    private void HandlePeriodicLobbyListRefresh()
    {
        // Only refresh the lobby list if we're not in a lobby and in the lobby scene
        if (joinedLobby != null || 
            !authenticationService.IsSignedIn || 
            SceneManager.GetActiveScene().name != SceneName._1_LobbySelect.ToString()) 
            return;
        
        lobbiesRefreshTimer -= Time.deltaTime;
        
        if (lobbiesRefreshTimer > 0) 
            return;
        
        lobbiesRefreshTimer = lobbiesRefreshTimerMax;
        
        ListLobbies();
    }

    private async void InitializeUnityAuthentication()
    {
        try
        {
            // Cannot initialize Unity Services more than once
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                authenticationService = AuthenticationService.Instance;
                return;
            }
        
            InitializationOptions initializationOptions = new();
        
            if (Debug.isDebugBuild)
                initializationOptions.SetProfile(Random.Range(0, 10000).ToString());

            await UnityServices.InitializeAsync(initializationOptions);
        
            authenticationService = AuthenticationService.Instance;
            await authenticationService.SignInAnonymouslyAsync();
        } 
        catch (Exception e)
        {
            Debug.LogError("[CRITICAL] Failed to initialize Unity Services: " + e);
        }
    }

    private bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == authenticationService.PlayerId;
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new()
            {
                Filters = new List<QueryFilter>
                {
                    new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                }
            };
        
            QueryResponse queryResponse = await lobbyService.QueryLobbiesAsync(queryLobbiesOptions);
            OnLobbyListChanged?.Invoke(queryResponse.Results);
        }
        catch (Exception e) 
        {
            Debug.LogError("[LobbyHandler] Error Listing Lobbies" + e);
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            return await relayService.CreateAllocationAsync(MAX_PLAYER_AMOUNT - 1);
        } 
        catch (Exception e)
        {
            Debug.LogError("[LobbyHandler] Error Allocating Relay" + e);
            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        { 
            return await relayService.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (Exception e)
        {
            Debug.LogError("[LobbyHandler] Error Getting Relay Join Code" + e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        { 
           return await relayService.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.LogError("[LobbyHandler] Error Joining Relay" + e);
            return default;
        }
    }

    public async void CreateLobby(string lobbyName, bool isPrivate)
    {
        OnCreateLobbyStarted?.Invoke();

        try
        {
            joinedLobby = await lobbyService.CreateLobbyAsync(lobbyName,
                MAX_PLAYER_AMOUNT,
                new CreateLobbyOptions
                {
                    IsPrivate = isPrivate
                });

            Allocation relayAllocation = await AllocateRelay();
            string relayJoinCode = await GetRelayJoinCode(relayAllocation);

            await lobbyService.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions {
                Data = new Dictionary<string, DataObject> {
                    {
                        RELAY_JOIN_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) 
                    }
                }
            });
            
            RelayServerData relayServerData = GenerateRelayServerData(relayAllocation);
            
            networkManager.GetComponent<UnityTransport>()
                .SetRelayServerData(relayServerData);
            playerActions.StartHost();
            
            SceneLoader.LoadNetworkScene(SceneName._2_JoinedLobby);
        }
        catch (Exception e) 
        {
            Debug.LogError("[LobbyHandler] Error creating lobby: " + e);
            DeleteLobby();
            OnCreateLobbyFailed?.Invoke();
        }
    }

    public async void QuickJoin()
    {
        OnJoinStarted?.Invoke();

        try
        {
            joinedLobby = await lobbyService.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value;
            JoinAllocation joinAllocation = await relayService.JoinAllocationAsync(relayJoinCode);

            RelayServerData relayServerData = GenerateRelayServerData(joinAllocation);
            
            networkManager.GetComponent<UnityTransport>()
                .SetRelayServerData(relayServerData);
            playerActions.StartClient();
        }
        catch (Exception e)
        { 
            Debug.Log("[LobbyHandler] Quick Join Failed: " + e);
            LeaveLobby();
            OnQuickJoinFailed?.Invoke();
        }
    }
    
    private RelayServerData GenerateRelayServerData(JoinAllocation joinAllocation)
    {
        return new RelayServerData(joinAllocation, RELAY_CONNECTION_TYPE);
    }

    private RelayServerData GenerateRelayServerData(Allocation joinAllocation)
    {
        return new RelayServerData(joinAllocation, RELAY_CONNECTION_TYPE);
    }
    
    public async void JoinWithCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke();
        
        try 
        {
            joinedLobby = await lobbyService.JoinLobbyByCodeAsync(lobbyCode);
            
            string relayJoinCode = joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
                
            RelayServerData relayServerData = GenerateRelayServerData(joinAllocation);
            
            networkManager.GetComponent<UnityTransport>()
                .SetRelayServerData(relayServerData);
            
            playerActions.StartClient();
        }
        catch (Exception e) 
        {
            Debug.Log("[LobbyHandler] Join With Code Failed: " + e);
            OnJoinWithCodeFailed?.Invoke();
        }
    }
    
    public async void JoinWithId(string lobbyId)
    {
        OnJoinStarted?.Invoke();
        
        try 
        {
            joinedLobby = await lobbyService.JoinLobbyByIdAsync(lobbyId);
            
            string relayJoinCode = joinedLobby.Data[RELAY_JOIN_CODE_KEY].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
                
            RelayServerData relayServerData = GenerateRelayServerData(joinAllocation);

            networkManager.GetComponent<UnityTransport>()
                .SetRelayServerData(relayServerData);
            playerActions.StartClient();
        }
        catch (Exception e) 
        {
            Debug.Log("[LobbyHandler] Join With Id Failed: " + e);
            OnJoinWithCodeFailed?.Invoke();
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby == null)
            return;
        
        try
        {
            await lobbyService.DeleteLobbyAsync(joinedLobby.Id);
        
            joinedLobby = null;
        } 
        catch (Exception e)
        {
            Debug.LogError("[LobbyHandler] Error Deleting Lobby: " + e);
        }
    }

    public async void LeaveLobby()
    {
        if (joinedLobby == null)
            return;

        try
        {
            await lobbyService.RemovePlayerAsync(joinedLobby.Id, authenticationService.PlayerId);
        
            joinedLobby = null;
        } 
        catch (LobbyServiceException e)
        {
            Debug.LogError("[LobbyHandler] Error Leaving Lobby: " + e);
        }
    }
    
    public async void KickPlayer(string playerId)
    {
        // Must be lobby host to kick players
        if (!IsLobbyHost())
            return;
        
        try
        {
            await lobbyService.RemovePlayerAsync(joinedLobby.Id, playerId);
        } 
        catch (LobbyServiceException e)
        {
            Debug.LogError("[LobbyHandler] Error Kicking Player: " + e);
        }
    }
    
    public Lobby GetLobby()
    {
        return joinedLobby;
    }
    
    private void StartGame() 
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("STARTING GAME!!!");
            SceneLoader.LoadNetworkScene(SceneName._4_Game);
        }
    }
}