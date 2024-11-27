using System.Collections.Generic;
using Michsky.MUIP;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private ButtonManager mainMenuButton;
    [SerializeField] private ButtonManager createLobbyButton;
    [SerializeField] private ButtonManager quickJoinButton;
    [SerializeField] private ButtonManager joinWithCodeButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Transform lobbyListContainer;
    [SerializeField] private Transform lobbyItemTemplate;
    
    // Dependencies
    private PlayerAttributes playerAttributes;
    private LobbyHandler lobbyHandler;
    private NetworkManager networkManager;
    
    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuClick);
        createLobbyButton.onClick.AddListener(OnCreateLobbyClick);
        quickJoinButton.onClick.AddListener(OnQuickJoinClick);
        joinWithCodeButton.onClick.AddListener(OnJoinWithCodeClick);
        
        lobbyItemTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        playerAttributes = PlayerAttributes.Instance;
        lobbyHandler = LobbyHandler.Instance;
        networkManager = NetworkManager.Singleton;

        playerNameInputField.text = playerAttributes.GetLocalPlayerName();
        playerNameInputField.onValueChanged.AddListener(OnPlayerNameChanged);
        
        CustomInputField playerNameInputFieldModernUI = playerNameInputField.GetComponent<CustomInputField>();
        playerNameInputFieldModernUI.UpdateStateInstant();
        
        lobbyHandler.OnLobbyListChanged += OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }
    
    private void OnDestroy()
    {
        if (lobbyHandler != null)
            lobbyHandler.OnLobbyListChanged -= OnLobbyListChanged;
    }

    private void OnLobbyListChanged(List<Lobby> lobbyList)
    {
        UpdateLobbyList(lobbyList);
    }
    
    private void OnPlayerNameChanged(string newText)
    {
        playerAttributes.SetLocalPlayerName(newText);
    }

    private void OnMainMenuClick()
    {
        if (networkManager.IsServer)
            lobbyHandler.DeleteLobby();
        else
            lobbyHandler.LeaveLobby();
        
        SceneLoader.LoadScene(SceneName._0_MainMenu);
    }
    
    private void OnCreateLobbyClick()
    {
        lobbyCreateUI.SetShow(true);
    }
    
    private void OnQuickJoinClick()
    {
        lobbyHandler.QuickJoin();
    }
    
    private void OnJoinWithCodeClick()
    {
        lobbyHandler.JoinWithCode(joinCodeInputField.text);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        // Remove all existing lobby buttons
        foreach (Transform child in lobbyListContainer)
        {
            if (child == lobbyItemTemplate) 
                continue;
            
            Destroy(child.gameObject);
        }
        
        // Add a button for each lobby
        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyButtonTransform = Instantiate(lobbyItemTemplate, lobbyListContainer);
            lobbyButtonTransform.gameObject.SetActive(true);
            
            lobbyButtonTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
}
