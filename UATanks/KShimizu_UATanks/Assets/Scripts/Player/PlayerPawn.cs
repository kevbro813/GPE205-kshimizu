using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player Pawn component, this contains functions unique to the player, child of TankPawn
public class PlayerPawn : TankPawn
{
    public PlayerController playerController;
    public override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();

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
