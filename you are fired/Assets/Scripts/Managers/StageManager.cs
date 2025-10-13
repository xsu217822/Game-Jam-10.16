using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private StageConfig config;

    private bool endingShown = false;
    private Player player;
    [SerializeField] private GameObject pauseMenuUI; // 指向你的暂停菜单UI
    private bool isPaused = false;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        // 这里可做关卡初始化、刷怪 Setup 等
    }

    private void Update()
    {
        if (endingShown) return;

        // 你的胜败条件（示例）
        bool playerDead = (player == null) || (player.CurrentHealth <= 0);
        bool stageCleared = CheckStageClear(); // TODO：按你们实际逻辑实现

        if (playerDead)
        {
            ShowFailEndingThenBackToMenu();
        }
        else if (stageCleared)
        {
            if (config.isFinalStage)
            {
                ShowFinalEndingThenBackToMenu();
            }
            else
            {
                GoNextLevel();
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void ShowFailEndingThenBackToMenu()
    {
        endingShown = true;
        Services.UI.ShowEndingUI(config.failEndingUIPrefab, () =>
        {
            Services.Game.LoadMainMenu();
        });
    }

    private void ShowFinalEndingThenBackToMenu()
    {
        endingShown = true;
        var ui = config.successEndingUIPrefab != null ? config.successEndingUIPrefab
                                                      : config.failEndingUIPrefab; // 兜底
        Services.UI.ShowEndingUI(ui, () =>
        {
            Services.Game.LoadMainMenu();
        });
    }

    private void GoNextLevel()
    {
        endingShown = true; // 防止二次触发
        Services.Game.OnStageClear(); // 直接切到下一关
    }

    private bool CheckStageClear()
    {
        // 例子：打死 Boss / 存活到计时 / 清空存活敌人
        // return Services.Spawn.AllObjectivesCleared();
        return false;
    }
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);   // 显示暂停菜单
        Time.timeScale = 0f;           // 暂停游戏
        isPaused = true;
        Cursor.lockState = CursorLockMode.None; // 解锁鼠标（如果你锁定过）
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);  // 关闭暂停菜单
        Time.timeScale = 1f;           // 恢复游戏
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked; // 重新锁定鼠标（按需）
        Cursor.visible = false;
    }
}
