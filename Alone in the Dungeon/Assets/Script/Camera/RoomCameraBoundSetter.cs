using UnityEngine;
using Unity.Cinemachine;

public class RoomCameraBoundSetter : MonoBehaviour
{
    [Header("房间边界（自动寻找同级的 Virtual 物体）")]
    public Collider2D roomBounds; 

    private CinemachineConfiner2D confiner;

    private void Start()
    {
        // 如果没有手动指定边界，尝试从同级子物体 Virtual 获取
        if (roomBounds == null)
        {
            Transform virtualObj = transform.parent.Find("Virtual");
            if (virtualObj != null)
                roomBounds = virtualObj.GetComponent<Collider2D>();
        }
        CinemachineBrain brain = Camera.main?.GetComponent<CinemachineBrain>();
        if (brain != null && brain.ActiveVirtualCamera != null)
        {
            // ActiveVirtualCamera 返回 ICinemachineCamera，可以尝试转换为具体的 CinemachineCamera
            if (brain.ActiveVirtualCamera is CinemachineCamera vcam)
            {
                confiner = vcam.GetComponent<CinemachineConfiner2D>();
            }
        }

        // 如果上述方法失败，回退到查找场景中的第一个 CinemachineCamera（仅作备用）
        if (confiner == null)
        {
            CinemachineCamera vcam = FindFirstObjectByType<CinemachineCamera>();
            if (vcam != null)
                confiner = vcam.GetComponent<CinemachineConfiner2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的是否为玩家
        if (other.CompareTag("Player") && confiner != null && roomBounds != null)
        {
            // 更新边界并刷新缓存
            confiner.BoundingShape2D = roomBounds;
            confiner.InvalidateBoundingShapeCache();
        }
    }
}