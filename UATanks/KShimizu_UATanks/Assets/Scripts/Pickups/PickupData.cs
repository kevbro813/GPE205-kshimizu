using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void OnActivate(TankData tank)
    {
        tank.maxTankHealth += maxHealthMod;
        if (tank.tankHealth + healthPickup <= tank.maxTankHealth)
        {
            tank.tankHealth += healthPickup;
        }
        else
        {
            tank.tankHealth = tank.maxTankHealth;
        }
        
        if (tank.currentAmmo + ammoPickup <= tank.maxAmmo)
        {
            tank.currentAmmo += ammoPickup;
        }
        else
        {
            tank.currentAmmo = tank.maxAmmo;
        }
        tank.coins += coinPickup;

        tank.forwardSpeed *= speedMod;
        tank.reverseSpeed *= speedMod;
        tank.cannonDelay /= fireRateMod;
        

        if (isInvisiblePowerup == true)
        {
            tank.isInvisible = true; // Activate isInvisible
            if (tank.CompareTag("Player"))
            {
                MeshRenderer[] meshRenderer = tank.gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mesh in meshRenderer)
                {
                    Color color = mesh.material.color;
                    color.a = 0.5f;
                    mesh.material.color = color;
                }
            }
            tank.isInvisible = true; // Activate isInvisible
            if (tank.CompareTag("Enemy"))
            {
                MeshRenderer[] meshRenderer = tank.gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mesh in meshRenderer)
                {
                    Color color = mesh.material.color;
                    color.a = 0.1f;
                    mesh.material.color = color;
                }
            }
        }
        if (isInvulnerablePowerup == true)
        {
            tank.isInvulnerable = true; // Activate Invulnerability
        }
        if (isInfiniteAmmoPowerup == true)
        {
            tank.isInfiniteAmmo = true; // Activate isInfiniteAmmo
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
