using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent of all AI and player Data components
public abstract class TankData : MonoBehaviour
{
    // Public variables for tanks
    public float tankHealth; // Tank's health
    public float maxTankHealth;
    public float forwardSpeed = 1.0f; // Tank's forward speed multiplier
    public float reverseSpeed = 0.5f; // Tank's reverse speed multiplier
    public float rotationSpeed = 1.0f; // Tank's rotation speed multiplier
    public float projectileDuration = 2.0f; // Duration the projectile exists before it disappears (explodes)
    public float cannonDelay = 3.0f; // Delay while tank is reloading
    public float shellForce = 1.0f; // Speed of the shell
    public float shellDamage = 20.0f; // Damage dealt by the shell
    public float criticalHealth = 60.0f;
    public int currentAmmo;
    public int maxAmmo = 10;
    public int coins;
    public bool isInvulnerable;
    public bool isInfiniteAmmo;
    public bool isInvisible;
    
    // Method to check if tank is out of health and destroys the tank if tankHealth <= 0
    public virtual void TankDestroyed()
    {
        if (tankHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
