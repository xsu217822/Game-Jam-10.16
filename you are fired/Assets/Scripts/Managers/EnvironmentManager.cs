// Assets/Scripts/EnvironmentManager.cs
using UnityEngine;

public class EnvironmentManager : MonoBehaviour, IEnvironmentBuilder
{
    [SerializeField] private Transform root;
    private readonly System.Collections.Generic.List<GameObject> pool = new();
    private int used = 0;

    public void Build(LevelConfig v)
    {
        if (!root) root = new GameObject("EnvRoot").transform;
        Clear();

        float cs = Mathf.Max(0.01f, v.cellSize);
        float ox = v.origin.x, oy = v.origin.y;

        // µÿ√Ê
        if (v.floorSprite)
        {
            for (int y = 0; y < v.height; y++)
                for (int x = 0; x < v.width; x++)
                    Spawn(v.floorSprite, v.sortingLayer, v.floorOrder,
                        new Vector3(ox + (x + 0.5f) * cs, oy + (y + 0.5f) * cs, 0), 0f);
        }

        // «Ω
        if (v.wallSprite && v.borderThickness > 0)
        {
            int t = v.borderThickness;
            for (int x = 0; x < v.width; x++)
                for (int k = 0; k < t; k++)
                {
                    Spawn(v.wallSprite, v.sortingLayer, v.wallOrder,
                        new Vector3(ox + (x + 0.5f) * cs, oy + (0.5f + k) * cs, 0), 0);
                    Spawn(v.wallSprite, v.sortingLayer, v.wallOrder,
                        new Vector3(ox + (x + 0.5f) * cs, oy + (v.height - 0.5f - k) * cs, 0), 0);
                }
            for (int y = 0; y < v.height; y++)
                for (int k = 0; k < t; k++)
                {
                    Spawn(v.wallSprite, v.sortingLayer, v.wallOrder,
                        new Vector3(ox + (0.5f + k) * cs, oy + (y + 0.5f) * cs, 0), 0);
                    Spawn(v.wallSprite, v.sortingLayer, v.wallOrder,
                        new Vector3(ox + (v.width - 0.5f - k) * cs, oy + (y + 0.5f) * cs, 0), 0);
                }
        }

        // Ω«
        var TL = new Vector3(ox + 0.5f * cs, oy + (v.height - 0.5f) * cs, 0);
        var TR = new Vector3(ox + (v.width - 0.5f) * cs, oy + (v.height - 0.5f) * cs, 0);
        var BL = new Vector3(ox + 0.5f * cs, oy + 0.5f * cs, 0);
        var BR = new Vector3(ox + (v.width - 0.5f) * cs, oy + 0.5f * cs, 0);

        if (v.cornerTopLeft || v.cornerTopRight || v.cornerBottomLeft || v.cornerBottomRight)
        {
            if (v.cornerTopLeft) Spawn(v.cornerTopLeft, v.sortingLayer, v.wallOrder, TL, 0);
            if (v.cornerTopRight) Spawn(v.cornerTopRight, v.sortingLayer, v.wallOrder, TR, 0);
            if (v.cornerBottomLeft) Spawn(v.cornerBottomLeft, v.sortingLayer, v.wallOrder, BL, 0);
            if (v.cornerBottomRight) Spawn(v.cornerBottomRight, v.sortingLayer, v.wallOrder, BR, 0);
        }
        else if (v.cornerAny)
        {
            Spawn(v.cornerAny, v.sortingLayer, v.wallOrder, TL, 90);
            Spawn(v.cornerAny, v.sortingLayer, v.wallOrder, TR, 180);
            Spawn(v.cornerAny, v.sortingLayer, v.wallOrder, BR, 270);
            Spawn(v.cornerAny, v.sortingLayer, v.wallOrder, BL, 0);
        }
    }

    private void Ensure(int need)
    {
        while (pool.Count < need)
        {
            var go = new GameObject("tile", typeof(SpriteRenderer));
            go.transform.SetParent(root, false);
            pool.Add(go);
        }
    }

    private void Spawn(Sprite sp, string layer, int order, Vector3 pos, float rotZ)
    {
        Ensure(used + 1);
        var go = pool[used++]; go.SetActive(true);
        var t = go.transform; t.position = pos; t.rotation = Quaternion.Euler(0, 0, rotZ);
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = sp; sr.sortingLayerName = layer; sr.sortingOrder = order;
    }

    public void Clear()
    {
        for (int i = 0; i < pool.Count; i++) pool[i].SetActive(false);
        used = 0;
    }
}
