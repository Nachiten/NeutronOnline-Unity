using Michsky.MUIP;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private ButtonManager startButton;
    [SerializeField] private ButtonManager quitButton;
    
    private void Start()
    {
        startButton.onClick.AddListener(StartGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void StartGame()
    {
        SceneLoader.LoadScene(SceneName._1_LobbySelect);
    }
    
    private void QuitGame()
    {
        Application.Quit();
    }
}
