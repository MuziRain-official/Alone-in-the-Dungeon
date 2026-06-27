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
        // 延迟3秒销毁，让子弹飞出屏幕后仍可见一段时间
        Destroy(gameObject, 3f);
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
