using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneButtonManager : MonoBehaviour
{
    [Header("Scene Indexes")]
    public int homeSceneIndex = 0;    // Index for Home Scene
    public int gameSceneIndex = 1;    // Index for Game Scene

    /// <summary>
    /// Called by the Play button to go to the game scene
    /// </summary>
    public void OnPlayButton()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }

    /// <summary>
    /// Called by the Restart button to reload the current scene
    /// </summary>
    public void OnRestartButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Called by the Back to Home button to go to the home scene
    /// </summary>
    public void OnBackToHomeButton()
    {
        SceneManager.LoadScene(homeSceneIndex);
    }
}
