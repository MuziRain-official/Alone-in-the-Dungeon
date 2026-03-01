/* PickableWeapon.cs
 * 场景内可拾取的武器
 */

using PlayerController;
using UnityEngine;

public class PickableWeapon : MonoBehaviour
{
    [Header("武器预制体")]
    public GameObject weaponPrefab;  // 对应的武器预制体

    [Header("设置")]
    public string weaponName = "武器";
    public Sprite icon;  // 拾取时显示的图标

    void Start()
    {
        // 禁用武器瞄准脚本 - 当地面上的武器不应该跟随鼠标旋转
        WeaponAimBasic aimBasic = GetComponent<WeaponAimBasic>();
        if (aimBasic != null)
        {
            aimBasic.enabled = false;
        }
    }

    /// <summary>
    /// 交互方法（由玩家按下交互键时调用）
    /// </summary>
    public void Interact()
    {
        // 查找玩家身上的 WeaponController
        PlayerController.WeaponController weaponController =
            FindFirstObjectByType<PlayerController.WeaponController>();

        if (weaponController != null)
        {
            // 拾取新武器，丢弃旧武器
            weaponController.PickUpWeapon(weaponPrefab);

            // 销毁拾取物
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 设置武器数据（供外部调用）
    /// </summary>
    public void Setup(GameObject prefab, string name)
    {
        weaponPrefab = prefab;
        weaponName = name;
    }
}
