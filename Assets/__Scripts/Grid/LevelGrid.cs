using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : Singleton<LevelGrid>
{
    public Action<GridElement, GridPosition, GridPosition> OnAnyGridElementMovedGridPosition;

    [SerializeField] private bool showDebugObjects;
    
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private Transform gridDebugObjectParent;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    
    private List<GridSystem<GridObject>> gridSystems;
    private GridElement neutron;

    protected override void Awake()
    {
        base.Awake();
        
        InitializeGridSystems();
    }

    private void InitializeGridSystems()
    {
        gridSystems = new List<GridSystem<GridObject>>();
        
        GridSystem<GridObject> gridSystem = new(width, height, cellSize, 
            (gridSystem, gridPosition) => new GridObject(gridSystem, gridPosition));

        if (showDebugObjects)
            gridSystem.CreateDebugObjects(gridDebugObjectPrefab, gridDebugObjectParent);
        
        gridSystems.Add(gridSystem);
    }

    /// <summary>
    /// Add a GridElement to the grid at the given grid position
    /// </summary>
    /// <param name="gridPos"> The grid position to add the GridElement to </param>
    /// <param name="gridElement"> The GridElement to add to the grid </param>
    public void AddGridElementAtGridPos(GridPosition gridPos, GridElement gridElement)
    {
        if (gridElement is Neutron)
        {
            neutron = gridElement;
        }
        
        GetGridObjectAtGridPos(gridPos).AddGridElement(gridElement);
    }

    /// <summary>
    /// Get the first GridElement at the given grid position
    /// </summary>
    /// <param name="gridPos"> The grid position to get the GridElement from </param>
    /// <returns> The first GridElement at the given grid position </returns>
    public GridElement GetGridElementAtGridPos(GridPosition gridPos)
    {
        return GetGridObjectAtGridPos(gridPos).GetGridElement();
    }
    
    public GridElement GetNeutron()
    {
        return neutron;
    }
    
    /// <summary>
    /// Gets the list of units at the given grid position
    /// </summary>
    /// <param name="gridPos"> The grid position to get the list of units from </param>
    /// <returns> The list of units at the given grid position </returns>
    public List<GridElement> GetGridElementsAtGridPos(GridPosition gridPos)
    {
        return GetGridObjectAtGridPos(gridPos).GetGridElementList();
    }

    /// <summary>
    /// Remove a GridElement from the given grid position
    /// </summary>
    /// <param name="gridPos"> The grid position to remove the GridElement from </param>
    /// <param name="gridElement"> The GridElement to remove from the grid </param>
    private void RemoveGridElementAtGridPos(GridPosition gridPos, GridElement gridElement)
    {
        GetGridObjectAtGridPos(gridPos).RemoveGridElement(gridElement);
    }

    /// <summary>
    /// Move a GridElement from fromGridPos to toGridPos
    /// </summary>
    /// <param name="gridElement"> The GridElement to move </param>
    /// <param name="fromGridPos"> The origin grid position </param>
    /// <param name="toGridPos"> The destination grid position </param>
    public void MoveGridElementGridPos(GridElement gridElement, GridPosition fromGridPos, GridPosition toGridPos)
    {
        RemoveGridElementAtGridPos(fromGridPos, gridElement);
        AddGridElementAtGridPos(toGridPos, gridElement);

        OnAnyGridElementMovedGridPosition?.Invoke(gridElement, fromGridPos, toGridPos);
    }

    /// <summary>
    /// Get if the given grid position is valid
    /// </summary>
    /// <param name="gridPos"> The grid position to check </param>
    /// <returns> True if the grid position is valid </returns>
    public bool GridPosIsValid(GridPosition gridPos)
    {
        return GetGridSystem().GridPosIsValid(gridPos);
    }

    /// <summary>
    /// Get if the given grid position has any GridElement
    /// </summary>
    /// <param name="gridPos"> The grid position to check </param>
    /// <returns> True if the grid position has any GridElement </returns>
    public bool GridPosHasAnyGridElement(GridPosition gridPos)
    {
        return GetGridObjectAtGridPos(gridPos).HasAnyGridElement();
    }

    /// <summary>
    /// Get the grid object at the given grid position
    /// </summary>
    /// <param name="gridPos"> The grid position to get the grid object from </param>
    /// <returns> The grid object at the given grid position </returns>
    private GridObject GetGridObjectAtGridPos(GridPosition gridPos)
    {
        return GetGridSystem().GetGridObjectAtGridPos(gridPos);
    }

    /// <summary>
    /// Get the grid position from the given world position
    /// </summary>
    /// <param name="worldPos"> The world position to get the grid position from </param>
    /// <returns> The grid position from the given world position </returns>
    public GridPosition GetGridPos(Vector3 worldPos)
    {
        return GetGridSystem().GetGridPos(worldPos);
    }

    /// <summary>
    /// Get the world position from the given grid position
    /// </summary>
    /// <param name="gridPos"> The grid position to get the world position from </param>
    /// <returns> The world position from the given grid position </returns>
    public Vector3 GetWorldPos(GridPosition gridPos)
    {
        return GetGridSystem().GetWorldPos(gridPos);
    }
    
    private GridSystem<GridObject> GetGridSystem(int index = 0) => gridSystems[index];
    public int GetWidth() => GetGridSystem().GetWidth();
    public int GetHeight() => GetGridSystem().GetHeight(); 
    
    public float GetCellSize() => cellSize;
}