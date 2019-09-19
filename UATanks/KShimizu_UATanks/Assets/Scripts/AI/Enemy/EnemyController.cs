using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy Controller can be used for an AI with all states
public class EnemyController : AIController
{
    public EnemyPawn enemyPawn;
    public EnemyData enemyData;
    private float stateEnterTime; // Saves the last time the AI transitioned to a new state
    private bool isFleeing = false; // Indicates whether the AI tank is in the act of fleeing (The flee state has multiple stages)
    public AIState aiState; // Allows the AI state to be changed in the Inspector
    public AIState tempState; // Saves the previous state temporarily (similar to a double buffer pattern) Used to transition from obstacle avoidance state back to the previous state
    public enum AIState { Idle, Patrol, Attack, Search, Pursue, Flee, Investigate, Avoidance, Alerted } // Selections for AI State
    void Start()
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
        if (aiState == AIState.Idle)
        {
            DoIdle();
            ChangeState(AIState.Patrol); // TEST PURPOSES ONLY - SO TANKS AREN'T STUCK IN IDLE
        }
        // PATROL STATE
        if (aiState == AIState.Patrol)
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
        if (aiState == AIState.Attack)
        {
            DoAttack();

            // If the player is seen but not in attack range, transition to pursue
            TransitionPursue();

            // If the player is not seen, transition to search (go to last known player location)
            TransitionSearch();
        }
        // SEARCH STATE
        if (aiState == AIState.Search)
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
        if (aiState == AIState.Pursue)
        {
            DoPursue();

            // If the player is seen and in firing range, transition to attack
            TransitionAttack();

            // If the player is not seen, transition to search (go to last known player location)
            TransitionSearch();
        }
        // INVESTIGATE STATE
        if (aiState == AIState.Investigate)
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
        if (aiState == AIState.Flee)
        {
            DoFlee();
        }
        // OBSTACLE AVOIDANCE STATE
        if (aiState == AIState.Avoidance)
        {
            DoObstacleAvoidance();
            /* The following will rotate the AI a random amount while avoiding obstacles, this is done by having a random duration during which the AI will rotate.
            This makes the AI have less predictable movement and creates unique patrol routes*/
            if (enemyData.randomRotation <= 0)
            {
                aiState = tempState; // Return AI to the previous state
                // Create a new random rotation for the next time the AI enters the Avoidance State
                enemyData.randomRotation = Random.Range(enemyData.rotationLow, enemyData.rotationHigh);
            }
            else
            {
                enemyData.randomRotation -= Time.deltaTime; // Decrement time
            }
        }
        // Run transition to alert function during all states
        TransitionAlerted();

        // Run transition to flee function during all states
        TransitionFlee();
        
        // Run transition to avoidance function during all states but Avoidance
        if (aiState != AIState.Avoidance)
        {
            TransitionAvoidance(); 
        }
        // ALERTED STATE
        if (aiState == AIState.Alerted)
        {
            DoAlerted();
            // If the AI is not currently alert, allow it to transition to patrol state
            if (enemyPawn.isAlertActive == false)
            {
                // If the player is not seen or heard, transition to patrol
                TransitionPatrol();
            }
            // If the player is seen while alert but out of firing range, transition to pursue
            TransitionPursue();

            // If the player is heard while alert, transition to investigate (go to sound origin)
            TransitionInvestigate();

            // If the player is seen and in firing range, transition to attack
            TransitionAttack();
        }
    }
// CALLS ENEMY PAWN FUNCTIONS
public void DoIdle()
    {
        enemyPawn.Idle();
    }
    public void DoPursue()
    {
        enemyPawn.Pursue();
    }
    public void DoAttack()
    {
        enemyPawn.Attack();
    }
    public void DoPatrol()
    {
        enemyPawn.Patrol();
    }
    public void DoFlee()
    {
        enemyPawn.Flee();
    }
    public void DoSearch()
    {
        enemyPawn.Search();
    }
    public void DoInvestigate()
    {
        enemyPawn.Investigate();
    }
    public void DoObstacleAvoidance()
    {
        enemyPawn.ObstacleAvoidance(enemyPawn.obstacleHit);
    }
    public void DoAlerted()
    {
        enemyPawn.Alerted();
    }
    // TRANSITION FUNCTIONS (Transitions AI to the various states)

    // Transition to Obstacle Avoidance State
    public void TransitionAvoidance()
    {
        // If an obstacle is detected...
        if(enemyPawn.ObstacleCheck() == true) // If an obstacle is detected...
        {
            enemyPawn.atWaypoint = false;
            tempState = aiState; // Save the current state
            ChangeState(AIState.Avoidance); // Change to Avoidance state
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
            enemyPawn.atWaypoint = false;
            ChangeState(AIState.Patrol); // Change to Patrol state
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
                enemyPawn.isAlertActive = false;
                ChangeState(AIState.Attack); // Change to Attack State
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
            enemyPawn.isAlertActive = false;
            ChangeState(AIState.Flee); // Change to Flee State
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
            enemyPawn.isAlertActive = false;
            ChangeState(AIState.Search); // Change to Search State
        }
    }
    // Transition to Investigate State
    public void TransitionInvestigate()
    {
        // If the player is not seen, but heard...
        if (aiVision.CanSee(GameManager.instance.playerTank) == false && aiHearing.canHear == true)
        {
            // isInvestigating must be true to run Investigate function, set once during transition
            enemyPawn.isInvestigating = true;
            // Reset boolean variables in case they are still true after a state change
            enemyPawn.isSearching = false;
            enemyPawn.atWaypoint = false;
            enemyPawn.atInvestigateLocation = false;
            enemyPawn.isAlertActive = false;
            ChangeState(AIState.Investigate); // Change to Investigate State
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
                enemyPawn.isAlertActive = false;
                ChangeState(AIState.Pursue); // Change to Pursue State
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
        enemyPawn.isAlertActive = false;
        // Do nothing for now
    }
    public void TransitionAlerted()
    {
        // If the player is not in attack range...
        if (GameManager.instance.isAlerted == true)
        {
            // isAlertActive must be true to run Alerted function, set once during transition
            enemyPawn.isAlertActive = true;

            tempState = aiState; // Save the current state

            // Reset boolean variables in case they are still true after a state change
            enemyPawn.isInvestigating = false;
            enemyPawn.isSearching = false;
            enemyPawn.atWaypoint = false;
            ChangeState(AIState.Alerted); // Change to Pursue State
        }
        if (GameManager.instance.isAlerted == false && enemyPawn.isAlertActive == true)
        {
            enemyPawn.isAlertActive = false;
            enemyPawn.isInvestigating = false;
            enemyPawn.isSearching = false;
            enemyPawn.atWaypoint = false;
            ChangeState(AIState.Patrol); // Return AI to patrol
        }
    }
    // Changes the AIState to a new state and keeps track of the time this is done
    public void ChangeState (AIState newState)
    {
        aiState = newState; // Set aiState to the new state
        stateEnterTime = Time.time; // Log the time the state change occurred
    }
}
