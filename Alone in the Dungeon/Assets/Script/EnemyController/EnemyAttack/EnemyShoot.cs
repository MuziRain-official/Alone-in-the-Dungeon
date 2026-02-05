using UnityEngine;
using System.Collections;
using PlayerController;

namespace EnemyController
{
    public class EnemyShoot : MonoBehaviour, IEnemyAttacker
    {
        [Header("射击设置")]
        public float attackRange = 5f;
        public float attackCooldown = 2f;
        public float shootingDuration = 1f; // 射击持续时间
        
        [Header("武器设置")]
        public GameObject soldierWeapon; // 拖拽赋值：SoldierWeapon子对象
        
        // 实现接口中的事件
        public event System.Action OnAttackStart;
        public event System.Action OnAttackEnd;
        
        private Transform playerTransform;
        private Rigidbody2D rb;
        private bool isAttacking;
        private EnemyFire enemyFire; // 改为EnemyFire组件
        private Coroutine shootingCoroutine; // 射击协程引用
        
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            
            if (PlayerManager.Instance != null)
            {
                playerTransform = PlayerManager.Instance.PlayerTransform;
            }
            
            // 确保武器子对象存在
            if (soldierWeapon != null)
            {
                soldierWeapon.SetActive(false); // 开始时失活
                enemyFire = soldierWeapon.GetComponent<EnemyFire>();
            }
            else
            {
                // 如果没有手动赋值，尝试自动查找
                soldierWeapon = transform.Find("SoldierWeapon")?.gameObject;
                if (soldierWeapon != null)
                {
                    soldierWeapon.SetActive(false);
                    enemyFire = soldierWeapon.GetComponent<EnemyFire>();
                }
                else
                {
                    Debug.LogWarning("未找到SoldierWeapon子对象，请手动赋值或确保子对象名称正确");
                }
            }
        }
        
        void Update()
        {            
            // 检测攻击条件
            if (!isAttacking && playerTransform != null)
            {
                float distance = Vector2.Distance(playerTransform.position, transform.position);
                if (distance <= attackRange) StartAttack();
            }
        }
        
        private void StartAttack()
        {
            isAttacking = true;
            OnAttackStart?.Invoke(); // 触发攻击开始事件
            
            // 执行射击行为
            shootingCoroutine = StartCoroutine(Shoot());
            
            // 开始冷却
            StartCoroutine(AttackCooldown());
        }
        
        private IEnumerator Shoot()
        {
            // 射击前停止移动
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            
            // 激活武器子对象
            if (soldierWeapon != null)
            {
                soldierWeapon.SetActive(true);
                
                // 如果需要，可以在这里设置武器的朝向
                if (playerTransform != null)
                {
                    // 计算朝向玩家的方向
                    Vector3 direction = playerTransform.position - transform.position;
                    direction.y = 0; // 保持水平，如果需要的话
                    soldierWeapon.transform.right = direction.normalized;
                }
                
                // 通知EnemyFire开始射击
                if (enemyFire != null)
                {
                    enemyFire.StartFiring();
                }
                
                Debug.Log("敌人开始射击");
                
                // 等待射击持续时间
                yield return new WaitForSeconds(shootingDuration);
                
                // 结束射击
                Debug.Log("敌人结束射击");
                
                // 停止射击并失活武器子对象
                if (enemyFire != null)
                {
                    enemyFire.StopFiring();
                }
                soldierWeapon.SetActive(false);
            }
            else
            {
                // 没有武器子对象，只等待射击持续时间
                yield return new WaitForSeconds(shootingDuration);
            }
            
            isAttacking = false;
            OnAttackEnd?.Invoke(); // 触发攻击结束事件
        }
        
        private IEnumerator AttackCooldown()
        {
            // 禁用攻击检测
            enabled = false;
            yield return new WaitForSeconds(attackCooldown);
            enabled = true;
        }
        
        // 接口实现
        public bool IsAttacking => isAttacking;
        
        // 如果需要提前停止射击（例如敌人死亡）
        public void StopShooting()
        {
            if (shootingCoroutine != null)
            {
                StopCoroutine(shootingCoroutine);
                isAttacking = false;
                
                // 确保武器被失活
                if (enemyFire != null)
                {
                    enemyFire.StopFiring();
                }
                if (soldierWeapon != null)
                {
                    soldierWeapon.SetActive(false);
                }
                
                OnAttackEnd?.Invoke();
            }
        }
    }
}