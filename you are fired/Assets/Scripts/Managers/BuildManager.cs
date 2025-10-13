// Assets/Scripts/BuildManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ϵͳ�ܹܣ�
/// - ���Ĺ�����ÿ�ؿ�ͷ��ѡװ������ʵ����װ��������ң�
/// - С�������ﵽ������ֵʱ������ѡ�ӳɡ����Ѽӳ�Ӧ�õ���ǰ����װ��
/// ������ChoiceUI��WeaponData(IEquipment/IEquipmentMod)��MicroModData��LevelConfig��Player
/// </summary>
public class BuildManager : MonoBehaviour, IBuildService
{
    [Header("��װ���ҵ����������=�ҵ�������ϣ�")]
    [SerializeField] private Transform equipRoot;

    // װ��ʵ������ҵ�ǰ���У�
    private readonly List<GameObject> equips = new();

    // С������������״̬
    private int xpAccum = 0;       // ��ǰ�ۼƾ���
    private int thresholdIdx = 0;  // �Ѿ��������ڼ�����ֵ
    private bool microOpen = false; // С����UI�Ƿ����ڴ򿪣������룩

    /// <summary>
    /// ���Ĺ������� LevelConfig.coreEquipPool �ж�ѡһ��ʵ����װ�����󶨵����
    /// </summary>
    public IEnumerator DoCoreBuild(LevelConfig cfg, Player player)
    {
        var pool = cfg.coreEquipPool;
        if (pool == null || pool.Length == 0 || !cfg.coreBuildUIPrefab) yield break;

        // ѡ������
        var labels = new string[pool.Length];
        for (int i = 0; i < pool.Length; i++)
            labels[i] = pool[i]?.displayName ?? $"Equip {i + 1}";

        // �� UI
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
                // �󶨵����
                var eq = go.GetComponent<IEquipment>();
                eq?.BindOwner(player);
                equips.Add(go);
            }
            done = true;
        });

        // ��ͣʱ��ֱ��ѡ�����
        Time.timeScale = 0f;
        while (!done) yield return null;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// ÿ�ؿ�ʼ����С�����ļ�����״̬�����������װ����
    /// </summary>
    public void ResetSession(LevelConfig cfg, Player player)
    {
        xpAccum = 0;
        thresholdIdx = 0;
        microOpen = false;
        // �����ѱ����ٵ�װ�����ã���ȫ�����
        for (int i = equips.Count - 1; i >= 0; i--)
            if (!equips[i]) equips.RemoveAt(i);
    }

    /// <summary>
    /// ��ˢ��ϵͳ��������þ���ʱ���á����ﵽ��ֵ������һ��С������
    /// </summary>
    public void OnGainExp(int exp, LevelConfig cfg, Player player)
    {
        xpAccum += Mathf.Max(0, exp);
        if (microOpen) return; // UI ���ڿ����Ȳ�������

        var th = cfg.xpThresholds;
        if (th != null && thresholdIdx < th.Length && xpAccum >= th[thresholdIdx])
        {
            thresholdIdx++;
            // ����һ��С����
            StartCoroutine(DoMicroBuild(cfg, player));
        }
    }

    /// <summary>
    /// С�������� LevelConfig.microPool ��ѡһ���Ѽӳ�Ӧ�õ����е�ǰװ����ʵ���� IEquipmentMod �ģ�
    /// </summary>
    public IEnumerator DoMicroBuild(LevelConfig cfg, Player player)
    {
        var pool = cfg.microPool;
        if (pool == null || pool.Length == 0 || !cfg.microBuildUIPrefab) yield break;

        microOpen = true;

        // ѡ������
        var labels = new string[pool.Length];
        for (int i = 0; i < pool.Length; i++)
            labels[i] = pool[i]?.displayName ?? $"Mod {i + 1}";

        // �� UI
        var ui = Instantiate(cfg.microBuildUIPrefab);
        var chooser = ui.GetComponent<ChoiceUI>() ?? ui.AddComponent<ChoiceUI>();

        bool done = false;
        chooser.Open(labels, pick =>
        {
            var idx = Mathf.Clamp(pick, 0, pool.Length - 1);
            var mod = pool[idx];
            // Ӧ�õ���ǰ����װ��
            for (int i = equips.Count - 1; i >= 0; i--)
            {
                var go = equips[i];
                if (!go) { equips.RemoveAt(i); continue; }
                var modder = go.GetComponent<IEquipmentMod>();
                modder?.ApplyMod(mod);
            }
            done = true;
        });

        // ��ͣʱ��ֱ��ѡ�����
        Time.timeScale = 0f;
        while (!done) yield return null;
        Time.timeScale = 1f;

        microOpen = false;
    }

    // ���� ��ѡ������ֻ�����ʵ�ǰװ��������/չʾ�ã�����
    public IReadOnlyList<GameObject> CurrentEquips => equips;
}
