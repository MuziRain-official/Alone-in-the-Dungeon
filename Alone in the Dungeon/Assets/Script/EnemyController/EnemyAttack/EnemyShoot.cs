using System;
using System.Collections;
using UnityEngine;
using GameFramework;

namespace EnemyController
{
    /// <summary>
    /// 敌人射击攻击 - 实现 IEnemyAttacker 接口
    /// </summary>
    public class EnemyShoot : MonoBehaviour, IEnemyAttacker
    {
        [Header("射击设置")]
        public float attackRange = 5f;
        public float attackCooldown = 2f;
        public float shootingDuration = 1f;

        [Header("武器设置")]
        public GameObject soldierWeapon;

        // 攻击事件
        public event Action OnAttackStart;
        public event Action OnAttackEnd;

        private Transform playerTransform;
        private Rigidbody2D rb;
        private bool isAttacking;
        private EnemyFire enemyFire;
        private Coroutine shootingCoroutine;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            TryGetPlayerTransform();

            if (soldierWeapon != null)
            {
                soldierWeapon.SetActive(false);
                enemyFire = soldierWeapon.GetComponent<EnemyFire>();
            }
        }

        void Update()
        {
            if (playerTransform == null)
            {
                TryGetPlayerTransform();
            }

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
            shootingCoroutine = StartCoroutine(Shoot());
            StartCoroutine(AttackCooldown());
        }

        private IEnumerator Shoot()
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            if (soldierWeapon != null)
            {
                soldierWeapon.SetActive(true);

                if (enemyFire != null)
                {
                    enemyFire.StartFiring();
                }

                yield return new WaitForSeconds(shootingDuration);

                if (enemyFire != null)
                {
                    enemyFire.StopFiring();
                }
                soldierWeapon.SetActive(false);
            }

            isAttacking = false;
            OnAttackEnd?.Invoke();
        }

        private IEnumerator AttackCooldown()
        {
            enabled = false;
            yield return new WaitForSeconds(attackCooldown);
            enabled = true;
        }

        public bool IsAttacking => isAttacking;
    }
}
