using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public GameObject prefab; // ����Ԥ�ƣ��ں� Enemy �ű���
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    // ����CD/Ͷ������Ժ�
}