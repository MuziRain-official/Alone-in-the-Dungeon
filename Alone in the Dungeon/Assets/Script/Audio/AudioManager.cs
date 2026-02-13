using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource gameOverMusic,backGroundMusic,winGameMusic;
    public AudioSource[] sfx;
    void Start()
    {
        instance = this;
        PlayerHealth.Instance.OnDeath += GameOver;
    }

    void Update()
    {
        
    }
    public void GameOver()
    {
        backGroundMusic.Stop();
        gameOverMusic.Play();
    }
    public void PlaySFX(int sfxnum)
    {
        sfx[sfxnum].Stop();
        sfx[sfxnum].Play();
    }
}
