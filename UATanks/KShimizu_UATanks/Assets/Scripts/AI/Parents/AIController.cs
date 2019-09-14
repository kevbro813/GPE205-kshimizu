using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIController : MonoBehaviour
{
    public TankPawn tankPawn;
    public TankData tankData;
    public AIVision aiVision;
    public AIHearing aiHearing;
    public SensoryRange sensoryRange;
}
