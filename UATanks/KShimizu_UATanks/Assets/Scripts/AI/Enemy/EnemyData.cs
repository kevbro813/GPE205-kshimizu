using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains all the variables common to all enemies
public class EnemyData : AIData
{
    public List<Transform> enemyWaypoints; // Array of all the enemy waypoints being used by the AI
    public int currentWaypoint = 0; // Tracks the waypoint that is currently being used by an AI
    public PatrolType patrolType; // Allows the patrol type to be changed in the Inspector
    public WaypointType waypointType; // Allows the type of waypoints used to be changed in the Inspector
    public int enemyListIndex;

    public enum WaypointType { Global, Local } // Selection for Waypoint Type (Global are waypoints saved in GameManager, local would be local waypoints that are used with prefabs in local space
    public enum PatrolType { Random, Stop, Loop, PingPong }; // Selections for Patrol Type
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        randomRotation = Random.Range(rotationLow, rotationHigh); // Create a randomRotation at start
    }
    void Awake()
    {
        //Sets the waypoints used by the AI to the global ones saved in the Game Manager, or local waypoints that can be added in the Inspector
        if (waypointType == WaypointType.Global)
        {
            enemyWaypoints = GameManager.instance.enemyWaypointsList;
        }
        if (patrolType == PatrolType.Random)
        {
            currentWaypoint = Random.Range(0, enemyWaypoints.Count);
        }
    }
    public override void Update()
    {
        base.Update();
    }
}
