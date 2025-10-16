// Assets/Scripts/LevelDirector.cs
//
// 【角色定位】
// LevelDirector 是整局（campaign）关卡流程的唯一“导演”（流程编排器）。
// ——它不做“具体工作”（不直接播剧情、不直接造环境、不直接生怪、不直接做构筑）
// ——它只“调用服务”（Cutscene/Environment/Enemy/Build）并决定“下一步是啥”。
// ——它在 Update() 中以一个小型状态机（Phase）推进“当前关卡”的全流程。
// 
// 【流程摘要（按相位 Phase）】
// 1) LevelIntro    : 开场剧情（优先页式pages，其次prefabs）
// 2) CoreBuild     : 本关“核心构筑选择”——两次选择：近战一次 + 远程一次（由 BuildManager 执行）
// 3) EnvBuild      : 生成环境（EnvManager），初始化敌人与本关构筑运行态（EnemyManager/BuildManager）
// 4) Combat        : 战斗循环（每帧Tick），同时监听：玩家死亡 / 敌人清空
// 5) FailCutscene  : 如果玩家死了，播失败剧情
// 6) Outro         : 如果清掉所有波次并且场上敌人清空，播本关收尾剧情
// 7) BetweenLevels : 清理当前关环境（不动玩家身上的武器），进入下一关或收尾
// 8) CampaignEnd   : 所有关卡完结，回主菜单（可配置）
//
// 【事件/数据流】
// EnemyManager.OnKillExp(exp) → LevelDirector.OnKill(exp) → BuildManager.OnGainExp(exp,...)
// BuildManager 内部达阈值会弹“小构筑UI”，只影响“本关已选的两件武器”。
//
// 【注意点】
// - busy 标志位用于“等待协程型服务完成”（如剧情/选择UI）。busy=true 时 Update() 不推进状态机。
// - 进入 EnvBuild 时订阅敌人击杀事件；在 FailCutscene/Outro 处记得退订，防止重复订阅。
// - 回主菜单时只通过 GameManager.I 调，LevelDirector 不持有对 GameManager 的序列化引用。
// - 跨关保留：环境会清，玩家身上武器不清（BuildManager 负责保留逻辑）。
//
using System;
using UnityEngine;

public class LevelDirector : MonoBehaviour
{
    [Header("四个关卡（Inspector里挂满）")]
    [SerializeField] private LevelConfig[] campaign;
    // ↑ 整个流程要打的关卡列表（通常为4个，但不限）。按索引顺序依次进行。
    //   每个 LevelConfig 是“关卡数据包”（环境/波次/剧情/构筑池/阈值等）。

    [Header("服务实现（拖引用到这四个上）")]
    [SerializeField] private MonoBehaviour cutsceneSvcObj; // CutsceneManager
    [SerializeField] private MonoBehaviour envBuilderObj;  // EnvironmentManager
    [SerializeField] private MonoBehaviour enemyMgrObj;    // EnemyManager
    [SerializeField] private MonoBehaviour buildSvcObj;    // BuildManager
    // ↑ 这里用 MonoBehaviour 接，然后在 Awake 里转成接口（解耦）。
    //   这样 LevelDirector 只依赖接口（ICutsceneService/IEnvironmentBuilder/IEnemyManager/IBuildService）。

    [Header("打完或失败是否回主菜单")]
    [SerializeField] private bool returnToMenuOnEnd = true;
    // ↑ 全流程结束（通关 or 失败）是否自动回主菜单。

    // 运行期通过接口与具体服务交互
    private ICutsceneService cutscene;
    private IEnvironmentBuilder env;
    private IEnemyManager enemy;
    private IBuildService build;

    // 对玩家的引用（用于把武器挂上、给敌人AI一个追踪目标等）
    private Player player;

    // 只读公开：当前关卡与索引（PauseManager 等系统可用）
    public LevelConfig CurrentLevel { get; private set; }
    public int CurrentLevelIndex { get; private set; } = -1;

    // 外抛事件：当关卡切换时通知（PauseManager 用于切换不同关的暂停UI）
    public event Action<LevelConfig, int> OnLevelChanged;

    // —— 内部状态机的所有相位 —— //
    enum Phase
    {
        Idle,           // 空转（一般不会长时间停在这里）
        LevelIntro,     // 开场剧情
        CoreBuild,      // 本关两次选择（近战+远程）
        EnvBuild,       // 生成环境、初始化敌人与构筑会话态
        Combat,         // 战斗循环（Tick 敌人管理器）
        FailCutscene,   // 失败剧情
        Outro,          // 收尾剧情（本关最后一波打完后）
        BetweenLevels,  // 清理当前关，跳转下一关
        CampaignEnd     // 全流程完结，回主菜单或停在Idle
    }

    // 当前相位
    private Phase phase = Phase.Idle;

    // busy=true 表示当前在等待一个协程型服务（剧情/选择UI）结束；等待期间不推进状态机
    private bool busy;

    // 标记 Combat 是否真的清完最后一波（用于进入 Outro 的合理性判断）
    private bool lastWaveCleared;

    private void Awake()
    {
        // 将序列化对象转成接口引用。若没配好会直接报错并停用本脚本。
        cutscene = cutsceneSvcObj as ICutsceneService;
        env = envBuilderObj as IEnvironmentBuilder;
        enemy = enemyMgrObj as IEnemyManager;
        build = buildSvcObj as IBuildService;

        if (cutscene == null || env == null || enemy == null || build == null)
        {
            Debug.LogError("LevelDirector: 服务未正确绑定。");
            enabled = false; // 禁用自己，避免后续 NullReference
        }
    }

    private void Start()
    {
        // 基础容错：campaign 为空就直接报错退出
        if (campaign == null || campaign.Length == 0)
        {
            Debug.LogError("LevelDirector: campaign 为空。");
            return;
        }

        // 找 Player：场景内没有就临时创建一个（最小可跑保障）
        player = FindObjectOfType<Player>() ?? new GameObject("Player").AddComponent<Player>();

        // 进入第 0 关（会触发 OnLevelChanged）
        GoToLevel(0);
    }

    private void Update()
    {
        // 忙于等待协程 或者 当前关卡为空 → 不推进状态机
        if (busy || CurrentLevel == null) return;

        // 核心小状态机：每帧推进一次
        switch (phase)
        {
            case Phase.LevelIntro:
                // —— 播开场剧情：优先 pages（淡入淡出+打字机），否则退回 prefab 序列 ——
                if (CurrentLevel.introPages != null && CurrentLevel.introPages.Length > 0)
                    StartRoutine(cutscene.PlayPages(CurrentLevel.introPages), Phase.CoreBuild);
                else if (CurrentLevel.introCutscenes != null && CurrentLevel.introCutscenes.Length > 0)
                    StartRoutine(cutscene.PlaySequence(CurrentLevel.introCutscenes), Phase.CoreBuild);
                else
                    phase = Phase.CoreBuild; // 没有配置剧情则直接进入核心构筑
                break;

            case Phase.CoreBuild:
                // —— 本关核心构筑：两次选择（近战一次 + 远程一次）——
                // 由 BuildManager 弹 ChoiceUI、实例化武器到 Player、记录“本关武器组/全部武器”
                StartRoutine(build.DoCoreBuild(CurrentLevel, player), Phase.EnvBuild);
                break;

            case Phase.EnvBuild:
                // —— 生成环境（地板/墙/角/排序等） ——
                env.Build(CurrentLevel);

                // —— 初始化敌人管理器：装载波次、绑定玩家Transform —— 
                enemy.Init(CurrentLevel, player.transform);

                // —— 重置“本关构筑”的运行态（经验计数/阈值索引/弹窗占用标志等）——
                //     注意：不会清历史武器；“本关武器组”由 DoCoreBuild 刚刚重建过
                build.ResetSession(CurrentLevel, player);

                // —— 订阅击杀经验事件：防止重复订阅，先退订再订阅 ——
                enemy.OnKillExp -= OnKill;
                enemy.OnKillExp += OnKill;

                // —— 清零“本关最后一波已清空”标志，进入战斗循环 ——
                lastWaveCleared = false;
                phase = Phase.Combat;
                break;

            case Phase.Combat:
                // —— 推进敌人进程：按波次时间发怪、维护存活列表、回收死亡对象 ——
                enemy.Tick(Time.deltaTime);

                // —— 玩家死亡：切入失败剧情 ——
                if (player == null || player.IsDead)
                {
                    phase = Phase.FailCutscene;
                    break;
                }

                // —— 所有波次都已派发 且 场上已无敌：进入收尾剧情 —— 
                if (enemy.AllWavesDispatched && enemy.AllCleared)
                {
                    lastWaveCleared = true;
                    phase = Phase.Outro;
                    break;
                }

                // 其它情况：继续战斗
                break;

            case Phase.FailCutscene:
                // —— 防止事件泄漏：退订击杀经验 —— 
                enemy.OnKillExp -= OnKill;

                // —— 播失败剧情（优先 pages，然后 prefabs） → 进入流程末尾 —— 
                if (CurrentLevel.failPages != null && CurrentLevel.failPages.Length > 0)
                    StartRoutine(cutscene.PlayPages(CurrentLevel.failPages), Phase.CampaignEnd);
                else if (CurrentLevel.failCutscenes != null && CurrentLevel.failCutscenes.Length > 0)
                    StartRoutine(cutscene.PlaySequence(CurrentLevel.failCutscenes), Phase.CampaignEnd);
                else
                    phase = Phase.CampaignEnd;
                break;

            case Phase.Outro:
                // —— 防止事件泄漏：退订击杀经验 —— 
                enemy.OnKillExp -= OnKill;

                // —— 容错：若并非“真正清关”，就继续战斗（正常不会触发） —— 
                if (!lastWaveCleared)
                {
                    phase = Phase.Combat;
                    break;
                }

                // —— 播本关收尾剧情（优先 pages，然后 prefabs），结束后进入“关间处理” —— 
                if (CurrentLevel.outroPages != null && CurrentLevel.outroPages.Length > 0)
                    StartRoutine(cutscene.PlayPages(CurrentLevel.outroPages), Phase.BetweenLevels);
                else if (CurrentLevel.outroCutscenes != null && CurrentLevel.outroCutscenes.Length > 0)
                    StartRoutine(cutscene.PlaySequence(CurrentLevel.outroCutscenes), Phase.BetweenLevels);
                else
                    phase = Phase.BetweenLevels;
                break;

            case Phase.BetweenLevels:
                // —— 清理当前关场景（只清环境，不动玩家与其身上的武器） —— 
                env.Clear();

                // —— 计算下一关索引；若没有下一关或本关被标记为最终关 → 结束流程 —— 
                int next = CurrentLevelIndex + 1;
                if (next >= campaign.Length || CurrentLevel.isFinalStage)
                    phase = Phase.CampaignEnd;
                else
                    GoToLevel(next); // 切换至下一关：会触发 OnLevelChanged，并进入 LevelIntro
                break;

            case Phase.CampaignEnd:
                // —— 全流程收尾：清环境（保险起见），按配置回主菜单或停在 Idle —— 
                env.Clear();
                if (returnToMenuOnEnd && GameManager.I != null)
                    GameManager.I.LoadMainMenu();
                else
                    phase = Phase.Idle;
                break;

            case Phase.Idle:
            default:
                // 空转：通常不会停留，除非流程被外界暂停于此
                break;
        }
    }

    /// <summary>
    /// 敌人死亡时的经验回调（由 EnemyManager 触发）：
    /// 这里把经验转交给 BuildManager，让它做经验累计与小构筑触发判断。
    /// 注意：小构筑仅作用于“本关已选的两件武器”，该集合由 BuildManager 维护。
    /// </summary>
    private void OnKill(int exp) => build.OnGainExp(exp, CurrentLevel, player);

    /// <summary>
    /// 切换到指定关卡索引：
    /// - 更新 CurrentLevel/CurrentLevelIndex
    /// - 通知外部监听者（如 PauseManager）OnLevelChanged
    /// - 进入 LevelIntro 相位
    /// </summary>
    private void GoToLevel(int idx)
    {
        CurrentLevelIndex = idx;
        CurrentLevel = campaign[idx];

        // 通知外部（暂停菜单不同皮肤、关卡名显示等都可以在这做）
        OnLevelChanged?.Invoke(CurrentLevel, CurrentLevelIndex);

        // 下一帧开始播开场
        phase = Phase.LevelIntro;
    }

    /// <summary>
    /// 启动一个协程，并在其结束后切换到指定相位。
    /// 同时将 busy=true，阻止 Update 推进状态机，直至协程完成。
    /// </summary>
    private void StartRoutine(System.Collections.IEnumerator itor, Phase nextPhase)
    {
        busy = true;
        StartCoroutine(Wrap(itor, nextPhase));
    }

    /// <summary>
    /// 协程包装：等待 itor 跑完 → busy=false → 跳到 nextPhase。
    /// 用这个统一处理“剧情播放/选择UI”等需要帧间等待的步骤。
    /// </summary>
    private System.Collections.IEnumerator Wrap(System.Collections.IEnumerator itor, Phase nextPhase)
    {
        yield return itor;   // 等待服务协程结束（如 PlayPages / DoCoreBuild）
        busy = false;        // 放行状态机
        phase = nextPhase;   // 跳转到目标相位（由 Update 推进后续逻辑）
    }
}
