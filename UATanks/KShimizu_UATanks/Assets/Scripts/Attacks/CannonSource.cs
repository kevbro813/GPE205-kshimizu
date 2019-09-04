using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonSource : MonoBehaviour
{
    public GameObject projectile; // Set the projectile in the inspector
    public PlayerData playerData;
    private Transform tf;
    private float lastCannonFire; // Variable that stores the last time the cannon was fired
    private bool cannonLoaded = true; // Bool is true when the timer expires, allowing the cannon can be fired again

    void Start()
    {
        tf = GetComponent<Transform>();
        playerData = GameManager.instance.playerData;
        lastCannonFire = Time.time; // Last cannon is set to Time.time by default
    }

    public void FireCannon()
    {
        // If the cannon is loaded...
        if (cannonLoaded == true)
        {
            cannonLoaded = false; // Set cannonLoaded to false

            // Create a projectile instance
            GameObject projectileClone = Instantiate(projectile, tf.position, tf.rotation, this.transform.parent) as GameObject;

            // Destroy the projectileClone after a set duration
            if (projectile != null)
            {
                Destroy(projectileClone, playerData.projectileDuration);
            }

            // Reset lastCannonFire
            lastCannonFire = Time.time;
        }

        // Timer used to determine if the cannon is loaded
        if (Time.time >= lastCannonFire + playerData.cannonDelay)
        {
            cannonLoaded = true;
        }
    }
}
