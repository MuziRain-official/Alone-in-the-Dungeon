using System;
using UnityEngine;
using GameFramework;
using EnemyController;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyHealth))]
public class BossLogic : MonoBehaviour
{
    [Header("固定点设置")]
    public Transform[] movePoints;           // 4个固定点

    [Header("战斗设置")]
    public int damageToPlayer = 1;           // 撞击玩家造成的伤害
    public float chaseRange = 8f;           // 追击范围
    public float arriveDistance = 0.5f;     // 到达固定点的判定距离
    public float shootDuration = 3f;        // 到达固定点后的射击持续时间

    [Header("射击设置")]
    public Transform shootPoint;
    public float shootInterval = 0.5f;       // 射击间隔
    public GameObject bullet;

    [Header("移动设置")]
    public float moveSpeed = 6f;
    public float chaseSpeed = 4f;

    [Header("击退设置")]
    public float knockbackSlowFactor = 0.75f;  // 减速系数
    public float knockbackDuration = 0.5f;      // 减速持续时间

    [Header("碰撞伤害设置")]
    public float damageCooldown = 0.5f;         // 撞击伤害冷却时间

    // 当前状态
    private enum BossState
    {
        Idle,           // 空闲/随机选择行为
        ChasePlayer,    // 追击玩家
        MoveToPoint,    // 移动到固定点
        Shooting        // 射击状态
    }
    private BossState currentState = BossState.Idle;

    // 状态相关
    private float stateTimer;
    private float shootTimer;
    private int currentPointIndex = -1;
    private bool isMoving = false;
    private float knockbackTimer = 0f;  // 击退/减速计时器
    private float damageCooldownTimer = 0f;  // 撞击伤害冷却计时器

    // 组件
    private Rigidbody2D rb;
    private Animator anim;
    private Transform playerTransform;
    private EnemyHealth enemyHealth;

    // 是否存活
    private bool IsAlive => enemyHealth != null && enemyHealth.IsAlive;

    // 激活状态
    private bool isActivated = false;

    // 音效播放状态
    private bool hasPlayedHalfHealthSfx = false;

    // 事件管理
    private EventManager eventManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();

        // 订阅受伤事件
        enemyHealth.OnDamaged += OnEnemyDamaged;
        enemyHealth.OnDied += OnEnemyDied;

        // 获取EventManager
        eventManager = EventManager.Instance;

        // 订阅玩家进入房间事件
        eventManager?.Subscribe<PlayerEnterRoomEvent>(OnPlayerEnterRoom);

        // 获取玩家引用
        TryGetPlayerTransform();
    }

    void OnDestroy()
    {
        // 取消订阅事件
        if (enemyHealth != null)
        {
            enemyHealth.OnDamaged -= OnEnemyDamaged;
            enemyHealth.OnDied -= OnEnemyDied;
        }

        // 取消订阅房间事件
        eventManager?.Unsubscribe<PlayerEnterRoomEvent>(OnPlayerEnterRoom);
    }

    private void OnPlayerEnterRoom(PlayerEnterRoomEvent evt)
    {
        // 检查是否是Boss所在房间
        Transform current = transform;
        while (current != null)
        {
            if (current.gameObject == evt.room)
            {
                ActivateBoss();
                break;
            }
            current = current.parent;
        }
    }

    private void ActivateBoss()
    {
        if (isActivated) return;

        isActivated = true;

        // 播放Boss战音乐
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayBossBattleMusic();
        }

        // 随机选择初始行为
        ChooseRandomBehavior();
    }

    private void OnEnemyDamaged(float _damage)
    {
        // 播放受伤动画
        if (anim != null)
        {
            anim.SetTrigger("isHurt");
        }

        // 检查血量是否过半（首次低于一半时切换到第二阶段音乐）
        if (!hasPlayedHalfHealthSfx && enemyHealth.CurrentHealth <= enemyHealth.MaxHealth / 2)
        {
            hasPlayedHalfHealthSfx = true;
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayBossSecondPhaseMusic();
            }
        }

        // 如果当前正在追击玩家，触发击退减速
        if (currentState == BossState.ChasePlayer)
        {
            knockbackTimer = knockbackDuration;
        }
    }

    private void OnEnemyDied()
    {
        // 播放死亡动画
        if (anim != null)
        {
            anim.SetTrigger("isDie");
        }

        // 停止移动
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        if (!IsAlive || !isActivated) return;

        // 更新状态计时器
        if (stateTimer > 0)
        {
            stateTimer -= Time.deltaTime;
        }

        // 射击计时器
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
        }

        // 击退/减速计时器
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
        }

        // 撞击伤害冷却计时器
        if (damageCooldownTimer > 0)
        {
            damageCooldownTimer -= Time.deltaTime;
        }

        // 获取玩家位置
        if (playerTransform == null)
        {
            TryGetPlayerTransform();
        }

        // 根据状态执行逻辑
        switch (currentState)
        {
            case BossState.Idle:
                if (stateTimer <= 0)
                {
                    ChooseRandomBehavior();
                }
                break;

            case BossState.ChasePlayer:
                HandleChasePlayer();
                break;

            case BossState.MoveToPoint:
                HandleMoveToPoint();
                break;

            case BossState.Shooting:
                HandleShooting();
                break;
        }

        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        if (!IsAlive || !isActivated) return;

        float currentSpeed = 0;
        Vector2 moveDir = Vector2.zero;

        if (currentState == BossState.ChasePlayer && playerTransform != null)
        {
            moveDir = (playerTransform.position - transform.position).normalized;
            currentSpeed = knockbackTimer > 0 ? chaseSpeed * knockbackSlowFactor : chaseSpeed;
        }
        else if (currentState == BossState.MoveToPoint && currentPointIndex >= 0 && movePoints[currentPointIndex] != null)
        {
            moveDir = (movePoints[currentPointIndex].position - transform.position).normalized;
            currentSpeed = moveSpeed;
        }

        rb.linearVelocity = moveDir * currentSpeed;

        if (moveDir.x != 0)
        {
            float scaleX = moveDir.x > 0 ? 1 : -1;
            transform.localScale = new Vector3(scaleX, 1, 1);
        }
    }

    private void ChooseRandomBehavior()
    {
        bool chasePlayer = UnityEngine.Random.value > 0.5f;

        if (chasePlayer && playerTransform != null &&
            Vector2.Distance(playerTransform.position, transform.position) <= chaseRange)
        {
            currentState = BossState.ChasePlayer;
            stateTimer = 5f;
        }
        else
        {
            if (movePoints != null && movePoints.Length > 0)
            {
                if (movePoints.Length > 1)
                {
                    int newIndex;
                    do
                    {
                        newIndex = UnityEngine.Random.Range(0, movePoints.Length);
                    } while (newIndex == currentPointIndex);
                    currentPointIndex = newIndex;
                }
                else
                {
                    currentPointIndex = 0;
                }
                currentState = BossState.MoveToPoint;
            }
            else
            {
                currentState = BossState.Idle;
                stateTimer = 2f;
            }
        }

        isMoving = true;
    }

    private void HandleChasePlayer()
    {
        if (stateTimer <= 0 || playerTransform == null)
        {
            currentState = BossState.Idle;
            stateTimer = 1f;
            isMoving = false;
        }
    }

    private void HandleMoveToPoint()
    {
        if (currentPointIndex < 0 || movePoints[currentPointIndex] == null)
        {
            currentState = BossState.Idle;
            stateTimer = 1f;
            isMoving = false;
            return;
        }

        float distance = Vector2.Distance(transform.position, movePoints[currentPointIndex].position);
        if (distance <= arriveDistance)
        {
            currentState = BossState.Shooting;
            stateTimer = shootDuration;
            shootTimer = 0;
            isMoving = false;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleShooting()
    {
        if (shootTimer <= 0)
        {
            Shoot();
            shootTimer = shootInterval;
        }

        if (stateTimer <= 0)
        {
            currentState = BossState.Idle;
            stateTimer = 1f;

            if (movePoints != null && movePoints.Length > 1)
            {
                int newIndex;
                do
                {
                    newIndex = UnityEngine.Random.Range(0, movePoints.Length);
                } while (newIndex == currentPointIndex);
                currentPointIndex = newIndex;
            }
        }

        if (playerTransform != null &&
            Vector2.Distance(playerTransform.position, transform.position) <= chaseRange)
        {
            currentState = BossState.ChasePlayer;
            stateTimer = 5f;
            isMoving = true;
        }
    }

    private void Shoot()
    {
        if (bullet != null && shootPoint != null && playerTransform != null)
        {
            GameObject bulletObj = Instantiate(bullet, shootPoint.position, Quaternion.identity);
            Vector2 direction = playerTransform.position - shootPoint.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bulletObj.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void UpdateAnimation()
    {
        if (anim != null)
        {
            anim.SetBool("isMove", isMoving);
        }
    }

    private void TryGetPlayerTransform()
    {
        var playerProvider = ServiceLocator.Instance?.Get<IPlayerProvider>();
        if (playerProvider != null)
        {
            playerTransform = playerProvider.PlayerTransform;
        }
        else if (PlayerController.PlayerManager.Instance != null)
        {
            playerTransform = PlayerController.PlayerManager.Instance.PlayerTransform;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsAlive || !isActivated) return;

        if (collision.gameObject.CompareTag("Player") && damageCooldownTimer <= 0)
        {
            IDamageable player = collision.gameObject.GetComponent<IDamageable>();
            if (player != null)
            {
                player.TakeDamage(damageToPlayer);
                damageCooldownTimer = damageCooldown;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (movePoints != null)
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < movePoints.Length; i++)
            {
                if (movePoints[i] != null)
                {
                    Gizmos.DrawWireSphere(movePoints[i].position, 0.5f);
                    Gizmos.DrawLine(transform.position, movePoints[i].position);
                }
            }
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
