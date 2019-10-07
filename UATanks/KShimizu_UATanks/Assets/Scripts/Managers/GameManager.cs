using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Game Manager singleton
public class GameManager : MonoBehaviour
{
    [Header("Game Manager")]
    [HideInInspector] public static GameManager instance;
    public string gameState = "pregame"; // Current game state
    [Space (10)]
    // Lists of available prefabs set in inspector
    [Header("Set Available Game Objects")]
    public GameObject tankPrototype; // Set in inspector
    public List<GameObject> enemyTankList; // List of enemy tanks, will be used for spawning enemies
    public List<GameObject> pickupList; // List of available pickups, set in inspector
    [Space(10)]
    // Lists of generated spawn points
    [Header("Lists of Spawnpoints")]
    public List<Transform> playerSpawnsList = new List<Transform>(); // List of all player spawns
    public List<Transform> enemySpawnsList; // List of all spawn points for enemies
    public List<Transform> pickupSpawnsList; // List of all pickup spawns, set automatically when map is generated
    [HideInInspector] public SpawnManager spawnManager; // Spawn manager component
    [Space(10)]
    // List of all enemy waypoints to use for patrolling
    [Header("List of Enemy Waypoints")]
    public List<Transform> enemyWaypointsList;
    [Space(10)]
    // Lists of all active player, enemy and pickup game objects
    [Header("List of Active Game Objects")]
    public List<GameObject> activePlayersList; // List of active player objects
    public List<GameObject> activeEnemiesList; // List of all current enemy objects
    public List<GameObject> activePickupsList; // List of all active pickups
    [Space(10)]
    // Lists of all active player, enemy and pickup data components
    [Header("List of Data Components and Pickup Objects")]
    public List<PlayerData> playerDataList; // List of active playerData components
    public List<EnemyData> enemyDataList; // List of all EnemyData components
    public List<PickupObject> pickupObjectList; // List of all active pickup objects
    [Space(10)]
    // Lists of all active tank game objects and tankData components
    [Header("List of All Active Tank Objects and Data Components")]
    public List<GameObject> tankObjectList; // List of all AI and player tank objects
    public List<TankData> tankDataList; // List of all AI and player tankData components
    [Space(10)]
    // Map specific variables
    [Header("Map Variables")]
    [HideInInspector] public MapGenerator mapGenerator; // Map Generator component
    [HideInInspector] public Room[,] grid; // Grid used to store procedurally generated map
    public bool isRandomEnemy = false; // Generate a random assortment of enemies or one based on a preset seed (Map of Day or Preset Seed)
    public int tileRows;
    public int tileColumns;
    [Space(10)]
    // Canvas game objects (used to activate/deactivate)
    [Header("Canvas Objects")]
    public GameObject StartGameMenu; // Start Game menu
    public GameObject pauseMenu; // Pause menu
    public GameObject scoreWindow; // Score window
    public GameObject endGame; // End Game screen
    public GameObject adminMenu; // Admin menu
    [HideInInspector] public UIManager uiManager; // UI manager component
    [Space(10)]
    // Camera components
    [Header("Camera")]
    public GameObject[] cameraObjects; // List of cameras
    public List<Camera> cameraComponents; // List of camera components
    [Space(10)]
    // HUD components
    [Header("HUD")]
    public GameObject[] hudObjects; // List of HUD objects
    public List<HUD> hudComponents; // List of HUD components
    [Space(10)]
    // Player variables
    [Header("Player Variables")]
    public float playerRespawnDelay = 1; // Duration for player respawning after death
    public int playersCreated = 0; // Tracks the number of players in the game
    public int playersAlive = 0;
    [Space(10)]
    // Enemy variables
    [Header("Enemy Variables")]
    [HideInInspector] public float enemiesSpawned = 0; // Tracks the number of enemies spawned into the world
    public float spawnDelay = 0; // Delay between enemies being spawned into the world
    public int maxEnemies = 8; // Maximum number of enmies spawned
    [Space(10)]
    // Pickup variables
    [Header("Pickup Variables")]
    public int maxPickups = 10; // Maximum number of pickups that will spawn, set in inspector
    public float pickupRespawnDelay = 5.0f; // This is the delay between when a pickup is taken and when it reappears
    [HideInInspector] public float currentPickupQuantity = 0; // Tracks the number of pickups spawned in the world
    public float pickupSpawnDelay = 0; // Delay to spawn pickups when the game is first started
    [Space(10)]
    // Sound Manager and Audio components
    [HideInInspector] public SoundManager soundManager; // Sound Manager component
    [HideInInspector] public AudioSource asMusic; // Music audio source component
    [HideInInspector] public AudioSource asSFX; // SFX audio source component
    [Space(10)]
    // Scoring
    [Header("Scoring")]
    [HideInInspector] public ScoreManager scoreManager; // Score Manager Component
    public List<ScoreData> highScores; // List of current high scores
    public List<ScoreData> currentScores; // List of scores of players currently playing
    public int maxScores = 10; // Maximum number of scores that are posted
    public float killMultiplier; // Multiplies score when the player kills a tank
    [Space(10)]
    // Player Location variables and AI alert
    [HideInInspector] public Vector3 lastPlayerLocation; // Last known location of the player, visible to all AI. Used in Alert system
    [HideInInspector] public Vector3 lastSoundLocation; // Last known sound detected, visible to all AI
    // Game variables
    [Header("Game Variables")]
    public bool isAlerted = false; // If the player is seen by a guard or captain, this will be set to true, calling other enemy tanks to go to last known player location
    public bool isMultiplayer = false; // Set by selecting the multiplayer option in game. Used by several functions to determine multiplayer
    public bool isPlayerOneDead = false; // Boolean that indicates whether player one is dead
    public bool isPlayerTwoDead = false; // Boolean that indicates whether player two is dead 
    public bool isGameReady = false; // Checks if game is ready to prevent win conditions from being instantly satisfied
    public string playerOneName;
    public string playerTwoName;
    public bool isPreviousGame = false; // Is there a previous game active
    public bool isPaused = false; // Is the game paused
    public bool isScoreDisplayed = false; // Is the score screen open
    public bool isAdminMenu = false; // Is the admin menu open
    [Space(10)]
    // Organization
    [Header("Organization")]
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
        spawnManager = GetComponent<SpawnManager>();
        uiManager = GetComponent<UIManager>();
        asMusic = GameObject.FindWithTag("MusicSource").GetComponent<AudioSource>();
        asSFX = GameObject.FindWithTag("SFXSource").GetComponent<AudioSource>();
        highScores = new List<ScoreData>(); // Reset highScores
        scoreManager = GetComponent<ScoreManager>();
        // Initialize, load and fill empty scores
        scoreManager.InitializeValues();
        scoreManager.LoadPlayerScores();
        scoreManager.FillEmptyScores();
        LoadSettings(); // Load settings on start
    } 
    void Update()
    {
        // Game Finite State Machine
        if (gameState == "pregame") // Pregame "Start Menu"
        {
            DoPregame();
        }
        if (gameState == "resume") // Resume game from pause
        {
            DoResumeGame();
            gameState = "active";
        }
        if (gameState == "active") // Active game state
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
        if (gameState == "pause") // Pause game
        {
            DoPause();
            if (isPaused == false)
            {
                gameState = "resume";
            }
            // Transitions in UI components
        }
        if (gameState == "title") // Title Screen
        {
            DoTitleScreen(); 
            gameState = "pregame";
            // Transitions in UI components
        }
        if (gameState == "quit") // Quit game
        {
            DoQuitGame(); // Ends game
        }
        if (gameState == "idle") // Idle
        {
            // Do Nothing.
        }
        if (gameState == "endgame") // End game screen
        {
            DoEndGame();
            // Transitions in UI components
        }
        if (gameState == "score") // Open score screen
        {
            DoScoreWindow();
            if (isScoreDisplayed == false)
            {
                gameState = "resume";
            }
        }
        if (gameState == "admin") // Open admin menu
        {
            DoAdminMenu();
            if (isAdminMenu == false)
            {
                gameState = "resume";
            }
        }
        if (gameState == "newgame") // Start a new game
        {
            DoNewGame();
            gameState = "active";
        }
        if (gameState == "continue") // Continue game
        {
            DoContinueGame();
            gameState = "active";
        }
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
    private IEnumerator GameReadyEvent()
    {
        yield return new WaitForSeconds(3);
        isGameReady = true;
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
            spawnManager.SpawnPlayerTank(); // Instantiate first player
            spawnManager.SpawnPlayerTank(); // Instantiate second player
            uiManager.TwoPlayerCamera(); // Sets cameras for two players
            uiManager.TwoPlayerHud(); // Sets HUDs for two players
            playerDataList[0].playerName = playerOneName;
            playerDataList[1].playerName = playerTwoName;
        }
        else
        {
            spawnManager.SpawnPlayerTank(); // Instantiate a new player 
            uiManager.SinglePlayerCamera(); // Sets camera for one player
            uiManager.SinglePlayerHUD(); // Sets HUD for one player
            playerDataList[0].playerName = playerOneName;
        }
        RandomEnemies(); // Sets enemies to random or preset
        StartCoroutine(spawnManager.SpawnEnemyEvent()); // Begin coroutine for SpawnEnemyEvent
        StartCoroutine(spawnManager.SpawnPickupEvent()); // Begin coroutine to spawn pickups
        // Creates a small time window to allow AI tanks to be spawned before the game is allowed to change states, prevents game from ending prematurely
        StartCoroutine(GameReadyEvent());
    }
    // Reset Game Function
    public void ResetGame()
    {
        // Destroy map objects
        for (int i = 0; i < tileRows; i++)
        {
            for (int j = 0; j < tileColumns; j++)
            {
                Destroy(GameManager.instance.grid[j, i].gameObject);
            }
        }
        // Destroy all game objects and component lists
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
        // Clear all Lists
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
        // Reset all game variables
        playersCreated = 0;
        playersAlive = 0;
        enemiesSpawned = 0;
        currentPickupQuantity = 0;
        isAlerted = false;
        isPlayerOneDead = false;
        isPlayerTwoDead = false;
        isGameReady = false;
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
    // Load settings function
    public void LoadSettings()
    {
        PauseMenu pauseMenuComponent = pauseMenu.GetComponent<PauseMenu>();
        soundManager.SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume"));
        soundManager.SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume"));
        pauseMenuComponent.musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        pauseMenuComponent.sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }
}
