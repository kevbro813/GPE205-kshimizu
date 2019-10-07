using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages spawning of all game objects into the world
public class SpawnManager : MonoBehaviour
{
    public void SpawnPlayerTank()
    {
        Transform spawnPoint = GameManager.instance.playerSpawnsList[Random.Range(0, GameManager.instance.playerSpawnsList.Count)]; // Random spawnPoint from list
        // Instantiate a player tank prefab
        GameObject playerClone = Instantiate(GameManager.instance.tankPrototype, spawnPoint.position, spawnPoint.rotation, GameManager.instance.playerTankShell);
        PlayerData tempPlayerData = playerClone.GetComponent<PlayerData>(); // Temporary playerData component variable

        GameManager.instance.playerSpawnsList.Remove(spawnPoint); // Remove spawn point from list when used

        GameManager.instance.activePlayersList.Add(playerClone); // Adds to list of all active player objects
        GameManager.instance.playerDataList.Add(tempPlayerData); // Adds to list of all active playerData components
        GameManager.instance.tankObjectList.Add(playerClone); // Adds to list of all active player and AI tank objects
        GameManager.instance.tankDataList.Add(playerClone.GetComponent<TankData>()); // Adds to list of all active tankData components

        // Set index numbers for both player and tank (used to remove objects and components from lists on death/removal)
        tempPlayerData.playerIndex = GameManager.instance.activePlayersList.Count - 1;
        int currentIndex = GameManager.instance.playerDataList[GameManager.instance.playersCreated].playerIndex;
        tempPlayerData.tankIndex = GameManager.instance.tankObjectList.Count - 1;

        // Link camera to playerClone
        GameManager.instance.cameraObjects[currentIndex].GetComponent<CameraFollow>().playerTank = playerClone;

        // Link hud elements to current playerClone
        if (GameManager.instance.isMultiplayer == true) // Check if multiplayer
        {
            // Connects HUDs to correct playerData components from list
            if (currentIndex == 0)
            {
                GameManager.instance.hudComponents[1].playerData = GameManager.instance.playerDataList[currentIndex];
            }
            if (currentIndex == 1)
            {
                GameManager.instance.hudComponents[2].playerData = GameManager.instance.playerDataList[currentIndex];
            }
        }
        else
        {
            // Singleplayer HUD
            GameManager.instance.hudComponents[currentIndex].playerData = GameManager.instance.playerDataList[currentIndex];
        }
        // Increase playerCount
        GameManager.instance.playersCreated++;
        GameManager.instance.playersAlive++; // Update players alive to use in game state transitions
    }
    public IEnumerator SpawnEnemyEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameManager.instance.spawnDelay);
            // Check that enemyCount does not exceed maxEnemies
            if (GameManager.instance.maxEnemies > GameManager.instance.enemiesSpawned)
            {
                Transform spawnPoint = GameManager.instance.enemySpawnsList[Random.Range(0, GameManager.instance.enemySpawnsList.Count)]; // Random spawnPoint from list
                GameObject randomEnemy = GameManager.instance.enemyTankList[Random.Range(0, GameManager.instance.enemyTankList.Count)]; // Random enemy from list

                // Create enemyClone instance
                GameObject enemyClone = Instantiate(randomEnemy, spawnPoint.position, spawnPoint.rotation, GameManager.instance.enemyTankShell) as GameObject;

                GameManager.instance.activeEnemiesList.Add(enemyClone); // Adds spawned enemy to a list of active enemies
                EnemyData tempEnemyData = enemyClone.GetComponent<EnemyData>(); // Get EnemyData component
                GameManager.instance.enemyDataList.Add(tempEnemyData); // Adds the EnemyData component to an active list
                GameManager.instance.tankObjectList.Add(enemyClone); // Adds to list of all active player and AI tank objects
                GameManager.instance.tankDataList.Add(tempEnemyData); // Adds to list of all active tankData components

                // Assigns the spawned enemy an index number, used to remove from the two lists above when the enemy tank is destroyed
                tempEnemyData.enemyListIndex = GameManager.instance.activeEnemiesList.Count - 1;
                tempEnemyData.tankIndex = GameManager.instance.tankObjectList.Count - 1;

                GameManager.instance.enemiesSpawned++; // Increase enemyCount by one
            }
        }
    }
    public IEnumerator SpawnPickupEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameManager.instance.pickupSpawnDelay); // TODO:Change to pickupRespawnDelay after testing

            if (GameManager.instance.maxPickups > GameManager.instance.currentPickupQuantity)
            {
                Transform spawnPoint = GameManager.instance.pickupSpawnsList[Random.Range(0, GameManager.instance.pickupSpawnsList.Count)]; // Set a random spawnpoint
                GameObject randomPickup = GameManager.instance.pickupList[Random.Range(0, GameManager.instance.pickupList.Count)]; // Set a random pickup

                // Instantiate pickup object
                GameObject pickupClone = Instantiate(randomPickup, spawnPoint.position, spawnPoint.rotation, GameManager.instance.pickupShell) as GameObject;

                GameManager.instance.activePickupsList.Add(pickupClone); // Add pickup to a list of active pickups
                PickupObject tempPickupObjects = pickupClone.GetComponent<PickupObject>(); // Get PickupObject component for spawned pickup
                GameManager.instance.pickupObjectList.Add(tempPickupObjects); // Add the PickupObject component to an active list

                // Assigns the spawned pickup an index number, used to remove from the two lists above when the pickup is used
                tempPickupObjects.pickupListIndex = GameManager.instance.activePickupsList.Count - 1; // Note: I am keeping this for now despite not destroying pickups when used

                GameManager.instance.currentPickupQuantity++; // Increase pickup quantity by one     
            }
        }
    }
}
