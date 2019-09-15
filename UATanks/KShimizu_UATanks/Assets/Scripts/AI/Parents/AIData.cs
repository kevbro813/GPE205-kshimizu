using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIData : TankData
{
    public float fieldOfView = 55.0f; // Set Field of View in Unity, default 45 degrees
    public float maxViewDistance = 5.0f; // Max view distance of the object, can be set in Unity
    public float maxAttackRange = 5.0f; // Max distance to be in attack range
    public float hearingRadius = 5.0f;
    public float waitDuration;
    public float waypointRange = 1.0f;
    public float searchDuration = 3;
    public float investigateDuration = 3;
    public float avoidanceDistance = 2;
    public float rotationHigh = 1;
    public float rotationLow = 3;
    public float randomRotation;
}
