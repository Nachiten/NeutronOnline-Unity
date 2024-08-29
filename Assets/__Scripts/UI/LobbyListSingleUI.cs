using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyNameText;
    
    private Lobby lobby;

    // Dependencies
    private LobbyHandler lobbyHandler;
    
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnLobbyJoinClick);
    }

    private void Start()
    {
        lobbyHandler = LobbyHandler.Instance;
    }

    public void SetLobby(Lobby _lobby)
    { 
        lobby = _lobby;
        lobbyNameText.text = lobby.Name;
    }
    
    private void OnLobbyJoinClick()
    {
        lobbyHandler.JoinWithId(lobby.Id);
    }
}
