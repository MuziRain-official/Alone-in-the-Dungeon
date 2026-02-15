using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    public static CameraLogic instance;
    public Transform target;

    void Awake()
    {
        instance = this;
    }
    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }
}

