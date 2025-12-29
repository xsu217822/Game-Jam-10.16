using UnityEngine;

public class BottleProjectile : MonoBehaviour
{
    public float explosionRadius = 1.5f;
    public LayerMask enemyLayer;
    public GameObject explosionFxPrefab;

    private float damage;
    private Vector2 velocity;

    public void Init(float damage, Vector2 dir, float speed)
    {
        this.damage = damage;
        velocity = dir.normalized * speed;
    }

    private void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            Explode();
        }
    }


    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius,
            enemyLayer
        );

        foreach (var hit in hits)
        {
            hit.GetComponent<IDamageable>()
               ?.TakeDamage(damage);
        }

        // explosion
        if (explosionFxPrefab)
        {
            Instantiate(
                explosionFxPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }
}
