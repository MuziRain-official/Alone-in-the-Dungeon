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

        // 统一使用事件中心订阅
        eventManager?.Subscribe<PlayerDamageEvent>(OnPlayerDamageEvent);
        eventManager?.Subscribe<PlayerHealEvent>(OnPlayerHealEvent);
        eventManager?.Subscribe<PlayerDeathEvent>(OnPlayerDeathEvent);

        // 订阅Boss相关事件
        eventManager?.Subscribe<BossActivationEvent>(OnBossActivationEvent);
        eventManager?.Subscribe<BossDamageEvent>(OnBossDamageEvent);
        eventManager?.Subscribe<EnemyDeathEvent>(OnEnemyDeathEvent);
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

    // Boss受伤事件处理
    private void OnBossDamageEvent(BossDamageEvent e)
    {
        UpdateBossHealth(e.currentHealth, e.maxHealth);
    }

    // 敌人死亡事件处理（用于Boss死亡）
    private void OnEnemyDeathEvent(EnemyDeathEvent e)
    {
        // 检查是否是Boss死亡
        if (currentBossHealth != null && e.enemy == currentBossHealth.gameObject)
        {
            // 延迟隐藏血条，让玩家看到Boss死亡
            Invoke(nameof(HideBossHealthBar), 1.5f);
        }
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

        currentBossHealth = null;
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
        // 清空所有事件订阅，防止重新开始时事件累积
        eventManager?.ClearAllEvents();

        // 清空所有服务，防止重新开始时服务累积
        ServiceLocator.Instance?.ClearAll();

        // 停止背景音乐 - 优先使用服务定位器，备用单例
        var audioService = ServiceLocator.Instance?.Get<IAudioService>();
        if (audioService != null)
        {
            audioService.StopMusic();
        }
        else if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
        }

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(NewGame);
    }

    public void ReturnToMainMenu()
    {
        // 清空所有事件订阅，防止返回主菜单时事件累积
        eventManager?.ClearAllEvents();

        // 清空所有服务，防止返回主菜单时服务累积
        ServiceLocator.Instance?.ClearAll();

        // 停止背景音乐，避免与主菜单音乐重叠
        var audioService = ServiceLocator.Instance?.Get<IAudioService>();
        if (audioService != null)
        {
            audioService.StopMusic();
        }
        else if (AudioManager.instance != null)
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

        // 取消事件中心订阅
        if (eventManager != null)
        {
            eventManager.Unsubscribe<PlayerDamageEvent>(OnPlayerDamageEvent);
            eventManager.Unsubscribe<PlayerHealEvent>(OnPlayerHealEvent);
            eventManager.Unsubscribe<PlayerDeathEvent>(OnPlayerDeathEvent);
            eventManager.Unsubscribe<BossActivationEvent>(OnBossActivationEvent);
            eventManager.Unsubscribe<BossDamageEvent>(OnBossDamageEvent);
            eventManager.Unsubscribe<EnemyDeathEvent>(OnEnemyDeathEvent);
        }
    }
}
