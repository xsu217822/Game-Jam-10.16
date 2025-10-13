// Assets/Scripts/SpawnManager.cs
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour, ISpawner
{
    public event System.Action<int> OnKillExp;

    private LevelConfig cfg;
    private Transform player;
    private readonly HashSet<LevelConfig.Wave> fired = new();
    private readonly List<Enemy> alive = new();
    private float time;

    public bool AllWavesDispatched => cfg == null || fired.Count >= (cfg.waves?.Length ?? 0);
    public bool AllCleared => alive.Count == 0;

    public void Init(LevelConfig c, Transform p)
    {
        cfg = c; player = p;
        fired.Clear(); alive.Clear(); time = 0f;
    }

    public void Tick(float dt)
    {
        time += dt;
        if (cfg?.waves == null) return;
        foreach (var w in cfg.waves)
        {
            if (!fired.Contains(w) && time >= w.atTime)
            {
                FireWave(w);
                fired.Add(w);
            }
        }
    }

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

