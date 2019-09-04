using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : TankData
{
    public int playerScore = 0; // Integer that tracks the player's score
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        TankDestroyed(); // Call the TankDestroyed method to check if the tank is out of health, and destroys the tank if necessary
    }
}
