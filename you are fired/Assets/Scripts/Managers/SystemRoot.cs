using UnityEngine;

public class SystemRoot : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private StageManager stage;
    [SerializeField] private SpawnManager spawn;
    [SerializeField] private UIManager ui;
    [SerializeField] private AudioManager audioMgr;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Services.Game = game;
        Services.Stage = stage;
        Services.Spawn = spawn;
        Services.UI = ui;
        Services.Audio = audioMgr;
    }

    private void Start()
    {
        // 进入游戏第一关（或读档）
        //Services.Game.BootToFirstStage();
    }
}
