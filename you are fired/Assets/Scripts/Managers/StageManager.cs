using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private StageConfig config;

    private bool endingShown = false;
    private Player player;

    // ��ʼ��
    private void Start()
    {
        Services.Stage = this;            // �á���ǰ�ء��ɱ�����ϵͳ�ҵ�����ѡ��
        player = FindObjectOfType<Player>();
        // Services.Spawn?.Setup(config); // ��� Spawn �Źؿ����������
    }

    // ÿ֡���ʤ������
    private void Update()
    {
        if (endingShown) return;

        // ���ʤ��������ʾ����
        bool playerDead = (player == null) || (player.CurrentHealth <= 0);
        bool stageCleared = CheckStageClear(); // TODO��������ʵ���߼�ʵ��

        // ���������������
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

    // ʧ�ܽ�֣��������˵�
    private void ShowFailEndingThenBackToMenu()
    {
        endingShown = true;

        if (Services.UI == null || config == null || config.failEndingUIPrefab == null)
        {
            Debug.LogError("StageManager: UI �� config δ���ã�ֱ�ӷ������˵���");
            Services.Game.LoadMainMenu();
            return;
        }

        Services.UI.ShowEndingUI(config.failEndingUIPrefab, () =>
        {
            Services.Game.LoadMainMenu();
        });
    }

    // �ɹ���֣��������˵�
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

    // ֱ�ӽ�����һ��
    private void GoNextLevel()
    {
        endingShown = true; // ��ֹ���δ���
        Services.Game.OnStageClear(); // ֱ���е���һ��
    }

    // ���ǵ�ʵ��ʤ������
    private bool CheckStageClear()
    {
        // ���ӣ����� Boss / ����ʱ / ��մ�����
        // return Services.Spawn.AllObjectivesCleared();
        return false;
    }
}
