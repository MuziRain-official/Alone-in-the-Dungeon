using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("子弹速度")]
    public float speed = 10f;
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
}
