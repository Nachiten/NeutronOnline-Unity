using System;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    [SerializeField] private TurnSystem turnSystem;

    // Player index
    public Action<int> OnPlayerWon;
    
    private void Start()
    {
        turnSystem.OnStateChanged += OnStateChanged;
    }
    
    private void OnStateChanged()
    {
        // This means the Electron finished moving
        if (turnSystem.IsMovingNeutron())
        {
            // If the neutron cannot move, the player who is NOT the current turn wins
            bool canNeutronMove = LevelGrid.Instance.GetNeutron().CanMove();

            if (canNeutronMove) 
                return;

            PlayerWins(turnSystem.GetCurrentPlayerIndex() == 0 ? 1 : 0);
        }
        // This means the Neutron finished moving
        else if (turnSystem.IsMovingElectron())
        {
            // If the Neutron is at the top row, player 2 wins
            // If the Neutron is at the bottom row, player 1 wins
            
            // If neutron y = 0, player 1 wins
            // If neutron y = height - 1, player 2 wins

            GridPosition neutronPosition = LevelGrid.Instance.GetNeutron().GetCurrentGridPosition();
            
            if (neutronPosition.y == 0)
            {
                PlayerWins(0);
            }
            else if (neutronPosition.y == LevelGrid.Instance.GetHeight() - 1)
            {
                PlayerWins(1);
            }
        }
    }
    
    private void PlayerWins(int playerIndex)
    {
        OnPlayerWon?.Invoke(playerIndex);
    }
}
