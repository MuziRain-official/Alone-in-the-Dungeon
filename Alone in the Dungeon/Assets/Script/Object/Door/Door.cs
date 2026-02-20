using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("门的状态")]
    public bool isOpen = false;

    [Header("旋转角度")]
    public float openRotation = -90f;   // 打开时的本地旋转（Z轴）
    public float closedRotation = 0f;    // 关闭时的本地旋转

    [Header("碰撞器（请拖拽赋值）")]
    public Collider2D triggerCollider;   // 用于交互的触发器（勾选 Is Trigger）
    public Collider2D physicsCollider;   // 用于物理阻挡的碰撞器（不勾选 Is Trigger）

    [Header("门的不同状态图片")]
    public Sprite closedSprite;
    public Sprite openSprite;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // 获取或添加 SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        // 确保触发器正确设置（以防万一）
        if (triggerCollider != null)
            triggerCollider.isTrigger = true;
        if (physicsCollider != null)
            physicsCollider.isTrigger = false;

        // 应用初始状态（旋转、图片、碰撞器大小）
        ApplyState();
    }

    // 交互方法（由其他脚本调用，如玩家按下键时）
    public void Interact()
    {
        isOpen = !isOpen;
        ApplyState();
    }

    private void ApplyState()
    {
        // 1. 旋转门
        float targetRotation = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);

        // 2. 切换图片
        Sprite targetSprite = isOpen ? openSprite : closedSprite;
        if (spriteRenderer != null && targetSprite != null)
        {
            spriteRenderer.sprite = targetSprite;

            // 3. 更新物理阻挡碰撞器的大小（假设是 BoxCollider2D）
            if (physicsCollider != null)
            {
                BoxCollider2D box = physicsCollider as BoxCollider2D;
                if (box != null)
                {
                    Bounds bounds = targetSprite.bounds;          // 获取图片的边界（单位：世界单位）
                    box.size = bounds.size;                        // 设置碰撞器大小
                    box.offset = bounds.center;                    // 设置偏移，使碰撞器居中
                }
            }

            // 4. 更新触发器碰撞器的大小（同样假设是 BoxCollider2D）
            if (triggerCollider != null)
            {
                BoxCollider2D triggerBox = triggerCollider as BoxCollider2D;
                if (triggerBox != null)
                {
                    Bounds bounds = targetSprite.bounds;
                    triggerBox.size = bounds.size;                 // 可乘以 1.2 让触发范围稍大（方便交互）
                    triggerBox.offset = bounds.center;
                }
            }
        }

        // 物理阻挡碰撞器始终启用（不需要额外控制，它一直有效）
    }
}