using UnityEngine;
using GameFramework;

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
    private Vector3 fireDirection;

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
        // 通过服务定位器获取玩家位置
        var playerProvider = ServiceLocator.Instance?.Get<IPlayerProvider>();
        Transform player = playerProvider?.PlayerTransform;

        // 备用方案
        if (player == null && PlayerController.PlayerManager.Instance != null)
        {
            player = PlayerController.PlayerManager.Instance.PlayerTransform;
        }

        if (player != null)
        {
            Vector3 dir = (player.position - firePoint.position).normalized;
            fireDirection = dir.sqrMagnitude > 0.001f ? dir : transform.right;
        }
        else
        {
            fireDirection = transform.right;
        }

        isFiring = true;
        nextFireTime = 0f;

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
