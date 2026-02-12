using UnityEngine;

public class HealthPhial : MonoBehaviour
{
    public int healAmount = 20;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth.Instance.Heal(healAmount); // 治疗玩家
            Destroy(gameObject); // 吃掉药水后销毁
        }
    }
}
