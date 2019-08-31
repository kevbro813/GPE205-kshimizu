using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerTank;
    public GameObject tankPrototype;
    public PlayerPawn playerPawn;
    public PlayerController playerController;
    public Transform playerSpawn;
    public Camera mainCamera;
    public PlayerData playerData;
    public List<GameObject> enemyTankList;
    public List<Transform> enemyTankSpawnList;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        playerController = GameObject.FindWithTag("GameManager").GetComponent<PlayerController>();
        mainCamera = Camera.main;
        CreatePlayerTank();
        UpdatePlayer();
    }
    void Update()
    {
        // Game State Machine
    }
    public void CreatePlayerTank()
    {
        // Instantiate a player tank prefab
        playerTank = Instantiate(tankPrototype, playerSpawn.position, playerSpawn.rotation) as GameObject;
    }

    public void UpdatePlayer()
    {
        // Add all variables that need to be updated when a new player instance is created
        playerPawn = playerTank.GetComponent<PlayerPawn>();
        mainCamera.GetComponent<CameraFollow>().playerTank = playerTank;
        playerData = playerTank.GetComponent<PlayerData>();
    }
    // Various state methods
}
