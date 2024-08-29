using Unity.Netcode;
using UnityEngine;

public class PlayerHiding : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
         MoveOutsideMapServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void MoveOutsideMapServerRpc()
    {
        MoveOutsideMapClientRpc();
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    private void MoveOutsideMapClientRpc()
    {
        Vector3 newPosition = new(0, -50, 0);
        
        transform.position = newPosition;
    }
}
