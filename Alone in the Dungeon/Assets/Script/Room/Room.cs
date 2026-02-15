using UnityEngine;
using EnemyController;  // 引用 EnemyHealth 所在的命名空间
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    public GameObject[] doors;          // 房间的门（在 Inspector 中拖拽赋值）
    private int enemyCount;              // 当前存活敌人数
    private List<EnemyHealth> enemies = new List<EnemyHealth>();  // 记录所有敌人，便于取消订阅

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 如果房间已清空，不再关门
            if (enemyCount <= 0)
                return;
            CloseDoors();
        }
    }

    void Start()
    {
        // 找到子物体 "Enemys"
        Transform enemysTransform = transform.Find("Enemys");
        if (enemysTransform != null)
        {
            // 获取所有 EnemyHealth 组件（包括子物体）
            EnemyHealth[] enemyArray = enemysTransform.GetComponentsInChildren<EnemyHealth>();
            enemies.AddRange(enemyArray);
            enemyCount = enemies.Count;

            // 订阅每个敌人的 OnDie 事件
            foreach (EnemyHealth enemy in enemies)
            {
                enemy.OnDie += HandleEnemyDefeated;
            }
        }

        // 如果房间一开始就没有敌人，直接开门
        if (enemyCount == 0)
        {
            OpenDoors();
        }
    }

    // 敌人死亡事件处理方法
    private void HandleEnemyDefeated()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            OpenDoors();
            AudioManager.instance.PlaySFX(5);
        }
        else
        {
            CloseDoors();
        }
    }

    // 开门逻辑
    private void OpenDoors()
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
                door.SetActive(false);
        }
    }

    // 关门逻辑
    private void CloseDoors()
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
                door.SetActive(true);
        }
    }

    // 当 Room 被销毁时，取消所有订阅，防止内存泄漏
    private void OnDestroy()
    {
        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy != null)
                enemy.OnDie -= HandleEnemyDefeated;
        }
    }
}