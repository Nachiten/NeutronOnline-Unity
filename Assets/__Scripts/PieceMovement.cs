using System;
using UnityEngine;

public class PieceMovement : MonoBehaviour
{
    [SerializeField] private GridPositionSelection gridPositionSelection;
    [SerializeField] private TurnSystem turnSystem;

    public Action<GridPosition> OnPieceSelected;
    public Action<GridPosition> OnPieceUnselected;
    public Action<GridPosition> OnMoveStarted;
    public Action<GridPosition> OnMoveEnded;
    
    private GridPosition selectedPiece = GridPosition.Null;
    
    private enum State
    {
        // Description: The player is selecting a piece to move
        SelectingPiece,
        // Description: The player is selecting a move position for the selected piece
        SelectingMove,
        // Description: The selected piece is moving to the selected move position
        MovingPiece
    }
    
    private State state;
    
    private void Start()
    {
        gridPositionSelection.OnGridPositionSelected += OnGridPositionSelected;
        LevelGrid.Instance.OnAnyGridElementMovedGridPosition += OnAnyGridElementMovedGridPosition;
        
        state = State.SelectingPiece;
    }

    private void OnAnyGridElementMovedGridPosition(GridElement gridElement, GridPosition fromGridPos, GridPosition toGridPos)
    {
        if (state != State.MovingPiece)
            return;
        
        GridElement selectedGridElement = LevelGrid.Instance.GetGridElementAtGridPos(toGridPos);
        
        if (gridElement != selectedGridElement)
            return;
        
        selectedPiece = GridPosition.Null;
        state = State.SelectingPiece;
        
        OnMoveEnded?.Invoke(toGridPos);
    }

    private void OnGridPositionSelected(GridPosition gridPosition)
    {
        switch (state)
        {
            case State.SelectingPiece:
                if (!GridPosIsValidToSelectPiece(gridPosition))
                    return;
                
                selectedPiece = gridPosition;
                OnPieceSelected?.Invoke(gridPosition);
                
                state = State.SelectingMove;
                break;
            case State.SelectingMove:
                if (gridPosition == selectedPiece)
                {
                    selectedPiece = GridPosition.Null;
                    state = State.SelectingPiece;
                    
                    OnPieceUnselected?.Invoke(gridPosition);
                    return;
                }
                
                if (GridPosIsValidToSelectPiece(gridPosition))
                { 
                    state = State.SelectingPiece;
                    OnGridPositionSelected(gridPosition);
                }
                
                GridElement selectedGridElement = LevelGrid.Instance.GetGridElementAtGridPos(selectedPiece);
                
                if (!selectedGridElement.IsMovePositionValid(gridPosition))
                    return;
                
                selectedGridElement.MoveToGridPositionServerRpc(gridPosition);
                state = State.MovingPiece;
                
                OnMoveStarted?.Invoke(gridPosition);
                break;
            case State.MovingPiece:
                // Do nothing, wait for event of piece finished moving
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private bool GridPosIsValidToSelectPiece(GridPosition gridPosition)
    {
        return LevelGrid.Instance.GridPosHasAnyGridElement(gridPosition) &&
               turnSystem.IsValidGridPosForTurn(gridPosition);
    }
}
