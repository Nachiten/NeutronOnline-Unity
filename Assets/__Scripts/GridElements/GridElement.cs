using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class GridElement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 20f;
    
    protected GridPosition currentGridPosition;
    protected List<GridPosition> availableMovePositions;
    
    private Vector3 targetPosition;
    private bool isMoving;

    protected void Start()
    {
        currentGridPosition = LevelGrid.Instance.GetGridPos(transform.position);
        LevelGrid.Instance.AddGridElementAtGridPos(currentGridPosition, this);
    }

    protected void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (!isMoving)
            return;

        // Calculate new position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (transform.position != targetPosition) 
            return;
        
        isMoving = false;
            
        GridPosition targetGridPos = LevelGrid.Instance.GetGridPos(targetPosition);
        GridPosition previousGridPos = currentGridPosition;
        
        FinishPieceMovementClientRpc(previousGridPos, targetGridPos);
    }
    
    [Rpc(SendTo.Everyone)]
    private void FinishPieceMovementClientRpc(GridPosition previousGridPos, GridPosition targetGridPos)
    {
        currentGridPosition = targetGridPos;
        
        LevelGrid.Instance.MoveGridElementGridPos(this, previousGridPos, targetGridPos);
    }
    
    [Rpc(SendTo.Server)]
    public void MoveToGridPositionServerRpc(GridPosition gridPos)
    {
        if (!IsMovePositionValid(gridPos))
        {
            Debug.LogError($"Invalid move position: {gridPos}");
            return;
        }
        
        targetPosition = LevelGrid.Instance.GetWorldPos(gridPos);
        isMoving = true;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;

        return obj.GetType() == GetType() && Equals((GridElement)obj);
    }

    private bool Equals(GridElement other)
    {
        return base.Equals(other) && currentGridPosition.Equals(other.currentGridPosition);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), currentGridPosition);
    }

    public override string ToString()
    {
        return $"GridElement: {currentGridPosition}";
    }
    
    public bool IsMovePositionValid(GridPosition gridPos)
    {
        CalculateAvailableMovePositions();
        
        return availableMovePositions.Contains(gridPos);
    }
    
    public abstract void CalculateAvailableMovePositions();
    
    public List<GridPosition> GetAvailableMovePositions() => availableMovePositions;
    
    public bool CanMove() 
    {
        CalculateAvailableMovePositions();
        return availableMovePositions.Count > 0;
    }
    
    public GridPosition GetCurrentGridPosition() => currentGridPosition;

}