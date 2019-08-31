﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Camera script used to follow the player
    public GameObject playerTank; // player object
    public float speed = 0.5f; // Sets the speed the camera follows behind player
    public float cameraTrailDistance = 15.0f;

    void Update()
    {
        float interpolate = speed * Time.deltaTime; // Calculate interpolate time based on speed
        Vector3 camera = this.transform.position; // Get the camera position
        camera.z = Mathf.Lerp(this.transform.position.z, (playerTank.transform.position.z - cameraTrailDistance), interpolate); // Lerp the camera's z position
        camera.x = Mathf.Lerp(this.transform.position.x, playerTank.transform.position.x, interpolate); // Lerp the camera's x position
        this.transform.position = camera; // Set the new position of the camera
    }
}
