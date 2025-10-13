// Assets/Scripts/Configs/LevelConfig.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Config (All-in-One)")]
public class LevelConfig : ScriptableObject
{
    [Header("标识")]
    public string displayName;
    public bool isFinalStage;

    [Header("视觉（不用Tilemap，纯Sprite生成）")]
    public Sprite floorSprite;
    public Sprite wallSprite;
    public Sprite cornerAny;
    public Sprite cornerTopLeft, cornerTopRight, cornerBottomLeft, cornerBottomRight;
    [Min(1)] public int width = 32;
    [Min(1)] public int height = 18;
    [Min(0)] public int borderThickness = 1;
    public float cellSize = 1f;
    public Vector2 origin = Vector2.zero;
    public string sortingLayer = "Default";
    public int floorOrder = 0, wallOrder = 1;

    [Header("敌人与刷怪")]
    public EnemyData[] enemyTypes;
    public Wave[] waves;
    [System.Serializable] public class Wave { public float atTime; public WaveEntry[] entries; }
    [System.Serializable] public class WaveEntry { public EnemyData enemy; public int count = 5; public Rect spawnRect; }

    [Header("经验与小构筑触发")]
    public int[] xpThresholds = new int[] { 10, 25, 45, 70 }; // 达到即弹小构筑
    public MicroModData[] microPool;      // 小构筑池（围绕装备加成）
    public GameObject microBuildUIPrefab; // 小构筑“多选一”UI（用 ChoiceUI）

    [Header("核心构筑（关卡开始触发：选装备）")]
    public WeaponData[] coreEquipPool;    // 可选装备池
    public GameObject coreBuildUIPrefab;  // 核心构筑 UI（用 ChoiceUI）

    [Header("剧情（UI或Timeline预制）")]
    public GameObject[] introCutscenes;   // 关前剧情
    public GameObject[] outroCutscenes;   // 通关剧情
    public GameObject[] failCutscenes;    // 失败剧情
}
