using UnityEngine;

[CreateAssetMenu(menuName = "Game/StageConfig")]
public class StageConfig : ScriptableObject
{
    public int stageIndex;                 // 0..3
    public bool isFinalStage = false;      // ֻ�����һ�ع���

    [Header("Ending UI Prefabs (Canvas ������Ԥ��)")]
    public GameObject failEndingUIPrefab;  // ����ʧ��ʱչʾ
    public GameObject successEndingUIPrefab; // �����һ����Ҫ��ǰ�漸�ؿ�����
}
