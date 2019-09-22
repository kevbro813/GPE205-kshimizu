using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component attached to a cannon shell, provides movement and collision detection
public class CannonShell : MonoBehaviour
{
    private Rigidbody rb;
    private TankData tankData;
    private PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tankData = GetComponentInParent<TankData>();
        playerData = GameManager.instance.playerData;
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
            EnemyData enemyData = col.gameObject.GetComponent<EnemyData>();
            if (enemyData.isInvulnerable == false)
            {
                // Deal damage to enemy tank
                enemyData.tankHealth -= tankData.shellDamage;
                Debug.Log(enemyData.tankHealth);
            }
            else
            {
                Debug.Log("Enemy is invulnerable");
            }
            Destroy(gameObject); // Destroy the cannon shell if it hits an enemy tank
        }
        else if (col.gameObject.CompareTag("Player")) // Check if the collider hit has the "Player" tag
        {
            PlayerData playerData = col.gameObject.GetComponent<PlayerData>();
            if (playerData.isInvulnerable == false)
            {
                // Deal damage to player tank
                playerData.tankHealth -= tankData.shellDamage;
            }
            else
            {
                Debug.Log("Player is invulnerable");
            }
            Destroy(gameObject); // Destroy the cannon shell if it hits an enemy tank
        }
        else if (col.gameObject.CompareTag("Arena"))
        {
            Destroy(gameObject); // Destroy the cannon shell if it hits any other collider
        }
    }
}
