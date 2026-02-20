using System;
using System.Collections;
using UnityEngine;
using GameFramework;

namespace EnemyController
{
    /// <summary>
    /// 敌人近战攻击 - 实现 IEnemyAttacker 接口
    /// 通过服务定位器获取玩家信息，通过 IDamageable 接口造成伤害
    /// </summary>
    public class EnemyAttack : MonoBehaviour, IEnemyAttacker
    {
        [Header("攻击设置")]
        public int damageAmount = 10;
        public float attackRange = 2f;
        public float attackCooldown = 2f;
        public float chargeSpeed = 8f;
        public float contactDamageInterval = 0.5f;

        // 攻击事件
        public event Action OnAttackStart;
        public event Action OnAttackEnd;

        private Transform playerTransform;
        private Rigidbody2D rb;
        private bool isAttacking;
        private float lastDamageTime;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            TryGetPlayerTransform();
        }

        void Update()
        {
            if (playerTransform == null)
            {
                TryGetPlayerTransform();
            }

            // 检测攻击条件
            if (!isAttacking && playerTransform != null)
            {
                float distance = Vector2.Distance(playerTransform.position, transform.position);
                if (distance <= attackRange) StartAttack();
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

        private void StartAttack()
        {
            isAttacking = true;
            OnAttackStart?.Invoke();

            Vector2 direction = (playerTransform.position - transform.position).normalized;
            StartCoroutine(ChargeAttack(direction));
            StartCoroutine(AttackCooldown());
        }

        private IEnumerator ChargeAttack(Vector2 direction)
        {
            float chargeTime = 0.3f;
            rb.linearVelocity = direction * chargeSpeed;

            yield return new WaitForSeconds(chargeTime);

            rb.linearVelocity = Vector2.zero;
            isAttacking = false;
            OnAttackEnd?.Invoke();
        }

        private IEnumerator AttackCooldown()
        {
            enabled = false;
            yield return new WaitForSeconds(attackCooldown);
            enabled = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isAttacking && collision.gameObject.CompareTag("Player"))
            {
                DealDamage(collision.gameObject);
                StopCoroutine(nameof(ChargeAttack));
                rb.linearVelocity = Vector2.zero;
                isAttacking = false;
                OnAttackEnd?.Invoke();
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (isAttacking && collision.gameObject.CompareTag("Player"))
            {
                DealDamage(collision.gameObject);
            }
        }

        private void DealDamage(GameObject player)
        {
            if (Time.time - lastDamageTime >= contactDamageInterval)
            {
                // 通过 IDamageable 接口造成伤害（解耦）
                var damageable = player.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damageAmount);
                }
                lastDamageTime = Time.time;
            }
        }

        public bool IsAttacking => isAttacking;
    }
}
