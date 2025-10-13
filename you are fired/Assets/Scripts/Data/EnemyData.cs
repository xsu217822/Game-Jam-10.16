using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public GameObject prefab; // 敌人预制（内含 Enemy 脚本）
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    // 攻击CD/投射物等以后补
}