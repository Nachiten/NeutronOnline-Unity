using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private Transform gridSystemVisualSingleParent;
    
    [SerializeField] private GridPositionSelection gridPositionSelection;
    [SerializeField] private PieceMovement pieceMovement;

    private GridSystemVisualSingle[,] gridSystemVisualSingles;
    private Dictionary<GridVisualType, Color> gridVisualTypeColors;

    private List<GridPosition> availableMovePositions;
    private GridPosition hoveredGridPosition = GridPosition.Null;
    private GridPosition selectedGridPosition = GridPosition.Null;

    private void Awake()
    { 
        List<GridVisualTypeColor> gridVisualTypeColorList = Resources.Load<GridVisualTypeColorsSO>(nameof(GridVisualTypeColorsSO)).gridVisualTypeColors;
        
        gridVisualTypeColors = new Dictionary<GridVisualType, Color>();
        
        foreach (GridVisualTypeColor gridVisualTypeColor in gridVisualTypeColorList)
            gridVisualTypeColors.Add(gridVisualTypeColor.gridVisualType, gridVisualTypeColor.color);
    }
    
    private void Start()
    {
        InstantiateGridSystemVisuals();

        gridPositionSelection.OnGridPositionHovered += OnGridPositionHovered;
        gridPositionSelection.OnGridPositionUnhovered += OnGridPositionUnhovered;
        
        pieceMovement.OnPieceSelected += OnPieceSelected;
        pieceMovement.OnPieceUnselected += OnPieceUnselected;
        pieceMovement.OnMoveStarted += OnMoveStarted;
    }

    private void OnMoveStarted(GridPosition gridPos)
    {
        ResetAllGridPositionsColor();
        availableMovePositions = null;
        selectedGridPosition = GridPosition.Null;
    }

    private void OnPieceSelected(GridPosition gridPos)
    {
        if (selectedGridPosition)
            OnPieceUnselected(selectedGridPosition);

        GridElement gridElement = LevelGrid.Instance.GetGridElementAtGridPos(gridPos);
        
        gridElement.CalculateAvailableMovePositions();
        availableMovePositions = gridElement.GetAvailableMovePositions();
        
        ColorGridPositions(availableMovePositions, GridVisualType.Yellow);
        
        SelectGridPosition(gridPos);
    }
    
    private void OnPieceUnselected(GridPosition gridPos)
    {
        ColorGridPositions(availableMovePositions, GridVisualType.White);
        UnselectGridPosition(gridPos);
    }

    private void OnGridPositionHovered(GridPosition hoveredGridPos)
    {
        if (hoveredGridPosition) 
            SetGridPositionsHovered(new List<GridPosition> { hoveredGridPosition }, false);
        
        hoveredGridPosition = hoveredGridPos;
        
        SetGridPositionsHovered(new List<GridPosition> {hoveredGridPos}, true);
    }

    private void OnGridPositionUnhovered(GridPosition gridPos)
    {
        SetGridPositionsHovered(new List<GridPosition> {gridPos}, false);
        
        hoveredGridPosition = GridPosition.Null;
    }
    
    private void SelectGridPosition(GridPosition gridPos)
    {
        ColorGridPositions(new List<GridPosition> {gridPos}, GridVisualType.Red);
        selectedGridPosition = gridPos;
    }
    
    private void UnselectGridPosition(GridPosition gridPos)
    {
        ResetGridPositionColor(gridPos);
        selectedGridPosition = GridPosition.Null;
    }

    private void InstantiateGridSystemVisuals()
    {
        int width = LevelGrid.Instance.GetWidth();
        int height = LevelGrid.Instance.GetHeight();

        gridSystemVisualSingles = new GridSystemVisualSingle[width, height];

        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            Vector3 worldPos = LevelGrid.Instance.GetWorldPos(new GridPosition(x, y));
            
            Transform gridSystemVisual =
                Instantiate(gridSystemVisualSinglePrefab, 
                    worldPos, 
                    Quaternion.identity, 
                    gridSystemVisualSingleParent);
                
            gridSystemVisualSingles[x, y] = gridSystemVisual.GetComponent<GridSystemVisualSingle>();
            gridSystemVisualSingles[x, y].SetDefaultColor(GetGridVisualTypeColor(GridVisualType.White));
        }
    }
    
    private void ColorGridPositions(List<GridPosition> gridPositions, GridVisualType gridVisualType)
    {
        gridPositions.ForEach(gridPosition =>
            gridSystemVisualSingles[gridPosition.x, gridPosition.y]
                .SetSpriteColor(GetGridVisualTypeColor(gridVisualType))
        );
    }
    
    private void SetGridPositionsHovered(List<GridPosition> gridPositions, bool hover)
    {
        gridPositions.ForEach(gridPosition => 
            gridSystemVisualSingles[gridPosition.x, gridPosition.y].SetHovered(hover)
        );
    }
    
    private void ResetGridPositionColor(GridPosition gridPosition)
    {
        gridSystemVisualSingles[gridPosition.x, gridPosition.y].ResetColor();
    }
    
    private void ResetGridPositionsColor(List<GridPosition> gridPositions)
    {
        gridPositions.ForEach(ResetGridPositionColor);
    }
    
    private void ResetAllGridPositionsColor()
    {
        foreach (GridSystemVisualSingle gridSystemVisualSingle in gridSystemVisualSingles)
            gridSystemVisualSingle.ResetColor();
    }
    
    private Color GetGridVisualTypeColor(GridVisualType gridVisualType)
    {
        return gridVisualTypeColors[gridVisualType];
    }
}