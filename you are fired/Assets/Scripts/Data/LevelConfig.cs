// Assets/Scripts/LevelConfig.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Config (Prefab Only)")]
public class LevelConfig : ScriptableObject
{
    [Header("标识")]
    public string displayName;
    public bool isFinalStage;

    [Header("尺寸与坐标")]
    [Min(1)] public int width = 32;          // 地板格子宽
    [Min(1)] public int height = 18;          // 地板格子高
    public float cellSize = 1f;               // 每格世界单位
    public Vector2 origin = Vector2.zero;     // 地图左下角世界坐标

    [Header("地形 Prefab")]
    public GameObject floorPrefab;            // 整砖
    public GameObject wallHalfPrefab;         // 半砖（左空右实）
    public GameObject corner3QPrefab;         // 三分之四砖（左上空）

    [Header("边框厚度（按半砖计数）")]
    [Min(0)] public int wallHalfThickness = 2; // 2 个半砖 ≈ 1 个整砖厚度

    [Header("对齐微调（按格尺寸比例）")]
    [Tooltip("半砖的中心距离边界的偏移，默认 0.25 = 1/4 格（使半砖‘实’半贴在内侧）")]
    [Range(0f, 0.5f)] public float wallHalfInsetFactor = 0.25f;
    [Tooltip("转角砖相对两条边的内缩，默认 0.25 = 1/4 格")]
    [Range(0f, 0.5f)] public float cornerInsetFactor = 0.25f;

    [Header("Prefab 朝向修正（度数）")]
    [Tooltip("当美术Prefab的基准朝向与代码假设相反时，统一加一个角度。常见设为 180。")]
    public float prefabRotBiasDeg = 0f;

    [Header("渲染层与顺序")]
    public string floorSortingLayer = "Ground";
    public int floorSortingOrder = 0;

    public string wallSortingLayer = "Walls";
    public int wallSortingOrder = 10;

    public string cornerSortingLayer = "Walls";
    public int cornerSortingOrder = 11;


    [Header("敌人与刷怪")]
    public EnemyData[] enemyTypes;
    public Wave[] waves;
    [System.Serializable] public class Wave { public float atTime; public WaveEntry[] entries; }
    [System.Serializable] public class WaveEntry { public EnemyData enemy; public int count = 5; public Rect spawnRect; }

    [Header("经验与小构筑")]
    public int[] xpThresholds = new int[] { 10, 25, 45, 70 };
    public MicroModData[] microPool;
    public GameObject microBuildUIPrefab;

    [Header("核心构筑（选装备）")]
    public WeaponData[] coreEquipPool;
    public GameObject coreBuildUIPrefab;

    [Header("剧情（UI或Timeline预制）")]
    public GameObject[] introCutscenes;
    public GameObject[] outroCutscenes;
    public GameObject[] failCutscenes;
}


