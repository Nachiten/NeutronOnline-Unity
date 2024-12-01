using Michsky.MUIP;
using Unity.Netcode;
using UnityEngine;

public class GameButtonsUI : MonoBehaviour
{
    [SerializeField] private ButtonManager mainMenuButton;
    [SerializeField] private ButtonManager restartGameButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        restartGameButton.onClick.AddListener(OnRestartGameButtonClicked);
    }

    private void Start()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            restartGameButton.gameObject.SetActive(false);
        }
    }

    private void OnRestartGameButtonClicked()
    {
        Debug.Log("[Host] Restarting Game!");
        SceneLoader.LoadNetworkScene(SceneName._4_Game);
    }

    private void OnMainMenuButtonClicked()
    {
        PlayerActions.Instance.Disconnect();
        
        SceneLoader.LoadScene(SceneName._0_MainMenu);
    }
}
