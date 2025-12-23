using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { MainMenu, Playing }

public class GameManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string levelScene = "LevelScene";


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
    }

    public void LoadMainMenu()
    {
        State = GameState.MainMenu;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
        // �����������첽��ɵģ������������һ֡��BGM
        Invoke(nameof(TryPlayMenuBgm), 0f);
    }

    public void StartGame()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelScene);
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
        // AudioManager 在 Start() 中会自动播放菜单音乐
        if (AudioManager.I != null && AudioManager.I.IsInCredits)
        {
            AudioManager.I.EndCredits();
        }
    }
}
