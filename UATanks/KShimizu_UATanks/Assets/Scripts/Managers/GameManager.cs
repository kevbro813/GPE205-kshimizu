using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Game Manager singleton
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [HideInInspector] public GameObject playerTank;
    public GameObject tankPrototype; // Set in inspector
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public Camera mainCamera;
    [HideInInspector] public PlayerData playerData;
    [HideInInspector] public HUD hud;
    [HideInInspector] public MapGenerator mapGenerator; // Map Generator component
    [HideInInspector] public Room[,] grid; // Grid used to store procedurally generated map
    [HideInInspector] public PlayerPawn playerPawn;
    [HideInInspector] public List<Transform> playerSpawnsList; // List of all player spawns
    public List<GameObject> enemyTankList; // List of enemy tanks, will be used for spawning enemies
    public List<Transform> enemySpawnsList; // List of all spawn points for enemies
    public List<Transform> enemyWaypointsList; // List of all enemy waypoints to use for patrolling
    public List<GameObject> activeEnemiesList; // List of all current enemy objects
    public List<EnemyData> enemyDataList; // List of all EnemyData components
    public List<GameObject> pickupList; // List of available pickups, set in inspector
    public List<Transform> pickupSpawnsList; // List of all pickup spawns, set automatically when map is generated
    public List<GameObject> activePickupList; // List of all active pickups
    public List<PickupObject> pickupObjectList; // List of all active pickup objects
    [HideInInspector] public float enemiesSpawned = 0; // Tracks the number of enemies spawned into the world
    public float spawnDelay = 0; // Delay between enemies being spawned into the world
    public int maxEnemies = 8; // Maximum number of enmies spawned
    public int maxPickups = 10; // Maximum number of pickups that will spawn, set in inspector
    public float pickupRespawnDelay = 5.0f; // This is the delay between when a pickup is taken and when it reappears
    [HideInInspector] public float currentPickupQuantity = 0; // Tracks the number of pickups spawned in the world
    public float pickupSpawnDelay = 0; // Delay to spawn pickups when the game is first started
    [HideInInspector] public Vector3 lastPlayerLocation; // Last known location of the player, visible to all AI. Used in Alert system
    [HideInInspector] public Vector3 lastSoundLocation; // Last known sound detected, visible to all AI
    public bool isAlerted = false; // If the player is seen by a guard or captain, this will be set to true, calling other enemy tanks to go to last known player location
    public bool isRandomEnemy = false; // Generate a random assortment of enemies or one based on a preset seed (Map of Day or Preset Seed)

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
        GetComponent<MapGenerator>().GenerateMap(); // Generates a new map
        SetRandomEnemies(); // Sets enemies to random or preset
        StartCoroutine(SpawnEnemyEvent()); // Begin coroutine for SpawnEnemyEvent
        SpawnPlayerTank(); // Instantiate a new player 
        StartCoroutine(SpawnPickupEvent()); // Begin coroutine to spawn pickups
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

                activeEnemiesList.Add(enemyClone); // Adds spawned enemy to a list of active enemies
                EnemyData tempEnemyData = enemyClone.GetComponent<EnemyData>(); // Get EnemyData component
                enemyDataList.Add(tempEnemyData); // Adds the EnemyData component to an active list

                // Assigns the spawned enemy an index number, used to remove from the two lists above when the enemy tank is destroyed
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
                Transform spawnPoint = pickupSpawnsList[Random.Range(0, pickupSpawnsList.Count)]; // Set a random spawnpoint
                GameObject randomPickup = pickupList[Random.Range(0, pickupList.Count)]; // Set a random pickup
                pickupSpawnsList.Remove(spawnPoint); // Remove spawnpoint from list of pickup spawns, prevents multiple instantiations at the same location

                // Instantiate pickup object
                GameObject pickupClone = Instantiate(randomPickup, spawnPoint.position, spawnPoint.rotation) as GameObject;

                activePickupList.Add(pickupClone); // Add pickup to a list of active pickups
                PickupObject tempPickupObjects = pickupClone.GetComponent<PickupObject>(); // Get PickupObject component for spawned pickup
                pickupObjectList.Add(tempPickupObjects); // Add the PickupObject component to an active list

                // Assigns the spawned pickup an index number, used to remove from the two lists above when the pickup is used
                tempPickupObjects.pickupListIndex = activePickupList.Count - 1; // Note: I am keeping this for now despite not destroying pickups when used

                currentPickupQuantity++; // Increase pickup quantity by one       
            }
        }
    }
    // Random Enemy option can be set to true on the Start Game Screen
    public void SetRandomEnemies()
    {
        if (isRandomEnemy == true)
        {
            Random.InitState(System.Environment.TickCount); // Randomization based on tick count
        }
        else
        {
            Random.InitState(mapGenerator.mapSeed); // Randomization based on the seed used to generate the map
        }
    }
    // TODO: Game state methods
}
