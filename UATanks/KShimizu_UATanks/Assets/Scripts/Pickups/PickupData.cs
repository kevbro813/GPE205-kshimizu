using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that holds all pickup data
[System.Serializable]
public class PickupData
{
    // Basic Pickups
    public float healthPickup = 0f;
    public int ammoPickup = 0;
    public int coinPickup = 0;

    // Permanent Powerups
    public float maxHealthMod = 0f;
    public float speedMod = 1.0f;

    // Temporary Powerups
    public float fireRateMod = 0f;
    public bool isInvisiblePowerup = false;
    public bool isInvulnerablePowerup = false;
    public bool isInfiniteAmmoPowerup = false;

    // Permanent or Duration
    public bool isPermanent = false;
    public float powerupDuration = 0f;

    // Function run to activate pickups
    public void OnActivate(TankData tank)
    {
        tank.maxTankHealth += maxHealthMod; // Increase max health (must be before healthPickup so currentHealth is also increased)

        // If healthPickup does not exceed maxTankHealth, then add the health
        if (tank.tankHealth + healthPickup <= tank.maxTankHealth)
        {
            tank.tankHealth += healthPickup;
        }
        // If healthPickup exceeds max health, then set tankHealth to max
        else
        {
            tank.tankHealth = tank.maxTankHealth;
        }
        
        // If ammoPickup does not exceed maxAmmo, then add the ammo
        if (tank.currentAmmo + ammoPickup <= tank.maxAmmo)
        {
            tank.currentAmmo += ammoPickup;
        }
        // If ammoPickup does exceed maxAmmo, then set currentAmmo to max
        else
        {
            tank.currentAmmo = tank.maxAmmo;
        }
        // Add coins
        tank.coins += coinPickup;  // TODO: Will be used later to purchase upgrades

        tank.forwardSpeed *= speedMod; // Increase forwardSpeed by speedMod
        tank.reverseSpeed *= speedMod; // Increase reverseSpeed by speedMod
        tank.cannonDelay /= fireRateMod; // Change cannonDelay to increase fireRate (Note that division is needed for this mod)
        
        // Invisibility Powerup
        if (isInvisiblePowerup == true)
        {
            tank.isInvisible = true; // Activate isInvisible
        }

        // Activate Invulnerability
        if (isInvulnerablePowerup == true)
        {
            tank.isInvulnerable = true; 
        }

        // Activate isInfiniteAmmo
        if (isInfiniteAmmoPowerup == true)
        {
            tank.isInfiniteAmmo = true; 
        }
    }
    public void OnDeactivate(TankData tank)
    {
        // These are intended to be permanent mods and pickups, but can still be made temporary
        tank.tankHealth -= healthPickup;
        tank.currentAmmo -= ammoPickup;
        tank.coins -= coinPickup;
        tank.maxTankHealth -= maxHealthMod;
        tank.forwardSpeed /= speedMod;
        tank.reverseSpeed /= speedMod;

        // These are intended to be temporary powerups
        tank.cannonDelay *= fireRateMod;
        tank.isInvisible = false; // Deactivate isInvisible
        tank.isInfiniteAmmo = false; // Deactivate isInfiniteAmmo
        tank.isInvulnerable = false; // Deactivate Invulnerability
    }
}
