using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    // Dependencies
    private PlayerActions playerActions;
    
    private void Start()
    {
        playerActions = PlayerActions.Instance;
        
        playerActions.OnTryingToJoinGame += OnTryingToJoinGame;
        playerActions.OnFailedToJoinGame += OnFailedToJoinGame;

        SetShow(false);
    }

    private void OnDestroy()
    {
        if (playerActions == null)
            return;
        
        playerActions.OnTryingToJoinGame -= OnTryingToJoinGame;
        playerActions.OnFailedToJoinGame -= OnFailedToJoinGame;
    }
    
    private void OnFailedToJoinGame()
    {
        SetShow(false);
    }

    private void OnTryingToJoinGame()
    {
        SetShow(true);
    }

    private void SetShow(bool show)
    {
        gameObject.SetActive(show);
    }
}