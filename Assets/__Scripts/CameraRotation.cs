using Unity.Netcode;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    private void Start()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
    }
}
