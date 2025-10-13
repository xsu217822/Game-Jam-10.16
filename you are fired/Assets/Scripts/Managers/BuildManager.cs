// Assets/Scripts/BuildManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 构筑系统总管：
/// - 核心构筑：每关开头“选装备”（实例化装备并绑定玩家）
/// - 小构筑：达到经验阈值时弹出“选加成”，把加成应用到当前所有装备
/// 依赖：ChoiceUI、WeaponData(IEquipment/IEquipmentMod)、MicroModData、LevelConfig、Player
/// </summary>
public class BuildManager : MonoBehaviour, IBuildService
{
    [Header("把装备挂到哪里（可留空=挂到玩家身上）")]
    [SerializeField] private Transform equipRoot;

    // 装备实例（玩家当前持有）
    private readonly List<GameObject> equips = new();

    // 小构筑的运行期状态
    private int xpAccum = 0;       // 当前累计经验
    private int thresholdIdx = 0;  // 已经触发到第几个阈值
    private bool microOpen = false; // 小构筑UI是否正在打开（防重入）

    /// <summary>
    /// 核心构筑：从 LevelConfig.coreEquipPool 中多选一，实例化装备并绑定到玩家
    /// </summary>
    public IEnumerator DoCoreBuild(LevelConfig cfg, Player player)
    {
        var pool = cfg.coreEquipPool;
        if (pool == null || pool.Length == 0 || !cfg.coreBuildUIPrefab) yield break;

        // 选项名称
        var labels = new string[pool.Length];
        for (int i = 0; i < pool.Length; i++)
            labels[i] = pool[i]?.displayName ?? $"Equip {i + 1}";

        // 打开 UI
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
                // 绑定到玩家
                var eq = go.GetComponent<IEquipment>();
                eq?.BindOwner(player);
                equips.Add(go);
            }
            done = true;
        });

        // 暂停时间直到选择完成
        Time.timeScale = 0f;
        while (!done) yield return null;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 每关开始重置小构筑的计数与状态（不清空已有装备）
    /// </summary>
    public void ResetSession(LevelConfig cfg, Player player)
    {
        xpAccum = 0;
        thresholdIdx = 0;
        microOpen = false;
        // 清理已被销毁的装备引用（安全起见）
        for (int i = equips.Count - 1; i >= 0; i--)
            if (!equips[i]) equips.RemoveAt(i);
    }

    /// <summary>
    /// 由刷怪系统驱动：获得经验时调用。若达到阈值，触发一次小构筑。
    /// </summary>
    public void OnGainExp(int exp, LevelConfig cfg, Player player)
    {
        xpAccum += Mathf.Max(0, exp);
        if (microOpen) return; // UI 正在开，先不叠触发

        var th = cfg.xpThresholds;
        if (th != null && thresholdIdx < th.Length && xpAccum >= th[thresholdIdx])
        {
            thresholdIdx++;
            // 触发一次小构筑
            StartCoroutine(DoMicroBuild(cfg, player));
        }
    }

    /// <summary>
    /// 小构筑：从 LevelConfig.microPool 多选一，把加成应用到所有当前装备（实现了 IEquipmentMod 的）
    /// </summary>
    public IEnumerator DoMicroBuild(LevelConfig cfg, Player player)
    {
        var pool = cfg.microPool;
        if (pool == null || pool.Length == 0 || !cfg.microBuildUIPrefab) yield break;

        microOpen = true;

        // 选项名称
        var labels = new string[pool.Length];
        for (int i = 0; i < pool.Length; i++)
            labels[i] = pool[i]?.displayName ?? $"Mod {i + 1}";

        // 打开 UI
        var ui = Instantiate(cfg.microBuildUIPrefab);
        var chooser = ui.GetComponent<ChoiceUI>() ?? ui.AddComponent<ChoiceUI>();

        bool done = false;
        chooser.Open(labels, pick =>
        {
            var idx = Mathf.Clamp(pick, 0, pool.Length - 1);
            var mod = pool[idx];
            // 应用到当前所有装备
            for (int i = equips.Count - 1; i >= 0; i--)
            {
                var go = equips[i];
                if (!go) { equips.RemoveAt(i); continue; }
                var modder = go.GetComponent<IEquipmentMod>();
                modder?.ApplyMod(mod);
            }
            done = true;
        });

        // 暂停时间直到选择完成
        Time.timeScale = 0f;
        while (!done) yield return null;
        Time.timeScale = 1f;

        microOpen = false;
    }

    // —— 可选：对外只读访问当前装备（调试/展示用）——
    public IReadOnlyList<GameObject> CurrentEquips => equips;
}
