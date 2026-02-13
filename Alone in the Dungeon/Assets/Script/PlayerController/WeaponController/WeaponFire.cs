/*WeaponFire.cs
*处理武器开火逻辑  
*/

using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponFire : MonoBehaviour
{
    [Header("子弹预制体")]
    public GameObject bullet;
    [Header("开火点")]
    public Transform firePoint;
    [Header("发射间隔时间")] 
    public float fireRate = 0.2f; 
    
    private float nextFireTime = 0f; // 计时器：下次允许发射的时间
    private bool isFiring = false;   // 状态：是否正在开火

    void Update()
    {
        // 如果正在开火且时间已到，发射子弹
        if (isFiring && Time.time >= nextFireTime)
        {
            AudioManager.instance.PlaySFX(0);
            Instantiate(bullet, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + fireRate; // 设置下次发射时间
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isFiring = true; // 按下时开始连射
            
            // 立即发射第一发子弹
            if (Time.time >= nextFireTime)
            {
                Instantiate(bullet, firePoint.position, firePoint.rotation);
                AudioManager.instance.PlaySFX(0);
                nextFireTime = Time.time + fireRate;
            }
        }
        else if (context.canceled)
        {
            isFiring = false; // 松开时停止连射
        }
    }
}