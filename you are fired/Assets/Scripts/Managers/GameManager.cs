using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { MainMenu, Playing }

public class GameManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string levelScene = "LevelScene";

    [Header("主菜单BGM（纯循环）")]
    [SerializeField] private AudioClip menuLoopBgm;

    public static GameManager I { get; private set; }
    public GameState State { get; private set; } = GameState.MainMenu;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        var cur = SceneManager.GetActiveScene().name;
        if (cur != mainMenuScene && cur != levelScene)
            LoadMainMenu();
        else if (cur == mainMenuScene)
            TryPlayMenuBgm();
    }

    public void LoadMainMenu()
    {
        State = GameState.MainMenu;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
        // 场景加载是异步完成的；保险起见，下一帧播BGM
        Invoke(nameof(TryPlayMenuBgm), 0f);
    }

    public void StartGame()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelScene);
        // 关卡BGM由 LevelDirector.OnLevelChanged 触发，不在此处播放
    }

    public void RestartGame()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelScene);
    }

    public void Pause(bool pause) => Time.timeScale = pause ? 0f : 1f;

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void TryPlayMenuBgm()
    {
        if (AudioManager.I != null)
            AudioManager.I.PlayMenuLoop(menuLoopBgm);
    }
}
