using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game Manager singleton
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerTank;
    public GameObject tankPrototype; // Set in inspector
    public PlayerController playerController;
    public Camera mainCamera;
    public PlayerData playerData;
    public HUD hud;
    public Room[,] grid;
    public MapGenerator mapGenerator;

    public PlayerPawn playerPawn;
    public List<Transform> playerSpawnsList;

    public List<GameObject> enemyTankList; // List of enemy tanks, will be used for spawning enemies
    public List<Transform> enemySpawnsList; // List of all spawn points for enemies
    public List<Transform> enemyWaypointsList; // Array of all enemy waypoints to use for patrolling. Add in inspector
    public List<GameObject> activeEnemiesList; // List of all current enemy objects
    public List<EnemyData> enemyDataList; // List of all EnemyData components
    public float spawnDelay = 0; // Delay between enemies being spawned into the world
    public int maxEnemies = 8; // Maximum number of enmies spawned
    private float enemiesSpawned = 0; // Tracks the number of enemies spawned into the world. May need to be public in the future

    public List<GameObject> pickupList;
    public List<Transform> pickupSpawnsList;
    public List<GameObject> activePickupList;
    public List<PickupObject> pickupObjectList;
    public int maxPickups = 10;
    public float pickupRespawnDelay = 5.0f;
    private float currentPickupQuantity = 0;
    public float pickupSpawnDelay = 0;

    public Vector3 lastPlayerLocation;
    public Vector3 lastSoundLocation;
    public bool isAlerted = false;
    public bool isRandomEnemy = false;

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
        mapGenerator = GetComponent<MapGenerator>();
        hud = GameObject.FindWithTag("HUD").GetComponent<HUD>();
    }
    void Update()
    {
        // TODO: Game State Machine
    }
    public void CreateNewGame()
    {
        GetComponent<MapGenerator>().GenerateMap();
        SetRandomEnemies();
        StartCoroutine(SpawnEnemyEvent()); // Begin coroutine for SpawnEnemyEvent
        SpawnPlayerTank(); // Instantiate a new player 
        StartCoroutine(SpawnPickupEvent());
    }
    public void SpawnPlayerTank()
    {
        Transform spawnPoint = playerSpawnsList[Random.Range(0, playerSpawnsList.Count)]; // Random spawnPoint from list
        enemySpawnsList.Remove(spawnPoint); // Remove spawn point from list when used
        // Instantiate a player tank prefab
        playerTank = Instantiate(tankPrototype, spawnPoint.position, spawnPoint.rotation) as GameObject;
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
        playerPawn.tf = playerPawn.GetComponent<Transform>();
    }
    private IEnumerator SpawnEnemyEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            // Check that enemyCount does not exceed maxEnemies
            if (maxEnemies > enemiesSpawned)
            {
                Transform spawnPoint = enemySpawnsList[Random.Range(0, enemySpawnsList.Count)]; // Random spawnPoint from list
                GameObject randomEnemy = enemyTankList[Random.Range(0, enemyTankList.Count)]; // Random enemy from list
                enemySpawnsList.Remove(spawnPoint); // Remove spawn point from list when used
                enemyTankList.Remove(randomEnemy); // Remove spawn point from list when used
                // Create enemyClone instance
                GameObject enemyClone = Instantiate(randomEnemy, spawnPoint.position, spawnPoint.rotation) as GameObject;

                activeEnemiesList.Add(enemyClone);
                EnemyData tempEnemyData = enemyClone.GetComponent<EnemyData>();
                enemyDataList.Add(tempEnemyData);
                tempEnemyData.enemyListIndex = activeEnemiesList.Count - 1;

                enemiesSpawned++; // Increase enemyCount by one           
            }
        }
    }
    private IEnumerator SpawnPickupEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(pickupSpawnDelay); // TODO:Change to pickupRespawnDelay after testing

            if (maxPickups > currentPickupQuantity)
            {
                Transform spawnPoint = pickupSpawnsList[Random.Range(0, pickupSpawnsList.Count)];
                GameObject randomPickup = pickupList[Random.Range(0, pickupList.Count)];
                pickupSpawnsList.Remove(spawnPoint);

                GameObject pickupClone = Instantiate(randomPickup, spawnPoint.position, spawnPoint.rotation) as GameObject;

                activePickupList.Add(pickupClone);
                PickupObject tempPickupObjects = pickupClone.GetComponent<PickupObject>();
                pickupObjectList.Add(tempPickupObjects);
                tempPickupObjects.pickupListIndex = activePickupList.Count - 1;

                currentPickupQuantity++;        
            }
        }
    }
    public void SetRandomEnemies()
    {
        if (isRandomEnemy == true)
        {
            Random.InitState(System.Environment.TickCount);
        }
        else
        {
            Random.InitState(mapGenerator.mapSeed);
        }
    }
    // TODO: Game state methods
}
