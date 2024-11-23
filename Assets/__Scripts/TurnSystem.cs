using System;
using Unity.Netcode;
using UnityEngine;

public class TurnSystem : NetworkBehaviour
{
    [SerializeField] private PieceMovement pieceMovement;
    
    public Action OnStateChanged;
    
    private int currentPlayerIndex;
    private int currentTurn;
    private int localPlayerIndex;

    private enum State
    {
        // The player is moving the Neutron
        MovingNeutron,
        // The player is moving an Electron
        MovingElectron,
        // The script is waiting for the movement to finish
        WaitingForMovementFinish
    }
    
    private State state;
    private State nextState;

    private void Awake()
    {
        currentPlayerIndex = 0;
        currentTurn = 0;
        
        SetState(State.MovingNeutron);
    }

    private void Start()
    {
        pieceMovement.OnMoveStarted += OnMoveStarted;
        pieceMovement.OnMoveEnded += OnMoveEnded;
        
        localPlayerIndex = PlayerDataHandler.Instance.GetLocalPlayerIndex();
    }

    private void OnMoveStarted(GridPosition newGridPos)
    {
        OnMoveStartedClientRpc();
    }
    
    [Rpc(SendTo.Everyone)]
    private void OnMoveStartedClientRpc()
    {
        Debug.Log("OnMoveStartedClientRpc");
        
        switch (state)
        {
            case State.MovingNeutron:
                nextState = State.MovingElectron;
                SetState(State.WaitingForMovementFinish);
                break;
            case State.MovingElectron:
                nextState = State.MovingNeutron;
                SetState(State.WaitingForMovementFinish);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void OnMoveEnded(GridPosition newGridPos)
    {
        OnMoveEndedClientRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void OnMoveEndedClientRpc()
    {
        Debug.Log("OnMoveEndedClientRpc");
        
        if (nextState == State.MovingNeutron)
        {
            StartNextTurn();
        }
        
        SetState(nextState);
    }
    
    private void StartNextTurn()
    {
        currentTurn++;
        currentPlayerIndex = currentTurn % 2;
    }
    
    public bool IsValidGridPosForTurn(GridPosition gridPosition)
    {
        GridElement gridElement = LevelGrid.Instance.GetGridElementAtGridPos(gridPosition);
        
        return state switch
        {
            State.MovingNeutron => gridElement is Neutron,
            State.MovingElectron => gridElement.TryGetComponent(out Electron electron) &&
                                    electron.GetPlayerIndex() == currentPlayerIndex,
            _ => false
        };
    }
    
    private void SetState(State newState)
    {
        state = newState;
        OnStateChanged?.Invoke();
    }
    
    public int GetCurrentPlayerIndex() => currentPlayerIndex;
    
    public bool IsMovingNeutron() => state == State.MovingNeutron;
    public bool IsMovingElectron() => state == State.MovingElectron;
    public bool IsLocalPlayerTurn() => currentPlayerIndex == localPlayerIndex;
}
