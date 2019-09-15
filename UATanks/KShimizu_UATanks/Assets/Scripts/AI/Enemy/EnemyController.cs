using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : AIController
{
    public EnemyPawn enemyPawn;
    public EnemyData enemyData;
    private float stateEnterTime;
    private float waitTime;
    private bool isFleeing = false;
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
        if (enemyData.aiState == EnemyData.AIState.Idle)
        {
            DoIdle();
            ChangeState(EnemyData.AIState.Patrol); // TEST PURPOSES ONLY - SO TANKS AREN'T STUCK IN IDLE
        }
        if (enemyData.aiState == EnemyData.AIState.Patrol)
        {
            DoPatrol();
            // Check if player is in senseRange
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
        if (enemyData.aiState == EnemyData.AIState.Attack)
        {
            DoAttack();

            // If the player is seen but not in attack range, transition to pursue
            TransitionPursue();

            // If the player is not seen, transition to search (go to last known player location)
            TransitionSearch();
        }
        if (enemyData.aiState == EnemyData.AIState.Search)
        {
            DoSearch();
            // TODO: wrap the patrol transition in an if statement to run once the tank has reached the last known player location after a fixed amount of time
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
        if (enemyData.aiState == EnemyData.AIState.Pursue)
        {
            DoPursue();

            // If the player is seen and in firing range, transition to attack
            TransitionAttack();

            // If the player is not seen, transition to search (go to last known player location)
            TransitionSearch();
        }
        if (enemyData.aiState == EnemyData.AIState.Investigate)
        {
            DoInvestigate();
            // TODO: wrap the patrol transition in an if statement to run once the tank has reached the sound origin location after a fixed amount of time
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
        if (enemyData.aiState == EnemyData.AIState.Flee)
        {
            DoFlee();
        }
        // Run transition to flee function during all states
        TransitionFlee();
        
        // Obstacle Avoidance
        if (enemyData.aiState == EnemyData.AIState.Avoidance)
        {
            DoObstacleAvoidance();    
            if (enemyData.randomRotation <= 0)
            {
                enemyData.aiState = enemyData.tempState; // return ai to previous state
                // Random rotation variable makes the AI rotation unpredictable
                enemyData.randomRotation = Random.Range(enemyData.rotationLow, enemyData.rotationHigh);
            }
            else
            {
                enemyData.randomRotation -= Time.deltaTime;
            }
        }
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
    // TRANSITIONS BETWEEN STAGES
    public void TransitionAvoidance()
    {
        // If an obstacle is detected...
        if(enemyPawn.ObstacleCheck() == true)
        {
            enemyData.tempState = enemyData.aiState; // Save the current state
            ChangeState(EnemyData.AIState.Avoidance); // Transition to Avoidance state
        }
    }
    public void TransitionPatrol()
    {
        if (aiVision.CanSee(GameManager.instance.playerTank) == false && aiHearing.canHear == false)
        {
            // Reset boolean variables in case they are still true after a state change
            enemyPawn.isInvestigating = false;
            enemyPawn.isSearching = false;
            ChangeState(EnemyData.AIState.Patrol);
        }
    }
    public void TransitionAttack()
    {
        if (aiVision.CanSee(GameManager.instance.playerTank) == true)
        {
            if (aiVision.AttackRange(aiVision.targetDistance) == true)
            {
                // Reset boolean variables in case they are still true after a state change
                enemyPawn.isInvestigating = false;
                enemyPawn.isSearching = false;
                enemyPawn.atWaypoint = false;
                ChangeState(EnemyData.AIState.Attack);
            }
        }
    }
    public void TransitionFlee()
    {
        // Flee if low on health
        if (enemyData.tankHealth <= enemyData.criticalHealth && isFleeing == false)
        {
            isFleeing = true;
            // Reset boolean variables in case they are still true after a state change
            enemyPawn.isInvestigating = false;
            enemyPawn.isSearching = false;
            enemyPawn.atWaypoint = false;
            ChangeState(EnemyData.AIState.Flee);
        }
    }
    public void TransitionSearch()
    {
        if (aiVision.CanSee(GameManager.instance.playerTank) == false)
        {
            // isSearching must be true to run Search method, set once during transition
            enemyPawn.isSearching = true;
            // Reset boolean variables in case they are still true after a state change
            enemyPawn.isInvestigating = false;
            enemyPawn.atWaypoint = false;
            enemyPawn.atSearchLocation = false;
            ChangeState(EnemyData.AIState.Search);
        }
    }
    public void TransitionInvestigate()
    {
        if (aiVision.CanSee(GameManager.instance.playerTank) == false && aiHearing.canHear == true)
        {
            // isInvestigating must be true to run Investigate method, set once during transition
            enemyPawn.isInvestigating = true;
            // Reset boolean variables in case they are still true after a state change
            enemyPawn.isSearching = false;
            enemyPawn.atWaypoint = false;
            enemyPawn.atInvestigateLocation = false;
            ChangeState(EnemyData.AIState.Investigate);
        }
    }
    public void TransitionPursue()
    {
        if (aiVision.CanSee(GameManager.instance.playerTank) == true)
        {
            if (aiVision.AttackRange(aiVision.targetDistance) == false)
            {
                // Reset boolean variables in case they are still true after a state change
                enemyPawn.isInvestigating = false;
                enemyPawn.isSearching = false;
                enemyPawn.atWaypoint = false;
                ChangeState(EnemyData.AIState.Pursue);
            }
        }
    }
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
        enemyData.aiState = newState;
        stateEnterTime = Time.time;
    }
}
