// Assets/Scripts/MicroModData.cs
using UnityEngine;

public enum MicroModType { DamageAdd, AttackRateMult, RangeMult, ProjectileCountAdd }

[CreateAssetMenu(menuName = "Game/Micro Mod Data")]
public class MicroModData : ScriptableObject
{
    public string displayName = "Mod";
    [TextArea] public string desc;
    public MicroModType type;
    public float value;
    public Sprite icon;
}
