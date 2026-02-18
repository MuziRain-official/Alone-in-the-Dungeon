using PlayerController;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    [Header("子弹设置")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    
    [Header("射击参数")]
    public float fireRate = 0.2f;
    public bool autoFire = true;
    
    private float nextFireTime = 0f;
    private bool isFiring = false;
    private Vector3 fireDirection;   // 当前攻击周期的固定方向

    void Update()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            FireBullet();
        }
    }

    private void FireBullet()
    {
        if (bulletPrefab == null || firePoint == null) return;

        // 使用当前攻击周期开始时的锁定方向生成子弹
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        if (bullet != null)
        {
            bullet.transform.right = fireDirection;
        }

        nextFireTime = Time.time + fireRate;
    }

    /// <summary>
    /// 开始攻击周期：锁定当前玩家位置的方向，并开始连续射击
    /// </summary>
    public void StartFiring()
    {
        // 关键修复：在攻击开始时实时获取玩家位置，计算并锁定方向
        Transform player = PlayerManager.Instance?.PlayerTransform;
        if (player != null)
        {
            Vector3 dir = (player.position - firePoint.position).normalized;
            // 防止方向为零向量（玩家距离过近）
            fireDirection = dir.sqrMagnitude > 0.001f ? dir : transform.right;
        }
        else
        {
            // 玩家不存在时，默认使用武器自身的右方向（通常与敌人朝向一致）
            fireDirection = transform.right;
        }

        isFiring = true;
        nextFireTime = 0f;   // 重置计时，保证立即发射第一发

        // 立即发射第一发（避免延迟一帧，确保攻击周期立即开始）
        if (Time.time >= nextFireTime)
        {
            FireBullet();
        }
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}