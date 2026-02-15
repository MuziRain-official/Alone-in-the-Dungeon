using System.Runtime.CompilerServices;
using UnityEngine;

public class Room : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CameraLogic.instance.ChangeTarget(transform);
        }
    }
}
