using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Identity")]
    public string displayName;
    public bool isFinalStage;

    [Header("Player/Enemy Data")]
    public PlayerData playerData; // 起始数值
    public EnemyData[] enemyTypes; // 本关可能出现的敌人模板

    [Header("Tiles (矩形铺设)")]
    public LevelRectPreset tilePreset; // 地面/墙/转角 与 宽高、厚度

    [Header("Spawn（最简单波次）")]
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
        public Rect spawnRect; // 世界坐标区域（或网格坐标，按你用）
    }

    [Header("LevelUp / CoreBuild（占位，后续细化）")]
    public AnimationCurve expCurve = AnimationCurve.Linear(1, 10, 50, 100);
    public int baseExpToLevel = 20;
    public int choicesPerLevel = 3;

    [Header("Endings UI")]
    public GameObject failEndingUIPrefab;
    public GameObject successEndingUIPrefab; // 仅最终关用
}
