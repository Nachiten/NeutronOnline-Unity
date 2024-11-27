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
            Destroy(gameObject);
            return;
        }
        
        networkManager.OnClientDisconnectCallback += OnHostDisconnected;

        SetShow(false);
    }
    
    private void OnDestroy()
    {
        if (networkManager == null || networkManager.IsServer)
            return;
        
        networkManager.OnClientDisconnectCallback -= OnHostDisconnected;
    }
    
    private void OnPlayAgainButtonClicked()
    {
        networkManager.Shutdown();
        SceneLoader.LoadScene(SceneName._0_MainMenu);
    }
    
    private void OnHostDisconnected(ulong clientId)
    {
        // Called on the client when the host disconnects
        
        SetShow(true);
        
        disconnectReasonText.text = "Host has disconnected!!";
    }

    private void SetShow(bool show)
    {
        gameObject.SetActive(show);
    }
}