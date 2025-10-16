// Assets/Scripts/LevelInterfaces.cs
using System.Collections;
using UnityEngine;

public interface ICutsceneService
{
    IEnumerator PlaySequence(GameObject[] prefabs);
    IEnumerator PlayPages(LevelConfig.StoryPage[] pages);
}

public interface IEnvironmentBuilder
{
    void Build(LevelConfig cfg);
    void Clear();
}

public interface IEnemyManager
{
    void Init(LevelConfig cfg, Transform player);
    void Tick(float dt);

    bool AllWavesDispatched { get; }
    bool AllCleared { get; }
    int AliveCount { get; }

    event System.Action<int> OnKillExp; // 敌人死亡给予经验
}

public interface IBuildService
{
    // 注意：每关开头都会调用一次，本关要做“两次选择”（近战一次＋远程一次）
    IEnumerator DoCoreBuild(LevelConfig cfg, Player player);

    // 小构筑逻辑（仅作用于“当前关所选的两件武器”）
    IEnumerator DoMicroBuild(LevelConfig cfg, Player player);

    // 敌人击杀回调（累经验，触发小构筑）
    void OnGainExp(int exp, LevelConfig cfg, Player player);

    // 开始本关时重置“经验与阈值索引”等运行期状态（不清空历史武器与已应用Mod）
    void ResetSession(LevelConfig cfg, Player player);
}

