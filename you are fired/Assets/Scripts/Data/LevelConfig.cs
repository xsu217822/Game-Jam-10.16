// Assets/Scripts/LevelConfig.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [Header("�ؿ����")]
    public string displayName;
    public bool isFinalStage;

    [Header("��ͼ�ߴ���ԭ��")]
    public Vector2 origin = Vector2.zero;
    public int width = 16;
    public int height = 9;
    public float cellSize = 1f;
    public float prefabRotBiasDeg = 0f;

    [Header("����/ǽ/�� Ԥ����")]
    public GameObject floorPrefab;
    public GameObject wallHalfPrefab;
    public GameObject corner3QPrefab;

    [Header("��Ⱦ����˳��")]
    public string floorSortingLayer = "Default";
    public int floorSortingOrder = 0;
    public string wallSortingLayer = "Default";
    public int wallSortingOrder = 10;
    public string cornerSortingLayer = "Default";
    public int cornerSortingOrder = 20;

    [Header("ǽ/�� ���ֲ���")]
    [Range(0, 0.5f)] public float wallHalfInsetFactor = 0.1f;
    public int wallHalfThickness = 1;
    [Range(0, 0.5f)] public float cornerInsetFactor = 0.1f;

    [Header("ˢ�ֲ���")]
    public Wave[] waves;

    [Header("������ֵ������С������")]
    public int[] xpThresholds = new int[] { 10, 30, 60 };

    [Header("���Ĺ��� UI Ԥ���壨Ҫ����� ChoiceUI��")]
    public GameObject coreBuildUIPrefab;

    [Header("С���� UI Ԥ���壨Ҫ����� ChoiceUI��")]
    public GameObject microBuildUIPrefab;

    [Header("����װ���أ���ս / Զ�̣����ظ�ѡһ�Σ�")]
    public WeaponData[] coreMeleePool;
    public WeaponData[] coreRangedPool;

    [Header("С�����أ�Mod��")]
    public MicroModData[] microPool;

    [Header("Prefabʽ���飨�������߼���")]
    public GameObject[] introCutscenes;
    public GameObject[] outroCutscenes;
    public GameObject[] failCutscenes;

    [Header("ҳʽ���飨���뵭�� + ���ֻ���")]
    public StoryPage[] introPages;
    public StoryPage[] outroPages;
    public StoryPage[] failPages;

    [Header("�ؿ�BGM")]
    public AudioClip levelIntroBgm;   // ����Ƭ�Σ���һ�Σ��ɿգ�
    public AudioClip levelLoopBgm;    // ѭ��Ƭ�Σ����ѭ����


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
        public float atTime;        // ��ʼʱ�䣨�룩
        public Entry[] entries;
    }

    [System.Serializable]
    public class Entry
    {
        public EnemyData enemy;     // �������ã�prefab + �������飩
        public int count = 3;
        public Rect spawnRect = new Rect(-7, -4, 14, 8);
    }
}
