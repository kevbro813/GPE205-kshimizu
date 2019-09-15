using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHearing : MonoBehaviour
{
    private GameObject playerTank; // player game object
    private Transform tf; // transform component
    private Transform ttf; // target (player) transform component
    private SphereCollider col;
    public AIData aiData;
    [HideInInspector] public bool canHear; // Used to activate the sound raycast
    [HideInInspector] public Vector3 lastSoundLocation; // lastSoundLocation vector set by raycast.point

    // Start is called before the first frame update
    void Start()
    {
        playerTank = GameManager.instance.playerTank;
        ttf = playerTank.GetComponent<Transform>(); // Get player transform component
        tf = GetComponent<Transform>();
        aiData = GetComponentInParent<AIData>();
        col = GetComponent<SphereCollider>();
        col.radius = aiData.hearingRadius; // Set sphere collider radius to hearing radius, set in inspector
    }
    public void OnTriggerEnter(Collider other) // OnTriggerEnter for intersecting sound collider
    {
        if (other.gameObject.CompareTag("Sound")) // If game object is tagged with sound, return true
        {
            canHear = true;
        }
    }

    public void OnTriggerExit(Collider other)  // OnTriggerExit for intersecting sound collider
    {
        if (other.gameObject.CompareTag("Sound")) // If game object is tagged with sound, return false
        {
            canHear = false;
        }
    }
    void Update()
    {
        if (canHear == true)
        {
            // Find the vector from current object to target
            Vector3 vectorToSound = ttf.position - tf.position;
           
            //RaycastHit from object to target, max hearing distance, and only affects objects in layermask
            RaycastHit hit;

            if (Physics.Raycast(tf.position, vectorToSound, out hit, col.radius))
            {
                if (hit.collider.CompareTag("Sound")) // If hit is player then...
                {
                    //Debug.DrawRay(tf.position, vectorToSound, Color.blue, col.radius); // Draw rays
                    lastSoundLocation = hit.point; // Save ray hit point as lastSoundLocation
                    //Debug.Log(lastSoundLocation);
                    GameManager.instance.lastSoundLocation = lastSoundLocation; // Set lastSoundLocation in GameManager
                }
            }
        }
    }
}
