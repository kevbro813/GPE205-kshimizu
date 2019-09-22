using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    public List<PickupData> powerups;
    public List<PickupData> expiredPowerups;
    public TankData tankData;
    private float duration;
    // Start is called before the first frame update
    void Start()
    {
        powerups = new List<PickupData>();
        tankData = GetComponent<TankData>();
    }

    public void Add(PickupData pickup)
    {
        pickup.OnActivate(tankData);
        if (!pickup.isPermanent)
        {
            powerups.Add(pickup);
        }  
    }
    // Update is called once per frame
    void Update()
    {
        foreach (PickupData pickup in powerups)
        {
           pickup.powerupDuration -= Time.deltaTime;
            if (pickup.powerupDuration <= 0)
            {
                expiredPowerups.Add(pickup);
            }
        }
        foreach (PickupData pickup in expiredPowerups)
        {
            pickup.OnDeactivate(tankData);
            powerups.Remove(pickup);
        }
        expiredPowerups.Clear();
    }
}
