using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class GridSystem<TGridObject>
{
    private readonly float cellSize;

    private readonly TGridObject[,] gridObjects;
    private readonly int height;
    private readonly int width;

    public GridSystem(int width, int height, float cellSize,
        Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        
        gridObjects = new TGridObject[width, height];
        
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            gridObjects[x, y] = createGridObject(this, new GridPosition(x, y));
        }
    }

    public Vector3 GetWorldPos(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, gridPosition.y, 0) * cellSize;
    }

    public GridPosition GetGridPos(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int y = Mathf.RoundToInt(worldPosition.y / cellSize);

        return new GridPosition(x, y);
    }

    public void CreateDebugObjects(Transform debugPrefab, Transform debugPrefabParent)
    {
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            GridPosition gridPosition = new(x, y);
            
            Transform debugTransform = Object.Instantiate(
                debugPrefab, 
                GetWorldPos(gridPosition), 
                Quaternion.identity,
                debugPrefabParent);
    
            GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
            gridDebugObject.SetGridObject(GetGridObjectAtGridPos(gridPosition));
        }
    }
    
    public TGridObject GetGridObjectAtGridPos(GridPosition gridPosition)
    {
        return gridObjects[gridPosition.x, gridPosition.y];
    }

    public bool GridPosIsValid(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 &&
               gridPosition.x < width &&
               gridPosition.y >= 0 &&
               gridPosition.y < height;
    }

    public int GetWidth() => width;
    public int GetHeight() => height;
}