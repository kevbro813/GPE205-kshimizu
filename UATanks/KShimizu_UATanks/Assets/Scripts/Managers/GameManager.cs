using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game Manager singleton
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerTank;
    public GameObject tankPrototype; // Set in inspector
    public PlayerPawn playerPawn;
    public PlayerController playerController;

    public Camera mainCamera;
    public PlayerData playerData;
    public HUD hud;

    public Transform playerSpawn;
    public List<GameObject> enemyTankList; // List of enemy tanks, will be used for spawning enemies
    public List<Transform> enemyTankSpawnsList; // List of all spawn points for enemies
    public List<Transform> enemyWaypointsList; // Array of all enemy waypoints to use for patrolling. Add in inspector

    public List<PickupObject> pickupList;
    public List<Transform> pickupSpawnsList;
    public Room[,] grid;

    public List<GameObject> currentEnemiesList; // List of all current enemy objects
    public List<EnemyData> enemyDataList; // List of all EnemyData components
    public float spawnDelay = 0; // Delay between enemies being spawned into the world
    public float maxEnemies = 8; // Maximum number of enmies spawned
    private float enemiesSpawned; // Tracks the number of enemies spawned into the world. May need to be public in the future
    public Vector3 lastPlayerLocation;
    public Vector3 lastSoundLocation;
    public bool isAlerted = false;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        mainCamera = Camera.main;
        hud = GameObject.FindWithTag("HUD").GetComponent<HUD>();
        CreateNewGame();
    }
    void Update()
    {
        // ***Game State Machine***

    }
    public void CreatePlayerTank()
    {
        // Instantiate a player tank prefab
        playerTank = Instantiate(tankPrototype, playerSpawn.position, playerSpawn.rotation) as GameObject;
        UpdatePlayerComponents();
    }

    // Add all variables that need to be updated when a new player instance is created
    public void UpdatePlayerComponents()
    {
        playerPawn = playerTank.GetComponent<PlayerPawn>();
        mainCamera.GetComponent<CameraFollow>().playerTank = playerTank;
        playerData = playerTank.GetComponent<PlayerData>();
        playerController.playerPawn = playerPawn;
        playerController.playerData = playerData;
        playerPawn.playerController = playerController;
        playerPawn.characterController = playerTank.GetComponent<CharacterController>();
        hud.playerData = playerData;
    }

    private IEnumerator SpawnEnemyEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            // Check that enemyCount does not exceed maxEnemies
            if (maxEnemies > enemiesSpawned)
            {
                Transform spawnPoint = enemyTankSpawnsList[Random.Range(0, enemyTankSpawnsList.Count)]; // Random spawnPoint from list
                GameObject randomEnemy = enemyTankList[Random.Range(0, enemyTankList.Count)]; // Random enemy from list
                enemyTankSpawnsList.Remove(spawnPoint); // Remove spawn point from list when used
                enemyTankList.Remove(randomEnemy); // Remove spawn point from list when used
                // Create enemyClone instance
                GameObject enemyClone = Instantiate(randomEnemy, spawnPoint.position, spawnPoint.rotation) as GameObject;

                currentEnemiesList.Add(enemyClone);
                EnemyData tempEnemyData = enemyClone.GetComponent<EnemyData>();
                enemyDataList.Add(tempEnemyData);
                tempEnemyData.enemyListIndex = currentEnemiesList.Count - 1;

                enemiesSpawned++; // Increase enemyCount by one           
            }
        }
    }
    public void CreateNewGame()
    {
        GetComponent<MapGenerator>().GenerateMap();
        StartCoroutine("SpawnEnemyEvent"); // Begin coroutine for SpawnEnemyEvent
        CreatePlayerTank(); // Instantiate a new player  
    }
    // ***Game state methods***
}
