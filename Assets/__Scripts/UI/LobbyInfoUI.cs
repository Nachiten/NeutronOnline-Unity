using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfoUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text lobbyCodeText;
    [SerializeField] private TMP_Text readyButtonText;

    private bool localPlayerReady;
    private bool isHost;
    
    // Dependencies
    private NetworkManager networkManager;
    private LobbyHandler lobbyHandler;
    private PlayerActions playerActions;
    private PlayerReady playerReady;
    
    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        readyButton.onClick.AddListener(OnReadyButtonClicked);
    }

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
        lobbyHandler = LobbyHandler.Instance;
        playerActions = PlayerActions.Instance;
        playerReady = PlayerReady.Instance;
        
        Lobby lobby = LobbyHandler.Instance.GetLobby();

        lobbyNameText.text = "Lobby Name: " + lobby.Name;
        lobbyCodeText.text = "Lobby Code: " + lobby.LobbyCode;

        isHost = networkManager.IsHost;
        
        if (isHost)
            readyButton.enabled = false;
        
        playerReady.OnSecondPlayerJoined += OnSecondPlayerJoined;
        playerReady.OnSecondPlayerLeft += OnSecondPlayerLeft;
    }

    private void OnSecondPlayerJoined()
    {
        readyButton.enabled = true;
    }
    
    private void OnSecondPlayerLeft()
    {
        readyButton.enabled = false;
    }

    private void OnDestroy()
    {
        // If the server is leaving, delete the lobby
        if (isHost)
            lobbyHandler.DeleteLobby();
    }

    private void OnMainMenuButtonClicked()
    {
        if (!isHost)
            lobbyHandler.LeaveLobby();
        
        playerActions.Disconnect();
        
        SceneLoader.LoadScene(SceneName._0_MainMenu);
    }

    private void OnReadyButtonClicked()
    {
        localPlayerReady = !localPlayerReady;
        playerReady.SetLocalPlayerReady(localPlayerReady);
        UpdateReadyButtonText();
    }

    private void UpdateReadyButtonText()
    {
        readyButtonText.text = localPlayerReady ? "NOT READY" : "READY";
    }
}