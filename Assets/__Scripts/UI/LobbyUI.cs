using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinWithCodeButton;
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;
    
    private bool selectFirstButton;
    
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
        
        lobbyTemplate.gameObject.SetActive(false);
        SelectFirstButton();
    }

    private void Start()
    {
        playerAttributes = PlayerAttributes.Instance;
        lobbyHandler = LobbyHandler.Instance;
        networkManager = NetworkManager.Singleton;
        
        playerNameInputField.text = playerAttributes.GetLocalPlayerName();
        playerNameInputField.onValueChanged.AddListener(OnPlayerNameChanged);
        
        lobbyHandler.OnLobbyListChanged += OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }
    
    private void LateUpdate()
    {
        if (!selectFirstButton)
            return;

        selectFirstButton = false;
        SelectFirstButton();
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
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) 
                continue;
            
            Destroy(child.gameObject);
        }
        
        // Add a button for each lobby
        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyButtonTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyButtonTransform.gameObject.SetActive(true);
            
            lobbyButtonTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            selectFirstButton = true;
    }

    private void SelectFirstButton()
    {
        createLobbyButton.Select();
    }
}
