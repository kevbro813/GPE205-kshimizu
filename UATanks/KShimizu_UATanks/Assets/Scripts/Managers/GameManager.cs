using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Game Manager singleton
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject tankPrototype; // Set in inspector
    [HideInInspector] public MapGenerator mapGenerator; // Map Generator component
    [HideInInspector] public Room[,] grid; // Grid used to store procedurally generated map
    public List<Transform> playerSpawnsList; // List of all player spawns
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

    public bool isMultiplayer = false;
    public int playerCount = 0;
    public PlayerController playerController;
    public GameObject[] cameraObjects;
    public List<Camera> cameraComponents;
    public List<PlayerData> playerData;
    public List<GameObject> playerObjectsList;
    public GameObject[] hudObjects;
    public List<HUD> hudComponents;
    public List<GameObject> tankObjects;
    public List<TankData> tankDataList;
    public Transform playerTankShell;
    public Transform enemyTankShell;
    public Transform pickupShell;
    public float killMultiplier;
    public List<Transform> activePlayerSpawnsList;
    public float playerRespawnDelay = 1;
    public string gameState = "pregame";
    public GameObject StartGameMenu;
    public GameObject pauseMenu;
    public SoundManager soundManager;
    public AudioSource asMusic;
    public AudioSource asSFX;
    public bool isPreviousGame = false;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // TODO: Allow Game to persist
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        mapGenerator = GetComponent<MapGenerator>();
        soundManager = GetComponentInChildren<SoundManager>();
        asMusic = GameObject.FindWithTag("MusicSource").GetComponent<AudioSource>();
        asSFX = GameObject.FindWithTag("SFXSource").GetComponent<AudioSource>();
        isPreviousGame = false;
    }
    void Update()
    {
        // TODO: Game State Machine
        if (gameState == "pregame")
        {
            DoPregame();
        }
        if (gameState == "resumegame")
        {
            DoResumeGame();
        }
        if (gameState == "active")
        {
            DoActive();
            if (Input.GetButton("Cancel"))
            {
                gameState = "pause";
            }
        }
        if (gameState == "pause")
        {
            DoPause();
        }
        if (gameState == "menu")
        {
            DoMenu();
            gameState = "idle";
        }
        if (gameState == "quit")
        {
            DoQuitGame();
        }
        if (gameState == "idle")
        {
            // Do Nothing.
        }
    }
    public void DoResumeGame()
    {
        Debug.Log("ResumeGame");
        StartGameMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }
    public void DoPregame()
    {
        StartGameMenu.SetActive(true);
        hudObjects[0].SetActive(false);
        hudObjects[1].SetActive(false);
        hudObjects[2].SetActive(false);
        pauseMenu.SetActive(false);
    }
    public void DoActive()
    {
        pauseMenu.SetActive(false);
        StartGameMenu.SetActive(false);

        // Enable all AI movement
        Time.timeScale = 1;

        // Enable player controller
        playerController.GetComponent<PlayerController>().enabled = true;
    }
    public void DoPause()
    {
        pauseMenu.SetActive(true);
        // Enable all AI movement
        Time.timeScale = 0;

        // Enable player controller
        playerController.GetComponent<PlayerController>().enabled = false;
    }
    public void DoMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void DoQuitGame()
    {
        Application.Quit();
    }
    public void CreateNewGame()
    {
        isPreviousGame = true;
        GetComponent<MapGenerator>().GenerateMap(); // Generates a new map
        if (isMultiplayer == true)
        {
            SpawnPlayerTank(); // Instantiate a new player 
            SpawnPlayerTank(); // Instantiate a new player 
            TwoPlayerCamera();
            TwoPlayerHud();
        }
        else
        {
            SpawnPlayerTank(); // Instantiate a new player 
            SinglePlayerCamera();
            SinglePlayerHUD();
        }
        SetRandomEnemies(); // Sets enemies to random or preset
        StartCoroutine(SpawnEnemyEvent()); // Begin coroutine for SpawnEnemyEvent
        StartCoroutine(SpawnPickupEvent()); // Begin coroutine to spawn pickups
        ResetPlayerSpawnsList();
    }
    public void SpawnPlayerTank()
    {
        Transform spawnPoint = activePlayerSpawnsList[Random.Range(0, playerSpawnsList.Count)]; // Random spawnPoint from list
        activePlayerSpawnsList.Remove(spawnPoint); // Remove spawn point from list when used
        // Instantiate a player tank prefab
        GameObject playerClone = Instantiate(tankPrototype, spawnPoint.position, spawnPoint.rotation, playerTankShell);
        PlayerData tempPlayerData = playerClone.GetComponent<PlayerData>();
        playerObjectsList.Add(playerClone);
        playerData.Add(tempPlayerData);
        tankObjects.Add(playerClone);
        tankDataList.Add(playerClone.GetComponent<TankData>());
        // Set index number
        tempPlayerData.playerIndex = playerObjectsList.Count - 1;
        int currentIndex = playerData[playerCount].playerIndex;

        tempPlayerData.tankIndex = tankObjects.Count - 1;

        // Link camera to playerClone
        cameraObjects[currentIndex].GetComponent<CameraFollow>().playerTank = playerClone;

        // Link hud elements to current playerClone
        if (isMultiplayer == true)
        {
            if (currentIndex == 0)
            {
                hudComponents[1].playerData = playerData[currentIndex];
            }
            if (currentIndex == 1)
            {
                hudComponents[2].playerData = playerData[currentIndex];
            }
        }
        else
        {
            hudComponents[currentIndex].playerData = playerData[currentIndex];
        }

        // Increase playerCount
        playerCount++;
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
                GameObject enemyClone = Instantiate(randomEnemy, spawnPoint.position, spawnPoint.rotation, enemyTankShell) as GameObject;

                activeEnemiesList.Add(enemyClone); // Adds spawned enemy to a list of active enemies
                EnemyData tempEnemyData = enemyClone.GetComponent<EnemyData>(); // Get EnemyData component
                enemyDataList.Add(tempEnemyData); // Adds the EnemyData component to an active list
                tankObjects.Add(enemyClone);
                tankDataList.Add(tempEnemyData.GetComponent<TankData>());
                // Assigns the spawned enemy an index number, used to remove from the two lists above when the enemy tank is destroyed
                tempEnemyData.enemyListIndex = activeEnemiesList.Count - 1;
                tempEnemyData.tankIndex = tankObjects.Count - 1;

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
                GameObject pickupClone = Instantiate(randomPickup, spawnPoint.position, spawnPoint.rotation, pickupShell) as GameObject;

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
    public void SinglePlayerCamera()
    {
        cameraObjects[0].SetActive(true);
        cameraObjects[1].SetActive(false);
        cameraComponents[0].rect = new Rect(0f, 0f, 1f, 1f);
    }
    public void TwoPlayerCamera()
    {
        cameraObjects[0].SetActive(true);
        cameraObjects[1].SetActive(true);
        cameraComponents[0].rect = new Rect(0f, 0.5f, 1f, 0.5f);
        cameraComponents[1].rect = new Rect(0f, 0f, 1f, 0.5f);
    }
    public void SinglePlayerHUD()
    {
        hudObjects[0].SetActive(true);
        hudObjects[1].SetActive(false);
        hudObjects[2].SetActive(false);
    }
    public void TwoPlayerHud()
    {
        hudObjects[0].SetActive(false);
        hudObjects[1].SetActive(true);
        hudObjects[2].SetActive(true);
    }
    public void ResetPlayerSpawnsList() // TODO: Make Coroutine? to prevent players spawning on top of each other when they die.
    {
        activePlayerSpawnsList.Clear();
        activePlayerSpawnsList = playerSpawnsList;
    }
}
