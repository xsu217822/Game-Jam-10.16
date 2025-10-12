using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private StageConfig config;

    private bool endingShown = false;
    private Player player;

    // 初始化
    private void Start()
    {
        Services.Stage = this;            // 让“当前关”可被其他系统找到（可选）
        player = FindObjectOfType<Player>();
        // Services.Spawn?.Setup(config); // 如果 Spawn 放关卡里，这里拉起
    }

    // 每帧检查胜败条件
    private void Update()
    {
        if (endingShown) return;

        // 你的胜败条件（示例）
        bool playerDead = (player == null) || (player.CurrentHealth <= 0);
        bool stageCleared = CheckStageClear(); // TODO：按你们实际逻辑实现

        // 根据条件触发结局
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

    }

    // 失败结局，返回主菜单
    private void ShowFailEndingThenBackToMenu()
    {
        endingShown = true;

        if (Services.UI == null || config == null || config.failEndingUIPrefab == null)
        {
            Debug.LogError("StageManager: UI 或 config 未设置，直接返回主菜单。");
            Services.Game.LoadMainMenu();
            return;
        }

        Services.UI.ShowEndingUI(config.failEndingUIPrefab, () =>
        {
            Services.Game.LoadMainMenu();
        });
    }

    // 成功结局，返回主菜单
    private void ShowFinalEndingThenBackToMenu()
    {
        endingShown = true;
        if (Services.UI == null || config == null)
        {
            Services.Game.LoadMainMenu();
            return;
        }
        var ui = config.successEndingUIPrefab ? config.successEndingUIPrefab : config.failEndingUIPrefab;
        if (ui == null)
        {
            Services.Game.LoadMainMenu();
            return;
        }
        Services.UI.ShowEndingUI(ui, () => Services.Game.LoadMainMenu());
    }

    // 直接进入下一关
    private void GoNextLevel()
    {
        endingShown = true; // 防止二次触发
        Services.Game.OnStageClear(); // 直接切到下一关
    }

    // 你们的实际胜利条件
    private bool CheckStageClear()
    {
        // 例子：打死 Boss / 存活到计时 / 清空存活敌人
        // return Services.Spawn.AllObjectivesCleared();
        return false;
    }
}
