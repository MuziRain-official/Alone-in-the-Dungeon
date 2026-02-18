using UnityEngine;
using PlayerController;  // 引入以使用 PlayerManager

namespace EnemyController
{
    /// <summary>
    /// 远程敌人远离逻辑：当玩家进入过近距离时，敌人主动远离至安全距离。
    /// 该脚本独立于 EnemyMovement，通过临时禁用 EnemyMovement 来接管移动控制。
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(EnemyMovement))]
    public class EnemyRetreat : MonoBehaviour
    {
        [Header("远离参数")]
        [Tooltip("当与玩家距离小于等于该值时开始远离")]
        public float retreatThreshold = 2f;

        [Tooltip("当与玩家距离大于等于该值时停止远离")]
        public float retreatDistance = 3f;

        [Tooltip("远离时的移动速度，若为0则使用 EnemyMovement 的移动速度")]
        public float retreatSpeed = 0f;

        [Tooltip("是否在攻击时也允许远离（若为false，则攻击期间不会触发远离）")]
        public bool retreatWhileAttacking = true;

        // 组件引用
        private Rigidbody2D rb;
        private EnemyMovement enemyMovement;
        private IEnemyAttacker enemyAttacker;   // 用于检查攻击状态
        private Transform playerTransform;

        private bool isRetreating = false;      // 当前是否处于远离状态

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            enemyMovement = GetComponent<EnemyMovement>();
            enemyAttacker = GetComponent<IEnemyAttacker>();

            if (PlayerManager.Instance != null)
            {
                playerTransform = PlayerManager.Instance.PlayerTransform;
            }
        }

        private void Update()
        {
            // 如果玩家引用丢失，尝试重新获取
            if (playerTransform == null && PlayerManager.Instance != null)
            {
                playerTransform = PlayerManager.Instance.PlayerTransform;
            }
        }

        private void FixedUpdate()
        {
            if (playerTransform == null)
            {
                // 玩家不存在时，确保停止远离并恢复 EnemyMovement
                if (isRetreating)
                {
                    StopRetreat();
                }
                return;
            }

            float distance = Vector2.Distance(transform.position, playerTransform.position);

            // 攻击状态检查
            bool isAttacking = false;
            if (!retreatWhileAttacking && enemyAttacker != null)
            {
                isAttacking = enemyAttacker.IsAttacking;
            }

            // 开始远离条件：未在远离、距离小于等于阈值、且（允许攻击时远离或当前不在攻击）
            if (!isRetreating && distance <= retreatThreshold && !isAttacking)
            {
                StartRetreat();
            }
            // 停止远离条件：正在远离且距离达到安全值
            else if (isRetreating && distance >= retreatDistance)
            {
                StopRetreat();
            }

            // 如果正在远离，执行远离移动
            if (isRetreating)
            {
                RetreatFromPlayer();
            }
        }

        /// <summary>
        /// 开始远离：禁用 EnemyMovement，并标记状态
        /// </summary>
        private void StartRetreat()
        {
            isRetreating = true;
            if (enemyMovement != null)
            {
                enemyMovement.enabled = false;   // 禁用原有追踪逻辑
            }
            // 可在此处触发远离开始事件或动画
        }

        /// <summary>
        /// 停止远离：重新启用 EnemyMovement，并清空速度
        /// </summary>
        private void StopRetreat()
        {
            isRetreating = false;
            if (enemyMovement != null)
            {
                enemyMovement.enabled = true;    // 恢复原有追踪逻辑
            }
            rb.linearVelocity = Vector2.zero;    // 停止当前移动
            // 可在此处触发远离结束事件
        }

        /// <summary>
        /// 执行远离移动：方向为从玩家指向敌人，速度由 retreatSpeed 或 enemyMovement.moveSpeed 决定
        /// </summary>
        private void RetreatFromPlayer()
        {
            // 远离方向
            Vector2 direction = (transform.position - playerTransform.position).normalized;
            float speed = retreatSpeed > 0 ? retreatSpeed : enemyMovement.moveSpeed;
            rb.linearVelocity = direction * speed;

            // 保持面向玩家（与 EnemyMovement 一致）
            UpdateFacingDirection();
        }

        /// <summary>
        /// 根据玩家位置更新敌人朝向（X轴缩放）
        /// </summary>
        private void UpdateFacingDirection()
        {
            if (playerTransform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }

        private void OnDisable()
        {
            // 当此脚本被禁用时，确保 EnemyMovement 重新启用（避免一直处于禁用状态）
            if (isRetreating && enemyMovement != null)
            {
                enemyMovement.enabled = true;
            }
        }
    }
}