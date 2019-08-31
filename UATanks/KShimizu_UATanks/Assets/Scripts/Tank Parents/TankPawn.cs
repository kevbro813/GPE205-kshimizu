using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPawn : MonoBehaviour
{
    public Transform tf;
    public PlayerData playerData;

    // Tank Movement Methods
    public void Forward()
    {
        tf.Translate((Vector3.forward * playerData.moveSpeed * Time.deltaTime), Space.Self);
    }
    public void Reverse()
    {
        tf.Translate((Vector3.back * playerData.moveSpeed * Time.deltaTime), Space.Self);
    }
    public void RotateRight()
    {
        tf.Rotate((Vector3.up * playerData.rotationSpeed * Time.deltaTime), Space.Self);
    }
    public void RotateLeft()
    {
        tf.Rotate((Vector3.down * playerData.rotationSpeed * Time.deltaTime), Space.Self);
    }

    // Tank Attack Methods
    public void SingleCannonFire()
    {

    }
}
