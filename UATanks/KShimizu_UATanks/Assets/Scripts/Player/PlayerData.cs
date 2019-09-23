using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds all the player data variables, most of the variables are found in the "TankData" parent
public class PlayerData : TankData
{
    public int playerScore = 0; // Integer that tracks the player's score
    public int playerLives = 3;
    public Transform tf;
    public override void Start()
    {
        base.Start();
        tf = GetComponent<Transform>();
    }
    void Update()
    {
        TankDestroyed(); // Call the TankDestroyed method to check if the tank is out of health, and destroys the tank if necessary
    }
    public override void TankDestroyed()
    {
        // On player death...
        if (tankHealth <= 0)
        {
            playerLives--; // Deduct one life

            // Reset player tank
            tankHealth = maxTankHealth;
            currentAmmo = maxAmmo;
            isInvisible = false;
            isInvulnerable = false;
            isInfiniteAmmo = false;

            // Generate random spawnpoint from list
            Transform spawnPoint = GameManager.instance.playerSpawnsList[Random.Range(0, GameManager.instance.playerSpawnsList.Count)]; 

            // Spawn player in new spawnpoint
            tf.position = spawnPoint.position;
        }
        // If the player is out of lives, destroy the player object
        if (playerLives < 0)
        {
            Destroy(this.gameObject);
            // TODO: Once game states has been completed, this should send the player to the "Lose" screen
        }
    }
}
