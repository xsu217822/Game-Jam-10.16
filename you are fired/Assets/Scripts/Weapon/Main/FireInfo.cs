using UnityEngine;

public struct FireInfo
{
    public GameObject player;

    public Vector3 origin;

    public Vector3 direction;

    public FireInfo(GameObject player, Vector3 origin, Vector3 direction)
    {
        this.player = player;
        this.origin = origin;
        this.direction = direction.normalized;
    }
}