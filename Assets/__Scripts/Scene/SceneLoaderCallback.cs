using UnityEngine;

/// <summary>
/// Calls a callback when the scene is loaded.
/// </summary>
public class SceneLoaderCallback : MonoBehaviour
{
    private bool isFirstUpdate = true;

    private void Update()
    {
        if (!isFirstUpdate) 
            return;
        
        isFirstUpdate = false;

        SceneLoader.LoaderCallback();
    }
}