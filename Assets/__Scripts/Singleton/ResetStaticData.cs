using UnityEngine;

/// <summary>
/// Resets the static data.
/// </summary>
public class ResetStaticData : MonoBehaviour
{
    private void Awake()
    {
        PlayerReady.ResetStaticData();
        
        // TODO - Check if there is more static data to reset
    }
}
