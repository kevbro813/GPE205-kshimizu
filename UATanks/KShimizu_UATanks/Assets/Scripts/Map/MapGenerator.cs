using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Component used to generate a map based on selected settings
public class MapGenerator : MonoBehaviour
{
    public GameObject[] mapTiles;
    // Used to change the size of the map
    public int tileRows; 
    public int tileColumns;

    // Set to 50.0f because that is the size of the map tiles
    private float roomWidth = 50.0f;
    private float roomHeight = 50.0f;

    private int randomTile;
    public int mapSeed; // Map seed can be set to map of day or random based on day/time
    public MapType mapType; // Enum with the map type options
    public enum MapType { MapOfTheDay, Random, PresetSeed }
    
    public void GenerateMap()
    {
        // If set to Map of the Day, use date to determine seed
        if (mapType == MapType.MapOfTheDay)
        {
            mapSeed = DateToInt(DateTime.Now.Date);
        }
        // If set to Random, use time to determine seed
        else if (mapType == MapType.Random)
        {
            mapSeed = DateToInt(DateTime.Now);
        }
        // If set to preset seed, use the seed set in the inspector
        else if (mapType == MapType.PresetSeed)
        {
            // Do nothing. Allows seed to be set in inspector
        }

        UnityEngine.Random.InitState(mapSeed); // Set randomization state to mapSeed

        GameManager.instance.grid = new Room[tileColumns, tileRows]; // Create a new grid
        
        // The following loops through rows and columns to generate the map
        for (int i = 0; i < tileRows; i++)
        {
            for (int j = 0; j < tileColumns; j++)
            {
                // Used to position the tile
                float xPosition = roomWidth * j;
                float zPosition = roomHeight * i;

                Vector3 tilePosition = new Vector3(xPosition, 0.0f, zPosition); // Set tile position

                // Instantiate the map tile
                GameObject tempTile = Instantiate(RandomTile(), tilePosition, Quaternion.identity) as GameObject;

                // Set parent of map tile
                tempTile.transform.parent = this.transform;

                // Name the map tile
                tempTile.name = "Room_" + j + "," + i;

                // Get the room component of the map tile
                Room tempRoom = tempTile.GetComponent<Room>();

                // Open Doors of the map tile based on where it is located
                if (i == 0)
                {
                    tempRoom.doorNorth.SetActive(false);
                }
                else if (i == tileRows - 1)
                {
                    tempRoom.doorSouth.SetActive(false);
                }
                else
                {
                    tempRoom.doorNorth.SetActive(false);
                    tempRoom.doorSouth.SetActive(false);
                }
                if (j == 0)
                {
                    tempRoom.doorEast.SetActive(false);
                }
                else if (j == tileColumns - 1)
                {
                    tempRoom.doorWest.SetActive(false);
                }
                else
                {
                    tempRoom.doorEast.SetActive(false);
                    tempRoom.doorWest.SetActive(false);
                }
                GameManager.instance.grid[j, i] = tempRoom; // Add the map tile to the grid saved in GameManager
            }
        }
        CreateSpawnList(); // Creates a list of all spawn points
        CreateWaypointList(); // Creates a list of all enemy waypoints
    }

    // Generates a random tile
    public GameObject RandomTile()
    {
        return mapTiles[UnityEngine.Random.Range(0, mapTiles.Length)];
    }
    // Used for random mapSeed
    public int DateToInt(DateTime dateTime)
    {
        return dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond;
    }
    // Creates a list of enemy waypoints
    public void CreateWaypointList()
    {
        GameObject[] tempEnemyWaypoints = GameObject.FindGameObjectsWithTag("EnemyWaypoint");

        // Loop through and add all enemyWaypoints ot the enemyWaypointList in GameManager
        for (int i = 0; i < tempEnemyWaypoints.Length; i++)
        {
            GameManager.instance.enemyWaypointsList.Add(tempEnemyWaypoints[i].GetComponent<Transform>());
        }
    }
    // Creates a list of all spawnpoints including enemy, player and pickups
    public void CreateSpawnList()
    {
        GameObject[] tempPlayerSpawns = GameObject.FindGameObjectsWithTag("PlayerSpawn");

        // Loop through and add all player spawns to the playerSpawnsList in GameManager
        for (int i = 0; i < tempPlayerSpawns.Length; i++)
        {
            GameManager.instance.playerSpawnsList.Add(tempPlayerSpawns[i].GetComponent<Transform>());
        }

        GameObject[] tempEnemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawn"); 

        // Loop through and add all enemy spawns in the enemySpawnsList in GameManager
        for (int i = 0; i < tempEnemySpawns.Length; i++)
        {
            GameManager.instance.enemySpawnsList.Add(tempEnemySpawns[i].GetComponent<Transform>());
        }

        GameObject[] tempPickupSpawns = GameObject.FindGameObjectsWithTag("PickupSpawn");

        // Loop through and add all pickup spawns in the pickupSpawnsList in GameManager
        for (int i = 0; i < tempPickupSpawns.Length; i++)
        {
            GameManager.instance.pickupSpawnsList.Add(tempPickupSpawns[i].GetComponent<Transform>());
        }
    }
}
