using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator : MonoBehaviour
{
    public GameObject[] mapTiles;
    public int tileRows;
    public int tileColumns;
    public float roomWidth = 50.0f;
    public float roomHeight = 50.0f;
    private int randomTile;
    public int mapSeed;
    public MapType mapType;
    public enum MapType { MapOfTheDay, Random, PresetSeed }
    
    // Start is called before the first frame update
    void Start()
    {

    }
    public void GenerateMap()
    {
        if (mapType == MapType.MapOfTheDay)
        {
            mapSeed = DateToInt(DateTime.Now.Date);
        }
        else if (mapType == MapType.Random)
        {
            mapSeed = DateToInt(DateTime.Now);
        }
        else if (mapType == MapType.PresetSeed)
        {
            // Do nothing. Allows seed to be set in inspector
        }

        UnityEngine.Random.InitState(mapSeed);

        GameManager.instance.grid = new Room[tileColumns, tileRows];
        
        for (int i = 0; i < tileRows; i++)
        {
            for (int j = 0; j < tileColumns; j++)
            {
                float xPosition = roomWidth * j;
                float zPosition = roomHeight * i;
                Vector3 tilePosition = new Vector3(xPosition, 0.0f, zPosition);
                GameObject tempTile = Instantiate(RandomTile(), tilePosition, Quaternion.identity) as GameObject;
                tempTile.transform.parent = this.transform;
                tempTile.name = "Room_" + j + "," + i;
                Room tempRoom = tempTile.GetComponent<Room>();

                // Open Doors
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
                GameManager.instance.grid[j, i] = tempRoom;
            }
        }
        CreateSpawnList();
        CreateWaypointList();
    }
    public GameObject RandomTile()
    {
        return mapTiles[UnityEngine.Random.Range(0, mapTiles.Length)];
    }
    public int DateToInt(DateTime dateTime)
    {
        return dateTime.Year + dateTime.Month + dateTime.Day + dateTime.Hour + dateTime.Minute + dateTime.Second + dateTime.Millisecond;
    }
    public void CreateWaypointList()
    {
        GameObject[] tempEnemyWaypoints = GameObject.FindGameObjectsWithTag("EnemyWaypoint");
        for (int i = 0; i < tempEnemyWaypoints.Length; i++)
        {
            GameManager.instance.enemyWaypointsList.Add(tempEnemyWaypoints[i].GetComponent<Transform>());
        }
    }
    public void CreateSpawnList()
    {
        GameObject[] tempEnemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawn");
        
        for (int i = 0; i < tempEnemySpawns.Length; i++)
        {
            GameManager.instance.enemyTankSpawnsList.Add(tempEnemySpawns[i].GetComponent<Transform>());
        }
        GameManager.instance.playerSpawn = GameObject.FindWithTag("PlayerSpawn").GetComponent<Transform>();
    }
}
