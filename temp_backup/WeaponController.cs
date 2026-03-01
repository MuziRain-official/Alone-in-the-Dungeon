/* WeaponController.cs
 * 玩家武器控制器，管理武器切换（武器作为子物体）
 */

using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class WeaponController : MonoBehaviour
    {
        [Header("武器列表")]
        public GameObject[] weapons;  // 所有武器子物体
        public int currentWeaponIndex = 0;

        private PlayerInput playerInput;
        private WeaponFire weaponFire;

        void Start()
        {
            playerInput = GetComponent<PlayerInput>();

            // 注册武器切换输入
            if (playerInput != null)
            {
                playerInput.actions["Next"].started += OnNextWeapon;
                playerInput.actions["Previous"].started += OnPreviousWeapon;
            }

            // 初始化武器
            UpdateWeaponState();
        }

        /// <summary>
        /// 切换到下一个武器
        /// </summary>
        private void OnNextWeapon(InputAction.CallbackContext context)
        {
            if (weapons == null || weapons.Length <= 1) return;
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
            UpdateWeaponState();
        }

        /// <summary>
        /// 切换到上一个武器
        /// </summary>
        private void OnPreviousWeapon(InputAction.CallbackContext context)
        {
            if (weapons == null || weapons.Length <= 1) return;
            currentWeaponIndex = (currentWeaponIndex - 1 + weapons.Length) % weapons.Length;
            UpdateWeaponState();
        }

        /// <summary>
        /// 更新武器状态
        /// </summary>
        private void UpdateWeaponState()
        {
            if (weapons == null || currentWeaponIndex < 0 || currentWeaponIndex >= weapons.Length) return;

            // 取消之前武器的输入
            if (weaponFire != null && playerInput != null)
            {
                playerInput.actions["Attack"].started -= weaponFire.OnFire;
                playerInput.actions["Attack"].canceled -= weaponFire.OnFire;
            }

            // 禁用所有武器
            for (int i = 0; i < weapons.Length; i++)
            {
                if (weapons[i] != null)
                {
                    weapons[i].SetActive(i == currentWeaponIndex);

                    // 启用当前武器的瞄准脚本
                    if (i == currentWeaponIndex)
                    {
                        WeaponAimBasic aimBasic = weapons[i].GetComponent<WeaponAimBasic>();
                        if (aimBasic != null) aimBasic.enabled = true;
                    }
                }
            }

            // 注册当前武器的输入
            GameObject currentWeapon = weapons[currentWeaponIndex];
            if (currentWeapon != null)
            {
                weaponFire = currentWeapon.GetComponent<WeaponFire>();
                if (weaponFire != null && playerInput != null)
                {
                    playerInput.actions["Attack"].started += weaponFire.OnFire;
                    playerInput.actions["Attack"].canceled += weaponFire.OnFire;
                }
            }
        }

        /// <summary>
        /// 获取当前武器
        /// </summary>
        public GameObject GetCurrentWeapon()
        {
            if (weapons != null && currentWeaponIndex >= 0 && currentWeaponIndex < weapons.Length)
                return weapons[currentWeaponIndex];
            return null;
        }

        /// <summary>
        /// 隐藏当前武器
        /// </summary>
        public void HideWeapon()
        {
            GameObject current = GetCurrentWeapon();
            if (current != null) current.SetActive(false);
        }

        /// <summary>
        /// 显示当前武器
        /// </summary>
        public void ShowWeapon()
        {
            GameObject current = GetCurrentWeapon();
            if (current != null) current.SetActive(true);
        }

        void OnDestroy()
        {
            if (playerInput != null)
            {
                if (weaponFire != null)
                {
                    playerInput.actions["Attack"].started -= weaponFire.OnFire;
                    playerInput.actions["Attack"].canceled -= weaponFire.OnFire;
                }
                playerInput.actions["Next"].started -= OnNextWeapon;
                playerInput.actions["Previous"].started -= OnPreviousWeapon;
            }
        }
    }
}
