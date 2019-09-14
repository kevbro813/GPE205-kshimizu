using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawn : AIPawn
{
    public EnemyData enemyData;
    private Vector3 vectorToTarget;
    private Quaternion targetRotation;
    private int currentWaypoint = 0;
    private float waitTime;
    public float waitDuration;
    private bool atWaypoint = false;
    public float waypointRange = 1.0f;
    private float searchTime;
    public float searchDuration = 3;
    private float investigateTime;
    public float investigateDuration = 3;
    private bool isPatrolForward = true;
    public bool isInvestigating = false;
    public bool isSearching = false;
    public Transform ptf;
    public PatrolType patrolType;
    public WaypointType waypointType;
    public Transform[] enemyWaypoints;
    public AIVision aiVision;
    public AIHearing aiHearing;
    public enum WaypointType { Global, Local}
    public enum PatrolType { Random, Stop, Loop, PingPong };

    void Start()
    {
        tf = GetComponent<Transform>();
        characterController = GetComponent<CharacterController>();
        enemyData = GetComponent<EnemyData>();
        ptf = GameManager.instance.playerTank.GetComponent<Transform>();
        aiVision = GetComponentInChildren<AIVision>();
        aiHearing = GetComponentInChildren<AIHearing>();
    }
    public void Awake()
    {
        if (waypointType == WaypointType.Global)
        {
            enemyWaypoints = GameManager.instance.enemyWaypoints;
        }
        if (patrolType == PatrolType.Random)
        {
            currentWaypoint = Random.Range(0, enemyWaypoints.Length);
        }
    }
    public bool FacingTarget(Vector3 target)
    {
        vectorToTarget = target - tf.position;

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
    public void RotateTowards(Vector3 target, float rotationSpeed)
    {
        vectorToTarget = target - tf.position;
        Quaternion targetRotation = Quaternion.LookRotation(vectorToTarget);
        tf.rotation = Quaternion.RotateTowards(tf.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    public void Patrol()
    {
        // If not facing target and not at waypoint
        if (FacingTarget(enemyWaypoints[currentWaypoint].position) == false && atWaypoint == false)
        {
            // Rotate towards target
            RotateTowards(enemyWaypoints[currentWaypoint].position, enemyData.rotationSpeed);
        }
        // If facing target and not at waypoint
        if (FacingTarget(enemyWaypoints[currentWaypoint].position) == true && atWaypoint == false)
        {
            // Move to target
            MoveTank(enemyData.forwardSpeed);
        }
        // Stop at target
        if (Vector3.SqrMagnitude(enemyWaypoints[currentWaypoint].position - tf.position) < (waypointRange * waypointRange))
        {
            atWaypoint = true; // Tank stops moving
            // After waitTime delay
            if (waitTime <= 0)
            {
                // Next waypoint is random
                if (patrolType == PatrolType.Random)
                {
                    RandomPatrol();
                }
                // Loop back to first waypoint
                if (patrolType == PatrolType.Loop)
                {
                    LoopPatrol();
                }
                // Stop at the last waypoint
                if (patrolType == PatrolType.Stop)
                {
                    StopPatrol();
                }
                // Reverse through the order of waypoints
                if (patrolType == PatrolType.PingPong)
                {
                    PingPongPatrol();
                }
                atWaypoint = false; // No longer at waypoint
                waitTime = waitDuration; // Reset waitTime
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }
    public void Flee()
    {
        // Flee when health is low
    }
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
    public void Search()
    {
        // Search for player at last known location
        if (FacingTarget(aiVision.lastPlayerLocation) == false)
        {
            // Rotate towards target
            RotateTowards(aiVision.lastPlayerLocation, enemyData.rotationSpeed);
        }
        // Stop when facing player
        if (FacingTarget(aiVision.lastPlayerLocation) == true)
        {
            // Move towards player
            MoveTank(enemyData.forwardSpeed);
        }
        // Rotate 360 degrees
    }
    public void Investigate()
    {
        isInvestigating = true;
        // If not facing the direction of the sound origin
        if (FacingTarget(aiHearing.lastSoundLocation) == false)
        {
            // Rotate towards sound origin
            RotateTowards(aiHearing.lastSoundLocation, enemyData.rotationSpeed);
        }
        // Stop when facing the sound origin
        if (FacingTarget(aiHearing.lastSoundLocation) == true)
        {
            // Move towards sound origin
            MoveTank(enemyData.forwardSpeed);
        }
        if (tf.position == aiHearing.lastSoundLocation)
        {
            Debug.Log("Done Investigating");
            isInvestigating = false;
        }
        // TODO: Rotate 360 degrees
    }
    public void Idle()
    {
        // Do nothing.
    }
    // Forward then reverse order waypoint movement
    public void PingPongPatrol()
    {
        if (currentWaypoint == enemyWaypoints.Length - 1)
        {
            isPatrolForward = false;
        }
        if (currentWaypoint == 0)
        {
            isPatrolForward = true;
        }
        if (isPatrolForward == true)
        {
            currentWaypoint++;
        }
        if (isPatrolForward == false)
        {
            currentWaypoint--;
        }
    }
    // Move to a random waypoint
    public void RandomPatrol()
    {
        currentWaypoint = Random.Range(0, enemyWaypoints.Length);
    }
    // Loop through waypoints
    public void LoopPatrol()
    {
        if (currentWaypoint < enemyWaypoints.Length - 1)
        {
            currentWaypoint++;
        }
        else
        {
            currentWaypoint = 0;
        }
    }
    // Stop at last waypoint
    public void StopPatrol()
    {
        if (currentWaypoint < enemyWaypoints.Length - 1)
        {
            currentWaypoint++;
        }
    }
}
