using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    
    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        SceneLoader.LoadScene(SceneName._1_LobbySelect);
    }
}
