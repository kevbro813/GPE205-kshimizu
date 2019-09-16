using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parent AI Controller
public abstract class AIController : MonoBehaviour
{
    // Components used by AI controllers
    public TankPawn tankPawn;
    public TankData tankData;
    public AIVision aiVision;
    public AIHearing aiHearing;
    public SensoryRange sensoryRange;
}
