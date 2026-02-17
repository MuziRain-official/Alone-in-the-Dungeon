using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Door currentDoor; // 当前处于触发器内的门

    /// <summary>
    /// 由 PlayerInput 的事件调用的方法
    /// </summary>
    public void TryInteract()
    {
        if (currentDoor != null)
        {
            currentDoor.Interact(); // 让当前门处理交互
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Door door = other.GetComponent<Door>();
        if (door != null)
        {
            currentDoor = door;
            // 可选：显示交互提示（如“按 F 开门”）
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Door door = other.GetComponent<Door>();
        if (door != null && door == currentDoor)
        {
            currentDoor = null;
            // 可选：隐藏交互提示
        }
    }
}