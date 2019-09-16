using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Captains will try to run away from players and alert other enemy tanks
public class CaptainController : EnemyController
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
                // If the player is heard while searching, transition to investigate (go to sound origin)
                TransitionInvestigate();
            }
            // If the player is seen and in firing range, transition to attack
            TransitionAttack();
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
            // If the player is seen and in firing range, transition to attack
            TransitionAttack();
        }
        // ATTACK STATE
        if (enemyData.aiState == EnemyData.AIState.Attack)
        {
            DoAttack();

            // Alert other enemy tanks with the player's location
            GameManager.instance.isAlerted = true;

            // If nothing is found return to "patrol"
            TransitionPatrol();
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
}
