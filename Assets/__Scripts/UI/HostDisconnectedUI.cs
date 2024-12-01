using Michsky.MUIP;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class HostDisconnectedUI : MonoBehaviour
{
    [SerializeField] private ButtonManager mainMenuButton;
    [SerializeField] private TMP_Text disconnectReasonText;

    // Dependencies
    private NetworkManager networkManager;
    
    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnPlayAgainButtonClicked);
    }

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
        
        if (networkManager.IsServer)
        {
            if (SceneLoader.GetCurrentScene() == SceneName._4_Game)
                networkManager.OnClientDisconnectCallback += OnOtherClientDisconnected;
        }
        else
        {
            networkManager.OnClientDisconnectCallback += OnHostDisconnected;
        }
        
        SetShow(false);
    }

    private void OnOtherClientDisconnected(ulong clientId)
    {
        // Called on the host when the other player disconnects

        if (SceneLoader.GetCurrentScene() != SceneName._4_Game)
            return;
    
        disconnectReasonText.text = "The other player has disconnected!!";
        SetShow(true);
    }
    
    private void OnHostDisconnected(ulong clientId)
    {
        // Called on the client when the host disconnects
        
        disconnectReasonText.text = "The host has disconnected!!";
        SetShow(true);
    }

    private void OnDestroy()
    {
        if (networkManager == null)
            return;
        
        if (networkManager.IsServer)
        {
            if (SceneLoader.GetCurrentScene() == SceneName._4_Game)
                networkManager.OnClientDisconnectCallback -= OnOtherClientDisconnected;
        }
        else
        {
            networkManager.OnClientDisconnectCallback -= OnHostDisconnected;
        }
    }
    
    private void OnPlayAgainButtonClicked()
    {
        networkManager.Shutdown();
        SceneLoader.LoadScene(SceneName._0_MainMenu);
    }

    private void SetShow(bool show)
    {
        gameObject.SetActive(show);
    }
}