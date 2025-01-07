using System;
using UnityEngine;

public class GridPositionSelection : MonoBehaviour
{
    [SerializeField] private WinManager winManager;
    [SerializeField] private TurnSystem turnSystem;
    [SerializeField] private HostDisconnectedUI hostDisconnectedUI;
    
    public event Action<GridPosition> OnGridPositionSelected;
    public event Action<GridPosition> OnGridPositionHovered;
    public event Action<GridPosition> OnGridPositionUnhovered;
    
    private GridPosition hoveredGridPosition = GridPosition.Null;
    
    private void Start()
    {
        winManager.OnPlayerWon += OnPlayerWon;
        hostDisconnectedUI.OnOtherPlayerDisconnected += OnOtherPlayerDisconnected;
    }
    
    private void Update()
    {
        HandleHovering();
        HandleSelection();
    }
    
    private void HandleHovering()
    {
        if (MobileBuildCheck.IsMobileBuild())
            return;
        
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
        if (!hoveredGridPosition || MobileBuildCheck.IsMobileBuild())
            return;
        
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
    
    private void OnOtherPlayerDisconnected()
    {
        UnhoverPosition();
        enabled = false;
    }
    

}
