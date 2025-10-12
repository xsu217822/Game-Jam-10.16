using UnityEngine;

public class SystemRoot : MonoBehaviour
{
    [SerializeField] private GameManager game;
    [SerializeField] private UIManager ui;
    [SerializeField] private AudioManager audioMgr;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Services.Game = game;
        Services.UI = ui;
        Services.Audio = audioMgr;
    }

    private void Start()
    {
        // ������Ϸ��һ�أ��������
        //Services.Game.BootToFirstStage();
    }
}
