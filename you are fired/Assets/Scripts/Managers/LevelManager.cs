using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap wallMap;

    // ������
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
            Debug.LogError("LevelManager: ��ǰ LevelConfig Ϊ�գ��������˵���");
            GameManager.I.LoadMainMenu();
            return;
        }

        // 1) ��/����ң���Ӧ��������ݣ����Լ��� Player ����Ҫ�� ApplyData��
        player = FindObjectOfType<Player>();
        if (!player)
        {
            player = new GameObject("Player").AddComponent<Player>();
        }
        if (cfg.playerData) player.ApplyData(cfg.playerData);

        // 2) �̾��� Tile������/ǽ/ת�ǣ�
        ApplyTiles(cfg.tilePreset);

        // 3) ��ʼ������
        fired.Clear();
        alive.Clear();
        elapsed = 0f;

        // 4) ����ѡ������BGM/HUD�������� Audio/UI ϵͳ�������������
        // AudioManager?.PlayBGM(cfg.bgm);
        // UIManager?.AttachHUD(cfg.hudPrefab);
    }

    private void Update()
    {
        if (endingShown) return;

        elapsed += Time.deltaTime;

        // ˢ�֣����㷢������Сʵ�֣�
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

        // �и����������
        bool playerDead = (player == null) || player.IsDead;
        if (playerDead)
        {
            ShowFailEndingThenMenu();
            return;
        }

        // ��ʤ�����в����ɷ��ҵ���ȫ��
        if (AllWavesDispatched() && alive.Count == 0)
        {
            if (cfg.isFinalStage) ShowFinalEndingThenMenu();
            else GameManager.I.NextLevelOrMenu();
        }
    }

    // ======= Tile ���� =======
    private void ApplyTiles(LevelRectPreset p)
    {
        if (!p || !floorMap || !wallMap) return;

        if (p.clearBeforeApply) { floorMap.ClearAllTiles(); wallMap.ClearAllTiles(); }

        var o = p.origin;
        int w = Mathf.Max(1, p.width), h = Mathf.Max(1, p.height);

        // ��������
        if (p.floorTile)
        {
            var area = new BoundsInt(o, new Vector3Int(w, h, 1));
            var tiles = new TileBase[w * h];
            for (int i = 0; i < tiles.Length; i++) tiles[i] = p.floorTile;
            floorMap.SetTilesBlock(area, tiles);
        }

        // Χǽ���
        if (p.wallTile && p.borderThickness > 0)
        {
            int t = p.borderThickness;
            // ��/��/��/��
            FillRect(wallMap, p.wallTile, new BoundsInt(o.x, o.y, 0, w, t, 1));
            FillRect(wallMap, p.wallTile, new BoundsInt(o.x, o.y + h - t, 0, w, t, 1));
            FillRect(wallMap, p.wallTile, new BoundsInt(o.x, o.y, 0, t, h, 1));
            FillRect(wallMap, p.wallTile, new BoundsInt(o.x + w - t, o.y, 0, t, h, 1));

            // �ǿ鸲��
            PlaceCorners(p, o, w, h);
        }

        if (p.compressBoundsAfterApply)
        {
            floorMap.CompressBounds();
            wallMap.CompressBounds();
        }
    }

    // �����ĸ��ǿ�
    private void PlaceCorners(LevelRectPreset p, Vector3Int o, int w, int h)
    {
        var bl = new Vector3Int(o.x, o.y, 0);
        var tl = new Vector3Int(o.x, o.y + h - 1, 0);
        var br = new Vector3Int(o.x + w - 1, o.y, 0);
        var tr = new Vector3Int(o.x + w - 1, o.y + h - 1, 0);

        // �ĸ�ר�ýǿ�����
        if (p.cornerTopLeft || p.cornerTopRight || p.cornerBottomLeft || p.cornerBottomRight)
        {
            if (p.cornerBottomLeft) wallMap.SetTile(bl, p.cornerBottomLeft);
            if (p.cornerTopLeft) wallMap.SetTile(tl, p.cornerTopLeft);
            if (p.cornerBottomRight) wallMap.SetTile(br, p.cornerBottomRight);
            if (p.cornerTopRight) wallMap.SetTile(tr, p.cornerTopRight);
            return;
        }

        // ֻ��һ�Žǿ飺����ת
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

    // ����������
    private static void FillRect(Tilemap tm, TileBase tile, BoundsInt rect)
    {
        int count = rect.size.x * rect.size.y;
        if (count <= 0) return;
        var tiles = new TileBase[count];
        for (int i = 0; i < count; i++) tiles[i] = tile;
        tm.SetTilesBlock(rect, tiles);
    }

    // ======= ˢ�� =======
    // ��ʵ�֣��������������λ��ˢ��ָ�������ĵ���
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

    // ======= ��� UI =======
    // �ؿ�ʧ�ܽ��
    private void ShowFailEndingThenMenu()
    {
        endingShown = true;
        Time.timeScale = 0f;
        var prefab = cfg.failEndingUIPrefab;
        var ui = prefab ? Instantiate(prefab) : null;
        BindEnding(ui, () => { Time.timeScale = 1f; GameManager.I.LoadMainMenu(); });
    }

    // ���ս��
    private void ShowFinalEndingThenMenu()
    {
        endingShown = true;
        Time.timeScale = 0f;
        var prefab = cfg.successEndingUIPrefab ? cfg.successEndingUIPrefab : cfg.failEndingUIPrefab;
        var ui = prefab ? Instantiate(prefab) : null;
        BindEnding(ui, () => { Time.timeScale = 1f; GameManager.I.LoadMainMenu(); });
    }

    // ���UI��
    private void BindEnding(GameObject ui, System.Action onClose)
    {
        if (!ui) { onClose?.Invoke(); return; }
        var ctrl = ui.GetComponent<EndingUIController>();
        if (!ctrl) ctrl = ui.AddComponent<EndingUIController>();
        ctrl.Bind(onClose);
    }
}
