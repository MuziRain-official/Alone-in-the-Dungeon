using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Slider HealthBar;
    public PlayerHealth playerHealth;

    public GameObject GameOverPanel;
    void Start()
    {
        Instance = this;
        playerHealth = PlayerHealth.Instance;
        playerHealth.OnDamage += UpdateHealthBar;
        playerHealth.OnDeath += ShowGameOverPanel;
    }
    void Update()
    {
        
    }

    public void UpdateHealthBar(float damage)
    {
        HealthBar.value = playerHealth.currentHealth / playerHealth.maxHealth;
    }
    
    public void ShowGameOverPanel()
    {
        GameOverPanel.SetActive(true);
    }
}
