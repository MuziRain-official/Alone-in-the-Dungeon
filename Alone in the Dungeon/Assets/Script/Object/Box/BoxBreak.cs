using UnityEngine;

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
        AudioManager.instance.PlaySFX(4);
        Destroy(gameObject);
        for(int i = 0; i < piecesNum; i++)
        {
            Instantiate(brokenPieces[i], transform.position, transform.rotation);
        }
        Instantiate(phial, transform.position, transform.rotation);
    }

}
