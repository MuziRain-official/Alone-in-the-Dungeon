/* PlayerLogic.cs
 * 处理玩家的行为逻辑
 */

using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerLogic : MonoBehaviour
{
    public float moveSpeed;
    private Vector2 m_currentMovementInput; //存储当前输入向量

    //供PlayerInput组件调用的方法
    public void OnMove(InputAction.CallbackContext context)
    {
        // 当按键按下或松开时，此方法会被调用
        m_currentMovementInput = context.ReadValue<Vector2>();
    }

    void Update()
    {
        //在Update中应用持续移动
        Vector3 movement = new Vector3(m_currentMovementInput.x, m_currentMovementInput.y, 0) * moveSpeed * Time.deltaTime;
        transform.position += movement;
    }
}