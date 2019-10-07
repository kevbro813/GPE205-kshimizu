using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component attached to a cannon shell, provides movement and collision detection
public class CannonShell : MonoBehaviour
{
    private Rigidbody rb;
    private TankData tankData;
    private Transform tf;
    public int originTankIndex;
    public GameObject explosionHit; // Set in inspector
    public GameObject explosionDestroy; // Set in inspector
    public float explosionHitDuration = 1.2f; // Duration of explosion hit FX
    public float explosionDestroyDuration = 1.2f; // Duration of explosion destroy FX

    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
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
                    GameManager.instance.soundManager.SoundTankHit(); // Play sound when tank is hit
                    tankData.score += enemyData.pointValue;
                    // Add explosionHit FX
                    GameObject explosionHitClone = Instantiate(explosionHit, tf.position, tf.rotation);
                    Destroy(explosionHitClone, explosionHitDuration);
                }
                else if (enemyData.tankHealth <= 0)
                {
                    tankData.score += (int)(enemyData.pointValue * GameManager.instance.killMultiplier); // Add score multiplier for destroying tank
                    // Check tank destroyed after collision, rather than checking in Update() to lower resource requirement
                    col.gameObject.GetComponent<EnemyPawn>().TankDestroyed();
                    // Add explosionDestroy FX
                    GameObject explosionDestroyClone = Instantiate(explosionDestroy, tf.position, tf.rotation);
                    Destroy(explosionDestroyClone, explosionDestroyDuration);
                }   
            }
            else
            {
                Debug.Log("Enemy is invulnerable"); // TODO: When message system is up, add enemy invulnerable notification
            }
            Destroy(this.gameObject); // Destroy the cannon shell if it hits an enemy tank
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
                    GameManager.instance.soundManager.SoundTankHit(); // Play sound when tank is hit
                    tankData.score += playerData.pointValue;
                    // Add explosionHit FX
                    GameObject explosionHitClone = Instantiate(explosionHit, tf.position, tf.rotation);
                    Destroy(explosionHitClone, explosionHitDuration);
                }
                else if (playerData.tankHealth <= 0)
                {
                    tankData.score += (int)(playerData.pointValue * GameManager.instance.killMultiplier); // Add score multiplier for destroying tank
                    // Check tank destroyed after collision, rather than checking in Update() to lower resource requirement
                    col.gameObject.GetComponent<PlayerPawn>().TankDestroyed();
                    // Add explosionDestroy FX
                    GameObject explosionDestroyClone = Instantiate(explosionDestroy, tf.position, tf.rotation);
                    Destroy(explosionDestroyClone, explosionDestroyDuration);
                }
            }
            else
            {
                Debug.Log("Player is invulnerable"); // TODO: When message system is up, add player invulnerable notification
            }

            Destroy(this.gameObject); // Destroy the cannon shell if it hits an enemy tank

        }
        else if (col.gameObject.CompareTag("Arena"))
        {
            GameObject explosionHitClone = Instantiate(explosionHit, tf.position, tf.rotation);
            Destroy(explosionHitClone, explosionHitDuration);
            Destroy(this.gameObject); // Destroy the cannon shell if it hits any other collider
        }
    }
}
