using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds all the player data variables, most of the variables are found in the "TankData" parent
public class PlayerData : TankData
{
    public int playerLives = 3;
    public Transform tf;
    public int playerIndex;
    public string playerName;
    public List<Transform> activeSpawnsList;
    public override void Start()
    {
        tf = GetComponent<Transform>();
        base.Start();
    }
    public override void Update()
    {
        base.Update();
    }
}