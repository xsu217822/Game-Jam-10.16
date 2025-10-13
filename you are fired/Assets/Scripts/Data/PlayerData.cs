using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    public int maxHealth = 100;
    public float moveSpeed = 5f;
    // 起始武器等以后补
}
