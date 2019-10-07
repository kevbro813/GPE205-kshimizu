using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Detects if an object is close to a spawnpoint. If an object is close, it will remove the spawn from the list of active spawns and add it once the object
is no longer near the spawnpoint*/
public class SpawnProximity : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        // Removes from list if a player, enemy or pickup object is near the respective spawnpoint
        if (other.CompareTag("Player")) 
        {
            GameManager.instance.playerSpawnsList.Remove(this.gameObject.GetComponent<Transform>());
        }
        if (other.CompareTag("Enemy"))
        {
            GameManager.instance.enemySpawnsList.Remove(this.gameObject.GetComponent<Transform>());
        }
        if (other.CompareTag("Pickup"))
        {
            GameManager.instance.pickupSpawnsList.Remove(this.gameObject.GetComponent<Transform>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // Adds spawnpoint back to list when the object leaves the trigger area
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerSpawnsList.Add(this.gameObject.GetComponent<Transform>());
        }
        if (other.CompareTag("Enemy"))
        {
            GameManager.instance.enemySpawnsList.Add(this.gameObject.GetComponent<Transform>());
        }
        if (other.CompareTag("Pickup"))
        {
            GameManager.instance.pickupSpawnsList.Add(this.gameObject.GetComponent<Transform>());
        }
    }
}
