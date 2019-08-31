using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : TankPawn
{

    public PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        playerController = GameManager.instance.playerController;
        playerData = GameManager.instance.playerData;
    }

    // Update is called once per frame
    void Update()
    {
        
    }   
    //public void RotateTowardsMouse()
    //{
    //    playerController.mousePosition.y = transform.position.y + transform.localScale.y;
    //    Debug.Log(playerController.mousePosition);
    //    tf.LookAt(playerController.mousePosition);
    //    //Vector3 direction = new Vector3(playerController.mousePosition.x - tf.position.x, tf.position.y, playerController.mousePosition.z - tf.position.z); // Set the direction to the mouse position on the x and y axis
    //    //tf.right -= direction; // Sets the transform direction to direction vector. Negative direction because default sprite faces down.
    //    // ***Use slerp to smooth tank rotation and add a lag***
    //}
}
