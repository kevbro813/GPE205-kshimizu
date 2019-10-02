using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player Pawn component, this contains functions unique to the player, child of TankPawn
public class PlayerPawn : TankPawn
{
    public PlayerController playerController;
    public PlayerData playerData;
    public override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        playerData = GetComponent<PlayerData>();
        tf = GetComponent<Transform>();
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
        tankCollider = GetComponent<SphereCollider>();
    }
    public override void TankDestroyed()
    {
        // Reset player tank
        playerData.isInvulnerable = true; // Player tank cannot take damage
        playerData.isInvisible = true; // Make player tank invisible to AI

        // Make player tank invisible by disabling the mesh renderer
        foreach (MeshRenderer mesh in meshRenderer)
        {
            mesh.enabled = false;
        }
        playerController.enabled = false; // Disable the playerController

        // If the player still has lives remaining...
        if (playerData.playerLives > 0)
        {
            StartCoroutine(RespawnPlayer()); // Respawn player
            playerData.playerLives--; // Deduct one life
        }
        // If the player is out of lives, destroy the player object
        else
        {
            GameManager.instance.tankObjects.Remove(this.gameObject); // Remove tank from active enemies list
            GameManager.instance.tankDataList.Remove(this.gameObject.GetComponent<PlayerData>()); // Remove tank from active enemies list
            GameManager.instance.playerObjectsList.Remove(this.gameObject); // Remove tank from active enemies list
            GameManager.instance.playerData.Remove(this.gameObject.GetComponent<PlayerData>()); // Remove tank from active enemies list
            // Set isPlayerDead boolean based on which player tank is destroyed
            if (playerData.playerIndex == 0)
            {
                GameManager.instance.isPlayerOneDead = true;
            }
            else if (playerData.playerIndex == 1)
            {
                GameManager.instance.isPlayerTwoDead = true;
            }
        }
    }
    // Coroutine to respawn player after a set duration (playerRespawnDelay)
    public IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(GameManager.instance.playerRespawnDelay);
        if (tf != null)
        {
            // Reset all powerups
            playerData.isInvisible = false;
            playerData.isInfiniteAmmo = false;
            playerData.isInvulnerable = false;
            playerData.tankHealth = playerData.maxTankHealth;
            playerData.currentAmmo = playerData.maxAmmo;

            // Make the tank visible
            foreach (MeshRenderer mesh in meshRenderer)
            {
                mesh.enabled = true;
            }
            playerController.enabled = true; // Activate the playerController to allow inputs

            // Generate random spawnpoint from list
            Transform spawnPoint = GameManager.instance.playerSpawnsList[Random.Range(0, GameManager.instance.playerSpawnsList.Count)];
            // Spawn player in new spawnpoint
            tf.position = spawnPoint.position;

            GameManager.instance.playerSpawnsList.Remove(spawnPoint); // Remove spawn point from list when used
        }
    }
    // TODO: Rotate tank turret using mouse
    public void RotateTowardsMouse()
    {
        playerController.mousePosition.y = transform.position.y + transform.localScale.y;
        Debug.Log(playerController.mousePosition);
        tf.LookAt(playerController.mousePosition);
        Vector3 direction = new Vector3(playerController.mousePosition.x - tf.position.x, tf.position.y, playerController.mousePosition.z - tf.position.z); // Set the direction to the mouse position on the x and y axis
        tf.right -= direction; // Sets the transform direction to direction vector. Negative direction because default sprite faces down.
        // ***Use slerp to smooth tank rotation and add a lag***
    }
}
