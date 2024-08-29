using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    private void OnMainMenuButtonClicked()
    {
        PlayerActions.Instance.Disconnect();
        
        SceneLoader.LoadScene(SceneName._0_MainMenu);
    }
}
