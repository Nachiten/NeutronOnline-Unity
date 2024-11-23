using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;

/// <summary>
/// All scenes in the game.
/// </summary>
public enum SceneName
{
    _0_MainMenu,
    _1_LobbySelect,
    _2_JoinedLobby,
    _3_Loading,
    _4_Game,
}

/// <summary>
/// Manages the scene loading process.
/// </summary>
public static class SceneLoader
{
    private static SceneName targetScene;
    
    /// <summary>
    /// Loads the given scene. (NOT network)
    /// </summary>
    /// <param name="_targetScene">Scene to load</param>
    public static void LoadScene(SceneName _targetScene)
    {
        targetScene = _targetScene;

        SceneManager.LoadScene(SceneName._3_Loading.ToString());
    }
    
    /// <summary>
    /// Loads the given scene. (network)
    /// </summary>
    /// <param name="_targetScene">Scene to load</param>
    public static void LoadNetworkScene(SceneName _targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(_targetScene.ToString(), LoadSceneMode.Single);
    }

    /// <summary>
    /// Finishes the scene loading process.
    /// </summary>
    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }

    /// <summary>
    /// Returns all levels in the game.
    /// </summary>
    /// <returns>All scenes of the game that are playing levels</returns>
    public static List<SceneName> GetLevels()
    {
        return new List<SceneName>
        {
            SceneName._4_Game,
        };
    }
}