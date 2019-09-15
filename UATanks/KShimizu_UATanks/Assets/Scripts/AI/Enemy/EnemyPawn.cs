using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawn : AIPawn
{
    public EnemyData enemyData;
    public Transform ptf;
    public AIVision aiVision;
    public AIHearing aiHearing;
    public bool atWaypoint = false;
    public bool atSearchLocation = false;
    public bool atInvestigateLocation = false;
    [HideInInspector] public bool isInvestigating = false;
    [HideInInspector] public bool isSearching = false;
    private Quaternion targetRotation;
    private float waitTime;
    private bool isPatrolForward = true;
    private float investigateTime;
    private float searchTime;
    private bool isTurned = false;
    public RaycastHit obstacleHit;
    void Start()
    {
        tf = GetComponent<Transform>();
        characterController = GetComponent<CharacterController>();
        enemyData = GetComponent<EnemyData>();
        ptf = GameManager.instance.playerTank.GetComponent<Transform>();
        aiVision = GetComponentInChildren<AIVision>();
        aiHearing = GetComponentInChildren<AIHearing>();
        investigateTime = enemyData.investigateDuration;
        searchTime = enemyData.searchDuration;
    }

    // Check if the tank is facing the target and return a bool
    public bool FacingTarget(Vector3 target)
    {
        Vector3 vectorToTarget = target - tf.position;

        Quaternion targetRotation = Quaternion.LookRotation(vectorToTarget);

        if (tf.rotation == targetRotation)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool BackToTarget(Vector3 target)
    {
        Vector3 vectorAwayFromTarget = (target - tf.position) * -1;

        Quaternion targetRotation = Quaternion.LookRotation(vectorAwayFromTarget);

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
        if (Physics.Raycast(tf.position, tf.forward, out obstacleHit, enemyData.avoidanceDistance))
        {
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
        // Flee when health is low
        if (BackToTarget(ptf.position) == false && isTurned == false)
        {
            RotateAway(ptf.position, enemyData.rotationSpeed);
        }
        if (BackToTarget(ptf.position) == true)
        {
            isTurned = true;
        }
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
                waitTime -= Time.deltaTime;
            }
        }
    }

    // Attack the player tank
    public void Attack()
    {
        // Rotate towards player
        if (FacingTarget(ptf.position) == false)
        {
            // Rotate towards target
            RotateTowards(ptf.position, enemyData.rotationSpeed);
        }
        // Stop when aimed at player
        if (FacingTarget(ptf.position) == true)
        {
            // Fire projectile
            SingleCannonFire();
        }
    }

    // Pursue the player tank
    public void Pursue()
    {
        // Chase after player
        if (FacingTarget(ptf.position) == false)
        {
            // Rotate towards target
            RotateTowards(ptf.position, enemyData.rotationSpeed);
        }
        // Stop when facing player
        if (FacingTarget(ptf.position) == true)
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
            if (FacingTarget(aiVision.lastPlayerLocation) == false && atSearchLocation == false)
            {
                // Rotate towards target
                RotateTowards(aiVision.lastPlayerLocation, enemyData.rotationSpeed);
            }
            // Stop when facing player
            if (FacingTarget(aiVision.lastPlayerLocation) == true && atSearchLocation == false)
            {
                // Move towards player
                MoveTank(enemyData.forwardSpeed);
            }
            // If the tank is within range of the last known location of the player...
            if (Vector3.SqrMagnitude(tf.position - aiVision.lastPlayerLocation) < (enemyData.waypointRange * enemyData.waypointRange))
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
            if (FacingTarget(aiHearing.lastSoundLocation) == false)
            {
                // Rotate towards sound origin
                RotateTowards(aiHearing.lastSoundLocation, enemyData.rotationSpeed);
            }
            // If the AI is facing the sound location...
            if (FacingTarget(aiHearing.lastSoundLocation) == true)
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
        if (enemyData.currentWaypoint == enemyData.enemyWaypoints.Length - 1)
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
        enemyData.currentWaypoint = Random.Range(0, enemyData.enemyWaypoints.Length);
    }

    // Loop through waypoints
    public void LoopPatrol()
    {
        if (enemyData.currentWaypoint < enemyData.enemyWaypoints.Length - 1)
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
        if (enemyData.currentWaypoint < enemyData.enemyWaypoints.Length - 1)
        {
            enemyData.currentWaypoint++;
        }
    }
}
