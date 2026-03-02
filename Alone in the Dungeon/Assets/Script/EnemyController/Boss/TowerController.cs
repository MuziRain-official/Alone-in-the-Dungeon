using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Boss房间的塔楼控制器
/// - Boss被击败后激活
/// - 玩家与其交互后可返回主菜单
/// </summary>
public class TowerController : MonoBehaviour
{
    [Header("场景设置")]
    public string mainMenuSceneName = "TitleScene";  // 主菜单场景名称

    [Header("交互设置")]
    public float interactRange = 1.5f;  // 交互范围
    public string playerTag = "Player";  // 玩家标签

    private Collider2D towerCollider;
    private PlayerInput playerInput;
    private Transform playerTransform;

    private void Awake()
    {
        towerCollider = GetComponent<Collider2D>();

        // 初始状态设为失活
        gameObject.SetActive(false);

        if (towerCollider != null)
        {
            towerCollider.enabled = false;
        }
    }

    private void OnDestroy()
    {
        // 取消输入事件订阅
        if (playerInput != null)
        {
            playerInput.actions["Interact"].started -= OnInteract;
        }
    }

    /// <summary>
    /// 激活塔楼 - 由BossLogic调用
    /// </summary>
    public void ActivateTower()
    {
        gameObject.SetActive(true);

        if (towerCollider != null)
        {
            towerCollider.enabled = true;
        }

        // 查找并订阅玩家输入
        TryFindPlayer();
    }

    /// <summary>
    /// 失活塔楼
    /// </summary>
    public void DeactivateTower()
    {
        if (towerCollider != null)
        {
            towerCollider.enabled = false;
        }
        gameObject.SetActive(false);
    }

    private void TryFindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player != null)
        {
            playerTransform = player.transform;
            playerInput = player.GetComponent<PlayerInput>();

            if (playerInput != null)
            {
                playerInput.actions["Interact"].started += OnInteract;
            }
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        // 检查对象是否已被销毁
        if (this == null || gameObject == null) return;

        if (!gameObject.activeInHierarchy) return;

        // 检查玩家是否在交互范围内
        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            if (distance <= interactRange)
            {
                ReturnToMainMenu();
            }
        }
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void ReturnToMainMenu()
    {
        // 清空所有事件订阅，防止返回主菜单时事件累积
        GameFramework.EventManager.Instance?.ClearAllEvents();

        // 清空所有服务，防止返回主菜单时服务累积
        GameFramework.ServiceLocator.Instance?.ClearAll();

        // 停止背景音乐
        var audioService = GameFramework.ServiceLocator.Instance?.Get<GameFramework.IAudioService>();
        if (audioService != null)
        {
            audioService.StopMusic();
        }
        else if (AudioManager.instance != null)
        {
            AudioManager.instance.StopMusic();
        }

        // 恢复时间
        Time.timeScale = 1f;

        // 加载主菜单
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // 调试用：在编辑器中显示交互范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
