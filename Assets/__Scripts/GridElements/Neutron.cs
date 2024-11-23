using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Can move 1 cell in any direction
/// </summary>
public class Neutron : GridElement
{
    public override void CalculateAvailableMovePositions()
    {
        List<Vector2> availableDirections = Constants.directions;
        
        List<GridPosition> _availableMovePositions = availableDirections.Select(
            dir => new GridPosition(currentGridPosition.x + (int) dir.x, currentGridPosition.y + (int) dir.y))
            .ToList();
        
        _availableMovePositions.RemoveAll(pos => 
            !LevelGrid.Instance.GridPosIsValid(pos) || 
            LevelGrid.Instance.GridPosHasAnyGridElement(pos));
        
        availableMovePositions = _availableMovePositions;
    }
}
