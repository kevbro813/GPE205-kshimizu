﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component that simulates AI vision. Uses raycasting within a set range (FoV and maxDistance) to detect the player
public class AIVision : MonoBehaviour
{
    private Transform ttf; // Target transform component
    private Transform tf;
    public AIData aiData;
    public float targetDistance; // Distance from the AI to the target
    [HideInInspector] public Vector3 lastPlayerLocation; // lastPlayerLocation vector set by raycast.point

    void Start()
    {
        aiData = GetComponentInParent<AIData>();
        tf = GetComponent<Transform>();
    }
    public bool CanSee(GameObject target)
    {
        ttf = target.GetComponent<Transform>(); // Get target transform component

        // Find the vector from current object to target

        Vector3 vectorToTarget = ttf.position - tf.position;

        // Find the distance between the two vectors in float to compare with maxViewDistance
        targetDistance = Vector3.Distance(ttf.position, tf.position);

        // Find the angle between the direction our agent is facing (forward in local space) and the vector to the target.
        float angleToTarget = Vector3.Angle(vectorToTarget, tf.forward);

        

        // If that angle is less than our field of view return true, else return false
        if (angleToTarget < aiData.fieldOfView && targetDistance < aiData.maxViewDistance)
        {
            int environmentLayer = LayerMask.NameToLayer("Environment"); // Add Walls layer to variable
            int playerLayer = LayerMask.NameToLayer("PlayerObject"); // Add Player layer to variable
            int layerMask = (1 << playerLayer) | (1 << environmentLayer); // Create layermask

            RaycastHit hit;

            // RaycastHit from object to target, maxview distance, and only affects objects in layermask
            if (Physics.Raycast(tf.position, vectorToTarget, out hit, aiData.maxViewDistance, layerMask))
            {
                if (hit.collider.CompareTag("Player")) // If hit is player then...
                {
                    //Debug.DrawRay(tf.position, vectorToTarget, Color.red, aiData.maxViewDistance); // Draw rays
                    lastPlayerLocation = hit.point; // Set lastPlayerLocation to raycast hit point
                    GameManager.instance.lastPlayerLocation = lastPlayerLocation; // Set lastPlayerLocation in GameManager
                    return true; // Returns true if the collider hit is the player
                }
            }
        }
        return false; // Returns false if anything else is hit
    }
    // Function to check whether the player is within attack rnage, returns a bool
    public bool AttackRange(float distanceToTarget)
    {
        if (distanceToTarget <= aiData.maxAttackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}