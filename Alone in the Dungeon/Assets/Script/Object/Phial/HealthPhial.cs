using UnityEngine;
using GameFramework;

public class HealthPhial : MonoBehaviour
{
    public int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var healable = collision.GetComponent<IHealable>();
            if (healable != null)
            {
                healable.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
