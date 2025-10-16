// Assets/Scripts/LevelConfig.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [Header("关卡标记")]
    public string displayName;
    public bool isFinalStage;

    [Header("地图尺寸与原点")]
    public Vector2 origin = Vector2.zero;
    public int width = 16;
    public int height = 9;
    public float cellSize = 1f;
    public float prefabRotBiasDeg = 0f;

    [Header("地面/墙/角 预制体")]
    public GameObject floorPrefab;
    public GameObject wallHalfPrefab;
    public GameObject corner3QPrefab;

    [Header("渲染层与顺序")]
    public string floorSortingLayer = "Default";
    public int floorSortingOrder = 0;
    public string wallSortingLayer = "Default";
    public int wallSortingOrder = 10;
    public string cornerSortingLayer = "Default";
    public int cornerSortingOrder = 20;

    [Header("墙/角 布局参数")]
    [Range(0, 0.5f)] public float wallHalfInsetFactor = 0.1f;
    public int wallHalfThickness = 1;
    [Range(0, 0.5f)] public float cornerInsetFactor = 0.1f;

    [Header("刷怪波次")]
    public Wave[] waves;

    [Header("经验阈值（触发小构筑）")]
    public int[] xpThresholds = new int[] { 10, 30, 60 };

    [Header("核心构筑 UI 预制体（要求挂有 ChoiceUI）")]
    public GameObject coreBuildUIPrefab;

    [Header("小构筑 UI 预制体（要求挂有 ChoiceUI）")]
    public GameObject microBuildUIPrefab;

    [Header("核心装备池：近战 / 远程（本关各选一次）")]
    public WeaponData[] coreMeleePool;
    public WeaponData[] coreRangedPool;

    [Header("小构筑池（Mod）")]
    public MicroModData[] microPool;

    [Header("Prefab式剧情（兼容老逻辑）")]
    public GameObject[] introCutscenes;
    public GameObject[] outroCutscenes;
    public GameObject[] failCutscenes;

    [Header("页式剧情（淡入淡出 + 打字机）")]
    public StoryPage[] introPages;
    public StoryPage[] outroPages;
    public StoryPage[] failPages;

    [Header("关卡BGM")]
    public AudioClip levelIntroBgm;   // 开场片段（播一次，可空）
    public AudioClip levelLoopBgm;    // 循环片段（必填，循环）


    [System.Serializable]
    public struct StoryPage
    {
        [TextArea(2, 5)] public string text;
        public Sprite image;
        public float holdSeconds;
    }

    [System.Serializable]
    public class Wave
    {
        public float atTime;        // 开始时间（秒）
        public Entry[] entries;
    }

    [System.Serializable]
    public class Entry
    {
        public EnemyData enemy;     // 敌人配置（prefab + 死亡经验）
        public int count = 3;
        public Rect spawnRect = new Rect(-7, -4, 14, 8);
    }
}
