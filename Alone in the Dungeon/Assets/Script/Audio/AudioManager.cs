using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour, GameFramework.IAudioService
{
    public static AudioManager instance;

    [Header("音乐")]
    public AudioSource gameOverMusic;
    public AudioSource backGroundMusic;
    public AudioSource winGameMusic;
    public AudioSource bossBattleMusic;      // Boss战音乐
    public AudioSource bossSecondPhaseMusic; // Boss第二阶段音乐

    [Header("音效")]
    public AudioSource[] sfx;

    [Header("设置")]
    public bool playMusicOnStart = true;
    public string mainMenuSceneName = "MainMenu"; // 主菜单场景名称

    private bool isGameScene = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // 只有是根物体时才调用 DontDestroyOnLoad
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }

            // 注册到服务定位器
            if (GameFramework.ServiceLocator.Instance != null)
            {
                GameFramework.ServiceLocator.Instance.Register<GameFramework.IAudioService>(this);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 订阅玩家死亡事件
        if (PlayerController.PlayerHealth.Instance != null)
        {
            PlayerController.PlayerHealth.Instance.OnDied += GameOver;
        }

        // 订阅场景加载完成事件
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 检查当前场景并播放音乐
        CheckAndPlayMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndPlayMusic();
    }

    private void CheckAndPlayMusic()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // 如果是主菜单场景，停止背景音乐
        if (currentScene == mainMenuSceneName || currentScene == "Menu" || currentScene == "MainMenu")
        {
            backGroundMusic?.Stop();
            isGameScene = false;
            return;
        }

        // 如果是游戏场景，播放背景音乐
        if (playMusicOnStart && !isGameScene)
        {
            PlayBackgroundMusic();
            isGameScene = true;
        }
    }

    public void PlayBackgroundMusic()
    {
        // 停止Boss音乐
        bossBattleMusic?.Stop();
        bossSecondPhaseMusic?.Stop();

        if (backGroundMusic != null && !backGroundMusic.isPlaying)
        {
            backGroundMusic.Play();
        }
    }

    public void GameOver()
    {
        backGroundMusic?.Stop();
        gameOverMusic?.Play();
    }

    public void PlaySFX(int sfxIndex)
    {
        if (sfx != null && sfxIndex >= 0 && sfxIndex < sfx.Length)
        {
            sfx[sfxIndex].Stop();
            sfx[sfxIndex].Play();
        }
    }

    public void PlayMusic(AudioSource music)
    {
        music?.Play();
    }

    public void StopMusic()
    {
        if (backGroundMusic != null)
        {
            backGroundMusic.Stop();
        }
        // 停止Boss战音乐
        StopBossMusic();
        isGameScene = false;
    }

    public void PlayBossBattleMusic()
    {
        // 停止当前背景音乐
        backGroundMusic?.Stop();

        // 播放Boss战音乐
        if (bossBattleMusic != null && !bossBattleMusic.isPlaying)
        {
            bossBattleMusic.Play();
        }
    }

    public void PlayBossSecondPhaseMusic()
    {
        // 停止Boss战音乐
        bossBattleMusic?.Stop();

        // 播放第二阶段音乐
        if (bossSecondPhaseMusic != null && !bossSecondPhaseMusic.isPlaying)
        {
            bossSecondPhaseMusic.Play();
        }
    }

    public void StopBossMusic()
    {
        bossBattleMusic?.Stop();
        bossSecondPhaseMusic?.Stop();
    }

    void OnDestroy()
    {
        // 取消场景加载订阅
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // 注销服务
        if (GameFramework.ServiceLocator.Instance != null)
        {
            GameFramework.ServiceLocator.Instance.Unregister<GameFramework.IAudioService>();
        }
    }
}
