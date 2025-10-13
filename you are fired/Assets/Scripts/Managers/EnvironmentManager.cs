// Assets/Scripts/EnvironmentManager.cs
using UnityEngine;

public class EnvironmentManager : MonoBehaviour, IEnvironmentBuilder
{
    [SerializeField] private Transform root;

    public void Build(LevelConfig cfg)
    {
        if (!root) root = new GameObject("EnvRoot").transform;
        Clear();

        float cs = Mathf.Max(0.01f, cfg.cellSize);
        float ox = cfg.origin.x, oy = cfg.origin.y;
        float L = ox, R = ox + cfg.width * cs;
        float B = oy, T = oy + cfg.height * cs;
        float rotBias = cfg.prefabRotBiasDeg;

        // ===== 1) 地板（整砖） =====
        if (cfg.floorPrefab)
        {
            for (int y = 0; y < cfg.height; y++)
                for (int x = 0; x < cfg.width; x++)
                {
                    var pos = new Vector3(ox + (x + 0.5f) * cs, oy + (y + 0.5f) * cs, 0);
                    var go = Instantiate(cfg.floorPrefab, pos, Quaternion.identity, root);
                    ForceZ(go, 0);
                    ApplySorting(go, cfg.floorSortingLayer, cfg.floorSortingOrder);
                    WarnIfNoCollider2D(go, "Floor");
                }
        }

        // ===== 2) 墙（半砖：左空右实） =====
        float inset = cfg.wallHalfInsetFactor * cs;
        int ht = Mathf.Max(0, cfg.wallHalfThickness);

        if (cfg.wallHalfPrefab && ht > 0)
        {
            // 左右竖墙
            for (int k = 0; k < ht; k++)
            {
                float xL = L + inset + cs * (0.5f * k);
                float xR = R - inset - cs * (0.5f * k);
                for (int y = 0; y < cfg.height; y++)
                {
                    float yc = oy + (y + 0.5f) * cs;

                    var goL = Instantiate(cfg.wallHalfPrefab, new Vector3(xL, yc, 0),
                        Quaternion.Euler(0, 0, 0 + rotBias), root);
                    ForceZ(goL, 0);
                    ApplySorting(goL, cfg.wallSortingLayer, cfg.wallSortingOrder);
                    WarnIfNoCollider2D(goL, "Wall(L)");

                    var goR = Instantiate(cfg.wallHalfPrefab, new Vector3(xR, yc, 0),
                        Quaternion.Euler(0, 0, 180 + rotBias), root);
                    ForceZ(goR, 0);
                    ApplySorting(goR, cfg.wallSortingLayer, cfg.wallSortingOrder);
                    WarnIfNoCollider2D(goR, "Wall(R)");
                }
            }

            // 上下横墙
            for (int k = 0; k < ht; k++)
            {
                float yB = B + inset + cs * (0.5f * k);
                float yT = T - inset - cs * (0.5f * k);
                for (int x = 0; x < cfg.width; x++)
                {
                    float xc = ox + (x + 0.5f) * cs;

                    var goB = Instantiate(cfg.wallHalfPrefab, new Vector3(xc, yB, 0),
                        Quaternion.Euler(0, 0, 90 + rotBias), root);
                    ForceZ(goB, 0);
                    ApplySorting(goB, cfg.wallSortingLayer, cfg.wallSortingOrder);
                    WarnIfNoCollider2D(goB, "Wall(B)");

                    var goT = Instantiate(cfg.wallHalfPrefab, new Vector3(xc, yT, 0),
                        Quaternion.Euler(0, 0, 270 + rotBias), root);
                    ForceZ(goT, 0);
                    ApplySorting(goT, cfg.wallSortingLayer, cfg.wallSortingOrder);
                    WarnIfNoCollider2D(goT, "Wall(T)");
                }
            }
        }

        // ===== 3) 四角（3/4砖：左上空） =====
        if (cfg.corner3QPrefab)
        {
            float cInset = cfg.cornerInsetFactor * cs;

            var tl = Instantiate(cfg.corner3QPrefab, new Vector3(L + cInset, T - cInset, 0),
                Quaternion.Euler(0, 0, 0 + rotBias), root);
            ForceZ(tl, 0); ApplySorting(tl, cfg.cornerSortingLayer, cfg.cornerSortingOrder);
            WarnIfNoCollider2D(tl, "Corner TL");

            var tr = Instantiate(cfg.corner3QPrefab, new Vector3(R - cInset, T - cInset, 0),
                Quaternion.Euler(0, 0, 270 + rotBias), root);
            ForceZ(tr, 0); ApplySorting(tr, cfg.cornerSortingLayer, cfg.cornerSortingOrder);
            WarnIfNoCollider2D(tr, "Corner TR");

            var br = Instantiate(cfg.corner3QPrefab, new Vector3(R - cInset, B + cInset, 0),
                Quaternion.Euler(0, 0, 180 + rotBias), root);
            ForceZ(br, 0); ApplySorting(br, cfg.cornerSortingLayer, cfg.cornerSortingOrder);
            WarnIfNoCollider2D(br, "Corner BR");

            var bl = Instantiate(cfg.corner3QPrefab, new Vector3(L + cInset, B + cInset, 0),
                Quaternion.Euler(0, 0, 90 + rotBias), root);
            ForceZ(bl, 0); ApplySorting(bl, cfg.cornerSortingLayer, cfg.cornerSortingOrder);
            WarnIfNoCollider2D(bl, "Corner BL");
        }
    }

    public void Clear()
    {
        if (!root) return;
        for (int i = root.childCount - 1; i >= 0; i--)
            Destroy(root.GetChild(i).gameObject);
    }

    // —— 工具：强制 Z = 0，避免你 prefab 自带偏移影响渲染/物理 —— 
    private static void ForceZ(GameObject go, float z)
    {
        var t = go.transform;
        var p = t.position; p.z = z; t.position = p;
        foreach (Transform c in t) { var cp = c.position; cp.z = z; c.position = cp; }
    }

    // —— 工具：递归设置 Sorting Layer / Order（子节点也一起设置）——
    private static void ApplySorting(GameObject go, string layer, int order)
    {
        var srs = go.GetComponentsInChildren<SpriteRenderer>(true);
        for (int i = 0; i < srs.Length; i++)
        {
            srs[i].sortingLayerName = string.IsNullOrEmpty(layer) ? srs[i].sortingLayerName : layer;
            srs[i].sortingOrder = order;
        }
    }

    // —— 工具：预警没有 2D 碰撞器（防止用到3D Collider）——
    private static void WarnIfNoCollider2D(GameObject go, string tag)
    {
        if (!go.GetComponentInChildren<Collider2D>(true))
        {
            Debug.LogWarning($"[Environment] {tag} prefab没有Collider2D（请使用 BoxCollider2D/PolygonCollider2D），路径：{go.name}");
        }
    }
}
