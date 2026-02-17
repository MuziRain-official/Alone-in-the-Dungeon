using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("门的状态")]
    public bool isOpen = false;

    [Header("门打开时的目标旋转角度")]
    public float openRotation = -90f; // 绕 Z 轴旋转，负数表示向外开（根据需求调整）

    [Header("门关闭时的旋转角度")]
    public float closedRotation = 0f;

    [Header("物理阻挡碰撞器（非触发器）")]
    public Collider2D physicsCollider; // 拖入子对象或同一对象的物理碰撞器

    [Header("可选：不同状态的图片")]
    public Sprite closedSprite;
    public Sprite openSprite;

    private SpriteRenderer spriteRenderer;
    private Collider2D triggerCollider; // 门上的触发器（用于交互检测）

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        // 确保门上有触发器用于交互检测
        triggerCollider = GetComponent<Collider2D>();
        if (triggerCollider != null && !triggerCollider.isTrigger)
            triggerCollider.isTrigger = true; // 强制设为触发器

        // 初始化物理阻挡
        if (physicsCollider != null)
        {
            physicsCollider.isTrigger = false;
            physicsCollider.enabled = !isOpen; // 关 → 启用；开 → 禁用
        }

        // 应用初始状态
        ApplyState();
    }

    public void Interact()
    {
        isOpen = !isOpen;
        ApplyState();
    }

    private void ApplyState()
    {
        // 旋转门
        float targetRotation = isOpen ? openRotation : closedRotation;
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);

        // 切换图片（如果提供了不同状态的图片）
        if (spriteRenderer != null)
        {
            if (isOpen && openSprite != null)
                spriteRenderer.sprite = openSprite;
            else if (!isOpen && closedSprite != null)
                spriteRenderer.sprite = closedSprite;
        }

        // 控制物理阻挡
        if (physicsCollider != null)
            physicsCollider.enabled = !isOpen;
    }
}