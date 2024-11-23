using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Can move in any direction, but it must move until it hits a wall or another grid element
/// </summary>
public class Electron : GridElement
{
    [SerializeField] private int playerIndex;
    
    public override void CalculateAvailableMovePositions()
    {
        List<Vector2> availableDirections = Constants.directions;
        List<GridPosition> _availableMovePositions = new();
        
        // For each available direction, keep saving the last valid position in a list
        foreach (Vector2 dir in availableDirections)
        {
            GridPosition lastValidPos = GridPosition.Null;
            GridPosition nextPos = GetNextGridPositionInDirection(currentGridPosition, dir);
            
            while (LevelGrid.Instance.GridPosIsValid(nextPos) && !LevelGrid.Instance.GridPosHasAnyGridElement(nextPos))
            {
                lastValidPos = nextPos;
                nextPos = GetNextGridPositionInDirection(lastValidPos, dir);
            }
            
            if (lastValidPos)
                _availableMovePositions.Add(lastValidPos);
        }
        
        availableMovePositions = _availableMovePositions;
    }
    
    private GridPosition GetNextGridPositionInDirection(GridPosition originGridPos, Vector2 direction)
    {
        return new GridPosition(originGridPos.x + (int) direction.x, originGridPos.y + (int) direction.y);
    }
    
    public int GetPlayerIndex() => playerIndex;
}
