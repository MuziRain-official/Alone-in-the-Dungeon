using UnityEngine;
using GameFramework;

public class TrapDamage : MonoBehaviour
{
    public int damageAmount = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var damageable = collision.GetComponent<IDamageable>();
            damageable?.TakeDamage(damageAmount);
        }
    }
}
