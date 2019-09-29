using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component that simulates AI hearing. Uses a sphere collider to detect nearby players
public class AIHearing : MonoBehaviour
{
    private Transform tf; // transform component
    private Transform ttf; // target (player) transform component
    private SphereCollider col; // Sphere collider
    public AIData aiData;



    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        aiData = GetComponentInParent<AIData>();
        col = GetComponent<SphereCollider>();
        col.radius = aiData.hearingRadius; // Set sphere collider radius to hearing radius, set in inspector
    }
    public void OnTriggerEnter(Collider other) // OnTriggerEnter for intersecting sound collider
    {
        if (other.gameObject.CompareTag("Sound")) // If game object is tagged with sound, return true
        {
            ttf = other.GetComponent<Transform>(); // Get player transform component
            aiData.canHear = true;
        }
    }

    public void OnTriggerExit(Collider other)  // OnTriggerExit for intersecting sound collider
    {
        if (other.gameObject.CompareTag("Sound")) // If game object is tagged with sound, return false
        {
            aiData.canHear = false;
        }
    }
    void Update()
    {
        if (aiData.canHear == true)
        {
            if (ttf != null) // Check that the target transform component is not null. Needed to prevent error when player is killed
            {
                // Find the vector from current object to target
                Vector3 vectorToSound = ttf.position - tf.position;

                RaycastHit hit; // Used to find location of hit

                //RaycastHit from AI to target
                if (Physics.Raycast(tf.position, vectorToSound, out hit, col.radius))
                {
                    if (hit.collider.CompareTag("Sound")) // If hit is player then...
                    {
                        aiData.lastSoundLocation = hit.point; // Save ray hit point as lastSoundLocation
                        GameManager.instance.lastSoundLocation = aiData.lastSoundLocation; // Set lastSoundLocation in GameManager
                    }
                }
            }
            else
            {
                aiData.canHear = false; // If ttf is null then the player was killed. This returns the AI back to patrol
            }
        }
    }
}
