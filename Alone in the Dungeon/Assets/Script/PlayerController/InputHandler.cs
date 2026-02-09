/* InputHandler.cs 
* 输入处理脚本，处理玩家输入
*/
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    
    public class InputHandler : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public InputAction InputActions;
             
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }
    }
}