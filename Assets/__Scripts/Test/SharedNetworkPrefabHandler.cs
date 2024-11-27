using System;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class SharedNetworkPrefabHandler : SingletonNetwork<SharedNetworkPrefabHandler>
{
    [SerializeField] private Transform sharedObjectPrefab;
    private Transform sharedObjectInstance;
    
    public override void OnNetworkSpawn()
    {
        // Instantiate in network the prefab

        if (IsServer)
        {
            sharedObjectInstance = Instantiate(sharedObjectPrefab);
            NetworkObject instanceNetworkObject = sharedObjectInstance.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn();
        }
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 50, 150, 100), "Move Shared Object"))
        {
            MoveSharedObjectServerRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void MoveSharedObjectServerRpc()
    {
        Vector3 centerPosition = new(0, 0, 0);
        Vector3 randomOffset = new(Random.Range(-4, 4), Random.Range(-4, 4), 0);
        
        sharedObjectInstance.position = centerPosition + randomOffset;
    }
}
