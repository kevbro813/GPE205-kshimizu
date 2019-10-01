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
    }
    public override void TankDestroyed()
    {
        StartCoroutine(RespawnPlayer());
        // Reset player tank
        playerData.tankHealth = playerData.maxTankHealth;
        playerData.currentAmmo = playerData.maxAmmo;
        playerData.isInvisible = false;
        playerData.isInvulnerable = true;
        playerData.isInfiniteAmmo = false;
        playerData.playerLives--; // Deduct one life
        // If the player is out of lives, destroy the player object
        if (playerData.playerLives < 0)
        {
            GameManager.instance.tankObjects.Remove(this.gameObject); // Remove tank from active enemies list
            GameManager.instance.tankDataList.Remove(this.gameObject.GetComponent<PlayerData>()); // Remove tank from active enemies list
            GameManager.instance.playerObjectsList.Remove(this.gameObject); // Remove tank from active enemies list
            GameManager.instance.playerData.Remove(this.gameObject.GetComponent<PlayerData>()); // Remove tank from active enemies list
            if (playerData.playerIndex == 0)
            {
                GameManager.instance.isPlayerOneDead = true;
            }
            else if (playerData.playerIndex == 1)
            {
                GameManager.instance.isPlayerTwoDead = true;
            }
            Destroy(this.gameObject);
            // TODO: Once game states has been completed, this should send the player to the "Lose" screen
        }
    }
    public IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(GameManager.instance.playerRespawnDelay);
        if (tf != null)
        {
            // Generate random spawnpoint from list
            Transform spawnPoint = GameManager.instance.playerSpawnsList[Random.Range(0, GameManager.instance.playerSpawnsList.Count)];
            // Spawn player in new spawnpoint
            tf.position = spawnPoint.position;
            playerData.isInvulnerable = false;
            GameManager.instance.activePlayerSpawnsList.Remove(spawnPoint); // Remove spawn point from list when used
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
