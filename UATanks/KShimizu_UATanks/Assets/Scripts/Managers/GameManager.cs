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
    public List<Transform> playerSpawnsList; // List of all player spawns
    public List<Transform> enemySpawnsList; // List of all spawn points for enemies
    public List<Transform> pickupSpawnsList; // List of all pickup spawns, set automatically when map is generated

    // List of all enemy waypoints to use for patrolling
    public List<Transform> enemyWaypointsList;

    // Lists of all active player, enemy and pickup game objects
    public List<GameObject> playerObjectsList; // List of active player objects
    public List<GameObject> activeEnemiesList; // List of all current enemy objects
    public List<GameObject> activePickupList; // List of all active pickups

    // Lists of all active player, enemy and pickup data components
    public List<PlayerData> playerData; // List of active playerData components
    public List<EnemyData> enemyDataList; // List of all EnemyData components
    public List<PickupObject> pickupObjectList; // List of all active pickup objects

    // Lists of all active tank game objects and tankData components
    public List<GameObject> tankObjects; // List of all AI and player tank objects
    public List<TankData> tankDataList; // List of all AI and player tankData components

    // Map specific variables
    [HideInInspector] public MapGenerator mapGenerator; // Map Generator component
    [HideInInspector] public Room[,] grid; // Grid used to store procedurally generated map
    public bool isRandomEnemy = false; // Generate a random assortment of enemies or one based on a preset seed (Map of Day or Preset Seed)

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
    public int playerCount = 0; // Tracks the number of players in the game

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
    public ScoreManager scoreManager; // Score Manager component
    public List<ScoreManager> highScores; // List of current high scores
    public List<ScoreManager> currentScores; // List of scores of players currently playing
    public int maxScores = 10; // Maximum number of scores that are posted
    public float killMultiplier; // Multiplies score when the player kills a tank

    // Player Location variables and AI alert
    [HideInInspector] public Vector3 lastPlayerLocation; // Last known location of the player, visible to all AI. Used in Alert system
    [HideInInspector] public Vector3 lastSoundLocation; // Last known sound detected, visible to all AI
    public bool isAlerted = false; // If the player is seen by a guard or captain, this will be set to true, calling other enemy tanks to go to last known player location
    
    // Game Mode variables
    public bool isMultiplayer = false; // Set by selecting the multiplayer option in game. Used by several functions to determine multiplayer
    public bool isPreviousGame = false; // Check if a game is already active, used to resume game
    public bool isPlayerOneDead = false; // Boolean that indicates whether player one is dead
    public bool isPlayerTwoDead = false; // Boolean that indicates whether player two is dead 
    public bool isGameReady = false; // Checks if game is ready to prevent win conditions from being instantly satisfied

    // Organization
    public Transform playerTankShell; // Empty parent component that houses all player tank objects
    public Transform enemyTankShell; // Empty parent component that houses all enemy tank objects
    public Transform pickupShell; // Empty parent component that houses all pickup objects

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
        isPreviousGame = false; // Set to begin new game when game first starts
        highScores = new List<ScoreManager>(); // Reset highScores
        InitializeValues(); 
        LoadPlayerScores();
        FillEmptyScores(); 
    }
    // Initialize ScoreManager
    public void InitializeValues()
    {
        scoreManager = new ScoreManager();
    }
    // Fill any empty scores (prevents "index out of range" error)
    public void FillEmptyScores()
    {
        // Loops through until highScores.Count = maxScores
        for (int i = highScores.Count; i < maxScores; i++)
        {
            // Adds a placeholder score
            ScoreManager sm = new ScoreManager();
            highScores.Add(sm);
        }
    }
    // Run functions to save scores
    public void OnSave()
    {
        SetScores();
        CheckScores();
        SavePlayerScores();
    }
    // Sort high scores, reverse order and limit quantity of scores to maxScores
    public void CheckScores()
    {
        highScores.Sort();
        highScores.Reverse();

        int numScores = Mathf.Min(maxScores, highScores.Count);
        highScores = highScores.GetRange(0, numScores);
    }
    // Adds new scores to highScores
    public void SetScores()
    {
        // Loop through active playerData and add playerName and scores to currentScores (needed to use scoreManager)
        for (int i = 0; i < playerData.Count; i++)
        {
            ScoreManager sm = new ScoreManager();
            currentScores.Add(sm);
            currentScores[i].savedScore = playerData[i].score;
            currentScores[i].savedPlayerName = playerData[i].playerName;
        }
        // Loop through currentScores and add them to the list of high scores
        for (int j = 0; j < currentScores.Count; j++)
        {
            ScoreManager sm = new ScoreManager();
            highScores.Add(sm);
            highScores[highScores.Count - 1].savedScore = currentScores[j].savedScore;
            highScores[highScores.Count - 1].savedPlayerName = currentScores[j].savedPlayerName;
        }
    }
    // Saves player scores to PlayerPrefs
    public void SavePlayerScores()
    {
        for (int i = 0; i < highScores.Count; i++)
        {
            ScoreManager sm = highScores[i];
            PlayerPrefs.SetFloat("Score" + i.ToString(), sm.savedScore);
            PlayerPrefs.SetString("PlayerName" + i.ToString(), sm.savedPlayerName);
        }
        PlayerPrefs.Save();
    }
    // Load score data from PlayerPrefs
    public void LoadPlayerScores()
    {
        string key;
        for (int i = 0; i < maxScores; i++)
        {
            Debug.Log("Loading Player Scores");
            ScoreManager sm = new ScoreManager();
            key = "Score" + i.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                sm.savedScore = PlayerPrefs.GetFloat(key);
            }
            else
            {
                break;
            }
            key = "PlayerName" + i.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                sm.savedPlayerName = PlayerPrefs.GetString(key);
            }
            else
            {
                break;
            }
            highScores.Add(sm);
        }
    }
    void Update()
    {
        // Game Finite State Machine
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
            // TODO: Move the following inputs to playerController
            if (Input.GetButton("Cancel"))
            {
                gameState = "pause"; // Pause when escape is pressed
            }
            if (isGameReady == true && isMultiplayer == true) // Check if game is multiplayer
            {
                // If player one and two are dead or one player is alive and all enemy AI are dead...
                if ((isPlayerOneDead == true && isPlayerTwoDead == true) || (playerObjectsList.Count == 1 && activeEnemiesList.Count == 0))
                {
                    OnSave();
                    gameState = "endgame"; // Transition to endgame
                }
            }
            else if (isGameReady == true && isMultiplayer == false) // If not multiplayer...
            {
                // If player one is dead or all enemy AI are dead and player one is still alive...
                if (isPlayerOneDead == true || (isPlayerOneDead == false && activeEnemiesList.Count == 0))
                {
                    OnSave();
                    gameState = "endgame"; // Transition to endgame
                }
            }
            // Open Score Window if "Tab" is pressed
            if (Input.GetButton("Tab"))
            {
                gameState = "score";
            }
            // Open Admin Menu if "F1" is pressed
            if (Input.GetButton("Admin"))
            {
                gameState = "admin";
            }
        }
        if (gameState == "pause")
        {
            DoPause();
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
            // Transitions in UI components
        }
        if (gameState == "admin")
        {
            DoAdminMenu();
            // Transitions in UI components
        }
    }
    // Open admin menu
    public void DoAdminMenu()
    {
        adminMenu.SetActive(true);
        pauseMenu.SetActive(false);
        StartGameMenu.SetActive(false);
        scoreWindow.SetActive(false);
        endGame.SetActive(false);
        // Disable all AI movement
        Time.timeScale = 0;

        // Disable player controller to prevent tank firing when leaving admin menu
        foreach (GameObject player in playerObjectsList)
        {
            player.GetComponent<PlayerController>().enabled = false;
        }
    }
    // Open score window
    public void DoScoreWindow()
    {
        pauseMenu.SetActive(false);
        StartGameMenu.SetActive(false);
        scoreWindow.SetActive(true);
        endGame.SetActive(false);
        adminMenu.SetActive(false);
        // Disable all AI movement
        Time.timeScale = 0;

        // Disable player controller to prevent tank firing when leaving score window
        foreach (GameObject player in playerObjectsList)
        {
            player.GetComponent<PlayerController>().enabled = false;
        }
    }
    // Open End Game window
    public void DoEndGame()
    {
        isPlayerOneDead = false;
        isPlayerTwoDead = false;
        isGameReady = false;
        pauseMenu.SetActive(false);
        StartGameMenu.SetActive(false);
        scoreWindow.SetActive(false);
        endGame.SetActive(true);
        adminMenu.SetActive(false);
    }
    // Resume game
    public void DoResumeGame()
    {
        StartGameMenu.SetActive(true);
        pauseMenu.SetActive(false);
        scoreWindow.SetActive(false);
        endGame.SetActive(false);
        adminMenu.SetActive(false);
    }
    // Pregame is default state (All canvases but Start Game Menu are inactive)
    public void DoPregame()
    {
        StartGameMenu.SetActive(true);
        hudObjects[0].SetActive(false);
        hudObjects[1].SetActive(false);
        hudObjects[2].SetActive(false);
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

        // Enable player controller to allow player tank to accept inputs
        foreach (GameObject player in playerObjectsList)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }
    }
    // Pauses the game and opens pause menu
    public void DoPause()
    {
        pauseMenu.SetActive(true);
        StartGameMenu.SetActive(false);
        scoreWindow.SetActive(false);
        endGame.SetActive(false);
        adminMenu.SetActive(false);
        // Disable all AI movement
        Time.timeScale = 0;

        // Disable player controller to prevent tank firing when unpausing
        foreach (GameObject player in playerObjectsList)
        {
            player.GetComponent<PlayerController>().enabled = false;
        }
    }
    // Opens Title Screen
    public void DoTitleScreen()
    {
        SceneManager.LoadScene(0);
    }
    // Exits the game
    public void DoQuitGame()
    {
        Application.Quit();
    }
    public void CreateNewGame()
    {
        isPreviousGame = true; // Once a new game is created, this boolean is set to true to indicate a game is active (in case player wants to resume game)
        GetComponent<MapGenerator>().GenerateMap(); // Generates a new map
        if (isMultiplayer == true) // Check if multiplayer
        {
            SpawnPlayerTank(); // Instantiate first player
            SpawnPlayerTank(); // Instantiate second player
            TwoPlayerCamera(); // Sets cameras for two players
            TwoPlayerHud(); // Sets HUDs for two players
        }
        else
        {
            SpawnPlayerTank(); // Instantiate a new player 
            SinglePlayerCamera(); // Sets camera for one player
            SinglePlayerHUD(); // Sets HUD for one player
        }
        SetRandomEnemies(); // Sets enemies to random or preset
        StartCoroutine(SpawnEnemyEvent()); // Begin coroutine for SpawnEnemyEvent
        StartCoroutine(SpawnPickupEvent()); // Begin coroutine to spawn pickups
        // Creates a small time window to allow AI tanks to be spawned before the game is allowed to change states, prevents game from ending prematurely
        StartCoroutine(GameReadyEvent()); 
    }
    public void SpawnPlayerTank()
    {
        Transform spawnPoint = playerSpawnsList[Random.Range(0, playerSpawnsList.Count)]; // Random spawnPoint from list
        // Instantiate a player tank prefab
        GameObject playerClone = Instantiate(tankPrototype, spawnPoint.position, spawnPoint.rotation, playerTankShell);
        PlayerData tempPlayerData = playerClone.GetComponent<PlayerData>(); // Temporary playerData component variable

        playerSpawnsList.Remove(spawnPoint); // Remove spawn point from list when used

        playerObjectsList.Add(playerClone); // Adds to list of all active player objects
        playerData.Add(tempPlayerData); // Adds to list of all active playerData components
        tankObjects.Add(playerClone); // Adds to list of all active player and AI tank objects
        tankDataList.Add(playerClone.GetComponent<TankData>()); // Adds to list of all active tankData components

        // Set index numbers for both player and tank (used to remove objects and components from lists on death/removal)
        tempPlayerData.playerIndex = playerObjectsList.Count - 1;
        int currentIndex = playerData[playerCount].playerIndex;
        tempPlayerData.tankIndex = tankObjects.Count - 1;

        // Link camera to playerClone
        cameraObjects[currentIndex].GetComponent<CameraFollow>().playerTank = playerClone;

        // Link hud elements to current playerClone
        if (isMultiplayer == true) // Check if multiplayer
        {
            // Connects HUDs to correct playerData components from list
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
            // Singleplayer HUD
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
                tankObjects.Add(enemyClone); // Adds to list of all active player and AI tank objects
                tankDataList.Add(tempEnemyData.GetComponent<TankData>()); // Adds to list of all active tankData components

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
    private IEnumerator GameReadyEvent()
    {
        yield return new WaitForSeconds(3);
        isGameReady = true;
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
}
