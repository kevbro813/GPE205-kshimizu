using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains all the Enemy Pawn Functions: movement, attacks, obstacle avoidance, etc.
public class EnemyPawn : AIPawn
{
    public EnemyData enemyData;
    //public Transform ptf; // Player transform component
    public AIVision aiVision; // Vision component
    public AIHearing aiHearing; // Hearing component
    public bool atWaypoint = false; // Indicates whether the AI is currently at a waypoint while patrolling
    public bool atSearchLocation = false; // Indicates whether the AI is currently at the last known player location while searching
    public bool atInvestigateLocation = false; // Indicates whether the AI is currently looking in the direction of a sound while investigating
    public bool atAlertLocation = false; // Indicates whether the AI is currently at the alert location
    public bool isInvestigating = false; // Switching from true to false allows the AI to transition from investigate state to patrol state
    public bool isSearching = false; // Switching from true to false allows the AI to transition from search state to patrol state
    public bool isAlertActive = false; // Switching from true to false allows the AI to transition from alert state to patrol state
    private float waitTime; // Used in timer to determine how long to wait at a waypoint while patrolling
    private bool isPatrolForward = true; // Used in "PingPong" patrol type to loop through the waypoints in reverse order
    private float investigateTime; // How long the AI will investigate before returning to patrol
    private float searchTime; // How long the AI will search before returning to patrol
    private float alertTime; // How long the AI will be alert before returning to patrol
    private bool isTurned = false; // Is the tank turned around (Used in Flee function)
    public RaycastHit obstacleHit; // Raycast hit for obstacles (Includes the arena and other enemy tanks)
    public override void Start()
    {
        base.Start();
        characterController = GetComponent<CharacterController>();
        enemyData = GetComponent<EnemyData>();
        //ptf = GameManager.instance.playerTank.GetComponent<Transform>();
        aiVision = GetComponentInChildren<AIVision>();
        aiHearing = GetComponentInChildren<AIHearing>();

        // Set investigateTime, searchTime and waitTime to the default values saved in Enemy Data
        investigateTime = enemyData.investigateDuration; 
        searchTime = enemyData.searchDuration;
        waitTime = enemyData.waitDuration;
        alertTime = enemyData.alertDuration;
    }
    // Function to destroy the enemy tank and increase the player's score
    public override void TankDestroyed()
    {
        GameManager.instance.activeEnemiesList.Remove(this.gameObject); // Remove tank from active enemies list
        GameManager.instance.tankObjects.Remove(this.gameObject); // Remove tank from active enemies list
        GameManager.instance.tankDataList.Remove(this.gameObject.GetComponent<EnemyData>()); // Remove tank from active enemies list
        GameManager.instance.enemyDataList.Remove(this.gameObject.GetComponent<EnemyData>()); // Remove tankData from list
        base.TankDestroyed(); // Destroys tank
    }
    // Check if the tank is facing the target and return a bool
    public bool FacingTarget(Vector3 target)
    {
        Vector3 vectorToTarget = target - tf.position; // Set target vector towards target

        Quaternion targetRotation = Quaternion.LookRotation(vectorToTarget); // Rotate towards target

        if (tf.rotation == targetRotation)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // Check if the tank is facing away from the target and return a bool
    public bool BackToTarget(Vector3 target)
    {
        Vector3 vectorAwayFromTarget = (target - tf.position) * -1; // Set vector in the opposite direction of target

        Quaternion targetRotation = Quaternion.LookRotation(vectorAwayFromTarget); // Rotate away from target

        if (tf.rotation == targetRotation)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // Rotate the tank towards a target
    public void RotateTowards(Vector3 target, float rotationSpeed)
    {
        Vector3 vectorToTarget = target - tf.position;
        Quaternion targetRotation = Quaternion.LookRotation(vectorToTarget);
        tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    // Rotate tank away from target
    public void RotateAway(Vector3 target, float rotationSpeed)
    {
        Vector3 vectorAwayFromTarget = (target - tf.position) * -1;
        Quaternion targetRotation = Quaternion.LookRotation(vectorAwayFromTarget);
        tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    // TODO: Create a random rotation value to add randomness to AI obstacle avoidance maneuvering
    public void RandomRotation(Vector3 target, float rotationSpeed)
    {
        Vector3 vectorAwayFromTarget = (target - tf.position) * enemyData.randomRotation;
        Debug.Log(enemyData.randomRotation);
        Quaternion targetRotation = Quaternion.LookRotation(vectorAwayFromTarget);
        tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // Rotate and move away from obstacles
    public void ObstacleAvoidance(RaycastHit hit)
    {
        RotateAway(obstacleHit.point, enemyData.rotationSpeed);
        MoveTank(enemyData.forwardSpeed);
    }

    // Check if there is an obstacle in the way and return a bool
    public bool ObstacleCheck()
    {
        // Raycast forward from AI
        if (Physics.Raycast(tf.position, tf.forward, out obstacleHit, enemyData.avoidanceDistance))
        {
            // If the collider hit is the arena or another enemy tank...
            if (obstacleHit.collider.CompareTag("Arena") || obstacleHit.collider.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }
    // Flee when health is low
    public void Flee()
    {
        // Rotate away from the target
        if (BackToTarget(enemyData.lastPlayerLocation) == false && isTurned == false)
        {
            RotateAway(enemyData.lastPlayerLocation, enemyData.rotationSpeed);
        }
        // Once facing away from target, set isTurned to true
        if (BackToTarget(enemyData.lastPlayerLocation) == true)
        {
            isTurned = true;
        }
        // When the tank is done rotating, move the tank forward
        if (isTurned == true)
        {
            MoveTank(enemyData.forwardSpeed);
        }
    }

    // Patrol function with alternative patrol types
    public void Patrol()
    { 
        // If not facing target and not at waypoint
        if (FacingTarget(enemyData.enemyWaypoints[enemyData.currentWaypoint].position) == false && atWaypoint == false)
        {
            // Rotate towards target
            RotateTowards(enemyData.enemyWaypoints[enemyData.currentWaypoint].position, enemyData.rotationSpeed);
        }
        // If facing target and not at waypoint
        if (FacingTarget(enemyData.enemyWaypoints[enemyData.currentWaypoint].position) == true && atWaypoint == false)
        {
            // Move to target
            MoveTank(enemyData.forwardSpeed);
        }
        // Stop at target
        if (Vector3.SqrMagnitude(enemyData.enemyWaypoints[enemyData.currentWaypoint].position - tf.position) < (enemyData.waypointRange * enemyData.waypointRange))
        {
            atWaypoint = true; // Tank stops moving
            // After waitTime delay
            if (waitTime <= 0)
            {
                // Next waypoint is random
                if (enemyData.patrolType == EnemyData.PatrolType.Random)
                {
                    RandomPatrol();
                }
                // Loop back to first waypoint
                if (enemyData.patrolType == EnemyData.PatrolType.Loop)
                {
                    LoopPatrol();
                }
                // Stop at the last waypoint
                if (enemyData.patrolType == EnemyData.PatrolType.Stop)
                {
                    StopPatrol();
                }
                // Reverse through the order of waypoints
                if (enemyData.patrolType == EnemyData.PatrolType.PingPong)
                {
                    PingPongPatrol();
                }
                atWaypoint = false; // No longer at waypoint
                waitTime = enemyData.waitDuration; // Reset waitTime
            }
            else
            {
                waitTime -= Time.deltaTime; // Decrement time
            }
        }
    }
    public void Alerted()
    {
        if (isAlertActive == true)
        {
            // Go to the last known player location (global)
            if (FacingTarget(GameManager.instance.lastPlayerLocation) == false && atAlertLocation == false)
            {
                // Rotate towards target
                RotateTowards(GameManager.instance.lastPlayerLocation, enemyData.rotationSpeed);
            }
            // Stop when facing target
            if (FacingTarget(GameManager.instance.lastPlayerLocation) == true && atAlertLocation == false)
            {
                // Move towards target
                MoveTank(enemyData.forwardSpeed);
            }
            // If the tank is within range of the last known location of the player...
            if (Vector3.SqrMagnitude(tf.position - GameManager.instance.lastPlayerLocation) < (enemyData.waypointRange * enemyData.waypointRange))
            {
                atAlertLocation = true; // AI tank is now at the location
                if (atAlertLocation == true)
                {
                    if (alertTime < 0) // After a delay...
                    {
                        alertTime = enemyData.alertDuration; // Reset timer
                        isAlertActive = false; // AI is no longer alert, allows AI to return to patrol
                        GameManager.instance.isAlerted = false;
                        /* Resets boolean that indicates whether AI is at the alert location
                        Prevents bug when changing states before AI finished moving to location */
                        atAlertLocation = false;
                    }
                    else
                    {
                        alertTime -= Time.deltaTime; // Decrement time
                    }
                }
            }
        }
    }
    // Attack the player tank
    public void Attack()
    {
        // Rotate towards player
        if (FacingTarget(enemyData.lastPlayerLocation) == false)
        {
            // Rotate towards target
            RotateTowards(enemyData.lastPlayerLocation, enemyData.rotationSpeed);
        }
        // Stop when aimed at player
        if (FacingTarget(enemyData.lastPlayerLocation) == true)
        {
            // Fire projectile
            SingleCannonFire();
        }
    }

    // Pursue the player tank
    public void Pursue()
    {
        // Chase after player
        if (FacingTarget(enemyData.lastPlayerLocation) == false)
        {
            // Rotate towards target
            RotateTowards(enemyData.lastPlayerLocation, enemyData.rotationSpeed);
        }
        // Stop when facing player
        if (FacingTarget(enemyData.lastPlayerLocation) == true)
        {
            // Move towards player
            MoveTank(enemyData.forwardSpeed);
        }
    }

    // Search when player tank goes from in view to out of view. Tank will navigate to the last location the player was seen
    public void Search()
    {
        if (isSearching == true)
        {
            // Search for player at last known location
            if (FacingTarget(enemyData.lastPlayerLocation) == false && atSearchLocation == false)
            {
                // Rotate towards target
                RotateTowards(enemyData.lastPlayerLocation, enemyData.rotationSpeed);
            }
            // Stop when facing player
            if (FacingTarget(enemyData.lastPlayerLocation) == true && atSearchLocation == false)
            {
                // Move towards player
                MoveTank(enemyData.forwardSpeed);
            }
            // If the tank is within range of the last known location of the player...
            if (Vector3.SqrMagnitude(tf.position - enemyData.lastPlayerLocation) < (enemyData.waypointRange * enemyData.waypointRange))
            {
                atSearchLocation = true; // AI tank is now at the location
                if (atSearchLocation == true)
                {
                    if(searchTime < 0) // After a delay...
                    {
                        searchTime = enemyData.searchDuration; // Reset timer
                        isSearching = false; // AI is no longer searching, allows AI to return to patrol
                        /* Resets boolean that indicates whether AI is at the search location
                        Prevents bug when changing states before AI finished looking at location */
                        atSearchLocation = false; 
                    }
                    else
                    {
                        searchTime -= Time.deltaTime; // Decrement time
                    }
                }
            }
            // TODO: Rotate 360 degrees
        }
    }

    // Investigate when the player is heard. AI will rotate towards the sound origin 
    public void Investigate()
    {
        if (isInvestigating == true)
        {
            // If not facing the direction of the sound origin
            if (FacingTarget(enemyData.lastSoundLocation) == false)
            {
                // Rotate towards sound origin
                RotateTowards(enemyData.lastSoundLocation, enemyData.rotationSpeed);
            }
            // If the AI is facing the sound location...
            if (FacingTarget(enemyData.lastSoundLocation) == true)
            {
                atInvestigateLocation = true; // Indicates the AI is currently facing the sound location
                if (atInvestigateLocation == true)
                {
                    if (investigateTime < 0) // After a delay...
                    {
                        investigateTime = enemyData.investigateDuration; // Reset timer
                        isInvestigating = false; // AI is no longer investigating, required for AI to return to patrol
                        /* Resets boolean that indicates the AI is at the investigation location 
                         Prevents bug when changing states before AI finished looking at location */
                        atInvestigateLocation = false; 
                    }
                    else
                    {
                        investigateTime -= Time.deltaTime; // Decrement time
                    }
                }
                // TODO: Rotate 360 degrees
            }
        }
    }

    // Tank in an idle state
    public void Idle()
    {
        // Do nothing.
    }

    // PATROL METHODS

    // Forward then reverse order waypoint movement
    public void PingPongPatrol()
    {
        if (enemyData.currentWaypoint == enemyData.enemyWaypoints.Count - 1)
        {
            isPatrolForward = false;
        }
        if (enemyData.currentWaypoint == 0)
        {
            isPatrolForward = true;
        }
        if (isPatrolForward == true)
        {
            enemyData.currentWaypoint++;
        }
        if (isPatrolForward == false)
        {
            enemyData.currentWaypoint--;
        }
    }
    // Move to a random waypoint
    public void RandomPatrol()
    {
        enemyData.currentWaypoint = Random.Range(0, enemyData.enemyWaypoints.Count);
    }
    // Loop through waypoints
    public void LoopPatrol()
    {
        if (enemyData.currentWaypoint < enemyData.enemyWaypoints.Count - 1)
        {
            enemyData.currentWaypoint++;
        }
        else
        {
            enemyData.currentWaypoint = 0;
        }
    }
    // Stop at last waypoint
    public void StopPatrol()
    {
        if (enemyData.currentWaypoint < enemyData.enemyWaypoints.Count - 1)
        {
            enemyData.currentWaypoint++;
        }
    }
}
