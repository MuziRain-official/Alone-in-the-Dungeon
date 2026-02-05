using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("子弹伤害")]
    public int damage = 5;
    [Header("子弹速度")]
    public float speed = 10f;
    [Header("子弹碰撞特效")]
    // public GameObject hitEffect;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = transform.right * speed;
    }
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 碰到玩家或障碍物时销毁子弹
        if (collision.CompareTag("Player") || collision.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
            // Instantiate(hitEffect, transform.position, transform.rotation);
            // 调用接口对敌人造成伤害
            var damageable = collision.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
    }
}
