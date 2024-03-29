﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Component that contains all HUD elements
public class HUD : MonoBehaviour
{
    [HideInInspector] public PlayerData playerData; 
    [Header("HUD Images")]
    public Image healthBar;
    public Image invulnerabilityIcon;
    public Image invisibilityIcon;
    public Image infiniteAmmoIcon;
    public Image tankRoundOne;
    public Image tankRoundTwo;
    public Image tankRoundThree;
    public Image tankRoundFour;
    public Image tankRoundFive;
    public Image playerLifeOne;
    public Image playerLifeTwo;
    public Image playerLifeThree;
    public Image reloadTimer;
    [Space(10)]
    [Header("HUD Text")]
    public Text coinAmount;
    public Text scoreValue;
    public Text playerName;

    // Start is called before the first frame update
    void Start()
    {
        healthBar.type = Image.Type.Filled;
        reloadTimer.type = Image.Type.Filled;
        SetIconTranslucent(invisibilityIcon);
        SetIconTranslucent(invulnerabilityIcon);
        SetIconTranslucent(infiniteAmmoIcon);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerData != null)
        {
            playerName.text = playerData.playerName; // Display player name
            healthBar.fillAmount = playerData.healthPercent; // Update health bar
            scoreValue.text = playerData.score.ToString(); // Update the score on the HUD
            coinAmount.text = playerData.coins.ToString(); // Update the coin on the HUD
            reloadTimer.fillAmount = playerData.reloadTimer; // Update reload timer

            // Set Invisibility Icon visibility
            if (playerData.isInvisible == true)
            {
                SetIconVisible(invisibilityIcon);
            }
            else
            {
                SetIconTranslucent(invisibilityIcon);
            }

            // Set Invulnerability Icon visibility
            if (playerData.isInvulnerable == true)
            {
                SetIconVisible(invulnerabilityIcon);
            }
            else
            {
                SetIconTranslucent(invulnerabilityIcon);
            }

            // Set Infinite Ammo Icon visibility
            if (playerData.isInfiniteAmmo == true)
            {
                SetIconVisible(infiniteAmmoIcon);
            }
            else
            {
                SetIconTranslucent(infiniteAmmoIcon);
            }

            // Tracks Ammo count visually
            if (playerData.currentAmmo == 5)
            {
                SetIconVisible(tankRoundOne);
                SetIconVisible(tankRoundTwo);
                SetIconVisible(tankRoundThree);
                SetIconVisible(tankRoundFour);
                SetIconVisible(tankRoundFive);
            }
            if (playerData.currentAmmo == 4)
            {
                SetIconVisible(tankRoundOne);
                SetIconVisible(tankRoundTwo);
                SetIconVisible(tankRoundThree);
                SetIconVisible(tankRoundFour);
                SetIconTranslucent(tankRoundFive);
            }
            if (playerData.currentAmmo == 3)
            {
                SetIconVisible(tankRoundOne);
                SetIconVisible(tankRoundTwo);
                SetIconVisible(tankRoundThree);
                SetIconTranslucent(tankRoundFour);
                SetIconTranslucent(tankRoundFive);
            }
            if (playerData.currentAmmo == 2)
            {
                SetIconVisible(tankRoundOne);
                SetIconVisible(tankRoundTwo);
                SetIconTranslucent(tankRoundThree);
                SetIconTranslucent(tankRoundFour);
                SetIconTranslucent(tankRoundFive);
            }
            if (playerData.currentAmmo == 1)
            {
                SetIconVisible(tankRoundOne);
                SetIconTranslucent(tankRoundTwo);
                SetIconTranslucent(tankRoundThree);
                SetIconTranslucent(tankRoundFour);
                SetIconTranslucent(tankRoundFive);
            }
            if (playerData.currentAmmo == 0)
            {
                SetIconTranslucent(tankRoundOne);
                SetIconTranslucent(tankRoundTwo);
                SetIconTranslucent(tankRoundThree);
                SetIconTranslucent(tankRoundFour);
                SetIconTranslucent(tankRoundFive);
            }

            // Player lives updates visually
            if (playerData.playerLives == 3)
            {
                SetIconVisible(playerLifeOne);
                SetIconVisible(playerLifeTwo);
                SetIconVisible(playerLifeThree);
            }
            if (playerData.playerLives == 2)
            {
                SetIconVisible(playerLifeOne);
                SetIconVisible(playerLifeTwo);
                SetIconTranslucent(playerLifeThree);
            }
            if (playerData.playerLives == 1)
            {
                SetIconVisible(playerLifeOne);
                SetIconTranslucent(playerLifeTwo);
                SetIconTranslucent(playerLifeThree);
            }
            if (playerData.playerLives == 0)
            {
                SetIconTranslucent(playerLifeOne);
                SetIconTranslucent(playerLifeTwo);
                SetIconTranslucent(playerLifeThree);
            }
        }
    }

    // Set icons to opaque or translucent (indicates whether powerups are active)
    private void SetIconVisible(Image image)
    {
        Color color = image.color;
        color.a = 1.0f; // Set to opaque
        image.color = color;
    }
    private void SetIconTranslucent(Image image)
    {
        Color color = image.color;
        color.a = 0.3f; // Set to translucent
        image.color = color;
    }
}
