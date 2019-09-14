using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankPawn : MonoBehaviour
{
    public Transform tf;
    public PlayerData playerData;
    public CharacterController characterController;
    public PlayerController playerController;
    public CannonSource cannonSource; // Component on child object that will instantiate the projectile


    // Tank forward and reverse movement method
    public void MoveTank(float moveSpeed)
    {
        // Forward and reverse movement using SimpleMove
        characterController.SimpleMove(tf.forward * moveSpeed * Time.deltaTime); // moveSpeed accounts for movement direction and forward/reverse speed
    }

    // Tank rotation method
    public void RotateTank(float rotationSpeed) // rotationSpeed also determines rotation direction
    {
        // Rotates the tank by multiplying the vector3 by the rotation speed and delta time
        tf.Rotate((Vector3.up * rotationSpeed * Time.deltaTime), Space.Self);
    }

    // Tank single cannon fire attack method
    public void SingleCannonFire()
    {
        // Set cannonSource if null
        if (cannonSource == null)
        {
            cannonSource = GetComponentInChildren<CannonSource>(); // Get the cannonSource component in the child of the tank object
        }
        else
        {
            cannonSource.FireCannon(); // Fire cannon
        }
    }
}
