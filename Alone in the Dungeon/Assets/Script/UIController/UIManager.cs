using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Slider HealthBar;
    public GameObject GameOverPanel;
    public GameObject PauseMenu;
    public string NewGame;      // 重新开始加载的场景名称
    public string MainMenu;     // 主菜单场景名称
    public bool isPaused;

    void Start()
    {
        // 单例初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 绑定玩家生命值事件
        PlayerHealth.Instance.OnDamage += UpdateHealthBar;
        PlayerHealth.Instance.OnHeal += UpdateHealthBar;
        PlayerHealth.Instance.OnDeath += ShowGameOverPanel;
    }

    // 更新血量显示
    public void UpdateHealthBar(float damage)
    {
        HealthBar.value = PlayerHealth.Instance.currentHealth / PlayerHealth.Instance.maxHealth;
    }

    // 显示游戏结束面板（同时关闭暂停菜单，确保状态一致）
    public void ShowGameOverPanel()
    {
        GameOverPanel.SetActive(true);
        if (PauseMenu.activeSelf)
        {
            PauseMenu.SetActive(false);
            isPaused = false;
            Time.timeScale = 1f; // 游戏结束通常让时间继续（或根据需要保持暂停）
        }
    }

    /// <summary>
    /// 由 InputSystem 调用的暂停切换方法（打开/关闭暂停菜单）
    /// </summary>
    public void TogglePauseMenu()
    {
        // 如果游戏已结束，不允许暂停
        if (GameOverPanel.activeSelf) return;

        isPaused = !isPaused;
        PauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    /// <summary>
    /// 继续游戏（由“继续游戏”按钮调用）
    /// </summary>
    public void ResumeGame()
    {
        isPaused = false;
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 重新开始游戏（由“重新开始”按钮调用）
    /// </summary>
    public void RestartGame()
    {
        Time.timeScale = 1f; // 确保时间恢复
        UnityEngine.SceneManagement.SceneManager.LoadScene(NewGame);
    }

    /// <summary>
    /// 返回主菜单（由主菜单按钮调用）
    /// </summary>
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenu);
    }
}