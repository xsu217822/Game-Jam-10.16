using UnityEngine;

[CreateAssetMenu(menuName = "Game/StageConfig")]
public class StageConfig : ScriptableObject
{
    public int stageIndex;                 // 0..3
    public bool isFinalStage = false;      // 只有最后一关勾上

    [Header("Ending UI Prefabs (Canvas 子物体预制)")]
    public GameObject failEndingUIPrefab;  // 本关失败时展示
    public GameObject successEndingUIPrefab; // 仅最后一关需要；前面几关可留空
}
