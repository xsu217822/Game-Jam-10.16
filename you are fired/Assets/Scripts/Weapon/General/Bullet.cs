using UnityEngine;

public class Bullet : MonoBehaviour
{
    float damage;
    Vector2 dir;
    float speed;

    public void Init(float dmg, Vector2 direction, float spd)
    {
        damage = dmg;
        dir = direction.normalized;
        speed = spd;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IDamageable>(out var dmg))
        {
            dmg.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
