using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { MainMenu, Playing, Paused }

public class GameManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string levelScene = "LevelScene"; // 唯一的游戏场景

    public static GameManager I { get; private set; }
    public GameState State { get; private set; } = GameState.MainMenu;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // 若当前不在主菜单或关卡场景，就回主菜单
        var cur = SceneManager.GetActiveScene().name;
        if (cur != mainMenuScene && cur != levelScene)
            LoadMainMenu();
    }

    // ===== 场景控制 =====
    public void LoadMainMenu()
    {
        State = GameState.MainMenu;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void StartGame() // 主菜单“开始”按钮调用这个
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelScene);
        // 进入 LevelScene 后，由 LevelDirector 自己按照 campaign 跑四关
    }

    public void RestartGame() // 可选：从头再来
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelScene);
    }

    public void Pause(bool pause)
    {
        State = pause ? GameState.Paused : GameState.Playing;
        Time.timeScale = pause ? 0f : 1f;
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
