using UnityEngine;
using UnityEngine.UI;
using GameFramework;

public class UIManager : MonoBehaviour, IUIService
{
    public static UIManager Instance;

    [Header("UI组件")]
    public Slider HealthBar;
    public Slider BossHealthSlider;
    public GameObject GameOverPanel;
    public GameObject PauseMenu;

    [Header("场景设置")]
    public string NewGame;
    public string MainMenu;

    public bool isPaused;

    // Boss血条相关
    private EnemyController.EnemyHealth currentBossHealth;
    private bool isBossActive = false;

    private EventManager eventManager;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 注册到服务定位器
        if (ServiceLocator.Instance != null)
        {
            ServiceLocator.Instance.Register<IUIService>(this);
        }
    }

    void Start()
    {
        eventManager = EventManager.Instance;

        // 订阅事件
        if (PlayerController.PlayerHealth.Instance != null)
        {
            PlayerController.PlayerHealth.Instance.OnDamaged += OnPlayerDamaged;
            PlayerController.PlayerHealth.Instance.OnHealed += OnPlayerHealed;
            PlayerController.PlayerHealth.Instance.OnDied += OnPlayerDied;
        }

        // 也订阅事件中心
        eventManager?.Subscribe<PlayerDamageEvent>(OnPlayerDamageEvent);
        eventManager?.Subscribe<PlayerHealEvent>(OnPlayerHealEvent);
        eventManager?.Subscribe<PlayerDeathEvent>(OnPlayerDeathEvent);

        // 订阅Boss激活事件
        eventManager?.Subscribe<BossActivationEvent>(OnBossActivationEvent);
    }

    // 通过事件中心处理
    private void OnPlayerDamageEvent(PlayerDamageEvent e)
    {
        UpdateHealthBar(e.currentHealth, e.maxHealth);
    }

    private void OnPlayerHealEvent(PlayerHealEvent e)
    {
        UpdateHealthBar(e.currentHealth, e.maxHealth);
    }

    private void OnPlayerDeathEvent(PlayerDeathEvent e)
    {
        ShowGameOverPanel();
    }

    // Boss激活事件处理
    private void OnBossActivationEvent(BossActivationEvent e)
    {
        ShowBossHealthBar(e.bossHealth);
    }

    // 直接回调处理（兼容）
    private void OnPlayerDamaged(float damage)
    {
        var player = PlayerController.PlayerHealth.Instance;
        if (player != null)
        {
            UpdateHealthBar(player.CurrentHealth, player.MaxHealth);
        }
    }

    private void OnPlayerHealed(float healAmount)
    {
        var player = PlayerController.PlayerHealth.Instance;
        if (player != null)
        {
            UpdateHealthBar(player.CurrentHealth, player.MaxHealth);
        }
    }

    private void OnPlayerDied()
    {
        ShowGameOverPanel();
    }

    // IUIService 实现
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (HealthBar != null)
        {
            HealthBar.value = currentHealth / maxHealth;
        }
    }

    public void ShowGameOverPanel()
    {
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
            if (PauseMenu != null && PauseMenu.activeSelf)
            {
                PauseMenu.SetActive(false);
                isPaused = false;
                Time.timeScale = 1f;
            }
        }
    }

    public void TogglePauseMenu()
    {
        if (GameOverPanel != null && GameOverPanel.activeSelf) return;

        isPaused = !isPaused;
        if (PauseMenu != null)
        {
            PauseMenu.SetActive(isPaused);
        }
        Time.timeScale = isPaused ? 0f : 1f;

        eventManager?.Publish(new GamePauseEvent { isPaused = isPaused });
    }

    #region Boss血条管理

    /// <summary>
    /// 显示Boss血条
    /// </summary>
    public void ShowBossHealthBar(EnemyController.EnemyHealth bossHealth)
    {
        if (BossHealthSlider == null) return;

        currentBossHealth = bossHealth;
        isBossActive = true;

        // 设置血条初始值
        if (bossHealth != null)
        {
            BossHealthSlider.maxValue = bossHealth.MaxHealth;
            BossHealthSlider.value = bossHealth.CurrentHealth;
            BossHealthSlider.gameObject.SetActive(true);

            // 订阅受伤和死亡事件
            bossHealth.OnDamaged += OnBossDamaged;
            bossHealth.OnDied += OnBossDied;
        }
    }

    /// <summary>
    /// 隐藏Boss血条
    /// </summary>
    public void HideBossHealthBar()
    {
        if (BossHealthSlider != null)
        {
            BossHealthSlider.gameObject.SetActive(false);
        }

        // 取消订阅事件
        if (currentBossHealth != null)
        {
            currentBossHealth.OnDamaged -= OnBossDamaged;
            currentBossHealth.OnDied -= OnBossDied;
            currentBossHealth = null;
        }

        isBossActive = false;
    }

    /// <summary>
    /// 更新Boss血条
    /// </summary>
    public void UpdateBossHealth(float currentHealth, float maxHealth)
    {
        if (BossHealthSlider != null && isBossActive)
        {
            BossHealthSlider.maxValue = maxHealth;
            BossHealthSlider.value = currentHealth;
        }
    }

    private void OnBossDamaged(float damage)
    {
        if (currentBossHealth != null)
        {
            UpdateBossHealth(currentBossHealth.CurrentHealth, currentBossHealth.MaxHealth);
        }
    }

    private void OnBossDied()
    {
        // 延迟隐藏血条，让玩家看到Boss死亡
        Invoke(nameof(HideBossHealthBar), 1.5f);
    }

    #endregion

    public void ResumeGame()
    {
        isPaused = false;
        if (PauseMenu != null)
        {
            PauseMenu.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        // 停止背景音乐
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
        }

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(NewGame);
    }

    public void ReturnToMainMenu()
    {
        // 停止背景音乐，避免与主菜单音乐重叠
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
        }

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(MainMenu);
    }

    void OnDestroy()
    {
        // 注销服务
        if (ServiceLocator.Instance != null)
        {
            ServiceLocator.Instance.Unregister<IUIService>();
        }

        // 取消订阅事件
        if (PlayerController.PlayerHealth.Instance != null)
        {
            PlayerController.PlayerHealth.Instance.OnDamaged -= OnPlayerDamaged;
            PlayerController.PlayerHealth.Instance.OnHealed -= OnPlayerHealed;
            PlayerController.PlayerHealth.Instance.OnDied -= OnPlayerDied;
        }

        if (eventManager != null)
        {
            eventManager.Unsubscribe<PlayerDamageEvent>(OnPlayerDamageEvent);
            eventManager.Unsubscribe<PlayerHealEvent>(OnPlayerHealEvent);
            eventManager.Unsubscribe<PlayerDeathEvent>(OnPlayerDeathEvent);
            eventManager.Unsubscribe<BossActivationEvent>(OnBossActivationEvent);
        }
    }
}
