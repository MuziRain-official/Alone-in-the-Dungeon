using PlayerController;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    [Header("子弹设置")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    
    [Header("射击参数")]
    public float fireRate = 0.2f; // 射击间隔时间
    public bool autoFire = true;  // 敌人武器总是自动射击
    
    private float nextFireTime = 0f;
    private bool isFiring = false;
    private Transform playerTransform; // 玩家Transform
    
    void Start()
    {
        // 获取玩家Transform
        if (PlayerManager.Instance != null)
        {
            playerTransform = PlayerManager.Instance.PlayerTransform;
        }
    }
    
    void Update()
    {
        // 如果正在射击且时间已到，发射子弹
        if (isFiring && Time.time >= nextFireTime)
        {
            FireBullet();
        }
    }
    
    // 发射子弹
    private void FireBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // 计算朝向玩家的方向
            Vector3 fireDirection = GetFireDirection();
            
            // 创建子弹并设置方向
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            
            // 设置子弹朝向玩家
            if (bullet != null)
            {
                // 假设子弹的右方向是前进方向
                bullet.transform.right = fireDirection;
                
                // 如果你使用Rigidbody2D控制子弹运动，可以在这里设置速度
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    bulletRb.linearVelocity = fireDirection * bulletRb.linearVelocity.magnitude;
                }
            }
            
            // 设置下次发射时间
            nextFireTime = Time.time + fireRate;
        }
    }
    
    // 计算朝向玩家的方向
    private Vector3 GetFireDirection()
    {
        Vector3 direction = Vector3.right; // 默认向右
        
        if (playerTransform != null)
        {
            // 计算从发射点到玩家的方向
            direction = (playerTransform.position - firePoint.position).normalized;
            
            // 确保方向不是零向量
            if (direction.magnitude < 0.01f)
            {
                direction = Vector3.right;
            }
        }
        
        return direction;
    }
    
    // 开始射击
    public void StartFiring()
    {
        isFiring = true;
        // 立即发射第一发子弹
        if (Time.time >= nextFireTime)
        {
            FireBullet();
        }
    }
    
    // 停止射击
    public void StopFiring()
    {
        isFiring = false;
    }
    
    // 可选：更新玩家Transform（如果玩家位置变化）
    public void UpdatePlayerTransform(Transform newPlayerTransform)
    {
        playerTransform = newPlayerTransform;
    }
}