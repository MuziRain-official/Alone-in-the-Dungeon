using UnityEngine;
using GameFramework;

public class EnemyBullet : MonoBehaviour
{
    [Header("子弹伤害")]
    public int damage = 5;
    [Header("子弹速度")]
    public float speed = 10f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = transform.right * speed;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Obstacle") ||
            collision.CompareTag("Object") || collision.CompareTag("Background"))
        {
            Destroy(gameObject);

            var damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}
