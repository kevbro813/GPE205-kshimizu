using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnProximity : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
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
