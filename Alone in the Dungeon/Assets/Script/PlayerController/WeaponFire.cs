/*WeaponFire.cs
*处理武器开火逻辑  
*/

using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponFire : MonoBehaviour
{
    public GameObject bullet; // 子弹预制体
    public Transform firePoint; // 开火点 
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Instantiate(bullet, firePoint.position, firePoint.rotation);
        }
    }
}