using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

// Pause menu that allows players to adjust volume, save and load settings
public class PauseMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    // Sliders that will adjust music and sound effects volume, respectively
    public void MusicSlider()
    {
        GameManager.instance.soundManager.SetMusicVolume(musicSlider.value);
    }
    public void SFXSlider()
    {
        GameManager.instance.soundManager.SetSFXVolume(sfxSlider.value);
    }

    // Save and Load volume settings
    public void Save()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }
    public void Load()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }

    // Menu buttons
    public void Resume()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        Save();
        GameManager.instance.gameState = "resume";
    }
    public void Pregame()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        Save();
        GameManager.instance.gameState = "pregame";
    }
    public void Quit()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        GameManager.instance.gameState = "quit";
    }
    public void OnApplicationQuit()
    {
        Save(); // Save settings if the player quits the game
    }
}
