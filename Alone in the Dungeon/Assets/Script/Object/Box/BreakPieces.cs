using UnityEngine;

public class BreakPieces : MonoBehaviour
{
    public Vector2 moveDirection;
    public float moveSpeed = 3f;
    public float deceleration = 2f;
    public float lifeTime = 1f;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        moveDirection.x = Random.Range(-moveSpeed, moveSpeed);
        moveDirection.y = Random.Range(-moveSpeed, moveSpeed);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        transform.position += (Vector3)(moveDirection * Time.deltaTime);
        moveDirection = Vector2.Lerp(moveDirection, Vector2.zero, deceleration * Time.deltaTime);
        FadeOut();
    }

    private void FadeOut()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)        {
            Destroy(gameObject);
        }
        else
        {
            Color color = spriteRenderer.color;
            color.a = lifeTime; // 透明度随时间减少
            spriteRenderer.color = color;
        }
    }
}
