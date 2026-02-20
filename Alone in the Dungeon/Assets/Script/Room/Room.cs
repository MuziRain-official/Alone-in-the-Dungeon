using System;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using EnemyController;

public class Room : MonoBehaviour
{
    [Header("房间设置")]
    public GameObject[] doors;
    private int enemyCount;
    private List<EnemyHealth> enemies = new List<EnemyHealth>();

    private EventManager eventManager;
    private IAudioService audioService;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (enemyCount <= 0)
                return;
            CloseDoors();

            // 发布玩家进入房间事件
            eventManager?.Publish(new PlayerEnterRoomEvent { room = gameObject });
        }
    }

    void Start()
    {
        // 获取服务
        eventManager = EventManager.Instance;
        audioService = ServiceLocator.Instance?.Get<IAudioService>();

        // 查找敌人
        Transform enemysTransform = transform.Find("Enemys");
        if (enemysTransform != null)
        {
            EnemyHealth[] enemyArray = enemysTransform.GetComponentsInChildren<EnemyHealth>();
            enemies.AddRange(enemyArray);
            enemyCount = enemies.Count;

            foreach (EnemyHealth enemy in enemies)
            {
                enemy.OnDied += HandleEnemyDefeated;
            }
        }

        if (enemyCount == 0)
        {
            OpenDoors();
        }
    }

    private void HandleEnemyDefeated()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            OpenDoors();
            // 播放房间清理音效
            if (audioService != null)
            {
                audioService.PlaySFX(5);
            }
            else if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX(5);
            }
            eventManager?.Publish(new RoomClearedEvent { room = gameObject });
        }
        else
        {
            CloseDoors();
        }
    }

    private void OpenDoors()
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
                door.SetActive(false);
        }
    }

    private void CloseDoors()
    {
        foreach (GameObject door in doors)
        {
            if (door != null)
                door.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.OnDied -= HandleEnemyDefeated;
            }
        }
    }
}
