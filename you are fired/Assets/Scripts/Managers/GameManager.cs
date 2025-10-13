using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { MainMenu, Playing, Paused }

public class GameManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string levelScene = "LevelScene"; // һ�����������й�

    [Header("Levels")]
    [SerializeField] private LevelConfigDatabase levelDB; // ��������Ĺ�Config��SO

    public static GameManager I { get; private set; }
    public GameState State { get; private set; } = GameState.MainMenu;
    public int CurrentLevelIndex { get; private set; } = 0; // 0..3

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // ���Bootֱ��������ű�������Ҳ�����������Զ������˵���
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
            Debug.LogError("GameManager: LevelConfigDatabase δ���û�Ϊ�ա�");
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
