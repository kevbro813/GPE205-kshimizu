using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public PickupData pickupData;

    public void OnTriggerEnter(Collider other)
    {
        PickupController powerupController = other.GetComponent<PickupController>();
        if (powerupController != null)
        {
            powerupController.Add(pickupData);

            Destroy(gameObject);
        }
    }
}
