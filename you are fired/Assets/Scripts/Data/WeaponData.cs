// Assets/Scripts/WeaponData.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEquipment { void BindOwner(Player p); }
public interface IEquipmentMod { void ApplyMod(MicroModData mod); }

[CreateAssetMenu(menuName = "Game/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public GameObject prefab;
    public string displayName = "Equipment";
    public Sprite icon;
}

