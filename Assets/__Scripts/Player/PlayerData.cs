using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData: IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public ushort colorId;
    
    public FixedString64Bytes name;
    public FixedString64Bytes playerId;
    
    public PlayerData(ulong clientId, string name, ushort colorId, string playerId)
    {
        this.clientId = clientId;
        this.name = name;
        this.colorId = colorId;
        this.playerId = playerId;
    }
    
    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId && 
               name == other.name && 
               colorId == other.colorId &&
               playerId == other.playerId;
    }
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref name);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerId);
    }
    
    public override string ToString()
    {
        return $"clientId: {clientId}, playerName: {name}, colorId: {colorId}";
    }
}
