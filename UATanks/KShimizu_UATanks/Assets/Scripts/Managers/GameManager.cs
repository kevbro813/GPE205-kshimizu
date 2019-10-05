using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Game Manager singleton
public class GameManager : MonoBehaviour
{
    // Game Manager
    public static GameManager instance;
    public string gameState = "pregame"; // Current game state

    // Lists of available prefabs set in inspector
    public GameObject tankPrototype; // Set in inspector
    public List<GameObject> enemyTankList; // List of enemy tanks, will be used for spawning enemies
    public List<GameObject> pickupList; // List of available pickups, set in inspector

    // Lists of generated spawn points
    public List<Transform> playerSpawnsList = new List<Transform>(); // List of all player spawns
    public List<Transform> enemySpawnsList; // List of all spawn points for enemies
    public List<Transform> pickupSpawnsList; // List of all pickup spawns, set automatically when map is generated

    // List of all enemy waypoints to use for patrolling
    public List<Transform> enemyWaypointsList;

    // Lists of all active player, enemy and pickup game objects
    public List<GameObject> activePlayersList; // List of active player objects
    public List<GameObject> activeEnemiesList; // List of all current enemy objects
    public List<GameObject> activePickupsList; // List of all active pickups

    // Lists of all active player, enemy and pickup data components
    public List<PlayerData> playerDataList; // List of active playerData components
    public List<EnemyData> enemyDataList; // List of all EnemyData components

    public List<PickupObject> pickupObjectList; // List of all active pickup objects

    // Lists of all active tank game objects and tankData components
    public List<GameObject> tankObjectList; // List of all AI and player tank objects
    public List<TankData> tankDataList; // List of all AI and player tankData components

    // Map specific variables
    [HideInInspector] public MapGenerator mapGenerator; // Map Generator component
    [HideInInspector] public Room[,] grid; // Grid used to store procedurally generated map
    public bool isRandomEnemy = false; // Generate a random assortment of enemies or one based on a preset seed (Map of Day or Preset Seed)
    public int tileRows;
    public int tileColumns;

    // Canvas game objects (used to activate/deactivate)
    public GameObject StartGameMenu; // Start Game menu
    public GameObject pauseMenu; // Pause menu
    public GameObject scoreWindow; // Score window
    public GameObject endGame; // End Game screen
    public GameObject adminMenu; // Admin menu

    // Camera components
    public GameObject[] cameraObjects; // List of cameras
    public List<Camera> cameraComponents; // List of camera components

    // HUD components
    public GameObject[] hudObjects; // List of HUD objects
    public List<HUD> hudComponents; // List of HUD components

    // Player variables
    public float playerRespawnDelay = 1; // Duration for player respawning after death
    public int playersCreated = 0; // Tracks the number of players in the game
    public int playersAlive = 0;

    // Enemy variables
    [HideInInspector] public float enemiesSpawned = 0; // Tracks the number of enemies spawned into the world
    public float spawnDelay = 0; // Delay between enemies being spawned into the world
    public int maxEnemies = 8; // Maximum number of enmies spawned

    // Pickup variables
    public int maxPickups = 10; // Maximum number of pickups that will spawn, set in inspector
    public float pickupRespawnDelay = 5.0f; // This is the delay between when a pickup is taken and when it reappears
    [HideInInspector] public float currentPickupQuantity = 0; // Tracks the number of pickups spawned in the world
    public float pickupSpawnDelay = 0; // Delay to spawn pickups when the game is first started

    // Sound Manager and Audio components
    public SoundManager soundManager; // Sound Manager component
    public AudioSource asMusic; // Music audio source component
    public AudioSource asSFX; // SFX audio source component

    // Scoring
    public ScoreManager scoreManager; // Score Manager Component
    public List<ScoreData> highScores; // List of current high scores
    public List<ScoreData> currentScores; // List of scores of players currently playing
    public int maxScores = 10; // Maximum number of scores that are posted
    public float killMultiplier; // Multiplies score when the player kills a tank

    // Player Location variables and AI alert
    [HideInInspector] public Vector3 lastPlayerLocation; // Last known location of the player, visible to all AI. Used in Alert system
    [HideInInspector] public Vector3 lastSoundLocation; // Last known sound detected, visible to all AI
    public bool isAlerted = false; // If the player is seen by a guard or captain, this will be set to true, calling other enemy tanks to go to last known player location
    
    // Game variables
    public bool isMultiplayer = false; // Set by selecting the multiplayer option in game. Used by several functions to determine multiplayer
    public bool isPlayerOneDead = false; // Boolean that indicates whether player one is dead
    public bool isPlayerTwoDead = false; // Boolean that indicates whether player two is dead 
    public bool isGameReady = false; // Checks if game is ready to prevent win conditions from being instantly satisfied
    public string playerOneName;
    public string playerTwoName;
    public bool isPreviousGame = false;

    // Organization
    public Transform playerTankShell; // Empty parent component that houses all player tank objects
    public Transform enemyTankShell; // Empty parent component that houses all enemy tank objects
    public Transform pickupShell; // Empty parent component that houses all pickup objects

    public bool isPaused = false;
    public bool isScoreDisplayed = false;
    public bool isAdminMenu = false;

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
        mapGenerator = GetComponent<MapGenerator>();
        soundManager = GetComponentInChildren<SoundManager>();
        asMusic = GameObject.FindWithTag("MusicSource").GetComponent<AudioSource>();
        asSFX = GameObject.FindWithTag("SFXSource").GetComponent<AudioSource>();
        highScores = new List<ScoreData>(); // Reset highScores
        scoreManager = GetComponent<ScoreManager>();
        scoreManager.InitializeValues();
        scoreManager.LoadPlayerScores();
        scoreManager.FillEmptyScores();
        LoadSettings();
    }
   
    void Update()
    {
        // Game Finite State Machine
        if (gameState == "pregame")
        {
            DoPregame();
        }
        if (gameState == "resume")
        {
            DoResumeGame();
            gameState = "active";
        }
        if (gameState == "active")
        {
            DoActive();
            if (isGameReady == true && isMultiplayer == true) // Check if game is multiplayer
            {
                // If both players are dead or one player is alive and all enemy AI are dead...
                if ((isPlayerOneDead == true && isPlayerTwoDead == true) || (playersAlive == 1 && activeEnemiesList.Count == 0))
                {
                    scoreManager.OnSave();
                    gameState = "endgame"; // Transition to endgame
                }
            }
            else if (isGameReady == true && isMultiplayer == false) // If not multiplayer...
            {
                // If player one is dead or all enemy AI are dead and player one is still alive...
                if (isPlayerOneDead == true || (isPlayerOneDead == false && activeEnemiesList.Count == 0))
                {
                    scoreManager.OnSave();
                    gameState = "endgame"; // Transition to endgame
                }
            }
            if (isPaused == true)
            {
                gameState = "pause";
            }
            if (isScoreDisplayed == true)
            {
                gameState = "score";
            }
            if (isAdminMenu == true)
            {
                gameState = "admin";
            }
        }
        if (gameState == "pause")
        {
            DoPause();
            if (isPaused == false)
            {
                gameState = "resume";
            }
            // Transitions in UI components
        }
        if (gameState == "title")
        {
            DoTitleScreen(); 
            gameState = "pregame";
            // Transitions in UI components
        }
        if (gameState == "quit")
        {
            DoQuitGame(); // Ends game
        }
        if (gameState == "idle")
        {
            // Do Nothing.
        }
        if (gameState == "endgame")
        {
            DoEndGame();
            // Transitions in UI components
        }
        if (gameState == "score")
        {
            DoScoreWindow();
            if (isScoreDisplayed == false)
            {
                gameState = "resume";
            }
            // Transitions in UI components
        }
        if (gameState == "admin")
        {
            DoAdminMenu();
            if (isAdminMenu == false)
            {
                gameState = "resume";
            }
            // Transitions in UI components
        }
        if (gameState == "newgame")
        {
            DoNewGame();
            gameState = "active";
        }
        if (gameState == "continue")
        {
            DoContinueGame();
            gameState = "active";
        }
    }
    public void ResetGame()
    {
        for (int i = 0; i < tileRows; i++)
        {
            for (int j = 0; j < tileColumns; j++)
            {
                Destroy(GameManager.instance.grid[j, i].gameObject);
            }
        }
        for (int i = 0; i < activePlayersList.Count; i++)
        {
            Destroy(activePlayersList[i]);
        }
        for (int i = 0; i < activeEnemiesList.Count; i++)
        {
            Destroy(activeEnemiesList[i]);
        }
        for (int i = 0; i < activePickupsList.Count; i++)
        {
            Destroy(activePickupsList[i]);
        }
        for (int i = 0; i < tankObjectList.Count; i++)
        {
            Destroy(tankObjectList[i]);
        }
        for (int i = 0; i < playerDataList.Count; i++)
        {
            Destroy(playerDataList[i]);
        }
        for (int i = 0; i < enemyDataList.Count; i++)
        {
            Destroy(enemyDataList[i]);
        }
        for (int i = 0; i < tankDataList.Count; i++)
        {
            Destroy(tankDataList[i]);
        }
        for (int i = 0; i < pickupObjectList.Count; i++)
        {
            Destroy(pickupObjectList[i]);
        }
        for (int i = 0; i < playerSpawnsList.Count; i++)
        {
            Destroy(playerSpawnsList[i]);
        }
        for (int i = 0; i < enemySpawnsList.Count; i++)
        {
            Destroy(enemySpawnsList[i]);
        }
        for (int i = 0; i < pickupSpawnsList.Count; i++)
        {
            Destroy(pickupSpawnsList[i]);
        }
        for (int i = 0; i < enemyWaypointsList.Count; i++)
        {
            Destroy(enemyWaypointsList[i]);
        }
        activePlayersList.Clear();
        activeEnemiesList.Clear();
        activePickupsList.Clear();
        tankObjectList.Clear();
        playerDataList.Clear();
        enemyDataList.Clear();
        tankDataList.Clear();
        pickupObjectList.Clear();
        playerSpawnsList.Clear();
        enemySpawnsList.Clear();
        pickupSpawnsList.Clear();
        enemyWaypointsList.Clear();
        playersCreated = 0;
        playersAlive = 0;
        enemiesSpawned = 0;
        currentPickupQuantity = 0;
        isAlerted = false;
        isPlayerOneDead = false;
        isPlayerTwoDead = false;
        isGameReady = false;
    }
        public void DoContinueGame()
    {
        StartGameMenu.SetActive(false);
        pauseMenu.SetActive(false);
        scoreWindow.SetActive(false);
        endGame.SetActive(false);
        adminMenu.SetActive(false);

        soundManager.PlayMusic();

        // Enable all AI movement
        Time.timeScale = 1;

        isPaused = false;
        isAdminMenu = false;
        isScoreDisplayed = false;
    }
    public void DoNewGame()
    {
        ResetGame();
        StartCoroutine(CreateGameEvent()); // Allows game to reset fully before creating a new game
        isPreviousGame = true;
        soundManager.PlayMusic();
        isPaused = false;
        isAdminMenu = false;
        isScoreDisplayed = false;
    }
    public IEnumerator CreateGameEvent()
    {
        yield return new WaitForSeconds(0.2f);
        CreateNewGame();
    }
    public void CreateNewGame()
    {
        GetComponent<MapGenerator>().GenerateMap(); // Generates a new map
        if (isMultiplayer == true) // Check if multiplayer
        {
            SpawnPlayerTank(); // Instantiate first player
            SpawnPlayerTank(); // Instantiate second player
            TwoPlayerCamera(); // Sets cameras for two players
            TwoPlayerHud(); // Sets HUDs for two players
            playerDataList[0].playerName = playerOneName;
            playerDataList[1].playerName = playerTwoName;
        }
        else
        {
            SpawnPlayerTank(); // Instantiate a new player 
            SinglePlayerCamera(); // Sets camera for one player
            SinglePlayerHUD(); // Sets HUD for one player
            playerDataList[0].playerName = playerOneName;
        }
        RandomEnemies(); // Sets enemies to random or preset
        StartCoroutine(SpawnEnemyEvent()); // Begin coroutine for SpawnEnemyEvent
        StartCoroutine(SpawnPickupEvent()); // Begin coroutine to spawn pickups
        // Creates a small time window to allow AI tanks to be spawned before the game is allowed to change states, prevents game from ending prematurely
        StartCoroutine(GameReadyEvent());
    }
    // Open admin menu
    public void DoAdminMenu()
    {
        adminMenu.SetActive(true);
        pauseMenu.SetActive(false);
        StartGameMenu.SetActive(false);
        scoreWindow.SetActive(false);
        endGame.SetActive(false);

        soundManager.PauseMusic();

        // Disable all AI movement
        Time.timeScale = 0;

        isPaused = false;
        isScoreDisplayed = false;
    }
    // Open score window
    public void DoScoreWindow()
    {
        pauseMenu.SetActive(false);
        StartGameMenu.SetActive(false);
        scoreWindow.SetActive(true);
        endGame.SetActive(false);
        adminMenu.SetActive(false);

        soundManager.PauseMusic();

        // Disable all AI movement
        Time.timeScale = 0;

        isPaused = false;
        isAdminMenu = false;
    }
    // Open End Game window
    public void DoEndGame()
    {
        pauseMenu.SetActive(false);
        StartGameMenu.SetActive(false);
        scoreWindow.SetActive(false);
        endGame.SetActive(true);
        adminMenu.SetActive(false);

        soundManager.PauseMusic();

        // Disable all AI movement
        Time.timeScale = 0;

        isPaused = false;
        isAdminMenu = false;
        isScoreDisplayed = false;
        isPlayerOneDead = false;
        isPlayerTwoDead = false;
        isGameReady = false;
        isPreviousGame = false;
    }
    // Resume game
    public void DoResumeGame()
    {
        StartGameMenu.SetActive(false);
        pauseMenu.SetActive(false);
        scoreWindow.SetActive(false);
        endGame.SetActive(false);
        adminMenu.SetActive(false);

        soundManager.PlayMusic();

        // Enable all AI movement
        Time.timeScale = 1;

        isPaused = false;
        isAdminMenu = false;
        isScoreDisplayed = false;
    }
    // Pregame is default state (All canvases but Start Game Menu are inactive)
    public void DoPregame()
    {
        StartGameMenu.SetActive(true);
        pauseMenu.SetActive(false);
        scoreWindow.SetActive(false);
        endGame.SetActive(false);
        adminMenu.SetActive(false);
    }
    // Set game to active (can be used to unpause game)
    public void DoActive()
    {
        pauseMenu.SetActive(false);
        StartGameMenu.SetActive(false);
        scoreWindow.SetActive(false);
        endGame.SetActive(false);
        adminMenu.SetActive(false);

        // Enable all AI movement
        Time.timeScale = 1;
    }
    // Pauses the game and opens pause menu
    public void DoPause()
    {
        pauseMenu.SetActive(true);
        StartGameMenu.SetActive(false);
        scoreWindow.SetActive(false);
        endGame.SetActive(false);
        adminMenu.SetActive(false);

        soundManager.PauseMusic();

        // Disable all AI movement
        Time.timeScale = 0;

        isAdminMenu = false;
        isScoreDisplayed = false;
    }
    // Opens Title Screen
    public void DoTitleScreen()
    {
        soundManager.PauseMusic();
        SceneManager.LoadScene(0);
    }
    // Exits the game
    public void DoQuitGame()
    {
        Application.Quit();
    }
    public void SpawnPlayerTank()
    {
        Transform spawnPoint = playerSpawnsList[Random.Range(0, playerSpawnsList.Count)]; // Random spawnPoint from list
        // Instantiate a player tank prefab
        GameObject playerClone = Instantiate(tankPrototype, spawnPoint.position, spawnPoint.rotation, playerTankShell);
        PlayerData tempPlayerData = playerClone.GetComponent<PlayerData>(); // Temporary playerData component variable

        playerSpawnsList.Remove(spawnPoint); // Remove spawn point from list when used

        activePlayersList.Add(playerClone); // Adds to list of all active player objects
        playerDataList.Add(tempPlayerData); // Adds to list of all active playerData components
        tankObjectList.Add(playerClone); // Adds to list of all active player and AI tank objects
        tankDataList.Add(playerClone.GetComponent<TankData>()); // Adds to list of all active tankData components

        // Set index numbers for both player and tank (used to remove objects and components from lists on death/removal)
        tempPlayerData.playerIndex = activePlayersList.Count - 1;
        int currentIndex = playerDataList[playersCreated].playerIndex;
        tempPlayerData.tankIndex = tankObjectList.Count - 1;

        // Link camera to playerClone
        cameraObjects[currentIndex].GetComponent<CameraFollow>().playerTank = playerClone;

        // Link hud elements to current playerClone
        if (isMultiplayer == true) // Check if multiplayer
        {
            // Connects HUDs to correct playerData components from list
            if (currentIndex == 0)
            {
                hudComponents[1].playerData = playerDataList[currentIndex];
            }
            if (currentIndex == 1)
            {
                hudComponents[2].playerData = playerDataList[currentIndex];
            }
        }
        else
        {
            // Singleplayer HUD
            hudComponents[currentIndex].playerData = playerDataList[currentIndex];
        }
        // Increase playerCount
        playersCreated++;
        playersAlive++; // Update players alive to use in game state transitions
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

                // Create enemyClone instance
                GameObject enemyClone = Instantiate(randomEnemy, spawnPoint.position, spawnPoint.rotation, enemyTankShell) as GameObject;

                activeEnemiesList.Add(enemyClone); // Adds spawned enemy to a list of active enemies
                EnemyData tempEnemyData = enemyClone.GetComponent<EnemyData>(); // Get EnemyData component
                enemyDataList.Add(tempEnemyData); // Adds the EnemyData component to an active list
                tankObjectList.Add(enemyClone); // Adds to list of all active player and AI tank objects
                tankDataList.Add(tempEnemyData); // Adds to list of all active tankData components

                // Assigns the spawned enemy an index number, used to remove from the two lists above when the enemy tank is destroyed
                tempEnemyData.enemyListIndex = activeEnemiesList.Count - 1;
                tempEnemyData.tankIndex = tankObjectList.Count - 1;

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

                // Instantiate pickup object
                GameObject pickupClone = Instantiate(randomPickup, spawnPoint.position, spawnPoint.rotation, pickupShell) as GameObject;

                activePickupsList.Add(pickupClone); // Add pickup to a list of active pickups
                PickupObject tempPickupObjects = pickupClone.GetComponent<PickupObject>(); // Get PickupObject component for spawned pickup
                pickupObjectList.Add(tempPickupObjects); // Add the PickupObject component to an active list

                // Assigns the spawned pickup an index number, used to remove from the two lists above when the pickup is used
                tempPickupObjects.pickupListIndex = activePickupsList.Count - 1; // Note: I am keeping this for now despite not destroying pickups when used

                currentPickupQuantity++; // Increase pickup quantity by one     
            }
        }
    }
    private IEnumerator GameReadyEvent()
    {
        yield return new WaitForSeconds(3);
        isGameReady = true;
    }
    // Random Enemy option can be set to true on the Start Game Screen
    public void RandomEnemies()
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
    // Set cameras to singleplayer mode
    public void SinglePlayerCamera()
    {
        cameraObjects[0].SetActive(true);
        cameraObjects[1].SetActive(false);
        cameraComponents[0].rect = new Rect(0f, 0f, 1f, 1f);
    }
    // Set cameras to two player mode
    public void TwoPlayerCamera()
    {
        cameraObjects[0].SetActive(true);
        cameraObjects[1].SetActive(true);
        cameraComponents[0].rect = new Rect(0f, 0.5f, 1f, 0.5f);
        cameraComponents[1].rect = new Rect(0f, 0f, 1f, 0.5f);
    }
    // Set HUD to singleplayer mode
    public void SinglePlayerHUD()
    {
        hudObjects[0].SetActive(true);
        hudObjects[1].SetActive(false);
        hudObjects[2].SetActive(false);
    }
    // Set HUD to two player mode
    public void TwoPlayerHud()
    {
        hudObjects[0].SetActive(false);
        hudObjects[1].SetActive(true);
        hudObjects[2].SetActive(true);
    }
    public void LoadSettings()
    {
        PauseMenu pauseMenuComponent = pauseMenu.GetComponent<PauseMenu>();
        pauseMenuComponent.musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        pauseMenuComponent.sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        soundManager.SetMusicVolume(pauseMenuComponent.musicSlider.value);    
        soundManager.SetSFXVolume(pauseMenuComponent.sfxSlider.value);
    }
}
