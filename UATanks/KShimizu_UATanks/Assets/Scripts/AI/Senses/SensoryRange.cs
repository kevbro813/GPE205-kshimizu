using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensoryRange : MonoBehaviour
{
    [HideInInspector] public bool inSenseRange; // Is the player inRange of sensory information

    public void OnTriggerEnter(Collider other) // OnTriggerEnter for intersecting sense collider
    {

        if (other.gameObject.CompareTag("PlayerSenses")) // If game object is tagged with senses
        {
            //Debug.Log("In Range");
            inSenseRange = true; // Player is in range
        }
    }

    public void OnTriggerExit(Collider other)  // OnTriggerExit for intersecting sense collider
    {
        if (other.gameObject.CompareTag("PlayerSenses")) // If game object is tagged with senses
        {
            //Debug.Log("Out of Range");
            inSenseRange = false; // Player is not in range
        }
    }
}
