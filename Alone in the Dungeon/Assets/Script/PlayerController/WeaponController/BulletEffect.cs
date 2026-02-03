using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [Header("销毁延迟时间")]
    public float destroyDelay = 0.3f; // 默认1秒后销毁

    void Start()
    {
        // 延迟指定时间后销毁对象
        Destroy(gameObject, destroyDelay);
    }
}