using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [Header("Runtime")]
    [SerializeField] private float lifeSeconds = 4f;

    private Rigidbody2D rb;
    private float damage;
    private GameObject owner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Make sure collider is trigger for simple hit scan
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnEnable()
    {
        Destroy(gameObject, lifeSeconds);
    }

    // Initialize bullet movement and damage
    public void Init(Vector2 dir, float speed, float damage, GameObject owner)
    {
        this.damage = damage;
        this.owner = owner;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = dir.normalized * speed;
        // Face movement direction (optional)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore hitting the shooter or its children
        if (owner && other.transform.IsChildOf(owner.transform)) return;

        // Only damage enemies
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Optionally destroy on hitting walls/ground (non-trigger colliders)
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
