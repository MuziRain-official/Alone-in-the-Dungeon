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
    private Transform playerTransform;
    private Vector3 fireDirection; // 保存第一发子弹的方向
    
    void Start()
    {
        if (PlayerManager.Instance != null)
        {
            playerTransform = PlayerManager.Instance.PlayerTransform;
        }
    }
    
    void Update()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            FireBullet();
        }
    }
    
    private void FireBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // 如果是第一发子弹，计算方向；否则使用保存的方向
            if (nextFireTime == 0f) // 首次射击
            {
                fireDirection = GetFireDirection();
            }
            
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            
            if (bullet != null)
            {
                bullet.transform.right = fireDirection;
            }
            
            nextFireTime = Time.time + fireRate;
        }
    }
    
    private Vector3 GetFireDirection()
    {
        Vector3 direction = Vector3.right;
        
        if (playerTransform != null)
        {
            direction = (playerTransform.position - firePoint.position).normalized;
            
            if (direction.magnitude < 0.01f)
            {
                direction = Vector3.right;
            }
        }
        
        return direction;
    }
    
    public void StartFiring()
    {
        isFiring = true;
        // 重置射击时间，下次射击将重新计算方向
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