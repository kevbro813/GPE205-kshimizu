using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Child of AI Controller, includes FSM for enemy tanks
public class EnemyController : AIController
{
    public EnemyPawn enemyPawn;
    public EnemyData enemyData;
    private float stateEnterTime; // Saves the last time the AI transitioned to a new state
    private bool isFleeing = false; // Indicates whether the AI tank is in the act of fleeing (The flee state has multiple stages)
    void Start(D:\Unity Projects\GPE205\GPE205-kshimizu\UATanks\KShimizu_UATanks\Assets\Scripts\AI\Friendly\FriendlyController.cs)
    {
        enemyPawn = GetComponent<EnemyPawn>();
        enemyData = GetComponent<EnemyData>();
        aiVision = GetComponentInChildren<AIVision>();
        aiHearing = GetComponentInChildren<AIHearing>();
        sensoryRange = GetComponentInChildren<SensoryRange>();
    }
    void Update()
    {
        // IDLE STATE
        if (enemyData.aiState == EnemyData.AIState.Idle)
        {
            DoIdle();
            ChangeState(EnemyData.AIState.Patrol); // TEST PURPOSES ONLY - SO TANKS AREN'T STUCK IN IDLE
        }
        // PATROL STATE
        if (enemyData.aiState == EnemyData.AIState.Patrol)
        {
            DoPatrol();
            // Check if player is in senseRange (AI is blind and deaf while patrolling, unless the player is within "sensory range." This is done to manage resources
            if (sensoryRange.inSenseRange == true)
            {
                // If the player is seen but not in attack range, transition to pursue
                TransitionPursue();

                // If the player is heard while searching, transition to investigate (go to sound origin)
                TransitionInvestigate();
            }
            // If the player is seen and in firing range, transition to attack
            TransitionAttack();
        }
        // ATTACK STATE
        if (enemyData.aiState == EnemyData.AIState.Attack)
        {
            DoAttack();

            // If the player is seen but not in attack range, transition to pursue
            TransitionPursue();

            // If the player is not seen, transition to search (go to last known player location)
            TransitionSearch();
        }
        // SEARCH STATE
        if (enemyData.aiState == EnemyData.AIState.Search)
        {
            DoSearch();
            // If the AI is not currently searching, allow it to transition to patrol state
            if (enemyPawn.isSearching == false)
            {
                // If the player is not seen or heard, transition to patrol
                TransitionPatrol();
            }
            // If the player is seen while searching but out of firing range, transition to pursue
            TransitionPursue();

            // If the player is heard while searching, transition to investigate (go to sound origin)
            TransitionInvestigate();

            // If the player is seen and in firing range, transition to attack
            TransitionAttack();
        }
        // PURSUE STATE
        if (enemyData.aiState == EnemyData.AIState.Pursue)
        {
            DoPursue();

            // If the player is seen and in firing range, transition to attack
            TransitionAttack();

            // If the player is not seen, transition to search (go to last known player location)
            TransitionSearch();
        }
        // INVESTIGATE STATE
        if (enemyData.aiState == EnemyData.AIState.Investigate)
        {
            DoInvestigate();
            // If the AI is not currently investigating, allow it to transition to patrol state
            if (enemyPawn.isInvestigating == false)
            {
                // If nothing is found return to "patrol"
                TransitionPatrol();
            }
            // If the player is seen "pursue"
            TransitionPursue();

            // If the player is seen and in firing range, transition to attack
            TransitionAttack();

        }
        // FLEE STATE
        if (enemyData.aiState == EnemyData.AIState.Flee)
        {
            DoFlee();
        }
        // OBSTACLE AVOIDANCE STATE
        if (enemyData.aiState == EnemyData.AIState.Avoidance)
        {
            DoObstacleAvoidance();
            /* The following will rotate the AI a random amount while avoiding obstacles, this is done by having a random duration during which the AI will rotate.
            This makes the AI have less predictable movement and creates unique patrol routes*/
            if (enemyData.randomRotation <= 0)
            {
                enemyData.aiState = enemyData.tempState; // Return AI to the previous state
                // Create a new random rotation for the next time the AI enters the Avoidance State
                enemyData.randomRotation = Random.Range(enemyData.rotationLow, enemyData.rotationHigh);
            }
            else
            {
                enemyData.randomRotation -= Time.deltaTime; // Decrement time
            }
        }

        // Run transition to flee function during all states
        TransitionFlee();
        
        // Run transition to avoidance function during all states but Avoidance
        if (enemyData.aiState != EnemyData.AIState.Avoidance)
        {
            TransitionAvoidance(); 
        }
    }
    // CALLS ENEMY PAWN FUNCTIONS
    public void DoIdle()
    {
        Debug.Log("Idle");
        enemyPawn.Idle();
    }
    public void DoPursue()
    {
        Debug.Log("Pursue");
        enemyPawn.Pursue();
    }
    public void DoAttack()
    {
        Debug.Log("Attack");
        enemyPawn.Attack();
    }
    public void DoPatrol()
    {
        Debug.Log("Patrol");
        enemyPawn.Patrol();
    }
    public void DoFlee()
    {
        Debug.Log("Flee");
        enemyPawn.Flee();
    }
    public void DoSearch()
    {
        Debug.Log("Search");
        enemyPawn.Search();
    }
    public void DoInvestigate()
    {
        Debug.Log("Investigate");
        enemyPawn.Investigate();
    }
    public void DoObstacleAvoidance()
    {
        Debug.Log("Obstacle Avoidance");
        enemyPawn.ObstacleAvoidance(enemyPawn.obstacleHit);
    }
    // TRANSITION FUNCTIONS (Transitions AI to the various states)

    // Transition to Obstacle Avoidance State
    public void TransitionAvoidance()
    {
        // If an obstacle is detected...
        if(enemyPawn.ObstacleCheck() == true) // If an obstacle is detected...
        {
            enemyData.tempState = enemyData.aiState; // Save the current state
            ChangeState(EnemyData.AIState.Avoidance); // Change to Avoidance state
        }
    }
    // Transition to Patrol State
    public void TransitionPatrol()
    {
        // If the player is not seen or heard...
        if (aiVision.CanSee(GameManager.instance.playerTank) == false && aiHearing.canHear == false)
        {
            // Reset boolean variables in case they are still true after a state change
            enemyPawn.isInvestigating = false;
            enemyPawn.isSearching = false;
            ChangeState(EnemyData.AIState.Patrol); // Change to Patrol state
        }
    }
    // Transition to Attack State
    public void TransitionAttack()
    {
        // If the player is seen...
        if (aiVision.CanSee(GameManager.instance.playerTank) == true)
        {
            // If the player is in attack range...
            if (aiVision.AttackRange(aiVision.targetDistance) == true)
            {
                // Reset boolean variables in case they are still true after a state change
                enemyPawn.isInvestigating = false;
                enemyPawn.isSearching = false;
                enemyPawn.atWaypoint = false;
                ChangeState(EnemyData.AIState.Attack); // Change to Attack State
            }
        }
    }
    // Transition to Flee State
    public void TransitionFlee()
    {
        // Flee if low on health
        if (enemyData.tankHealth <= enemyData.criticalHealth && isFleeing == false)
        {
            isFleeing = true; // Indicates the AI is currently fleeing. This will turn to false after a while, despite the AI still being in the flee state
            // Reset boolean variables in case they are still true after a state change
            enemyPawn.isInvestigating = false;
            enemyPawn.isSearching = false;
            enemyPawn.atWaypoint = false;
            ChangeState(EnemyData.AIState.Flee); // Change to Flee State
        }
    }
    // Transition to Search State
    public void TransitionSearch()
    {
        // If the player is no longer seen...
        if (aiVision.CanSee(GameManager.instance.playerTank) == false)
        {
            // isSearching must be true to run Search method, set once during transition
            enemyPawn.isSearching = true;
            // Reset boolean variables in case they are still true after a state change
            enemyPawn.isInvestigating = false;
            enemyPawn.atWaypoint = false;
            enemyPawn.atSearchLocation = false;
            ChangeState(EnemyData.AIState.Search); // Change to Search State
        }
    }
    // Transition to Investigate State
    public void TransitionInvestigate()
    {
        // If the player is not seen, but heard...
        if (aiVision.CanSee(GameManager.instance.playerTank) == false && aiHearing.canHear == true)
        {
            // isInvestigating must be true to run Investigate method, set once during transition
            enemyPawn.isInvestigating = true;
            // Reset boolean variables in case they are still true after a state change
            enemyPawn.isSearching = false;
            enemyPawn.atWaypoint = false;
            enemyPawn.atInvestigateLocation = false;
            ChangeState(EnemyData.AIState.Investigate); // Change to Investigate State
        }
    }
    // Transition to Pursue State
    public void TransitionPursue()
    {
        // If the player is seen...
        if (aiVision.CanSee(GameManager.instance.playerTank) == true)
        {
            // If the player is not in attack range...
            if (aiVision.AttackRange(aiVision.targetDistance) == false)
            {
                // Reset boolean variables in case they are still true after a state change
                enemyPawn.isInvestigating = false;
                enemyPawn.isSearching = false;
                enemyPawn.atWaypoint = false;
                ChangeState(EnemyData.AIState.Pursue); // Change to Pursue State
            }
        }
    }
    //Transition to Idle State
    public void TransitionIdle()
    {
        // Reset boolean variables in case they are still true after a state change
        enemyPawn.isInvestigating = false;
        enemyPawn.isSearching = false;
        enemyPawn.atWaypoint = false;
        // Do nothing for now
    }
    // Changes the AIState to a new state and keeps track of the time this is done
    public void ChangeState (EnemyData.AIState newState)
    {
        enemyData.aiState = newState; // Set aiState to the new state
        stateEnterTime = Time.time; // Log the time the state change occurred
    }
}
