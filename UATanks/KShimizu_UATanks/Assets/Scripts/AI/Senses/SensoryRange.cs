using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Component to detect whether the player is in "Sensory Range" of the AI. If the player is out of range, then the AI's 
vision and hearing scripts will not run. This is used to save resources since only the AI near a player will run these sensory functions*/
public class SensoryRange : MonoBehaviour
{
    [HideInInspector] public bool inSenseRange; // Is the player inRange of sensory information

    public void OnTriggerEnter(Collider other) // OnTriggerEnter for intersecting sense collider
    {

        if (other.gameObject.CompareTag("PlayerSenses")) // If game object is tagged with senses
        {
            inSenseRange = true; // Player is in range
        }
    }

    public void OnTriggerExit(Collider other)  // OnTriggerExit for intersecting sense collider
    {
        if (other.gameObject.CompareTag("PlayerSenses")) // If game object is tagged with senses
        {
            inSenseRange = false; // Player is not in range
        }
    }
}
