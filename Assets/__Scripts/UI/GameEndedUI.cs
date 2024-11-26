using TMPro;
using UnityEngine;

public class GameEndedUI : MonoBehaviour
{
    [SerializeField] private WinManager winManager;
    [SerializeField] private TMP_Text playerWonText;

    private void Start()
    {
        winManager.OnPlayerWon += OnPlayerWon;
        SetShow(false);
    }

    private void OnPlayerWon(int playerIndex)
    {
        playerWonText.text = $"Player {playerIndex + 1} won!!";
        SetShow(true);
    }
    
    private void SetShow(bool show)
    {
        gameObject.SetActive(show);
    }
}
