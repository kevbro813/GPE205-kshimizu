using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent AI Controller
public abstract class AIController : MonoBehaviour
{
    // Components used by AI controllers
    [HideInInspector] public TankPawn tankPawn;
    [HideInInspector] public TankData tankData;
    [HideInInspector] public AIVision aiVision;
    [HideInInspector] public AIHearing aiHearing;
    [HideInInspector] public SensoryRange sensoryRange;
}
