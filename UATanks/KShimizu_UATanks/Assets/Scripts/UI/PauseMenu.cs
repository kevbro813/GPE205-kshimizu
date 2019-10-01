using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PauseMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    public void MusicSlider()
    {
        GameManager.instance.soundManager.SetMusicVolume(musicSlider.value);
    }
    public void SFXSlider()
    {
        GameManager.instance.soundManager.SetSFXVolume(sfxSlider.value);
    }
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
    public void Resume()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        Save();
        GameManager.instance.gameState = "active";
    }
    public void Menu()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        Save();
        GameManager.instance.gameState = "menu";
    }
    public void Quit()
    {
        GameManager.instance.soundManager.SoundMenuButton();
        GameManager.instance.gameState = "quit";
    }
    public void OnApplicationQuit()
    {
        Save();
    }
}
