using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    private Transform ttf; // Target transform component
    private Transform tf; // This object's transform component
    public AIData aiData;
    public float targetDistance;
    [HideInInspector] public Vector3 lastPlayerLocation; // lastPlayerLocation vector set by raycast.point

    void Start()
    {
        aiData = GetComponentInParent<AIData>();
    }
    public bool CanSee(GameObject target)
    {
        ttf = target.GetComponent<Transform>(); // Get target transform component
        tf = GetComponent<Transform>(); // transform component

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
            if (Physics.Raycast(tf.position + (transform.up * 0.5f), vectorToTarget, out hit, aiData.maxViewDistance, layerMask))
            {
                if (hit.collider.CompareTag("Player")) // If hit is player then...
                {
                    Debug.DrawRay(tf.position + (transform.up * 0.5f), vectorToTarget, Color.red, aiData.maxViewDistance); // Draw rays
                    lastPlayerLocation = hit.point; // Set lastPlayerLocation to raycast hit point
                    GameManager.instance.lastPlayerLocation = lastPlayerLocation; // Set lastPlayerLocation in GameManager
                    return true;
                }
            }
        }
        return false;
    }
    public bool AttackRange(float distanceToTarget)
    {
        if (distanceToTarget <= aiData.maxAttackRange)
        {
            //Debug.Log("In Attack Range");
            return true;
        }
        else
        {
            //Debug.Log("Not in Attack Range");
            return false;
        }
    }
}
