using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : AIData
{
    public int pointValue; // Number of points granted to the player when the enemy tank is destroyed
    public PlayerData playerData;
    // Start is called before the first frame update
    void Start()
    {
        playerData = GameManager.instance.playerData;
    }

    // Update is called once per frame
    void Update()
    {
        TankDestroyed(); // Call the TankDestroyed method to check if the tank is out of health, and destroys the tank if necessary
    }
    public override void TankDestroyed()
    {
        base.TankDestroyed();
        // Add to player's score
        if(tankHealth <= 0)
        {
            playerData.playerScore += pointValue;
        }
    }
}
