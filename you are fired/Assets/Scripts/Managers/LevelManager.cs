// Assets/Scripts/Runtime/LevelManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("关卡列表（四关）")]
    [SerializeField] private LevelConfig[] campaign;

    [Header("场景引用（不用Tilemap）")]
    [SerializeField] private Transform tileRoot; // 空物体即可

    // === 运行时 ===
    private int levelIdx;
    private Player player;
    private readonly List<GameObject> tilePool = new();
    private int tileUsed;
    private readonly HashSet<LevelConfig.Wave> fired = new();
    private readonly List<Enemy> alive = new();
    private float elapsed;
    private bool levelRunning;

    // 经验/小构筑
    private int xp = 0;
    private int xpTier = 0; // 到第几个阈值了（用 LevelConfig.xpThresholds）

    // 装备（核心构筑）
    private readonly List<GameObject> equips = new(); // 实例化后的装备对象(挂IEquipment/IEquipmentMod)

    private void Start()
    {
        player = FindObjectOfType<Player>() ?? new GameObject("Player").AddComponent<Player>();
        StartCoroutine(RunCampaign());
    }

    private IEnumerator RunCampaign()
    {
        for (levelIdx = 0; levelIdx < campaign.Length;)
        {
            var cur = campaign[levelIdx];
            var next = (levelIdx + 1 < campaign.Length) ? campaign[levelIdx + 1] : null;

            // 关前剧情
            yield return PlayCutsceneSeq(cur.introCutscenes);

            // 核心构筑：选装备
            if (cur.coreEquipPool != null && cur.coreEquipPool.Length > 0 && cur.coreBuildUIPrefab)
                yield return OpenCoreBuild(cur);

            // 初始化本关
            InitLevel(cur, isFirst: levelIdx == 0);

            // 游戏循环
            var result = LevelResult.Running; levelRunning = true;
            while (levelRunning)
            {
                result = TickLevel(cur);
                if (result != LevelResult.Running) break;
                yield return null;
            }

            if (result == LevelResult.Fail)
            {
                yield return PlayCutsceneSeq(cur.failCutscenes);
                ClearLevel();
                // 回主菜单/或重开：按你项目需要来，这里直接结束
                yield break;
            }

            // 通关
            if (cur.isFinalStage || next == null)
            {
                yield return PlayCutsceneSeq(cur.outroCutscenes);
                ClearLevel();
                yield break;
            }
            else
            {
                // 通关剧情 + 背景初始化下一关
                bool outroDone = false;
                StartCoroutine(PlayCutsceneSeq(cur.outroCutscenes, () => outroDone = true));
                InitLevel(next, isFirst: false);
                while (!outroDone) yield return null;

                // 下一关开场剧情
                yield return PlayCutsceneSeq(next.introCutscenes);

                // 下一关核心构筑：是否再次选装备（按你需要，这里示例：每关都可以再选一件）
                if (next.coreEquipPool != null && next.coreEquipPool.Length > 0 && next.coreBuildUIPrefab)
                    yield return OpenCoreBuild(next);

                levelIdx++;
            }
        }
        ClearLevel();
    }

    enum LevelResult { Running, Clear, Fail }

    private LevelResult TickLevel(LevelConfig cfg)
    {
        elapsed += Time.deltaTime;

        // 刷怪
        if (cfg.waves != null)
        {
            foreach (var w in cfg.waves)
            {
                if (!fired.Contains(w) && elapsed >= w.atTime)
                {
                    FireWave(cfg, w);
                    fired.Add(w);
                }
            }
        }

        // 小构筑触发（经验达阈值）
        if (cfg.xpThresholds != null && xpTier < cfg.xpThresholds.Length && xp >= cfg.xpThresholds[xpTier])
        {
            StartCoroutine(OpenMicroBuild(cfg)); // 弹窗期间会暂停
            xpTier++;
        }

        // 判负
        if (player == null || player.IsDead) { levelRunning = false; return LevelResult.Fail; }

        // 判胜：派完波且清场
        if ((cfg.waves == null || fired.Count >= cfg.waves.Length) && alive.Count == 0)
        { levelRunning = false; return LevelResult.Clear; }

        return LevelResult.Running;
    }

    private void InitLevel(LevelConfig cfg, bool isFirst)
    {
        // 地图生成
        BuildTiles(cfg);

        // 刷怪状态
        fired.Clear(); alive.Clear(); elapsed = 0f; xpTier = 0; xp = 0;

        // 玩家（第一关可重置基础数值；后续保留成长/装备）
        if (isFirst) player.ApplyBase(hp: 100, speed: 5f);
    }

    private void ClearLevel()
    {
        // 清敌 & 清砖
        foreach (var e in alive) if (e) Destroy(e.gameObject);
        alive.Clear();
        for (int i = 0; i < tilePool.Count; i++) tilePool[i].SetActive(false);
        tileUsed = 0;
    }

    // ====== 砖块生成（不用Tilemap） ======
    private void BuildTiles(LevelConfig v)
    {
        if (!tileRoot) tileRoot = new GameObject("Tiles").transform;
        // 清现有
        for (int i = 0; i < tilePool.Count; i++) tilePool[i].SetActive(false);
        tileUsed = 0;

        var cs = Mathf.Max(0.01f, v.cellSize);
        var ox = v.origin.x; var oy = v.origin.y;

        // 地面
        if (v.floorSprite)
        {
            for (int y = 0; y < v.height; y++)
                for (int x = 0; x < v.width; x++)
                    SpawnTile(v.floorSprite, v.sortingLayer, v.floorOrder,
                        new Vector3(ox + (x + 0.5f) * cs, oy + (y + 0.5f) * cs, 0), 0f);
        }

        // 墙（四边）
        if (v.wallSprite && v.borderThickness > 0)
        {
            int t = v.borderThickness;
            for (int x = 0; x < v.width; x++)
                for (int k = 0; k < t; k++)
                {
                    SpawnTile(v.wallSprite, v.sortingLayer, v.wallOrder,
                        new Vector3(ox + (x + 0.5f) * cs, oy + (0.5f + k) * cs, 0), 0f);
                    SpawnTile(v.wallSprite, v.sortingLayer, v.wallOrder,
                        new Vector3(ox + (x + 0.5f) * cs, oy + (v.height - 0.5f - k) * cs, 0), 0f);
                }
            for (int y = 0; y < v.height; y++)
                for (int k = 0; k < t; k++)
                {
                    SpawnTile(v.wallSprite, v.sortingLayer, v.wallOrder,
                        new Vector3(ox + (0.5f + k) * cs, oy + (y + 0.5f) * cs, 0), 0f);
                    SpawnTile(v.wallSprite, v.sortingLayer, v.wallOrder,
                        new Vector3(ox + (v.width - 0.5f - k) * cs, oy + (y + 0.5f) * cs, 0), 0f);
                }
        }

        // 角
        var TL = new Vector3(ox + 0.5f * cs, oy + (v.height - 0.5f) * cs, 0);
        var TR = new Vector3(ox + (v.width - 0.5f) * cs, oy + (v.height - 0.5f) * cs, 0);
        var BL = new Vector3(ox + 0.5f * cs, oy + 0.5f * cs, 0);
        var BR = new Vector3(ox + (v.width - 0.5f) * cs, oy + 0.5f * cs, 0);

        if (v.cornerTopLeft || v.cornerTopRight || v.cornerBottomLeft || v.cornerBottomRight)
        {
            if (v.cornerTopLeft) SpawnTile(v.cornerTopLeft, v.sortingLayer, v.wallOrder, TL, 0);
            if (v.cornerTopRight) SpawnTile(v.cornerTopRight, v.sortingLayer, v.wallOrder, TR, 0);
            if (v.cornerBottomLeft) SpawnTile(v.cornerBottomLeft, v.sortingLayer, v.wallOrder, BL, 0);
            if (v.cornerBottomRight) SpawnTile(v.cornerBottomRight, v.sortingLayer, v.wallOrder, BR, 0);
        }
        else if (v.cornerAny)
        {
            SpawnTile(v.cornerAny, v.sortingLayer, v.wallOrder, TL, 90);
            SpawnTile(v.cornerAny, v.sortingLayer, v.wallOrder, TR, 180);
            SpawnTile(v.cornerAny, v.sortingLayer, v.wallOrder, BR, 270);
            SpawnTile(v.cornerAny, v.sortingLayer, v.wallOrder, BL, 0);
        }
    }

    private void EnsurePool(int need)
    {
        while (tilePool.Count < need)
        {
            var go = new GameObject("tile", typeof(SpriteRenderer));
            go.transform.SetParent(tileRoot, false);
            tilePool.Add(go);
        }
    }

    private void SpawnTile(Sprite sp, string layer, int order, Vector3 pos, float rotZ)
    {
        EnsurePool(tileUsed + 1);
        var go = tilePool[tileUsed++];
        go.SetActive(true);

        var t = go.transform;
        t.position = pos;
        t.rotation = Quaternion.Euler(0, 0, rotZ);

        var sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = sp; sr.sortingLayerName = layer; sr.sortingOrder = order;
    }

    // ====== 刷怪与经验 ======
    private void FireWave(LevelConfig cfg, LevelConfig.Wave w)
    {
        if (w.entries == null) return;
        foreach (var e in w.entries)
        {
            if (e.enemy == null || e.enemy.prefab == null) continue;
            for (int i = 0; i < e.count; i++)
            {
                var pos = new Vector3(
                    Random.Range(e.spawnRect.xMin, e.spawnRect.xMax),
                    Random.Range(e.spawnRect.yMin, e.spawnRect.yMax), 0);

                var go = Instantiate(e.enemy.prefab, pos, Quaternion.identity);
                var en = go.GetComponent<Enemy>() ?? go.AddComponent<Enemy>();
                en.Init(e.enemy, player.transform);
                en.OnDied += (dead) => {
                    alive.Remove(dead);
                    xp += e.enemy.expOnDie;   // 加经验 → Update 中会检查阈值触发小构筑
                };
                alive.Add(en);
            }
        }
    }

    // ====== 核心构筑（选装备） ======
    private IEnumerator OpenCoreBuild(LevelConfig cfg)
    {
        var pool = cfg.coreEquipPool;
        var labels = new string[pool.Length];
        for (int i = 0; i < pool.Length; i++) labels[i] = pool[i]?.displayName ?? $"Equip {i + 1}";

        Time.timeScale = 0f;
        var ui = Instantiate(cfg.coreBuildUIPrefab);
        var chooser = ui.GetComponent<ChoiceUI>() ?? ui.AddComponent<ChoiceUI>();

        bool done = false;
        chooser.Open(labels, pickIdx => {
            var w = pool[Mathf.Clamp(pickIdx, 0, pool.Length - 1)];
            if (w && w.prefab)
            {
                var inst = Instantiate(w.prefab, player.transform);
                // 绑定到玩家
                var eq = inst.GetComponent<IEquipment>();
                eq?.BindOwner(player);
                equips.Add(inst);
            }
            done = true;
        });

        while (!done) yield return null;
        Time.timeScale = 1f;
    }

    // ====== 小构筑（经验触发，围绕装备加成） ======
    private IEnumerator OpenMicroBuild(LevelConfig cfg)
    {
        if (cfg.microPool == null || cfg.microPool.Length == 0 || cfg.microBuildUIPrefab == null) yield break;

        var pool = cfg.microPool;
        var labels = new string[pool.Length];
        for (int i = 0; i < pool.Length; i++) labels[i] = pool[i]?.displayName ?? $"Mod {i + 1}";

        Time.timeScale = 0f;
        var ui = Instantiate(cfg.microBuildUIPrefab);
        var chooser = ui.GetComponent<ChoiceUI>() ?? ui.AddComponent<ChoiceUI>();

        bool done = false;
        chooser.Open(labels, pickIdx => {
            var mod = pool[Mathf.Clamp(pickIdx, 0, pool.Length - 1)];
            // 应用到所有当前装备（若装备实现了 IEquipmentMod）
            foreach (var go in equips)
            {
                if (!go) continue;
                var modder = go.GetComponent<IEquipmentMod>();
                modder?.ApplyMod(mod);
            }
            done = true;
        });

        while (!done) yield return null;
        Time.timeScale = 1f;
    }

    // ====== 剧情序列（UI/Timeline 都可） ======
    private IEnumerator PlayCutsceneSeq(GameObject[] prefabs, System.Action onAllDone = null)
    {
        if (prefabs == null || prefabs.Length == 0) { onAllDone?.Invoke(); yield break; }
        Time.timeScale = 0f;
        for (int i = 0; i < prefabs.Length; i++)
        {
            var go = Instantiate(prefabs[i]);
            // 若是Timeline/按钮，请在预制里自行处理关闭后销毁；这里简单等待销毁
            while (go != null) yield return null; // 预制自行销毁后继续
        }
        Time.timeScale = 1f;
        onAllDone?.Invoke();
    }
}
