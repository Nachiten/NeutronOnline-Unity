using System;
using UnityEngine;

public class GridPositionSelection : MonoBehaviour
{
    [SerializeField] private WinManager winManager;
    [SerializeField] private TurnSystem turnSystem;
    
    public event Action<GridPosition> OnGridPositionSelected;
    public event Action<GridPosition> OnGridPositionHovered;
    public event Action<GridPosition> OnGridPositionUnhovered;
    
    private GridPosition hoveredGridPosition = GridPosition.Null;
    
    private void Start()
    {
        winManager.OnPlayerWon += OnPlayerWon;
    }

    private void Update()
    {
        HandleHovering();
        HandleSelection();
    }
    
    private void HandleHovering()
    {
        GridPosition newHoveredGridPosition = MouseWorldVisual.GetMouseGridPosition();

        if (newHoveredGridPosition == hoveredGridPosition)
            return;
        
        if (!LevelGrid.Instance.GridPosIsValid(newHoveredGridPosition))
        {
            if (hoveredGridPosition)
                UnhoverPosition();
            
            return;
        }
        
        HoverPosition(newHoveredGridPosition);
    }
    
    private void HandleSelection()
    {
        if (!turnSystem.IsLocalPlayerTurn())
            return;
        
        if (!InputManager.Instance.WasPrimaryActionReleasedThisFrame())
            return;
        
        GridPosition newSelectedGridPosition = MouseWorldVisual.GetMouseGridPosition();
        
        if (!LevelGrid.Instance.GridPosIsValid(newSelectedGridPosition))
            return;
        
        SelectPosition(newSelectedGridPosition);
    }
    
    private void HoverPosition(GridPosition gridPosition)
    {
        hoveredGridPosition = gridPosition;
        OnGridPositionHovered?.Invoke(hoveredGridPosition);
    }
    
    private void UnhoverPosition()
    {
        OnGridPositionUnhovered?.Invoke(hoveredGridPosition);
        hoveredGridPosition = GridPosition.Null;
    }
    
    private void SelectPosition(GridPosition gridPosition)
    {
        OnGridPositionSelected?.Invoke(gridPosition);
    }
    
    private void OnPlayerWon(int obj)
    {
        UnhoverPosition();
        enabled = false;
    }
}
