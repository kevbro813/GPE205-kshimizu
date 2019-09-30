using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public GameObject sfxOrigin;
    public GameObject musicOrigin;
    public AudioSource asSFX;
    public AudioSource asMusic;

    public AudioClip inGameMusic;
    public AudioClip fireCannon;
    public AudioClip tankDestroyed;
    public AudioClip tankHit;
    public AudioClip powerup;
    public AudioClip menuButton;

    // Start is called before the first frame update
    void Start()
    {
        sfxOrigin = GameObject.FindWithTag("SFXSource");
        musicOrigin = GameObject.FindWithTag("MusicSource");
        asSFX = GameManager.instance.asSFX;
        asMusic = GameManager.instance.asMusic;
    }
    public void SetMusicVolume(float volume)
    {
        asMusic.volume = volume;
    }
    public void SetSFXVolume(float volume)
    {
        asSFX.volume = volume;
    }
    public void PlayMusic()
    {
        asMusic.clip = inGameMusic;
        asMusic.Play();
    }
    public void PauseMusic()
    {
        asMusic.Pause();
    }
    public void SoundFireCannon()
    {
        asSFX.clip = fireCannon;
        asSFX.Play();
    }
    public void SoundTankDestroyed()
    {
        asSFX.clip = tankDestroyed;
        asSFX.Play();
    }
    public void SoundTankHit()
    {
        asSFX.clip = tankHit;
        asSFX.Play();
    }
    public void SoundPowerup()
    {
        asSFX.clip = powerup;
        asSFX.Play();
    }
    public void SoundMenuButton()
    {
        asSFX.clip = menuButton;
        asSFX.Play();
    }
}
