using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component attached to a child of the player and AI tank objects. Instantiates a projectile for the main cannon
public class CannonSource : MonoBehaviour
{
    public GameObject projectile; // Set the projectile in the inspector
    public TankData tankData;
    private Transform tf;
    private float lastCannonFire; // Variable that stores the last time the cannon was fired
    private bool cannonLoaded = true; // Bool is true when the timer expires, allowing the cannon can be fired again

    void Start()
    {
        tf = GetComponent<Transform>();
        tankData = GetComponentInParent<TankData>();
        lastCannonFire = Time.time; // Last cannon is set to Time.time by default
    }

    public void FireCannon()
    {
        // Timer used to determine if the cannon is loaded
        if (Time.time >= lastCannonFire + tankData.cannonDelay)
        {
            cannonLoaded = true;
        }
        // If the cannon is loaded...
        if (cannonLoaded == true)
        {
            // If the tank has ammo or has the infinite ammo powerup...
            if (tankData.currentAmmo > 0 || tankData.isInfiniteAmmo == true)
            {
                cannonLoaded = false; // Set cannonLoaded to false
                tankData.isInvisible = false; // Make the tank visible after a round is fired
                
                if (tankData.isInfiniteAmmo == false) // If the tank does not have the infinite ammo powerup...
                {
                    tankData.currentAmmo--; // Deduct one round from currentAmmo
                }
                // Create a projectile instance
                GameObject projectileClone = Instantiate(projectile, tf.position, tf.rotation, this.transform.parent) as GameObject;

                // Destroy the projectileClone after a set duration
                if (projectile != null)
                {
                    Destroy(projectileClone, tankData.projectileDuration);
                }
                // Reset lastCannonFire
                lastCannonFire = Time.time;
            }
        }
    }
}
