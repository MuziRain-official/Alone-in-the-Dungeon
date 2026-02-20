using UnityEngine;
using System;
using GameFramework;

namespace EnemyController
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("移动设置")]
        public float moveSpeed = 3f;
        public float trackingRange = 5f;

        [Header("巡逻设置")]
        public Vector2[] patrolOffsets;             // 相对于初始位置的偏移点数组
        public float patrolPauseDuration = 1f;      // 到达巡逻点后的停留时间
        public float patrolTimeout = 3f;             // 移动超时时间

        // 移动状态事件
        public event Action<bool> OnMovementChanged;

        private Rigidbody2D rb;
        private Transform playerTransform;
        private bool isMoving = false;
        private IEnemyAttacker enemyAttacker;

        // 巡逻相关变量
        private Vector2 patrolCenter;                // 初始位置（中心点）
        private Vector2 patrolTarget;                 // 当前巡逻目标点
        private float patrolPauseTimer = 0f;
        private float moveStartTime;

        // 敌人状态枚举
        private enum EnemyState
        {
            Tracking,       // 追踪玩家
            Patrolling      // 巡逻
        }
        private EnemyState currentState;

        private const float ARRIVE_DISTANCE = 0.1f;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            enemyAttacker = GetComponent<IEnemyAttacker>();

            patrolCenter = transform.position;

            // 通过服务定位器获取玩家信息（解耦）
            TryGetPlayerTransform();

            // 初始化状态
            if (playerTransform != null && Vector2.Distance(playerTransform.position, transform.position) <= trackingRange)
            {
                currentState = EnemyState.Tracking;
            }
            else
            {
                EnterPatrolState();
            }
        }

        void Update()
        {
            if (playerTransform == null)
            {
                TryGetPlayerTransform();
            }
        }

        private void TryGetPlayerTransform()
        {
            var playerProvider = ServiceLocator.Instance?.Get<IPlayerProvider>();
            if (playerProvider != null)
            {
                playerTransform = playerProvider.PlayerTransform;
            }
            // 备用方案：直接检查PlayerManager
            else if (PlayerController.PlayerManager.Instance != null)
            {
                playerTransform = PlayerController.PlayerManager.Instance.PlayerTransform;
            }
        }

        private void FixedUpdate()
        {
            // 如果正在攻击，不执行任何移动逻辑
            if (enemyAttacker != null && enemyAttacker.IsAttacking)
            {
                return;
            }

            bool playerInRange = playerTransform != null &&
                                Vector2.Distance(playerTransform.position, transform.position) <= trackingRange;
            switch (currentState)
            {
                case EnemyState.Tracking:
                    if (playerInRange)
                    {
                        MoveTowards(playerTransform.position);
                    }
                    else
                    {
                        // 丢失玩家，进入巡逻状态
                        EnterPatrolState();
                    }
                    break;

                case EnemyState.Patrolling:
                    if (playerInRange)
                    {
                        // 发现玩家，切换为追踪
                        currentState = EnemyState.Tracking;
                        StopMoving();
                    }
                    else
                    {
                        HandlePatrolling();
                    }
                    break;
            }
        }

        private void HandlePatrolling()
        {
            if (isMoving)
            {
                // 超时检查
                if (Time.time - moveStartTime > patrolTimeout)
                {
                    StopMoving();
                    patrolPauseTimer = 0f;
                    return;
                }

                MoveTowards(patrolTarget);

                // 到达目标点
                if (Vector2.Distance(transform.position, patrolTarget) <= ARRIVE_DISTANCE)
                {
                    StopMoving();
                    patrolPauseTimer = patrolPauseDuration;
                }
            }
            else
            {
                patrolPauseTimer -= Time.fixedDeltaTime;
                if (patrolPauseTimer <= 0f)
                {
                    ChooseNewPatrolTarget();
                    moveStartTime = Time.time;
                    MoveTowards(patrolTarget);
                }
            }
        }

        private void ChooseNewPatrolTarget()
        {
            if (patrolOffsets == null || patrolOffsets.Length == 0)
            {
                patrolTarget = transform.position;
                Debug.LogWarning("巡逻偏移数组为空，敌人将停留在原地！");
                return;
            }

            int index = UnityEngine.Random.Range(0, patrolOffsets.Length);
            patrolTarget = patrolCenter + patrolOffsets[index];
        }

        private void EnterPatrolState()
        {
            currentState = EnemyState.Patrolling;
            StopMoving();
            patrolPauseTimer = patrolPauseDuration;
        }

        private void MoveTowards(Vector2 target)
        {
            Vector2 direction = (target - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;

            if (!isMoving)
            {
                isMoving = true;
                OnMovementChanged?.Invoke(true);
            }

            UpdateFacingDirection(direction);
        }

        private void StopMoving()
        {
            rb.linearVelocity = Vector2.zero;

            if (isMoving)
            {
                isMoving = false;
                OnMovementChanged?.Invoke(false);
            }
        }

        private void UpdateFacingDirection(Vector2 direction)
        {
            if (direction.x != 0)
            {
                float scaleX = direction.x < 0 ? -1 : 1;
                transform.localScale = new Vector3(scaleX, 1, 1);
            }
        }

        public bool IsMoving => isMoving;
    }
}
