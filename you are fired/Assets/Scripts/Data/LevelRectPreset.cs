using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Game/Level Rect Preset")]
public class LevelRectPreset : ScriptableObject
{
    public Vector3Int origin = Vector3Int.zero;
    [Min(1)] public int width = 32, height = 18;
    public TileBase floorTile;
    public TileBase wallTile;
    [Min(0)] public int borderThickness = 1;

    // 转角：四个专用其一，或只给一个 cornerTile 用旋转
    public TileBase cornerTile;
    public TileBase cornerTopLeft, cornerTopRight, cornerBottomLeft, cornerBottomRight;

    public bool clearBeforeApply = true;
    public bool compressBoundsAfterApply = true;
}
