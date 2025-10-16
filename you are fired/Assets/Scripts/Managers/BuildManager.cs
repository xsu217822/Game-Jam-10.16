// Assets/Scripts/BuildManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 构筑系统总管：
/// - 核心构筑：每关开头“两次选择”（近战一次、远程一次），实例化装备并绑定玩家
/// - 小构筑：达到经验阈值时弹出“选加成”，把加成仅应用到【本关所选】的装备组
/// - 保留策略：所有关卡已选择的武器会一直保留到通关（总共4关=8件）
/// - 运行期状态：经验、阈值索引、【本关武器组】列表、全部装备列表
/// </summary>
public class BuildManager : MonoBehaviour, IBuildService
{
    [Header("把装备挂到哪里（空=挂到玩家身上）")]
    [SerializeField] private Transform equipRoot;

    // 全部装备（跨关累计）
    private readonly List<GameObject> allEquips = new();

    // 本关装备（仅本关构筑作用的目标集合）
    private readonly List<GameObject> currentLevelEquips = new();

    // 小构筑的运行期状态
    private int xpAccum = 0;       // 当前关累计经验
    private int thresholdIdx = 0;  // 当前关已触发到第几个阈值
    private bool microOpen = false;

    public IEnumerator DoCoreBuild(LevelConfig cfg, Player player)
    {
        // 每关开始：清空“本关装备组”列表（但不清历史装备）
        currentLevelEquips.Clear();

        // 依次选择：近战 → 远程
        yield return PickFromPool(cfg.coreMeleePool, cfg, player, currentLevelEquips);
        yield return PickFromPool(cfg.coreRangedPool, cfg, player, currentLevelEquips);

        // 选完后，本关两件已加入 currentLevelEquips & allEquips
        // 后续小构筑仅针对 currentLevelEquips 应用
    }

    private IEnumerator PickFromPool(WeaponData[] pool, LevelConfig cfg, Player player, List<GameObject> levelEquipCollector)
    {
        if (pool == null || pool.Length == 0 || !cfg.coreBuildUIPrefab) yield break;

        var labels = new string[pool.Length];
        for (int i = 0; i < pool.Length; i++)
            labels[i] = pool[i]?.displayName ?? $"Equip {i + 1}";

        var ui = Instantiate(cfg.coreBuildUIPrefab);
        var chooser = ui.GetComponent<ChoiceUI>() ?? ui.AddComponent<ChoiceUI>();

        bool done = false;
        chooser.Open(labels, pick =>
        {
            var idx = Mathf.Clamp(pick, 0, pool.Length - 1);
            var w = pool[idx];
            if (w && w.prefab)
            {
                var parent = equipRoot ? equipRoot : player.transform;
                var go = Instantiate(w.prefab, parent);
                var eq = go.GetComponent<IEquipment>();
                eq?.BindOwner(player);

                // 计入“全部装备”与“本关装备”
                allEquips.Add(go);
                levelEquipCollector.Add(go);
            }
            done = true;
        });

        Time.timeScale = 0f;
        while (!done) yield return null;
        Time.timeScale = 1f;
    }

    public void ResetSession(LevelConfig cfg, Player player)
    {
        // 只重置小构筑运行期数据；不清“本关装备组”（它由 DoCoreBuild 重建）
        xpAccum = 0;
        thresholdIdx = 0;
        microOpen = false;

        // 清理已被销毁的引用（全局与本关）
        for (int i = allEquips.Count - 1; i >= 0; i--)
            if (!allEquips[i]) allEquips.RemoveAt(i);
        for (int i = currentLevelEquips.Count - 1; i >= 0; i--)
            if (!currentLevelEquips[i]) currentLevelEquips.RemoveAt(i);
    }

    public void OnGainExp(int exp, LevelConfig cfg, Player player)
    {
        xpAccum += Mathf.Max(0, exp);
        if (microOpen) return;

        var th = cfg.xpThresholds;
        if (th != null && thresholdIdx < th.Length && xpAccum >= th[thresholdIdx])
        {
            thresholdIdx++;
            StartCoroutine(DoMicroBuild(cfg, player)); // 仅作用 currentLevelEquips
        }
    }

    public IEnumerator DoMicroBuild(LevelConfig cfg, Player player)
    {
        var pool = cfg.microPool;
        if (pool == null || pool.Length == 0 || !cfg.microBuildUIPrefab) yield break;

        microOpen = true;

        var labels = new string[pool.Length];
        for (int i = 0; i < pool.Length; i++)
            labels[i] = pool[i]?.displayName ?? $"Mod {i + 1}";

        var ui = Instantiate(cfg.microBuildUIPrefab);
        var chooser = ui.GetComponent<ChoiceUI>() ?? ui.AddComponent<ChoiceUI>();

        bool done = false;
        chooser.Open(labels, pick =>
        {
            var idx = Mathf.Clamp(pick, 0, pool.Length - 1);
            var mod = pool[idx];

            // ——关键：只对“本关两件武器”应用Mod——
            for (int i = currentLevelEquips.Count - 1; i >= 0; i--)
            {
                var go = currentLevelEquips[i];
                if (!go) { currentLevelEquips.RemoveAt(i); continue; }
                var modder = go.GetComponent<IEquipmentMod>();
                modder?.ApplyMod(mod);
            }
            done = true;
        });

        Time.timeScale = 0f;
        while (!done) yield return null;
        Time.timeScale = 1f;

        microOpen = false;
    }

    // 调试/展示用
    public IReadOnlyList<GameObject> AllEquips => allEquips;
    public IReadOnlyList<GameObject> CurrentLevelEquips => currentLevelEquips;
}
