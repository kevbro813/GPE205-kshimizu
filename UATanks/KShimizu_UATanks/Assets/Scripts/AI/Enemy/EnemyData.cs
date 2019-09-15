using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : AIData
{
    public int pointValue; // Number of points granted to the player when the enemy tank is destroyed
    public PlayerData playerData;
    public Transform[] enemyWaypoints;
    public int currentWaypoint = 0;
    public PatrolType patrolType;
    public WaypointType waypointType;
    public AIState aiState;
    public AIState tempState;
    public enum WaypointType { Global, Local }
    public enum PatrolType { Random, Stop, Loop, PingPong };
    public enum AIState { Idle, Patrol, Attack, Search, Pursue, Flee, Investigate, Avoidance }
    // Start is called before the first frame update
    void Start()
    {
        playerData = GameManager.instance.playerData;
        randomRotation = Random.Range(rotationLow, rotationHigh);
    }
    void Awake()
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
    // Update is called once per frame
    void Update()
    {
        TankDestroyed(); // Call the TankDestroyed method to check if the tank is out of health, and destroys the tank if necessary
    }
    public override void TankDestroyed()
    {
        base.TankDestroyed();
        // Add to player's score
        if(tankHealth <= 0)
        {
            playerData.playerScore += pointValue;
        }
    }
}
