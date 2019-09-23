using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pickup controller is used to activate and deactivate pickups/powerups
public class PickupController : MonoBehaviour
{
    [HideInInspector] public List<PickupData> pickups;
    [HideInInspector] public TankData tankData;
    private float duration;
    // Start is called before the first frame update
    void Start()
    {
        pickups = new List<PickupData>(); // Create a new list of pickups
        tankData = GetComponent<TankData>();
    }

    // Add pickup to active list
    public void Add(PickupData pickup)
    {
        pickup.OnActivate(tankData); // Activate pickup
        if (!pickup.isPermanent)
        {
            // Add non-permanent pickups to list
            pickups.Add(pickup);
            duration = pickup.powerupDuration; // Set duration to the value set in the inspector (Allows the 
        }  
    }
    // Update is called once per frame
    void Update()
    {
        List<PickupData> expiredPickups = new List<PickupData>(); // Create expiredPickups list

        // Loop to check if pickups have expired
        foreach (PickupData pickup in pickups)
        {
            duration -= Time.deltaTime; // Decrement time from pickup duration
            if (duration <= 0)
            {
                expiredPickups.Add(pickup); // If the pickup has expired, add it to the expiredPickups list
            }
        }
        // Loop to deactivate expired pickups and remove them from active list
        foreach (PickupData pickup in expiredPickups)
        {
            pickup.OnDeactivate(tankData); // Deactivate expired pickups
            pickups.Remove(pickup); // Remove deactivated pickups from pickups list
        }
        expiredPickups.Clear(); // Clear expiredPickups list
    }
}
