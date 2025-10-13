using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { MainMenu, Playing, Paused }

public class GameManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string levelScene = "LevelScene"; // 一个场景跑所有关

    [Header("Levels")]
    [SerializeField] private LevelConfigDatabase levelDB; // 拖入包含四关Config的SO

    public static GameManager I { get; private set; }
    public GameState State { get; private set; } = GameState.MainMenu;
    public int CurrentLevelIndex { get; private set; } = 0; // 0..3

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // 如果Boot直接用这个脚本启动，也可以在这里自动进主菜单：
        if (SceneManager.GetActiveScene().name != mainMenuScene &&
            SceneManager.GetActiveScene().name != levelScene)
        {
            LoadMainMenu();
        }
    }

    // === Public API ===
    public void LoadMainMenu()
    {
        State = GameState.MainMenu;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void StartGameAt(int levelIndex)
    {
        if (levelDB == null || levelDB.levels == null || levelDB.levels.Length == 0)
        {
            Debug.LogError("GameManager: LevelConfigDatabase 未设置或为空。");
            return;
        }

        CurrentLevelIndex = Mathf.Clamp(levelIndex, 0, levelDB.levels.Length - 1);
        State = GameState.Playing;
        SceneManager.LoadScene(levelScene);
    }

    public void NextLevelOrMenu()
    {
        CurrentLevelIndex++;
        if (levelDB != null && CurrentLevelIndex < levelDB.levels.Length)
        {
            State = GameState.Playing;
            SceneManager.LoadScene(levelScene);
        }
        else
        {
            LoadMainMenu();
        }
    }

    public void Pause(bool pause)
    {
        State = pause ? GameState.Paused : GameState.Playing;
        Time.timeScale = pause ? 0f : 1f;
    }

    public LevelConfig GetCurrentLevelConfig()
    {
        if (levelDB == null || levelDB.levels == null || levelDB.levels.Length == 0)
            return null;
        int idx = Mathf.Clamp(CurrentLevelIndex, 0, levelDB.levels.Length - 1);
        return levelDB.levels[idx];
    }
}
