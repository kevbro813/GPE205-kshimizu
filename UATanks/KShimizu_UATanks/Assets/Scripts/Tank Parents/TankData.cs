using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent of all AI and player Data components
public abstract class TankData : MonoBehaviour
{
    // Public variables for tanks
    [Header("Tank Health")]
    public float tankHealth; // Tank's health
    public float maxTankHealth; // Tanks max health
    public float criticalHealth = 60.0f; // Threshold before AI tank will turn and run
    [Space(10)]
    [Header("Movement and Rotation Speed")]
    public float forwardSpeed = 1.0f; // Tank's forward speed multiplier
    public float reverseSpeed = 0.5f; // Tank's reverse speed multiplier
    public float rotationSpeed = 1.0f; // Tank's rotation speed multiplier
    [Space(10)]
    [Header("Projectile Statistics")]
    public float projectileDuration = 2.0f; // Duration the projectile exists before it disappears (explodes)
    public float cannonDelay = 3.0f; // Delay while tank is reloading
    public float shellForce = 1.0f; // Speed of the shell
    public float shellDamage = 20.0f; // Damage dealt by the shell   
    public int currentAmmo; // Tracks player's current ammo
    public int maxAmmo = 10; // Maximum ammo
    [Space(10)]
    [Header("Powerup Booleans")]
    public bool isInvulnerable; // If true, the tank cannot be damaged
    public bool isInfiniteAmmo; // If true the tank has infinite ammo
    public bool isInvisible; // If true, the player tank can only be heard by AI, AI tanks will be translucent
    [Space(10)]
    [Header("Other")]
    public int coins; // Coins will be used to purchase upgrades
    public float translucency; // Set level of translucency 0 to 1
    public int tankIndex;
    public float score = 0; // Integer that tracks the player's score
    public int pointValue; // Number of points granted to the player when the enemy tank is destroyed
    public float healthPercent; // Percent of health used for health bar
    public float reloadTimer; // Reload percentage used for reload timer
    private CannonSource cannonSource; // Cannon source component in child, used for reload timer

    public virtual void Start()
    {
        // Set health and ammo to max at start
        tankHealth = maxTankHealth;
        currentAmmo = maxAmmo;
        cannonSource = GetComponentInChildren<CannonSource>();
    }
    public virtual void Update()
    {
        healthPercent = tankHealth / maxTankHealth; // Determine health percentage

        // Determine reload time
        float timeRemaining = (cannonSource.lastCannonFire + cannonDelay) - Time.time;  
        if (timeRemaining > 0)
        {
            reloadTimer = (cannonDelay - timeRemaining) / cannonDelay;
        }
         else
        {
            reloadTimer = 1.0f;
        }       
    }
}
