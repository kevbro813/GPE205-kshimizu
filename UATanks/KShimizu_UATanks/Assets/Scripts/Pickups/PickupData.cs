using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PickupData
{
    // Pickups
    public float healthPickup = 0f;
    public int ammoPickup = 0;
    public int coinPickup = 0;

    // Permanent Powerups
    public float maxHealthMod = 0f;
    public float speedMod = 1.0f;

    // Temporary Powerups
    public float fireRateMod = 0f;
    public bool isPermanent = false;
    public float powerupDuration = 0f;
    public bool isInvisiblePickup = false;
    public bool isInvulnerablePickup = false;
    public bool isInfiniteAmmoPickup = false;

    public void OnActivate(TankData tank)
    {
        tank.tankHealth += healthPickup;
        tank.currentAmmo += ammoPickup;
        tank.coins += coinPickup;
        tank.forwardSpeed *= speedMod;
        tank.reverseSpeed *= speedMod;
        tank.cannonDelay /= fireRateMod;
        if (isInvisiblePickup == true)
        {
            tank.isInvisible = true; // Activate isInvisible
            if (tank.CompareTag("Player"))
            {
                Debug.Log("Is player invisible check");
                MeshRenderer[] meshRenderer = tank.gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mesh in meshRenderer)
                {
                    Color color = mesh.material.color;
                    color.a = 0.5f;
                    mesh.material.color = color;
                }
            }
        }
        if (isInvulnerablePickup == true)
        {
            tank.isInvulnerable = true; // Activate Invulnerability
        }
        if (isInfiniteAmmoPickup == true)
        {
            tank.isInfiniteAmmo = true; // Activate isInfiniteAmmo
        }
    }
    public void OnDeactivate(TankData tank)
    {
        tank.tankHealth -= healthPickup;
        tank.currentAmmo -= ammoPickup;
        tank.coins -= coinPickup;
        tank.forwardSpeed /= speedMod;
        tank.reverseSpeed /= speedMod;
        tank.cannonDelay *= fireRateMod;
        tank.isInvisible = false; // Deactivate isInvisible
        tank.isInfiniteAmmo = false; // Deactivate isInfiniteAmmo
        tank.isInvulnerable = false; // Deactivate Invulnerability
    }
}
