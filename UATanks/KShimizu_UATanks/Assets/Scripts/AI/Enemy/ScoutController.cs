using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Scout will chase after player and go to last known player location when alerted
public class ScoutController : EnemyController
{
    private float stateEnterTime; // Saves the last time the AI transitioned to a new state
    private bool isFleeing = false; // Indicates whether the AI tank is in the act of fleeing (The flee state has multiple stages)
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

        // Run transition to avoidance function during all states but Avoidance
        if (enemyData.aiState != EnemyData.AIState.Avoidance)
        {
            TransitionAvoidance();
        }
        // Run transition to alert function during all states
        TransitionAlerted();
        // ALERTED STATE
        if (enemyData.aiState == EnemyData.AIState.Alerted)
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
}
