using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerTank;
    public GameObject tankPrototype; // Set in inspector
    public PlayerPawn playerPawn;
    public PlayerController playerController;
    public Transform playerSpawn;
    public Camera mainCamera;
    public PlayerData playerData;
    public HUD hud;
    public List<GameObject> enemyTankList; // List of enemy tanks, will be used for spawning enemies
    public List<Transform> enemyTankSpawnList; // List of all spawn points for enemies
    public GameObject[] enemyObjectsArray; // Array of all current enemy objects
    public EnemyData[] enemyDataArray; // Array of all EnemyData components
    public float spawnDelay = 0; // Delay between enemies being spawned into the world
    public float maxEnemies = 8; // Maximum number of enmies spawned
    private float enemiesSpawned; // Tracks the number of enemies spawned into the world. May need to be public in the future
    


    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        playerController = GameObject.FindWithTag("GameManager").GetComponent<PlayerController>();
        mainCamera = Camera.main;
        hud = GameObject.FindWithTag("HUD").GetComponent<HUD>();
        CreatePlayerTank(); // Instantiate a new player
        // Start enemy spawning
        StartCoroutine("SpawnEnemyEvent"); // Begin coroutine for SpawnEnemyEvent
    }
    void Update()
    {
        UpdateEnemyTankData();
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
        playerPawn.playerData = playerData;
        playerPawn.characterController = playerTank.GetComponent<CharacterController>();
        hud.playerData = playerData;
    }

    // Creates a list of all enemy objects and a second list with all of the enemy objects' EnemyData components
    public void UpdateEnemyTankData()
    {
        enemyObjectsArray = GameObject.FindGameObjectsWithTag("Enemy"); // Find all objects with Enemy Tag and add them to the array
        enemyDataArray = new EnemyData[enemyObjectsArray.Length]; // Array for EnemyData components

        // Loop through enemyObjectsArray, get their EnemyData components and add them to the enemyDataArray
        for (int i = 0; i < enemyObjectsArray.Length; i++)
        {
            enemyDataArray[i] = enemyObjectsArray[i].GetComponent<EnemyData>();
        }
    }
    private IEnumerator SpawnEnemyEvent()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);
            // Check that enemyCount does not exceed maxEnemies
            if (maxEnemies > enemiesSpawned)
            {
                Transform spawnPoint = enemyTankSpawnList[Random.Range(0, enemyTankSpawnList.Count)]; // Random spawnPoint from list
                GameObject randomEnemy = enemyTankList[Random.Range(0, enemyTankList.Count)]; // Random enemy from list
                enemyTankSpawnList.Remove(spawnPoint); // Remove spawn point from list when used
                // Create enemyClone instance
                GameObject enemyClone = Instantiate(randomEnemy, spawnPoint.position, spawnPoint.rotation) as GameObject;

                enemiesSpawned++; // Increase enemyCount by one
            }
        }
    }
    // ***Game state methods***
}
