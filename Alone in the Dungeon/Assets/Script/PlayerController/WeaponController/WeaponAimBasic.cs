/* WeaponAimBasic.cs
 * 基础武器瞄准脚本
 * 支持两种模式：
 * 1. 作为玩家子对象时，通过 transform.parent 自动获取玩家引用
 * 2. 独立存在时，通过 SetPlayerTransform 显式设置玩家引用
 */

using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerController
{
    public class WeaponAimBasic : MonoBehaviour
    {
        private Camera m_mainCamera;
        private Transform m_playerTransform;
        private bool m_isEquipped = false;  // 武器是否被装备

        void Start()
        {
            m_mainCamera = Camera.main;

            // 检查是否已被装备（通过父对象是否为 WeaponPoint）
            CheckIfEquipped();
        }

        /// <summary>
        /// 检查武器是否被装备
        /// </summary>
        private void CheckIfEquipped()
        {
            if (transform.parent != null && transform.parent.name == "WeaponPoint")
            {
                m_isEquipped = true;
                m_playerTransform = transform.parent.parent;  // WeaponPoint 的父对象是 Player
                enabled = true;
            }
            else
            {
                m_isEquipped = false;
                enabled = false;  // 未装备时禁用脚本
            }
        }

        /// <summary>
        /// 显式设置玩家变换引用（供 WeaponController 调用）
        /// </summary>
        public void SetPlayerTransform(Transform playerTransform)
        {
            m_playerTransform = playerTransform;
            m_isEquipped = playerTransform != null;
        }

        /// <summary>
        /// 设置武器为已装备状态
        /// </summary>
        public void SetEquipped(bool equipped)
        {
            m_isEquipped = equipped;
            enabled = equipped;
        }

        void Update()
        {
            // 未装备时不执行任何操作
            if (!m_isEquipped) return;

            // 更新武器朝向
            UpdateWeaponRotation();

            // 根据玩家朝向调整武器翻转
            if (m_playerTransform != null)
            {
                transform.localScale = m_playerTransform.localScale.x < 0
                    ? new Vector3(-1, -1, 1)
                    : Vector3.one;
            }
        }
        
        private void UpdateWeaponRotation()
        {
            if (m_mainCamera == null) return;
            
            // 获取鼠标位置
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mousePosition = m_mainCamera.ScreenToWorldPoint(
                new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, m_mainCamera.nearClipPlane));
            
            // 计算方向并旋转
            Vector2 direction = mousePosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}