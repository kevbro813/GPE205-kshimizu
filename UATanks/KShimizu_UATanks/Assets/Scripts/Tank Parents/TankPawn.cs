using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent component of all AI and player pawns
public abstract class TankPawn : MonoBehaviour
{
    public Transform tf;
    public TankData tankData;
    public CharacterController characterController;
    public CannonSource cannonSource; // Component on child object that will instantiate the projectile
    public MeshRenderer[] meshRenderer;
    public SphereCollider tankCollider;

    public virtual void Start()
    {
        tankData = GetComponent<TankData>();
        tf = GetComponent<Transform>();
        cannonSource = GetComponentInChildren<CannonSource>();
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
    }
    // Method to check if tank is out of health and destroys the tank if tankHealth <= 0
    public virtual void TankDestroyed()
    {
        GameManager.instance.soundManager.SoundTankDestroyed();
        Destroy(this.gameObject);
    }
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
    // Set tank to translucent on screen
    public void SetInvisible()
    {
        // Loop through all MeshRenderer components in children to make translucent
        foreach (MeshRenderer mesh in meshRenderer)
        {
            Color color = mesh.material.color;
            color.a = tankData.translucency;
            mesh.material.color = color;
        }
    }
    // Set tank to opaque on screen
    public void SetVisible()
    {
        foreach (MeshRenderer mesh in meshRenderer)
        {
            Color color = mesh.material.color;
            color.a = 1.0f; // Set to opaque
            mesh.material.color = color;
        }
    }
}
