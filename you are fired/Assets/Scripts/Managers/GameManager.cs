using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameFlowState { Boot, Menu, Playing, StageClear, GameOver, Ending }

public class GameManager : MonoBehaviour
{
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string[] levelScenes = { "Level1", "Level2", "Level3", "Level4" };

    private int currentLevel = -1;
    public GameFlowState State { get; private set; } = GameFlowState.Boot;

    public void StartFromLevel(int index)
    {
        currentLevel = index;
        LoadLevel(currentLevel);
    }

    public void LoadMainMenu()
    {
        State = GameFlowState.Menu;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelScenes.Length)
        {
            Debug.LogError("Invalid level index");
            return;
        }

        State = GameFlowState.Playing;
        SceneManager.LoadScene(levelScenes[index]);
    }

    public void OnStageClear()
    {
        currentLevel++;
        if (currentLevel < levelScenes.Length)
            LoadLevel(currentLevel);
        else
            LoadMainMenu(); // 理论上走不到；最终结局 UI 里会回主菜单
    }

    public void OnGameOver()
    {
        State = GameFlowState.GameOver;
        SceneManager.LoadScene(mainMenuScene);
    }
}
