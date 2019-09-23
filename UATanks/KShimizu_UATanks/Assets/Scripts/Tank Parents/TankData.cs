using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent of all AI and player Data components
public abstract class TankData : MonoBehaviour
{
    // Public variables for tanks
    public float tankHealth; // Tank's health
    public float maxTankHealth; // Tanks max health
    public float forwardSpeed = 1.0f; // Tank's forward speed multiplier
    public float reverseSpeed = 0.5f; // Tank's reverse speed multiplier
    public float rotationSpeed = 1.0f; // Tank's rotation speed multiplier
    public float projectileDuration = 2.0f; // Duration the projectile exists before it disappears (explodes)
    public float cannonDelay = 3.0f; // Delay while tank is reloading
    public float shellForce = 1.0f; // Speed of the shell
    public float shellDamage = 20.0f; // Damage dealt by the shell
    public float criticalHealth = 60.0f; // Threshold before AI tank will turn and run
    public int currentAmmo; // Tracks player's current ammo
    public int maxAmmo = 10; // Maximum ammo
    public int coins; // Coins will be used to purchase upgrades
    public bool isInvulnerable; // If true, the tank cannot be damaged
    public bool isInfiniteAmmo; // If true the tank has infinite ammo
    public bool isInvisible; // If true, the player tank can only be heard by AI, AI tanks will be translucent
    public float translucency; // Set level of translucency 0 to 1
    
    public virtual void Start()
    {
        // Set health and ammo to max at start
        tankHealth = maxTankHealth;
        currentAmmo = maxAmmo;
    }
    // Method to check if tank is out of health and destroys the tank if tankHealth <= 0
    public virtual void TankDestroyed()
    {
        if (tankHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
