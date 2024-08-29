using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoSingleUI : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private TMP_Text playerIdText;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerReadyText;
    [SerializeField] private Transform playerInfoContainer;
    [SerializeField] private Image playerImage;

    // Dependencies
    private PlayerReady playerReady;
    private PlayerDataHandler playerDataHandler;
    private PlayerAttributes playerAttributes;
    private NetworkManager networkManager;
    
    private void Start()
    {
        playerReady = PlayerReady.Instance;
        playerDataHandler = PlayerDataHandler.Instance;
        playerAttributes = PlayerAttributes.Instance;
        networkManager = NetworkManager.Singleton;
        
        playerReady.OnReadyChanged += UpdatePlayer;
        playerDataHandler.OnPlayerDataChanged += UpdatePlayer;
        
        UpdatePlayer();
    }

    private void OnDestroy()
    {
        if (playerReady != null)
            playerReady.OnReadyChanged -= UpdatePlayer;
        
        if (playerDataHandler != null)
            playerDataHandler.OnPlayerDataChanged -= UpdatePlayer;
    }

    private void UpdatePlayer()
    {
        bool playerConnected = playerDataHandler.IsPlayerIndexConnected(playerIndex);
        SetShow(playerConnected);
        
        playerIdText.text = "Player " + (playerIndex + 1);
        
        if (!playerConnected)
            return;

        PlayerData playerData = playerDataHandler.GetPlayerDataFromPlayerIndex(playerIndex);
        
        if (playerData.clientId == networkManager.LocalClientId)
            playerIdText.text += " (You)";
        
        playerNameText.text = playerData.name.ToString();
        playerReadyText.text = playerReady.IsPlayerReady(playerData.clientId) ? "READY" : "NOT READY";
        playerImage.color = playerAttributes.GetColorFromColorId(playerData.colorId);
    }
    
    private void SetShow(bool show)
    {
        playerInfoContainer.gameObject.SetActive(show);
    }
}
