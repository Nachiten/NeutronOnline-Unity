using System;
using UnityEngine;

public class Logger : MonoBehaviour
{
    [SerializeField] private TurnSystem turnSystem;
    [SerializeField] private WinManager winManager;

    private void Start()
    {
        LevelGrid.Instance.OnAnyGridElementMovedGridPosition += LogPieceMovement;
        winManager.OnPlayerWon += LogPlayerWon;
        
        LogGameStarted();
    }

    private void LogGameStarted()
    {
        Debug.Log("Game started!");
    }

    private void LogPieceMovement(GridElement gridElement, GridPosition previousGridPos, GridPosition targetGridPos)
    {
        try
        {
            int currentPlayerIndex = turnSystem.GetCurrentPlayerIndex();
            string playerName = PlayerDataHandler.Instance.GetPlayerNameFromPlayerIndex(currentPlayerIndex);
        
            Debug.Log($"FinishPieceMovementClientRpc: {playerName} moved {gridElement.name}: {previousGridPos} -> {targetGridPos}");
        } 
        catch (Exception e)
        {
            Debug.LogError($"Error in LogPieceMovement: {e}");
        }
    }
    
    private void LogPlayerWon(int playerIndex)
    {
        try
        {
            string playerName = PlayerDataHandler.Instance.GetPlayerNameFromPlayerIndex(playerIndex);
            Debug.Log($"Player {playerName} won!");
        } 
        catch (Exception e)
        {
            Debug.LogError($"Error in LogPlayerWon: {e}");
        }
    }
}
