using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages camera and HUD settings for single/multiplayer
public class UIManager : MonoBehaviour
{
    // Set cameras to singleplayer mode
    public void SinglePlayerCamera()
    {
        GameManager.instance.cameraObjects[0].SetActive(true);
        GameManager.instance.cameraObjects[1].SetActive(false);
        GameManager.instance.cameraComponents[0].rect = new Rect(0f, 0f, 1f, 1f);
    }
    // Set cameras to two player mode
    public void TwoPlayerCamera()
    {
        GameManager.instance.cameraObjects[0].SetActive(true);
        GameManager.instance.cameraObjects[1].SetActive(true);
        GameManager.instance.cameraComponents[0].rect = new Rect(0f, 0.5f, 1f, 0.5f);
        GameManager.instance.cameraComponents[1].rect = new Rect(0f, 0f, 1f, 0.5f);
    }
    // Set HUD to singleplayer mode
    public void SinglePlayerHUD()
    {
        GameManager.instance.hudObjects[0].SetActive(true);
        GameManager.instance.hudObjects[1].SetActive(false);
        GameManager.instance.hudObjects[2].SetActive(false);
    }
    // Set HUD to two player mode
    public void TwoPlayerHud()
    {
        GameManager.instance.hudObjects[0].SetActive(false);
        GameManager.instance.hudObjects[1].SetActive(true);
        GameManager.instance.hudObjects[2].SetActive(true);
    }
}
