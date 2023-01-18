using UnityEngine;
using UnityEngine.SceneManagement;

// PlayerPrefs Values "CurrentLevel"
public class LevelManager : MonoBehaviour
{
    public string currentLevel;

    public static LevelManager Instance;
    private void Awake()
    {
        Instance = this;
        currentLevel = PlayerPrefs.GetString("CurrentLevel");
    }

    public void ChangeScene() => SceneManager.LoadScene(currentLevel);
    public static void ChangeScene(string newScene) => SceneManager.LoadScene(newScene);
    

}
