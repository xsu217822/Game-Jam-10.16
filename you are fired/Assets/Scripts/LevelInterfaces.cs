// Assets/Scripts/LevelInterfaces.cs
using System.Collections;
using UnityEngine;

public interface ICutsceneService
{
    IEnumerator PlaySequence(GameObject[] prefabs);
}

public interface IEnvironmentBuilder
{
    void Build(LevelConfig cfg);
    void Clear();
}

public interface ISpawner
{
    void Init(LevelConfig cfg, Transform player);
    void Tick(float dt);
    bool AllWavesDispatched { get; }
    bool AllCleared { get; }
    event System.Action<int> OnKillExp; // 敌人死亡给予经验
}

public interface IBuildService
{
    IEnumerator DoCoreBuild(LevelConfig cfg, Player player);
    IEnumerator DoMicroBuild(LevelConfig cfg, Player player);
    void OnGainExp(int exp, LevelConfig cfg, Player player);
    void ResetSession(LevelConfig cfg, Player player);
}

