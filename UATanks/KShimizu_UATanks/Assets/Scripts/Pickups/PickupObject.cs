using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public PickupData pickupData;
    public int pickupListIndex;
    public SphereCollider sphereCollider;
    public MeshRenderer[] meshRenderer;
    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
    }
    public void OnTriggerEnter(Collider other)
    {
        PickupController pickupController = other.GetComponent<PickupController>();
        if (pickupController != null)
        {
            pickupController.Add(pickupData);
            sphereCollider.enabled = false;
            for (int i = 0; i < meshRenderer.Length; i++)
            {
                meshRenderer[i].enabled = false;
            }
            StartCoroutine("RespawnPickupEvent");
        }
    }
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
