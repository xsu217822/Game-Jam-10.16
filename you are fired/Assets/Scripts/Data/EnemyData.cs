// Assets/Scripts/EnemyData.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public GameObject prefab;
    public int maxHealth = 10;
    public float moveSpeed = 2f;
    public int expOnDie = 3;
}
