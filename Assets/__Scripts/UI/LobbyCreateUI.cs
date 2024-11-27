using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private ButtonManager createPublicButton;
    [SerializeField] private ButtonManager createPrivateButton;
    [SerializeField] private TMP_InputField lobbyNameInputField;

    // Dependencies
    private LobbyHandler lobbyHandler;
    
    private void Awake()
    {
        closeButton.onClick.AddListener(OnCloseClick);
        createPublicButton.onClick.AddListener(OnCreatePublicClick);
        createPrivateButton.onClick.AddListener(OnCreatePrivateClick);
        
        SetShow(false);
    }

    private void Start()
    {
        lobbyHandler = LobbyHandler.Instance;
        
        lobbyNameInputField.onValueChanged.AddListener(OnLobbyNameChanged);
    }

    private void OnLobbyNameChanged(string newText)
    {
        newText = newText.Trim();
        
        // Max 30 characters
        if (lobbyNameInputField.text.Length > 30)
        {
            newText = newText.Substring(0, 30);
        }
        
        lobbyNameInputField.text = newText;
    }

    private void OnCloseClick()
    {
        SetShow(false);
    }

    private void OnCreatePublicClick()
    {
        lobbyHandler.CreateLobby(GetLobbyName(), false);
    }

    private void OnCreatePrivateClick()
    {
        lobbyHandler.CreateLobby(GetLobbyName(), true);
    }

    private string GetLobbyName()
    {
        const string defaultLobbyName = "Lobby Name";

        if (string.IsNullOrEmpty(lobbyNameInputField.text) || lobbyNameInputField.text == defaultLobbyName)
            lobbyNameInputField.text = defaultLobbyName + " " + Random.Range(1000, 10000);

        return lobbyNameInputField.text;
    }
    
    public void SetShow(bool show)
    {
        gameObject.SetActive(show);
    }
}
