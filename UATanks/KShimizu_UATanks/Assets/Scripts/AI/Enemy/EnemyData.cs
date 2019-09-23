﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Contains all the variables common to all enemies
public class EnemyData : AIData
{
    public int pointValue; // Number of points granted to the player when the enemy tank is destroyed
    public PlayerData playerData;
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
        playerData = GameManager.instance.playerData;
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
    // Update is called once per frame
    void Update()
    {
        TankDestroyed(); // Call the TankDestroyed method to check if the tank is out of health, and destroys the tank if necessary
    }
    // Function to destroy the enemy tank and increase the player's score
    public override void TankDestroyed()
    {
        if(tankHealth <= 0)
        {
            playerData.playerScore += pointValue; // Add to player's score
            GameManager.instance.activeEnemiesList.Remove(this.gameObject); // Remove tank from active enemies list
            GameManager.instance.enemyDataList.Remove(this.gameObject.GetComponent<EnemyData>()); // Remove tankData from list
        }
        base.TankDestroyed(); // Destroys tank
    }
}
