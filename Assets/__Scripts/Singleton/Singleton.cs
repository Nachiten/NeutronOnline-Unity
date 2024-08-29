using UnityEngine;

/// <summary>
/// MonoBehaviour singleton.
/// </summary>
/// <typeparam name="T">Type of the singleton.</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
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