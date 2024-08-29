using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Resets the singletons.
/// </summary>
public class ResetSingletons : MonoBehaviour
{
    private void Awake()
    {
        PlayerDataHandler.DestroyInstance();
        PlayerActions.DestroyInstance();
        PlayerAttributes.DestroyInstance();
        LobbyHandler.DestroyInstance();
        
        if (NetworkManager.Singleton != null)
            Destroy(NetworkManager.Singleton.gameObject);
    }
}
