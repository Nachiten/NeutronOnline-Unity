using Michsky.MUIP;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private ButtonManager closeButton;

    // Dependencies
    private LobbyHandler lobbyHandler;
    private PlayerActions playerActions;
    private NetworkManager networkManager;
    
    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void Start()
    {
        lobbyHandler = LobbyHandler.Instance;
        playerActions = PlayerActions.Instance;
        networkManager = NetworkManager.Singleton;
        
        lobbyHandler.OnCreateLobbyStarted += OnCreateLobbyStarted;
        lobbyHandler.OnCreateLobbyFailed += OnCreateLobbyFailed;
        lobbyHandler.OnJoinStarted += OnJoinStarted;
        lobbyHandler.OnJoinWithCodeFailed += OnJoinWithCodeFailed;
        lobbyHandler.OnQuickJoinFailed += OnQuickJoinFailed;
        
        playerActions.OnFailedToJoinGame += OnFailedToJoinGame;
        playerActions.OnTryingToJoinGame += OnTryingToJoinGame;

        SetShow(false);
    }
    
    private void OnDestroy()
    {
        if (playerActions)
            playerActions.OnFailedToJoinGame -= OnFailedToJoinGame;

        if (lobbyHandler)
        {
            lobbyHandler.OnCreateLobbyStarted -= OnCreateLobbyStarted;
            lobbyHandler.OnCreateLobbyFailed -= OnCreateLobbyFailed;
            lobbyHandler.OnJoinStarted -= OnJoinStarted;
            lobbyHandler.OnJoinWithCodeFailed -= OnJoinWithCodeFailed;
            lobbyHandler.OnQuickJoinFailed -= OnQuickJoinFailed;
        }
    }

    private void OnTryingToJoinGame()
    {
        SetShow(false);
    }

    private void OnQuickJoinFailed()
    {
        ShowMessage("Could not find any lobby to Quick Join!");
    }

    private void OnJoinWithCodeFailed()
    {
        ShowMessage("Could not find that lobby!");
    }

    private void OnJoinStarted()
    {
        ShowMessage("Joining lobby...");
    }

    private void OnCreateLobbyStarted()
    {
        ShowMessage("Creating lobby...");
    }

    private void OnCreateLobbyFailed()
    {
        ShowMessage("Failed to create lobby!");
    }
    
    private void OnFailedToJoinGame()
    {
        string disconnectReason = networkManager.DisconnectReason;
        ShowMessage(disconnectReason == "" ? "Failed to connect" : disconnectReason);
    }

    private void ShowMessage(string message)
    {
        SetShow(true);
        messageText.text = message;
    }
    
    private void OnCloseButtonClick()
    {
        SetShow(false);
    }

    private void SetShow(bool show)
    {
        gameObject.SetActive(show);
    }
}