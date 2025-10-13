// Assets/Scripts/BuildManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour, IBuildService
{
    [SerializeField] private Transform equipRoot; // 可为空：默认挂到玩家身上
    private readonly List<GameObject> equips = new();
    private int xpAccum = 0;
    private int thresholdIdx = 0;
    private bool microOpen = false;

    public IEnumerator DoCoreBuild(LevelConfig cfg, Player player)
    {
        var pool = cfg.coreEquipPool;
        if (pool == null || pool.Length == 0 || !cfg.coreBuildUIPrefab) yield break;

        var labels = new string[pool.Length];
        for (int i = 0; i < pool.Length; i++) labels[i] = pool[i]?.displayName ?? $"Equip {i + 1}";

        var ui = Instantiate(cfg.coreBuildUIPrefab);
        var chooser = ui.GetComponent<ChoiceUI>() ?? ui.AddComponent<ChoiceUI>();

        bool done = false;
        chooser.Open(labels, pick =>
        {
            var w = pool[Mathf.Clamp(pick, 0, pool.Length - 1)];
            if (w && w.prefab)
            {
                var parent = equipRoot ? equipRoot : player.transform;
                var go = Instantiate(w.prefab, parent);
                var eq = go.GetComponent<IEquipment>(); eq?.BindOwner(player);
                equips.Add(go);
            }
            done = true;
        });

        Time.timeScale = 0f;
        while (!done) yield return null;
        Time.timeScale = 1f;
    }

    public void ResetSession(LevelConfig cfg, Player player)
    {
        xpAccum = 0; thresholdIdx = 0; microOpen = false;
    }

    public void OnGainExp(int exp, LevelConfig cfg, Player player)
    {
        xpAccum += exp;
        if (microOpen) return;

        if (cfg.xpThresholds != null &&
            thresholdIdx < cfg.xpThresholds.Length &&
            xpAccum >= cfg.xpThresholds[thresholdIdx])
        {
            thresholdIdx++;
            // 弹一次小构筑
            player.StartCoroutine(DoMicroBuild(cfg, player));
        }
    }
}
