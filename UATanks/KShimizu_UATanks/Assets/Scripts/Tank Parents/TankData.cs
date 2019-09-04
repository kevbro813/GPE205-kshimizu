using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TankData : MonoBehaviour
{
    // Public variables for tanks
    public float tankHealth;
    public float tankArmor;
    public float forwardSpeed = 1.0f;
    public float reverseSpeed = 0.5f;
    public float rotationSpeed = 1.0f;
    public float projectileDuration = 2.0f;
    public float cannonDelay = 3.0f;
    public float shellForce = 1.0f; // Speed of the shell
    public float shellDamage = 20.0f; // Damage dealt by the shell

    // Method to check if tank is out of health and destroys the tank if tankHealth <= 0
    public virtual void TankDestroyed()
    {
        if (tankHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
