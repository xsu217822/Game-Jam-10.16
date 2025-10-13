using System.Collections;
using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    [SerializeField] private LevelConfig[] campaign;

    [Header("Services")]
    [SerializeField] private MonoBehaviour cutsceneSvcObj;   // 绑定 CutsceneManager
    [SerializeField] private MonoBehaviour envBuilderObj;    // 绑定 EnvironmentManager
    [SerializeField] private MonoBehaviour spawnerObj;       // 绑定 SpawnManager
    [SerializeField] private MonoBehaviour buildSvcObj;      // 绑定 BuildManager

    ICutsceneService cutscene;
    IEnvironmentBuilder env;
    ISpawner spawner;
    IBuildService build;

    Player player;

    private void Awake()
    {
        cutscene = (ICutsceneService)cutsceneSvcObj;
        env = (IEnvironmentBuilder)envBuilderObj;
        spawner = (ISpawner)spawnerObj;
        build = (IBuildService)buildSvcObj;
    }

    private void Start()
    {
        player = FindObjectOfType<Player>() ?? new GameObject("Player").AddComponent<Player>();
        StartCoroutine(RunCampaign());
    }

    private IEnumerator RunCampaign()
    {
        for (int i = 0; i < campaign.Length;)
        {
            var cur = campaign[i];
            var next = (i + 1 < campaign.Length) ? campaign[i + 1] : null;

            // 1) 关前剧情
            yield return cutscene.PlaySequence(cur.introCutscenes);

            // 2) 核心构筑（选装备）
            yield return build.DoCoreBuild(cur, player);

            // 3) 环境生成 + 刷怪初始化
            env.Build(cur);
            spawner.Init(cur, player);
            build.ResetSession(cur, player);

            // 订阅经验事件 → 小构筑阈值判断
            spawner.OnKillExp += exp => build.OnGainExp(exp, cur, player);

            // 4) 关卡循环
            bool fail = false, clear = false;
            float t = 0f;
            while (true)
            {
                t += Time.deltaTime;
                spawner.Tick(Time.deltaTime);

                // 小构筑由 build.OnGainExp 内部触发；也可以在这里让 build 查询阈值主动弹
                // 失败/胜利判定
                if (player == null || player.IsDead) { fail = true; break; }
                if (spawner.AllWavesDispatched && spawner.AllCleared) { clear = true; break; }

                yield return null;
            }

            spawner.OnKillExp -= exp => build.OnGainExp(exp, cur, player);

            if (fail)
            {
                // 失败剧情 → 回主菜单或重置
                yield return cutscene.PlaySequence(cur.failCutscenes);
                env.Clear();
                yield break;
            }

            // 通关
            if (cur.isFinalStage || next == null)
            {
                yield return cutscene.PlaySequence(cur.outroCutscenes);
                env.Clear();
                yield break;
            }

            // 中场：结尾剧情（可并行加载下一关，但单场景下直接复用即可）
            yield return cutscene.PlaySequence(cur.outroCutscenes);

            // 下一关开场剧情 + 核心构筑
            yield return cutscene.PlaySequence(next.introCutscenes);
            yield return build.DoCoreBuild(next, player);

            i++;
        }
    }
}
