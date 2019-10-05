using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sound manager manages all SFX and music
public class SoundManager : MonoBehaviour
{
    public GameObject sfxOrigin;
    public GameObject musicOrigin;

    // Using two audio sources to play music and sound effects simultaneously
    public AudioSource asSFX; // SFX audio source
    public AudioSource asMusic; // Music audio source

    // List of audio clips (Set in inspector)
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
    // Functions to set Music and SFX volume
    public void SetMusicVolume(float volume)
    {
        if (asMusic != null)
        {
            asMusic.volume = volume;
        }
    }
    public void SetSFXVolume(float volume)
    {
        if (asSFX != null)
        {
            asSFX.volume = volume;
        }
    }

    // Play and pause game music functions
    public void PlayMusic()
    {
        asMusic.clip = inGameMusic;
        asMusic.Play();
    }
    public void PauseMusic()
    {
        asMusic.Pause();
    }

    // Play SFX
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
