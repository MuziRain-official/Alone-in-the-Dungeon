/* WeaponController.cs
 * 玩家武器控制器
 * 管理武器拾取和装备
 */

using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class WeaponController : MonoBehaviour
    {
        [Header("初始武器")]
        public GameObject initialWeapon;  // 初始武器预制体

        [Header("武器挂载点")]
        public Transform weaponMountPoint;  // 武器挂载点位置（WeaponPoint）

        [Header("拾取检测区域")]
        public Collider2D pickupTrigger;  // 拾取检测触发器

        private GameObject currentEquippedWeapon;  // 当前装备的武器对象
        private GameObject nearestPickupWeapon;  // 当前范围内最近的可拾取武器

        void Start()
        {
            // 获取武器挂载点
            if (weaponMountPoint == null)
            {
                weaponMountPoint = transform.Find("WeaponPoint");
            }

            // 装备初始武器
            if (initialWeapon != null && weaponMountPoint != null)
            {
                EquipWeapon(Instantiate(initialWeapon, weaponMountPoint));
            }
        }

        /// <summary>
        /// 交互事件（供 InputSystem 绑定）- 用于拾取武器
        /// </summary>
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                TryPickupWeapon();
            }
        }

        /// <summary>
        /// 拾取触发器进入
        /// </summary>
        void OnTriggerEnter2D(Collider2D other)
        {
            // 只检测武器（通过是否有 WeaponFire 组件判断）
            if (other.GetComponent<WeaponFire>() != null)
            {
                if (nearestPickupWeapon == null)
                {
                    nearestPickupWeapon = other.gameObject;
                }
                else
                {
                    // 比较距离，选择更近的
                    float currentDist = Vector2.Distance(transform.position, nearestPickupWeapon.transform.position);
                    float newDist = Vector2.Distance(transform.position, other.transform.position);
                    if (newDist < currentDist)
                    {
                        nearestPickupWeapon = other.gameObject;
                    }
                }
            }
        }

        /// <summary>
        /// 拾取触发器退出
        /// </summary>
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject == nearestPickupWeapon)
            {
                nearestPickupWeapon = null;
            }
        }

        /// <summary>
        /// 尝试拾取武器
        /// </summary>
        public void TryPickupWeapon()
        {
            if (nearestPickupWeapon != null)
            {
                EquipWeapon(nearestPickupWeapon);
                nearestPickupWeapon = null;
            }
        }

        /// <summary>
        /// 装备武器（替换当前武器）
        /// </summary>
        public void EquipWeapon(GameObject weapon)
        {
            // 如果已有装备的武器，放到地上
            if (currentEquippedWeapon != null)
            {
                DropWeapon();
            }

            // 将武器移到挂载点
            if (weaponMountPoint != null)
            {
                weapon.transform.SetParent(weaponMountPoint);
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;

                // 设置 WeaponAimBasic 的玩家引用并启用
                if (weapon.TryGetComponent(out WeaponAimBasic aimBasic))
                {
                    aimBasic.SetPlayerTransform(transform);
                    aimBasic.SetEquipped(true);
                }

                // 确保激活
                weapon.SetActive(true);

                // 禁用物理（不再需要物理效果）
                Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.simulated = false;
                }

                // 禁用碰撞器（不再需要碰撞）
                Collider2D col = weapon.GetComponent<Collider2D>();
                if (col != null)
                {
                    col.enabled = false;
                }

                currentEquippedWeapon = weapon;
            }
        }

        /// <summary>
        /// 丢弃当前武器（放到地上）
        /// </summary>
        public void DropWeapon()
        {
            if (currentEquippedWeapon != null)
            {
                // 禁用 WeaponAimBasic
                if (currentEquippedWeapon.TryGetComponent(out WeaponAimBasic aimBasic))
                {
                    aimBasic.SetEquipped(false);
                }

                // 移到世界空间
                currentEquippedWeapon.transform.SetParent(null);

                // 激活物理
                Rigidbody2D rb = currentEquippedWeapon.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.simulated = true;
                }

                // 激活碰撞器
                Collider2D col = currentEquippedWeapon.GetComponent<Collider2D>();
                if (col != null)
                {
                    col.enabled = true;
                }

                currentEquippedWeapon = null;
            }
        }

        /// <summary>
        /// 是否有装备武器
        /// </summary>
        public bool HasWeapon()
        {
            return currentEquippedWeapon != null;
        }

        /// <summary>
        /// 开火事件（供 InputSystem 绑定）
        /// </summary>
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (currentEquippedWeapon != null)
            {
                WeaponFire weaponFire = currentEquippedWeapon.GetComponent<WeaponFire>();
                if (weaponFire != null)
                {
                    weaponFire.OnFire(context);
                }
            }
        }

        /// <summary>
        /// 获取当前武器
        /// </summary>
        public GameObject GetCurrentWeapon()
        {
            return currentEquippedWeapon;
        }

        /// <summary>
        /// 隐藏当前武器
        /// </summary>
        public void HideWeapon()
        {
            if (currentEquippedWeapon != null)
                currentEquippedWeapon.SetActive(false);
        }

        /// <summary>
        /// 显示当前武器
        /// </summary>
        public void ShowWeapon()
        {
            if (currentEquippedWeapon != null)
                currentEquippedWeapon.SetActive(true);
        }

        void OnDrawGizmosSelected()
        {
            // 显示拾取触发器范围
            if (pickupTrigger != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(pickupTrigger.bounds.center, pickupTrigger.bounds.size);
            }
        }
    }
}
