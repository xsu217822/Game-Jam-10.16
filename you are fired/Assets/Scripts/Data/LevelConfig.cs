using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Identity")]
    public string displayName;
    public bool isFinalStage;

    [Header("Player/Enemy Data")]
    public PlayerData playerData; // ��ʼ��ֵ
    public EnemyData[] enemyTypes; // ���ؿ��ܳ��ֵĵ���ģ��

    [Header("Tiles (��������)")]
    public LevelRectPreset tilePreset; // ����/ǽ/ת�� �� ��ߡ����

    [Header("Spawn����򵥲��Σ�")]
    public Wave[] waves;
    [System.Serializable]
    public class Wave
    {
        public float atTime;
        public WaveEntry[] entries;
    }
    [System.Serializable]
    public class WaveEntry
    {
        public EnemyData enemy;
        public int count = 5;
        public Rect spawnRect; // �����������򣨻��������꣬�����ã�
    }

    [Header("LevelUp / CoreBuild��ռλ������ϸ����")]
    public AnimationCurve expCurve = AnimationCurve.Linear(1, 10, 50, 100);
    public int baseExpToLevel = 20;
    public int choicesPerLevel = 3;

    [Header("Endings UI")]
    public GameObject failEndingUIPrefab;
    public GameObject successEndingUIPrefab; // �����չ���
}
