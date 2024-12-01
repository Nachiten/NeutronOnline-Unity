using System.Collections.Generic;
using System.Linq;
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
        joinCodeInputField.onValueChanged.AddListener(OnJoinCodeChanged);
        
        CustomInputField playerNameInputFieldModernUI = playerNameInputField.GetComponent<CustomInputField>();
        playerNameInputFieldModernUI.UpdateStateInstant();
        
        lobbyHandler.OnLobbyListChanged += OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void OnJoinCodeChanged(string newText)
    {
        newText = newText.Trim().ToUpper();
        
        if (newText.Length > 6)
            newText = newText.Substring(0, 6);
        
        joinCodeInputField.text = newText;
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
        newText = newText.Trim();
        
        // Max 30 characters
        if (newText.Length > 30)
        {
            newText = newText.Substring(0, 30);
        }
        
        playerNameInputField.text = newText;
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

    private List<Lobby> previousLobbyList = new();
    
    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        if (ListsAreIdentical(previousLobbyList, lobbyList))
            return;
        
        previousLobbyList = lobbyList;
        
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
    
    private bool ListsAreIdentical(List<Lobby> list1, List<Lobby> list2)
    {
        if (list1.Count != list2.Count)
            return false;
     
        // Check if lists are identical by LobbyCode
        return !list1.Where((t, i) => t.LobbyCode != list2[i].LobbyCode).Any();
    }
}
