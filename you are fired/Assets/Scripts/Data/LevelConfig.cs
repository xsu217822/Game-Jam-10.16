// Assets/Scripts/LevelConfig.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Config (All-in-One)")]
public class LevelConfig : ScriptableObject
{
    [Header("标识")]
    public string displayName;
    public bool isFinalStage;

    [Header("视觉（纯Sprite，不用Tilemap）")]
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
    public int[] xpThresholds = new int[] { 10, 25, 45, 70 };
    public MicroModData[] microPool;
    public GameObject microBuildUIPrefab;

    [Header("核心构筑（关卡开始选装备）")]
    public WeaponData[] coreEquipPool;
    public GameObject coreBuildUIPrefab;

    [Header("剧情（UI或Timeline预制）")]
    public GameObject[] introCutscenes;
    public GameObject[] outroCutscenes;
    public GameObject[] failCutscenes;
}

