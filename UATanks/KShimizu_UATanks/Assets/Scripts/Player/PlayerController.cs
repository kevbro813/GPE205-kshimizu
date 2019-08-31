using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerPawn playerPawn;
    public float inputVertical;
    public float inputHorizontal;
    public Vector3 mousePosition;

    // Start is called before the first frame update
    void Start()
    {
        playerPawn = GameManager.instance.playerPawn;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetButton("Vertical"))
        {
            inputVertical = Input.GetAxis("Vertical");
            if (inputVertical > 0)
            {
                playerPawn.Forward();
            }
            if (inputVertical < 0)
            {
                playerPawn.Reverse();
            }
        }
        if (Input.GetButton("Horizontal"))
        {
            inputHorizontal = Input.GetAxis("Horizontal");
            if (inputHorizontal > 0)
            {
                playerPawn.RotateRight();
            }
            if (inputHorizontal < 0)
            {
                playerPawn.RotateLeft();
            }
        }
    }
    void Update()
    {
        //mousePosition = Input.mousePosition;
        //playerPawn.RotateTowardsMouse();
    }
}
