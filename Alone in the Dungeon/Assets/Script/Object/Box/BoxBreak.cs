using UnityEngine;
using GameFramework;

public class BoxBreak : MonoBehaviour
{
    public GameObject[] brokenPieces;
    public GameObject phial;
    private int piecesNum = 6;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("PlayerBullet"))
        {
            SmashBox();
        }
    }

    private void SmashBox()
    {
        // 播放音效 - 优先使用服务定位器
        var audioService = ServiceLocator.Instance?.Get<IAudioService>();
        if (audioService != null)
        {
            audioService.PlaySFX(4);
        }
        else if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(4);
        }

        Destroy(gameObject);
        for (int i = 0; i < piecesNum; i++)
        {
            Instantiate(brokenPieces[i], transform.position, transform.rotation);
        }
        Instantiate(phial, transform.position, transform.rotation);
    }
}
