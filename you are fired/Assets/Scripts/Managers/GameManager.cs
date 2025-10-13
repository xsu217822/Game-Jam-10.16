using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { MainMenu, Playing, Paused }

public class GameManager : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string levelScene = "LevelScene"; // Ψһ����Ϸ����

    public static GameManager I { get; private set; }
    public GameState State { get; private set; } = GameState.MainMenu;

    private void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        // ����ǰ�������˵���ؿ��������ͻ����˵�
        var cur = SceneManager.GetActiveScene().name;
        if (cur != mainMenuScene && cur != levelScene)
            LoadMainMenu();
    }

    // ===== �������� =====
    public void LoadMainMenu()
    {
        State = GameState.MainMenu;
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void StartGame() // ���˵�����ʼ����ť�������
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelScene);
        // ���� LevelScene ���� LevelDirector �Լ����� campaign ���Ĺ�
    }

    public void RestartGame() // ��ѡ����ͷ����
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
