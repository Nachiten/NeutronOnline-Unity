using Unity.Netcode;
using UnityEngine;

public class RpcTest : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (IsOwner) 
        {
            ServerRpc(OwnerClientId);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ClientsAndHostsRpc(ulong clientId)
    {
        Debug.Log($"[ClientsAndHostsRpc] {OwnerClientId} Received the RPC from {clientId}");
    }

    [Rpc(SendTo.Server)]
    private void ServerRpc(ulong clientId)
    {
        Debug.Log($"[ServerRpc] {OwnerClientId} Received the RPC from {clientId}");
        ClientsAndHostsRpc(clientId);
    }
    
    [Rpc(SendTo.NotServer)]
    private void NotServerRpc(ulong clientId)
    {
        Debug.Log($"[NotServerRpc] {OwnerClientId} Received the RPC from {clientId}");
    }
}
