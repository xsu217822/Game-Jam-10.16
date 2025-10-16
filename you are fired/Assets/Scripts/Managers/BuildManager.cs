// Assets/Scripts/BuildManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ϵͳ�ܹܣ�
/// - ���Ĺ�����ÿ�ؿ�ͷ������ѡ�񡱣���սһ�Ρ�Զ��һ�Σ���ʵ����װ���������
/// - С�������ﵽ������ֵʱ������ѡ�ӳɡ����Ѽӳɽ�Ӧ�õ���������ѡ����װ����
/// - �������ԣ����йؿ���ѡ���������һֱ������ͨ�أ��ܹ�4��=8����
/// - ������״̬�����顢��ֵ�����������������顿�б�ȫ��װ���б�
/// </summary>
public class BuildManager : MonoBehaviour, IBuildService
{
    [Header("��װ���ҵ������=�ҵ�������ϣ�")]
    [SerializeField] private Transform equipRoot;

    // ȫ��װ��������ۼƣ�
    private readonly List<GameObject> allEquips = new();

    // ����װ���������ع������õ�Ŀ�꼯�ϣ�
    private readonly List<GameObject> currentLevelEquips = new();

    // С������������״̬
    private int xpAccum = 0;       // ��ǰ���ۼƾ���
    private int thresholdIdx = 0;  // ��ǰ���Ѵ������ڼ�����ֵ
    private bool microOpen = false;

    public IEnumerator DoCoreBuild(LevelConfig cfg, Player player)
    {
        // ÿ�ؿ�ʼ����ա�����װ���顱�б���������ʷװ����
        currentLevelEquips.Clear();

        // ����ѡ�񣺽�ս �� Զ��
        yield return PickFromPool(cfg.coreMeleePool, cfg, player, currentLevelEquips);
        yield return PickFromPool(cfg.coreRangedPool, cfg, player, currentLevelEquips);

        // ѡ��󣬱��������Ѽ��� currentLevelEquips & allEquips
        // ����С��������� currentLevelEquips Ӧ��
    }

    private IEnumerator PickFromPool(WeaponData[] pool, LevelConfig cfg, Player player, List<GameObject> levelEquipCollector)
    {
        if (pool == null || pool.Length == 0 || !cfg.coreBuildUIPrefab) yield break;

        var labels = new string[pool.Length];
        for (int i = 0; i < pool.Length; i++)
            labels[i] = pool[i]?.displayName ?? $"Equip {i + 1}";

        var ui = Instantiate(cfg.coreBuildUIPrefab);
        var chooser = ui.GetComponent<ChoiceUI>() ?? ui.AddComponent<ChoiceUI>();

        bool done = false;
        chooser.Open(labels, pick =>
        {
            var idx = Mathf.Clamp(pick, 0, pool.Length - 1);
            var w = pool[idx];
            if (w && w.prefab)
            {
                var parent = equipRoot ? equipRoot : player.transform;
                var go = Instantiate(w.prefab, parent);
                var eq = go.GetComponent<IEquipment>();
                eq?.BindOwner(player);

                // ���롰ȫ��װ�����롰����װ����
                allEquips.Add(go);
                levelEquipCollector.Add(go);
            }
            done = true;
        });

        Time.timeScale = 0f;
        while (!done) yield return null;
        Time.timeScale = 1f;
    }

    public void ResetSession(LevelConfig cfg, Player player)
    {
        // ֻ����С�������������ݣ����塰����װ���顱������ DoCoreBuild �ؽ���
        xpAccum = 0;
        thresholdIdx = 0;
        microOpen = false;

        // �����ѱ����ٵ����ã�ȫ���뱾�أ�
        for (int i = allEquips.Count - 1; i >= 0; i--)
            if (!allEquips[i]) allEquips.RemoveAt(i);
        for (int i = currentLevelEquips.Count - 1; i >= 0; i--)
            if (!currentLevelEquips[i]) currentLevelEquips.RemoveAt(i);
    }

    public void OnGainExp(int exp, LevelConfig cfg, Player player)
    {
        xpAccum += Mathf.Max(0, exp);
        if (microOpen) return;

        var th = cfg.xpThresholds;
        if (th != null && thresholdIdx < th.Length && xpAccum >= th[thresholdIdx])
        {
            thresholdIdx++;
            StartCoroutine(DoMicroBuild(cfg, player)); // ������ currentLevelEquips
        }
    }

    public IEnumerator DoMicroBuild(LevelConfig cfg, Player player)
    {
        var pool = cfg.microPool;
        if (pool == null || pool.Length == 0 || !cfg.microBuildUIPrefab) yield break;

        microOpen = true;

        var labels = new string[pool.Length];
        for (int i = 0; i < pool.Length; i++)
            labels[i] = pool[i]?.displayName ?? $"Mod {i + 1}";

        var ui = Instantiate(cfg.microBuildUIPrefab);
        var chooser = ui.GetComponent<ChoiceUI>() ?? ui.AddComponent<ChoiceUI>();

        bool done = false;
        chooser.Open(labels, pick =>
        {
            var idx = Mathf.Clamp(pick, 0, pool.Length - 1);
            var mod = pool[idx];

            // �����ؼ���ֻ�ԡ���������������Ӧ��Mod����
            for (int i = currentLevelEquips.Count - 1; i >= 0; i--)
            {
                var go = currentLevelEquips[i];
                if (!go) { currentLevelEquips.RemoveAt(i); continue; }
                var modder = go.GetComponent<IEquipmentMod>();
                modder?.ApplyMod(mod);
            }
            done = true;
        });

        Time.timeScale = 0f;
        while (!done) yield return null;
        Time.timeScale = 1f;

        microOpen = false;
    }

    // ����/չʾ��
    public IReadOnlyList<GameObject> AllEquips => allEquips;
    public IReadOnlyList<GameObject> CurrentLevelEquips => currentLevelEquips;
}
