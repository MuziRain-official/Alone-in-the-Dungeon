/* PlayerLogic.cs
 * 处理玩家的行为逻辑
 */

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLogic : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("移动速度")] 
    public float moveSpeed = 5f;
    [Tooltip("是否归一化输入向量")]
    public bool normalizeInput = true;
    
    [Header("物理设置")]
    [Tooltip("刚体阻力")] 
    public float drag = 5f;
    [Tooltip("是否冻结旋转")]
    public bool freezeRotation = true;
    
    private Vector2 m_currentMovementInput;
    private Rigidbody2D m_rb;

    [System.Obsolete]
    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

    // 供PlayerInput组件调用的方法
    public void OnMove(InputAction.CallbackContext context)
    {
        m_currentMovementInput = context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Vector2 input = m_currentMovementInput;
        
        // 处理斜向移动速度问题
        if (normalizeInput && input.magnitude > 1f)
        {
            input = input.normalized;
        }
        
        Vector2 movement = input * moveSpeed;
        m_rb.linearVelocity = movement;
    }
}