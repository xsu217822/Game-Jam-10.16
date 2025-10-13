using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap wallMap;

    // 运行期
    private LevelConfig cfg;
    private readonly HashSet<LevelConfig.Wave> fired = new();
    private readonly List<Enemy> alive = new();
    private float elapsed;
    private Player player;
    private bool endingShown;

    private void Start()
    {
        cfg = GameManager.I.GetCurrentLevelConfig();
        if (cfg == null)
        {
            Debug.LogError("LevelManager: 当前 LevelConfig 为空，返回主菜单。");
            GameManager.I.LoadMainMenu();
            return;
        }

        // 1) 找/造玩家，并应用玩家数据（你自己的 Player 类里要有 ApplyData）
        player = FindObjectOfType<Player>();
        if (!player)
        {
            player = new GameObject("Player").AddComponent<Player>();
        }
        if (cfg.playerData) player.ApplyData(cfg.playerData);

        // 2) 铺矩形 Tile（地面/墙/转角）
        ApplyTiles(cfg.tilePreset);

        // 3) 初始化波次
        fired.Clear();
        alive.Clear();
        elapsed = 0f;

        // 4) （可选）播放BGM/HUD：你若有 Audio/UI 系统，可在这里接入
        // AudioManager?.PlayBGM(cfg.bgm);
        // UIManager?.AttachHUD(cfg.hudPrefab);
    }

    private void Update()
    {
        if (endingShown) return;

        elapsed += Time.deltaTime;

        // 刷怪：到点发波（最小实现）
        if (cfg.waves != null)
        {
            foreach (var w in cfg.waves)
            {
                if (!fired.Contains(w) && elapsed >= w.atTime)
                {
                    FireWave(w);
                    fired.Add(w);
                }
            }
        }

        // 判负：玩家死亡
        bool playerDead = (player == null) || player.IsDead;
        if (playerDead)
        {
            ShowFailEndingThenMenu();
            return;
        }

        // 判胜：所有波已派发且敌人全清
        if (AllWavesDispatched() && alive.Count == 0)
        {
            if (cfg.isFinalStage) ShowFinalEndingThenMenu();
            else GameManager.I.NextLevelOrMenu();
        }
    }

    // ======= Tile 铺设 =======
    private void ApplyTiles(LevelRectPreset p)
    {
        if (!p || !floorMap || !wallMap) return;

        if (p.clearBeforeApply) { floorMap.ClearAllTiles(); wallMap.ClearAllTiles(); }

        var o = p.origin;
        int w = Mathf.Max(1, p.width), h = Mathf.Max(1, p.height);

        // 地面整块
        if (p.floorTile)
        {
            var area = new BoundsInt(o, new Vector3Int(w, h, 1));
            var tiles = new TileBase[w * h];
            for (int i = 0; i < tiles.Length; i++) tiles[i] = p.floorTile;
            floorMap.SetTilesBlock(area, tiles);
        }

        // 围墙与角
        if (p.wallTile && p.borderThickness > 0)
        {
            int t = p.borderThickness;
            // 下/上/左/右
            FillRect(wallMap, p.wallTile, new BoundsInt(o.x, o.y, 0, w, t, 1));
            FillRect(wallMap, p.wallTile, new BoundsInt(o.x, o.y + h - t, 0, w, t, 1));
            FillRect(wallMap, p.wallTile, new BoundsInt(o.x, o.y, 0, t, h, 1));
            FillRect(wallMap, p.wallTile, new BoundsInt(o.x + w - t, o.y, 0, t, h, 1));

            // 角块覆盖
            PlaceCorners(p, o, w, h);
        }

        if (p.compressBoundsAfterApply)
        {
            floorMap.CompressBounds();
            wallMap.CompressBounds();
        }
    }

    // 放置四个角块
    private void PlaceCorners(LevelRectPreset p, Vector3Int o, int w, int h)
    {
        var bl = new Vector3Int(o.x, o.y, 0);
        var tl = new Vector3Int(o.x, o.y + h - 1, 0);
        var br = new Vector3Int(o.x + w - 1, o.y, 0);
        var tr = new Vector3Int(o.x + w - 1, o.y + h - 1, 0);

        // 四个专用角块优先
        if (p.cornerTopLeft || p.cornerTopRight || p.cornerBottomLeft || p.cornerBottomRight)
        {
            if (p.cornerBottomLeft) wallMap.SetTile(bl, p.cornerBottomLeft);
            if (p.cornerTopLeft) wallMap.SetTile(tl, p.cornerTopLeft);
            if (p.cornerBottomRight) wallMap.SetTile(br, p.cornerBottomRight);
            if (p.cornerTopRight) wallMap.SetTile(tr, p.cornerTopRight);
            return;
        }

        // 只有一张角块：用旋转
        if (p.cornerTile)
        {
            wallMap.SetTile(bl, p.cornerTile);
            wallMap.SetTile(tl, p.cornerTile);
            wallMap.SetTile(tr, p.cornerTile);
            wallMap.SetTile(br, p.cornerTile);

            wallMap.SetTransformMatrix(bl, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 0)));
            wallMap.SetTransformMatrix(tl, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90)));
            wallMap.SetTransformMatrix(tr, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180)));
            wallMap.SetTransformMatrix(br, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 270)));
        }
    }

    // 填充矩形区域
    private static void FillRect(Tilemap tm, TileBase tile, BoundsInt rect)
    {
        int count = rect.size.x * rect.size.y;
        if (count <= 0) return;
        var tiles = new TileBase[count];
        for (int i = 0; i < count; i++) tiles[i] = tile;
        tm.SetTilesBlock(rect, tiles);
    }

    // ======= 刷怪 =======
    // 简单实现：按波次配置随机位置刷出指定数量的敌人
    private void FireWave(LevelConfig.Wave w)
    {
        if (w.entries == null) return;
        foreach (var e in w.entries)
        {
            if (!e.enemy || !e.enemy.prefab) continue;
            for (int i = 0; i < e.count; i++)
            {
                var pos = new Vector3(
                    Random.Range(e.spawnRect.xMin, e.spawnRect.xMax),
                    Random.Range(e.spawnRect.yMin, e.spawnRect.yMax), 0);

                var go = Instantiate(e.enemy.prefab, pos, Quaternion.identity);
                var enemy = go.GetComponent<Enemy>();
                if (!enemy) enemy = go.AddComponent<Enemy>();
                enemy.ApplyData(e.enemy);
                enemy.OnDied += () => alive.Remove(enemy);
                alive.Add(enemy);
            }
        }
    }

    private bool AllWavesDispatched() => cfg.waves == null || fired.Count >= cfg.waves.Length;

    // ======= 结局 UI =======
    // 关卡失败结局
    private void ShowFailEndingThenMenu()
    {
        endingShown = true;
        Time.timeScale = 0f;
        var prefab = cfg.failEndingUIPrefab;
        var ui = prefab ? Instantiate(prefab) : null;
        BindEnding(ui, () => { Time.timeScale = 1f; GameManager.I.LoadMainMenu(); });
    }

    // 最终结局
    private void ShowFinalEndingThenMenu()
    {
        endingShown = true;
        Time.timeScale = 0f;
        var prefab = cfg.successEndingUIPrefab ? cfg.successEndingUIPrefab : cfg.failEndingUIPrefab;
        var ui = prefab ? Instantiate(prefab) : null;
        BindEnding(ui, () => { Time.timeScale = 1f; GameManager.I.LoadMainMenu(); });
    }

    // 结局UI绑定
    private void BindEnding(GameObject ui, System.Action onClose)
    {
        if (!ui) { onClose?.Invoke(); return; }
        var ctrl = ui.GetComponent<EndingUIController>();
        if (!ctrl) ctrl = ui.AddComponent<EndingUIController>();
        ctrl.Bind(onClose);
    }
}
