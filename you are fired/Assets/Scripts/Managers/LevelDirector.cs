// Assets/Scripts/LevelDirector.cs
using System.Collections;                    // 协程
using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    [Header("本轮要跑的关卡（按顺序）")]
    [SerializeField] private LevelConfig[] campaign;

    [Header("服务实现（在 Inspector 里拖组件）")]
    [SerializeField] private MonoBehaviour cutsceneSvcObj;   // CutsceneManager
    [SerializeField] private MonoBehaviour envBuilderObj;    // EnvironmentManager
    [SerializeField] private MonoBehaviour spawnerObj;       // SpawnManager
    [SerializeField] private MonoBehaviour buildSvcObj;      // BuildManager

    [Header("通关/失败后的去向")]
    [SerializeField] private bool returnToMenuOnEnd = false;  // 结局/失败后回主菜单

    // 接口引用（只依赖接口，不依赖具体类）
    private ICutsceneService cutscene;
    private IEnvironmentBuilder env;
    private ISpawner spawner;
    private IBuildService build;

    private Player player;

    private void Awake()
    {
        // 把 MonoBehaviour 强转为接口；缺哪个就报错防止空引用
        cutscene = cutsceneSvcObj as ICutsceneService;
        env = envBuilderObj as IEnvironmentBuilder;
        spawner = spawnerObj as ISpawner;
        build = buildSvcObj as IBuildService;

        if (cutscene == null || env == null || spawner == null || build == null)
        {
            Debug.LogError("LevelDirector: 服务对象未正确绑定到接口（检查四个 *_Obj 引用）。");
        }
    }

    private void Start()
    {
        if (campaign == null || campaign.Length == 0)
        {
            Debug.LogError("LevelDirector: campaign 为空，无法开跑。");
            return;
        }

        player = FindObjectOfType<Player>() ?? new GameObject("Player").AddComponent<Player>();
        StartCoroutine(RunCampaign());
    }

    private IEnumerator RunCampaign()
    {
        for (int i = 0; i < campaign.Length;)
        {
            var cur = campaign[i];
            var next = (i + 1 < campaign.Length) ? campaign[i + 1] : null;

            // —— 1) 关前剧情 ——
            if (cur.introCutscenes != null && cur.introCutscenes.Length > 0)
                yield return cutscene.PlaySequence(cur.introCutscenes);

            // —— 2) 核心构筑（选装备） ——
            yield return build.DoCoreBuild(cur, player);

            // —— 3) 环境生成 + 刷怪初始化 + 构筑会话重置 ——
            env.Build(cur);
            spawner.Init(cur, player.transform);       // 关键：传 Transform
            build.ResetSession(cur, player);

            // 订阅经验事件（保存委托以便解绑）
            System.Action<int> expHandler = null;
            expHandler = exp => build.OnGainExp(exp, cur, player);
            spawner.OnKillExp += expHandler;

            // —— 4) 关卡循环（胜/负判定） ——
            bool fail = false, clear = false;
            while (true)
            {
                spawner.Tick(Time.deltaTime);

                if (player == null || player.IsDead) { fail = true; break; }
                if (spawner.AllWavesDispatched && spawner.AllCleared) { clear = true; break; }

                yield return null;
            }

            // 解绑经验事件
            spawner.OnKillExp -= expHandler;

            if (fail)
            {
                // 失败剧情
                if (cur.failCutscenes != null && cur.failCutscenes.Length > 0)
                    yield return cutscene.PlaySequence(cur.failCutscenes);

                env.Clear();

                if (returnToMenuOnEnd && GameManager.I != null)
                    GameManager.I.LoadMainMenu();
                yield break;
            }

            // —— 通关分支 ——
            if (cur.isFinalStage || next == null)
            {
                if (cur.outroCutscenes != null && cur.outroCutscenes.Length > 0)
                    yield return cutscene.PlaySequence(cur.outroCutscenes);

                env.Clear();

                if (returnToMenuOnEnd && GameManager.I != null)
                    GameManager.I.LoadMainMenu();
                yield break;
            }

            // —— 5) 过场 + 下一关开场 —— 
            if (cur.outroCutscenes != null && cur.outroCutscenes.Length > 0)
                yield return cutscene.PlaySequence(cur.outroCutscenes);

            if (next.introCutscenes != null && next.introCutscenes.Length > 0)
                yield return cutscene.PlaySequence(next.introCutscenes);

            // 下一关开打前再次给一轮核心构筑（如果你不想每关都给，可以注释掉）
            yield return build.DoCoreBuild(next, player);

            // 进入下一关
            i++;
        }
    }
}
