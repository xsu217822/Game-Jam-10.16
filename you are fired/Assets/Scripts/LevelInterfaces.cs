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

    event System.Action<int> OnKillExp; // �����������辭��
}

public interface IBuildService
{
    // ע�⣺ÿ�ؿ�ͷ�������һ�Σ�����Ҫ��������ѡ�񡱣���սһ�Σ�Զ��һ�Σ�
    IEnumerator DoCoreBuild(LevelConfig cfg, Player player);

    // С�����߼����������ڡ���ǰ����ѡ��������������
    IEnumerator DoMicroBuild(LevelConfig cfg, Player player);

    // ���˻�ɱ�ص����۾��飬����С������
    void OnGainExp(int exp, LevelConfig cfg, Player player);

    // ��ʼ����ʱ���á���������ֵ��������������״̬���������ʷ��������Ӧ��Mod��
    void ResetSession(LevelConfig cfg, Player player);
}

