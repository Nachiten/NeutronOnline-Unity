using Unity.Netcode;

/// <summary>
/// Network behaviour singleton.
/// </summary>
/// <typeparam name="T">Type of the singleton.</typeparam>
public class SingletonNetwork<T> : NetworkBehaviour where T : NetworkBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        Instance = this as T;
    }
    
    public static void DestroyInstance()
    {
        if (Instance == null)
            return;
        
        Destroy(Instance.gameObject);
    }
}