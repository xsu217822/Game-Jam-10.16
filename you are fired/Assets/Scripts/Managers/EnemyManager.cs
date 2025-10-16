// Assets/Scripts/EnemyManager.cs
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour, IEnemyManager
{
    public event System.Action<int> OnKillExp;

    private LevelConfig cfg;
    private Transform player;
    private readonly HashSet<LevelConfig.Wave> fired = new();
    private readonly List<Enemy> alive = new();
    private float time;

    public bool AllWavesDispatched => cfg == null || fired.Count >= (cfg.waves?.Length ?? 0);
    public bool AllCleared => alive.Count == 0;
    public int AliveCount => alive.Count;

    public void Init(LevelConfig c, Transform p)
    {
        cfg = c;
        player = p;
        fired.Clear();
        time = 0f;

        // 清理上一关遗留
        for (int i = alive.Count - 1; i >= 0; i--)
            if (alive[i] != null) Destroy(alive[i].gameObject);
        alive.Clear();
    }

    public void Tick(float dt)
    {
        time += dt;
        if (cfg?.waves == null) return;

        foreach (var w in cfg.waves)
        {
            if (!fired.Contains(w) && time >= w.atTime)
            {
                SpawnWave(w);
                fired.Add(w);
            }
        }

        // 清理已销毁引用
        for (int i = alive.Count - 1; i >= 0; i--)
            if (!alive[i]) alive.RemoveAt(i);
    }

    private void SpawnWave(LevelConfig.Wave w)
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
                var en = go.GetComponent<Enemy>() ?? go.AddComponent<Enemy>();
                en.Init(e.enemy, player);

                en.OnDied += dead =>
                {
                    alive.Remove(dead);
                    OnKillExp?.Invoke(e.enemy.expOnDie);
                };

                alive.Add(en);
            }
        }
    }
}
