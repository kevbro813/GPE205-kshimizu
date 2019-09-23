using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    public List<PickupData> pickups; 
    public TankData tankData;
    private float duration;
    // Start is called before the first frame update
    void Start()
    {
        pickups = new List<PickupData>();
        tankData = GetComponent<TankData>();
    }

    public void Add(PickupData pickup)
    {
        pickup.OnActivate(tankData);
        if (!pickup.isPermanent)
        {
            pickups.Add(pickup);
            duration = pickup.powerupDuration;
        }  
    }
    // Update is called once per frame
    void Update()
    {
        List<PickupData> expiredPickups = new List<PickupData>();
        foreach (PickupData pickup in pickups)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                expiredPickups.Add(pickup);
            }
        }
        foreach (PickupData pickup in expiredPickups)
        {
            pickup.OnDeactivate(tankData);
            pickups.Remove(pickup);
        }
        expiredPickups.Clear();
    }
}
