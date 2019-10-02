using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player Controller component, accepts all player inputs
public class PlayerController : MonoBehaviour
{
    public PlayerPawn playerPawn;
    public PlayerData playerData;
    private float inputVertical; // Variable for player vertical input, used for tank forward/reverse movement
    private float inputHorizontal; // Variable for player horizontal input, used for tank rotation
    private float moveSpeed; // Float variable that is passed into MoveTank method
    private float rotateSpeed; // Float variable that is passed into RotateTank method
    public Vector3 mousePosition; // Stores the mouse cursor position, will be used for turret rotation

    private void Start()
    {
        playerPawn = GetComponent<PlayerPawn>();
        playerData = GetComponent<PlayerData>();
    }
    void Update()
    {
        if (playerData != null) // Check that playerData List does not return null
        {
            // Player one controls
            if (playerData.playerIndex == 0)
            {
                // Set variable for vertical and horizontal inputs
                inputVertical = Input.GetAxis("Vertical");
                inputHorizontal = Input.GetAxis("Horizontal");
                // Set moveSpeed based on whether inputVertical is a negative or positive value
                if (inputVertical > 0)
                {
                    moveSpeed = inputVertical * playerData.forwardSpeed; // If positive, use forwardSpeed
                }
                else if (inputVertical < 0)
                {
                    moveSpeed = inputVertical * playerData.reverseSpeed; // If negative, use reverseSpeed
                }

                // If inputVertical = 0 then set moveSpeed to 0, this fixes the tank sliding without player input
                else if (inputVertical == 0)
                {
                    moveSpeed = 0;
                }
                rotateSpeed = inputHorizontal * playerData.rotationSpeed; // Set rotateSpeed

                playerPawn.MoveTank(moveSpeed); // Use moveSpeed as the parameter
                playerPawn.RotateTank(rotateSpeed); // Use rotateSpeed as the parameter

                // Fire a single tank round when the "Fire1" button is pressed (default is mouse0)
                if (Input.GetButton("FireOne"))
                {
                    playerPawn.SingleCannonFire();
                }
            }
            // Player two controls
            else if (playerData.playerIndex == 1)
            {
                // Set variable for vertical and horizontal inputs
                inputVertical = Input.GetAxis("VerticalTwo");
                inputHorizontal = Input.GetAxis("HorizontalTwo");
                if (playerData != null)
                {
                    // Set moveSpeed based on whether inputVertical is a negative or positive value
                    if (inputVertical > 0)
                    {
                        moveSpeed = inputVertical * playerData.forwardSpeed; // If positive, use forwardSpeed
                    }
                    else if (inputVertical < 0)
                    {
                        moveSpeed = inputVertical * playerData.reverseSpeed; // If negative, use reverseSpeed
                    }

                    // If inputVertical = 0 then set moveSpeed to 0, this fixes the tank sliding without player input
                    else if (inputVertical == 0)
                    {
                        moveSpeed = 0;
                    }
                    rotateSpeed = inputHorizontal * playerData.rotationSpeed; // Set rotateSpeed

                    playerPawn.MoveTank(moveSpeed); // Use moveSpeed as the parameter
                    playerPawn.RotateTank(rotateSpeed); // Use rotateSpeed as the parameter

                    // Fire a single tank round when the "Fire1" button is pressed (default is mouse0)
                    if (Input.GetButton("FireTwo"))
                    {
                        playerPawn.SingleCannonFire();
                    }
                }
            }
            // Set player to invisible or visible
            if (playerData.isInvisible == true)
            {
                playerPawn.SetInvisible();
            }
            else
            {
                playerPawn.SetVisible();
            }

        }
    }
}
