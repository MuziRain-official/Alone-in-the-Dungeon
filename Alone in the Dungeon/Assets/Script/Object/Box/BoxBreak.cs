using UnityEngine;

public class BoxBreak : MonoBehaviour
{
    public GameObject[] brokenPieces;
    private int piecesNum = 6;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SmashBox();
        }
    }
    private void SmashBox()
    {
        Destroy(gameObject);
        for(int i = 0; i < piecesNum; i++)
        {
            Instantiate(brokenPieces[i], transform.position, transform.rotation);
        }
    }

}
