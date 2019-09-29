using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component attached to a cannon shell, provides movement and collision detection
public class CannonShell : MonoBehaviour
{
    private Rigidbody rb;
    private TankData tankData;
    public int originTankIndex;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tankData = GetComponentInParent<TankData>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move the projectile forward using physics AddForce
        rb.AddForce(transform.up * tankData.shellForce);
    }

    // Detect if the shell collides with another collider
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Enemy")) // Check if the collider hit has the "Enemy" tag
        {
            EnemyData enemyData = col.gameObject.GetComponent<EnemyData>(); // Get EnemyData component
            if (enemyData.isInvulnerable == false) // If the enemy is NOT invulnerable...
            {
                // Deal damage to enemy tank
                enemyData.tankHealth -= tankData.shellDamage; // Deal damage
                if (enemyData.tankHealth > 0)
                {
                    tankData.score += enemyData.pointValue;
                }
                else if (enemyData.tankHealth <= 0)
                {
                    tankData.score += (int)(enemyData.pointValue * GameManager.instance.killMultiplier);
                    col.gameObject.GetComponent<EnemyPawn>().TankDestroyed();
                }
            }
            else
            {
                Debug.Log("Enemy is invulnerable"); // TODO: When message system is up, add enemy invulnerable notification
            }
            Destroy(gameObject); // Destroy the cannon shell if it hits an enemy tank
        }
        else if (col.gameObject.CompareTag("Player")) // Check if the collider hit has the "Player" tag
        {
            PlayerData playerData = col.gameObject.GetComponent<PlayerData>(); // Get PlayerData component
            if (playerData.isInvulnerable == false) // If the player is NOT invulnerable
            {
                // Deal damage to player tank
                playerData.tankHealth -= tankData.shellDamage;
                
                if (playerData.tankHealth > 0)
                {
                    tankData.score += playerData.pointValue;
                }
                else if (playerData.tankHealth <= 0)
                {
                    tankData.score += (int)(playerData.pointValue * GameManager.instance.killMultiplier);
                    col.gameObject.GetComponent<PlayerPawn>().TankDestroyed();
                }
            }
            else
            {
                Debug.Log("Player is invulnerable"); // TODO: When message system is up, add player invulnerable notification
            }
            Destroy(gameObject); // Destroy the cannon shell if it hits an enemy tank
        }
        else if (col.gameObject.CompareTag("Arena"))
        {
            Destroy(gameObject); // Destroy the cannon shell if it hits any other collider
        }
    }
}
