// Assets/Scripts/LevelConfig.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level Config (Prefab Only)")]
public class LevelConfig : ScriptableObject
{
    [Header("��ʶ")]
    public string displayName;
    public bool isFinalStage;

    [Header("�ߴ�������")]
    [Min(1)] public int width = 32;          // �ذ���ӿ�
    [Min(1)] public int height = 18;          // �ذ���Ӹ�
    public float cellSize = 1f;               // ÿ�����絥λ
    public Vector2 origin = Vector2.zero;     // ��ͼ���½���������

    [Header("���� Prefab")]
    public GameObject floorPrefab;            // ��ש
    public GameObject wallHalfPrefab;         // ��ש�������ʵ��
    public GameObject corner3QPrefab;         // ����֮��ש�����Ͽգ�

    [Header("�߿��ȣ�����ש������")]
    [Min(0)] public int wallHalfThickness = 2; // 2 ����ש �� 1 ����ש���

    [Header("����΢��������ߴ������")]
    [Tooltip("��ש�����ľ���߽��ƫ�ƣ�Ĭ�� 0.25 = 1/4 ��ʹ��ש��ʵ���������ڲࣩ")]
    [Range(0f, 0.5f)] public float wallHalfInsetFactor = 0.25f;
    [Tooltip("ת��ש��������ߵ�������Ĭ�� 0.25 = 1/4 ��")]
    [Range(0f, 0.5f)] public float cornerInsetFactor = 0.25f;

    [Header("Prefab ����������������")]
    [Tooltip("������Prefab�Ļ�׼�������������෴ʱ��ͳһ��һ���Ƕȡ�������Ϊ 180��")]
    public float prefabRotBiasDeg = 0f;

    [Header("��Ⱦ����˳��")]
    public string floorSortingLayer = "Ground";
    public int floorSortingOrder = 0;

    public string wallSortingLayer = "Walls";
    public int wallSortingOrder = 10;

    public string cornerSortingLayer = "Walls";
    public int cornerSortingOrder = 11;


    [Header("������ˢ��")]
    public EnemyData[] enemyTypes;
    public Wave[] waves;
    [System.Serializable] public class Wave { public float atTime; public WaveEntry[] entries; }
    [System.Serializable] public class WaveEntry { public EnemyData enemy; public int count = 5; public Rect spawnRect; }

    [Header("������С����")]
    public int[] xpThresholds = new int[] { 10, 25, 45, 70 };
    public MicroModData[] microPool;
    public GameObject microBuildUIPrefab;

    [Header("���Ĺ�����ѡװ����")]
    public WeaponData[] coreEquipPool;
    public GameObject coreBuildUIPrefab;

    [Header("���飨UI��TimelineԤ�ƣ�")]
    public GameObject[] introCutscenes;
    public GameObject[] outroCutscenes;
    public GameObject[] failCutscenes;
}


