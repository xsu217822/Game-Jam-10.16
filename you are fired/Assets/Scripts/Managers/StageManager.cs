using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private StageConfig config;

    private bool endingShown = false;
    private Player player;
    [SerializeField] private GameObject pauseMenuUI; // ָ�������ͣ�˵�UI
    private bool isPaused = false;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        // ��������ؿ���ʼ����ˢ�� Setup ��
    }

    private void Update()
    {
        if (endingShown) return;

        // ���ʤ��������ʾ����
        bool playerDead = (player == null) || (player.CurrentHealth <= 0);
        bool stageCleared = CheckStageClear(); // TODO��������ʵ���߼�ʵ��

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
                                                      : config.failEndingUIPrefab; // ����
        Services.UI.ShowEndingUI(ui, () =>
        {
            Services.Game.LoadMainMenu();
        });
    }

    private void GoNextLevel()
    {
        endingShown = true; // ��ֹ���δ���
        Services.Game.OnStageClear(); // ֱ���е���һ��
    }

    private bool CheckStageClear()
    {
        // ���ӣ����� Boss / ����ʱ / ��մ�����
        // return Services.Spawn.AllObjectivesCleared();
        return false;
    }
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);   // ��ʾ��ͣ�˵�
        Time.timeScale = 0f;           // ��ͣ��Ϸ
        isPaused = true;
        Cursor.lockState = CursorLockMode.None; // ������꣨�������������
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);  // �ر���ͣ�˵�
        Time.timeScale = 1f;           // �ָ���Ϸ
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked; // ����������꣨���裩
        Cursor.visible = false;
    }
}
