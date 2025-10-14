// Assets/Scripts/LevelDirector.cs
using System;                            // Added for Action / events
using System.Collections;               // Э��
using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    [Header("����Ҫ�ܵĹؿ�����˳��")]
    [SerializeField] private LevelConfig[] campaign;

    [Header("����ʵ�֣��� Inspector ���������")]
    [SerializeField] private MonoBehaviour cutsceneSvcObj;   // CutsceneManager
    [SerializeField] private MonoBehaviour envBuilderObj;    // EnvironmentManager
    [SerializeField] private MonoBehaviour spawnerObj;       // SpawnManager
    [SerializeField] private MonoBehaviour buildSvcObj;      // BuildManager

    [Header("ͨ��/ʧ�ܺ��ȥ��")]
    [SerializeField] private bool returnToMenuOnEnd = false;  // ���/ʧ�ܺ�����˵�

    // �ӿ����ã�ֻ�����ӿڣ������������ࣩ
    private ICutsceneService cutscene;
    private IEnvironmentBuilder env;
    private ISpawner spawner;
    private IBuildService build;

    private Player player;

    // Exposed current level info
    public LevelConfig CurrentLevel { get; private set; }
    public int CurrentLevelIndex { get; private set; } = -1;
    public event Action<LevelConfig, int> OnLevelChanged;

    // �ؼ�������������ʧ�ܣ�ֱ�ӽ��ýű���������� NullReference
    private void Awake()
    {
        cutscene = cutsceneSvcObj as ICutsceneService;
        env = envBuilderObj as IEnvironmentBuilder;
        spawner = spawnerObj as ISpawner;
        build = buildSvcObj as IBuildService;

        if (cutscene == null || env == null || spawner == null || build == null)
        {
            Debug.LogError("LevelDirector: �������δ��ȷ�󶨵��ӿڣ�����ĸ� *_Obj ���ã���");
            enabled = false; // ��ֹ����Э��ʹ�õ�������
        }
    }

    private void Start()
    {
        if (campaign == null || campaign.Length == 0)
        {
            Debug.LogError("LevelDirector: campaign Ϊ�գ��޷����ܡ�");
            return;
        }

        player = FindObjectOfType<Player>() ?? new GameObject("Player").AddComponent<Player>();
        StartCoroutine(RunCampaign());
    }

    private IEnumerator RunCampaign()
    {
        for (int i = 0; i < campaign.Length;)
        {
            // Update current level state & notify listeners (e.g. PauseManager)
            CurrentLevelIndex = i;
            CurrentLevel = campaign[i];
            OnLevelChanged?.Invoke(CurrentLevel, CurrentLevelIndex);

            var cur = CurrentLevel;
            var next = (i + 1 < campaign.Length) ? campaign[i + 1] : null;

            // ���� 1) ��ǰ���� ����
            if (cur.introCutscenes != null && cur.introCutscenes.Length > 0)
                yield return cutscene.PlaySequence(cur.introCutscenes);

            // ���� 2) ���Ĺ�����ѡװ���� ����
            yield return build.DoCoreBuild(cur, player);

            // ���� 3) �������� + ˢ�ֳ�ʼ�� + �����Ự���� ����
            env.Build(cur);
            spawner.Init(cur, player.transform);       // �ؼ����� Transform
            build.ResetSession(cur, player);

            // ���ľ����¼�������ί���Ա���
            System.Action<int> expHandler = null;
            expHandler = exp => build.OnGainExp(exp, cur, player);
            spawner.OnKillExp += expHandler;

            // ���� 4) �ؿ�ѭ����ʤ/���ж��� ����
            bool fail = false, clear = false;
            while (true)
            {
                spawner.Tick(Time.deltaTime);

                if (player == null || player.IsDead) { fail = true; break; }
                if (spawner.AllWavesDispatched && spawner.AllCleared) { clear = true; break; }

                yield return null;
            }

            // ������¼�
            spawner.OnKillExp -= expHandler;

            if (fail)
            {
                // ʧ�ܾ���
                if (cur.failCutscenes != null && cur.failCutscenes.Length > 0)
                    yield return cutscene.PlaySequence(cur.failCutscenes);

                env.Clear();

                if (returnToMenuOnEnd && GameManager.I != null)
                    GameManager.I.LoadMainMenu();
                yield break;
            }

            // ���� ͨ�ط�֧ ����
            if (cur.isFinalStage || next == null)
            {
                if (cur.outroCutscenes != null && cur.outroCutscenes.Length > 0)
                    yield return cutscene.PlaySequence(cur.outroCutscenes);

                env.Clear();

                if (returnToMenuOnEnd && GameManager.I != null)
                    GameManager.I.LoadMainMenu();
                yield break;
            }

            // ���� 5) ���� + ��һ�ؿ��� ���� 
            if (cur.outroCutscenes != null && cur.outroCutscenes.Length > 0)
                yield return cutscene.PlaySequence(cur.outroCutscenes);

            if (next.introCutscenes != null && next.introCutscenes.Length > 0)
                yield return cutscene.PlaySequence(next.introCutscenes);

            // ��һ�ؿ���ǰ�ٴθ�һ�ֺ��Ĺ���������㲻��ÿ�ض���������ע�͵��
            yield return build.DoCoreBuild(next, player);

            // ������һ��
            i++;
        }
    }
}
