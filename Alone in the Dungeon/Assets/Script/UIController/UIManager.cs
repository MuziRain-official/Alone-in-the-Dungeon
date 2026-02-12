using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Slider HealthBar;
    public GameObject GameOverPanel;
    void Start()
    {
        Instance = this;
        PlayerHealth.Instance.OnDamage += UpdateHealthBar;
        PlayerHealth.Instance.OnHeal += UpdateHealthBar;
        PlayerHealth.Instance.OnDeath += ShowGameOverPanel;
    }
    void Update()
    {
        
    }

    public void UpdateHealthBar(float damage)
    {
        HealthBar.value = PlayerHealth.Instance.currentHealth / PlayerHealth.Instance.maxHealth;
    }
    
    public void ShowGameOverPanel()
    {
        GameOverPanel.SetActive(true);
    }
}
