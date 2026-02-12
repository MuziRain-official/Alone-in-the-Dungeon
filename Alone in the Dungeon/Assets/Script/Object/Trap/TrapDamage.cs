using UnityEngine;

public class TrapDamage : MonoBehaviour
{
    public int damageAmount = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth.Instance.TakeDamage(damageAmount);
        }
    }
}
