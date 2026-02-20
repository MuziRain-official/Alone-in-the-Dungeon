using UnityEngine;
using GameFramework;

namespace EnemyController
{
    /// <summary>
    /// 远程敌人远离逻辑：当玩家进入过近距离时，敌人主动远离至安全距离。
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(EnemyMovement))]
    public class EnemyRetreat : MonoBehaviour
    {
        [Header("远离参数")]
        public float retreatThreshold = 2f;
        public float retreatDistance = 3f;
        public float retreatSpeed = 0f;
        public bool retreatWhileAttacking = true;

        private Rigidbody2D rb;
        private EnemyMovement enemyMovement;
        private IEnemyAttacker enemyAttacker;
        private Transform playerTransform;

        private bool isRetreating = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            enemyMovement = GetComponent<EnemyMovement>();
            enemyAttacker = GetComponent<IEnemyAttacker>();
            TryGetPlayerTransform();
        }

        private void Update()
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
            else if (PlayerController.PlayerManager.Instance != null)
            {
                playerTransform = PlayerController.PlayerManager.Instance.PlayerTransform;
            }
        }

        private void FixedUpdate()
        {
            if (playerTransform == null)
            {
                if (isRetreating)
                {
                    StopRetreat();
                }
                return;
            }

            float distance = Vector2.Distance(transform.position, playerTransform.position);

            bool isAttacking = false;
            if (!retreatWhileAttacking && enemyAttacker != null)
            {
                isAttacking = enemyAttacker.IsAttacking;
            }

            if (!isRetreating && distance <= retreatThreshold && !isAttacking)
            {
                StartRetreat();
            }
            else if (isRetreating && distance >= retreatDistance)
            {
                StopRetreat();
            }

            if (isRetreating)
            {
                RetreatFromPlayer();
            }
        }

        private void StartRetreat()
        {
            isRetreating = true;
            if (enemyMovement != null)
            {
                enemyMovement.enabled = false;
            }
        }

        private void StopRetreat()
        {
            isRetreating = false;
            if (enemyMovement != null)
            {
                enemyMovement.enabled = true;
            }
            rb.linearVelocity = Vector2.zero;
        }

        private void RetreatFromPlayer()
        {
            Vector2 direction = (transform.position - playerTransform.position).normalized;
            float speed = retreatSpeed > 0 ? retreatSpeed : enemyMovement.moveSpeed;
            rb.linearVelocity = direction * speed;
            UpdateFacingDirection();
        }

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
            if (isRetreating && enemyMovement != null)
            {
                enemyMovement.enabled = true;
            }
        }
    }
}
