using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component attached to a pickup object
public class PickupObject : MonoBehaviour
{
    public PickupData pickupData;
    public int pickupListIndex; // Pickup index is used to add or remove from list of active pickups
    [HideInInspector] public SphereCollider sphereCollider;
    [HideInInspector] public MeshRenderer[] meshRenderer;
    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
    }

    // Trigger if the pickup collides with an object that has a PickupController component
    public void OnTriggerEnter(Collider other)
    {
        PickupController pickupController = other.GetComponent<PickupController>();
        if (pickupController != null) // Check that the object has a PickupController
        {
            pickupController.Add(pickupData);

            // Rather than destroying the pickup, I decided to disable the sphere collider and mesh renderer
            sphereCollider.enabled = false;
            // Loop through all meshRenderer components and m
            for (int i = 0; i < meshRenderer.Length; i++)
            {
                meshRenderer[i].enabled = false;
            }
            StartCoroutine("RespawnPickupEvent"); // Start respawn pickup event
        }
    }
    // Coroutine used to reactivate the sphere collider and mesh renderer after pickupRespawnDelay has elapsed
    private IEnumerator RespawnPickupEvent()
    {
        yield return new WaitForSeconds(GameManager.instance.pickupRespawnDelay);
        sphereCollider.enabled = true;
        for (int i = 0; i < meshRenderer.Length; i++)
        {
            meshRenderer[i].enabled = true;
        }
    }
}
