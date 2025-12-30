using UnityEngine;

public class KeyboardBullet : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] letterSprites; 

    private float damage;
    private Vector2 velocity;

    public void Init(float damage, Vector2 dir, float speed)
    {
        this.damage = damage;
        velocity = dir.normalized * speed;

        if (letterSprites != null && letterSprites.Length > 0)
        {
            spriteRenderer.sprite =
                letterSprites[Random.Range(0, letterSprites.Length)];
        }
    }

    private void Update()
    {
        transform.position += (Vector3)(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null && !dmg.IsDead)
        {
            dmg.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
