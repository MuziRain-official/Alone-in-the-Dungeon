/* WeaponManager.cs
 * 独立于玩家的武器管理器，使用对象池管理不同武器
 * 创建时间: 2026-03-02
 */

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace PlayerController
{
    public class WeaponManager : MonoBehaviour
    {
        [Header("武器预制体列表")]
        public GameObject[] weaponPrefabs;  // 武器预制体数组

        [Header("对象池设置")]
        public int poolSize = 2;  // 每个武器类型在池中的数量

        private Dictionary<int, List<GameObject>> weaponPools;  // 武器对象池
        private GameObject currentWeapon;  // 当前激活的武器
        private int currentWeaponIndex = -1;
        private Transform playerTransform;  // 玩家变换引用
        private WeaponFire weaponFire;  // 当前武器的开火脚本
        private PlayerInput playerInput;  // 玩家输入引用
        private Camera mainCamera;

        void Awake()
        {
            weaponPools = new Dictionary<int, List<GameObject>>();
            mainCamera = Camera.main;
        }

        void Start()
        {
            // 延迟一秒后尝试获取玩家引用（确保玩家已初始化）
            Invoke(nameof(TryGetPlayerReference), 1f);
        }

        /// <summary>
        /// 初始化（公共方法，供外部调用）
        /// </summary>
        public void Initialize()
        {
            InitializePools();
            TryGetPlayerReference();
        }

        /// <summary>
        /// 初始化武器对象池
        /// </summary>
        public void InitializePools()
        {
            if (weaponPrefabs == null || weaponPrefabs.Length == 0) return;

            for (int i = 0; i < weaponPrefabs.Length; i++)
            {
                if (weaponPrefabs[i] == null) continue;

                List<GameObject> pool = new List<GameObject>();

                for (int j = 0; j < poolSize; j++)
                {
                    GameObject weapon = Instantiate(weaponPrefabs[i], transform);
                    weapon.SetActive(false);
                    pool.Add(weapon);

                    // 设置武器脚本的玩家引用
                    SetupWeaponPlayerReference(weapon);
                }

                weaponPools.Add(i, pool);
            }
        }

        /// <summary>
        /// 设置武器的玩家引用（供武器脚本使用）
        /// </summary>
        private void SetupWeaponPlayerReference(GameObject weapon)
        {
            // 设置 WeaponAimBasic 的玩家引用
            WeaponAimBasic aimBasic = weapon.GetComponent<WeaponAimBasic>();
            if (aimBasic != null)
            {
                aimBasic.SetPlayerTransform(playerTransform);
            }
        }

        /// <summary>
        /// 尝试获取玩家引用
        /// </summary>
        public void TryGetPlayerReference()
        {
            // 查找玩家
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerInput = player.GetComponent<PlayerInput>();

                // 更新所有武器的玩家引用
                UpdateAllWeaponPlayerReferences();

                // 注册输入事件
                RegisterInputEvents();

                // 激活第一把武器
                if (weaponPrefabs != null && weaponPrefabs.Length > 0)
                {
                    SwitchWeapon(0);
                }
            }
            else
            {
                // 如果没找到，每秒重试一次
                Invoke(nameof(TryGetPlayerReference), 1f);
            }
        }

        /// <summary>
        /// 更新所有武器的玩家引用
        /// </summary>
        private void UpdateAllWeaponPlayerReferences()
        {
            foreach (var pool in weaponPools.Values)
            {
                foreach (GameObject weapon in pool)
                {
                    SetupWeaponPlayerReference(weapon);
                }
            }
        }

        /// <summary>
        /// 注册输入事件
        /// </summary>
        private void RegisterInputEvents()
        {
            if (playerInput == null) return;

            playerInput.actions["Next"].started += OnNextWeapon;
            playerInput.actions["Previous"].started += OnPreviousWeapon;
            playerInput.actions["Attack"].started += OnAttackStarted;
            playerInput.actions["Attack"].canceled += OnAttackCanceled;
        }

        /// <summary>
        /// 切换到下一个武器
        /// </summary>
        private void OnNextWeapon(InputAction.CallbackContext context)
        {
            if (weaponPrefabs == null || weaponPrefabs.Length <= 1) return;
            int newIndex = (currentWeaponIndex + 1) % weaponPrefabs.Length;
            SwitchWeapon(newIndex);
        }

        /// <summary>
        /// 切换到上一个武器
        /// </summary>
        private void OnPreviousWeapon(InputAction.CallbackContext context)
        {
            if (weaponPrefabs == null || weaponPrefabs.Length <= 1) return;
            int newIndex = (currentWeaponIndex - 1 + weaponPrefabs.Length) % weaponPrefabs.Length;
            SwitchWeapon(newIndex);
        }

        /// <summary>
        /// 攻击开始
        /// </summary>
        private void OnAttackStarted(InputAction.CallbackContext context)
        {
            if (weaponFire != null)
            {
                weaponFire.OnFire(context);
            }
        }

        /// <summary>
        /// 攻击取消
        /// </summary>
        private void OnAttackCanceled(InputAction.CallbackContext context)
        {
            if (weaponFire != null)
            {
                weaponFire.OnFire(context);
            }
        }

        /// <summary>
        /// 开火事件（供 InputSystem 绑定）
        /// </summary>
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (weaponFire != null)
            {
                weaponFire.OnFire(context);
            }
        }

        /// <summary>
        /// 切换武器
        /// </summary>
        public void SwitchWeapon(int index)
        {
            if (weaponPrefabs == null || index < 0 || index >= weaponPrefabs.Length) return;
            if (index == currentWeaponIndex && currentWeapon != null) return;

            // 停用当前武器
            if (currentWeapon != null)
            {
                currentWeapon.SetActive(false);
            }

            // 激活新武器（从池中获取）
            currentWeapon = GetWeaponFromPool(index);
            if (currentWeapon != null)
            {
                currentWeapon.SetActive(true);
                currentWeaponIndex = index;

                // 获取开火脚本
                weaponFire = currentWeapon.GetComponent<WeaponFire>();

                // 更新武器跟随
                UpdateWeaponFollow();
            }
        }

        /// <summary>
        /// 从对象池获取武器
        /// </summary>
        private GameObject GetWeaponFromPool(int index)
        {
            if (!weaponPools.ContainsKey(index)) return null;

            List<GameObject> pool = weaponPools[index];
            foreach (GameObject weapon in pool)
            {
                if (!weapon.activeInHierarchy)
                {
                    return weapon;
                }
            }

            // 如果池中的都激活了，扩展池
            GameObject newWeapon = Instantiate(weaponPrefabs[index], transform);
            SetupWeaponPlayerReference(newWeapon);
            newWeapon.SetActive(true);
            pool.Add(newWeapon);

            return newWeapon;
        }

        /// <summary>
        /// 更新武器跟随
        /// </summary>
        private void UpdateWeaponFollow()
        {
            if (currentWeapon == null || playerTransform == null) return;

            // 将武器位置设置到玩家附近（可以根据需要调整偏移）
            // 这里假设武器有自己的位置逻辑，只同步父变换
            WeaponAimBasic aimBasic = currentWeapon.GetComponent<WeaponAimBasic>();
            if (aimBasic != null)
            {
                aimBasic.SetPlayerTransform(playerTransform);
            }
        }

        void Update()
        {
            // 持续更新武器跟随
            if (currentWeapon != null && playerTransform != null)
            {
                UpdateWeaponFollow();
            }
        }

        /// <summary>
        /// 隐藏当前武器
        /// </summary>
        public void HideWeapon()
        {
            if (currentWeapon != null)
            {
                currentWeapon.SetActive(false);
            }
        }

        /// <summary>
        /// 显示当前武器
        /// </summary>
        public void ShowWeapon()
        {
            if (currentWeapon != null)
            {
                currentWeapon.SetActive(true);
            }
        }

        /// <summary>
        /// 获取当前武器
        /// </summary>
        public GameObject GetCurrentWeapon()
        {
            return currentWeapon;
        }

        /// <summary>
        /// 获取当前武器索引
        /// </summary>
        public int GetCurrentWeaponIndex()
        {
            return currentWeaponIndex;
        }

        void OnDestroy()
        {
            // 取消输入注册
            if (playerInput != null)
            {
                playerInput.actions["Next"].started -= OnNextWeapon;
                playerInput.actions["Previous"].started -= OnPreviousWeapon;
                playerInput.actions["Attack"].started -= OnAttackStarted;
                playerInput.actions["Attack"].canceled -= OnAttackCanceled;
            }
        }
    }
}
