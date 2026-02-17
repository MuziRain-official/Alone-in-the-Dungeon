using UnityEngine;

public class SignPost : MonoBehaviour
{
    [Header("提示 UI（Canvas或文字面板）")]
    public GameObject signUI; // 拖入 SignCanvas

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            signUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            signUI.SetActive(false);
        }
    }
}