using System.Collections.Generic;
using System.Linq;

public class GridObject
{
    private readonly GridPosition gridPosition;
    private GridSystem<GridObject> gridSystem;
    private readonly List<GridElement> gridElements;

    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        gridElements = new List<GridElement>();
    }

    public override string ToString()
    {
        string unitString = gridElements.Aggregate("", (current, gridElement) => current + (gridElement.name + "\n"));

        return $"{gridPosition.ToString()}\n{unitString}";
    }

    public void AddGridElement(GridElement gridElement)
    {
        gridElements.Add(gridElement);
    }

    public void RemoveGridElement(GridElement gridElement)
    {
        gridElements.Remove(gridElement);
    }

    public List<GridElement> GetGridElementList()
    {
        return gridElements;
    }

    public bool HasAnyGridElement()
    {
        return gridElements.Count > 0;
    }

    public GridElement GetGridElement()
    {
        return HasAnyGridElement() ? gridElements[0] : null;
    }
}