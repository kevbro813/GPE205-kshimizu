using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : AIController
{
    public EnemyPawn enemyPawn;
    public EnemyData enemyData;
    public AIState aiState;
    private float stateEnterTime;
    public enum AIState { Idle, Patrol, Attack, Search, Pursue, Flee, Investigate }

    // Start is called before the first frame update
    void Start()
    {
        enemyPawn = GetComponent<EnemyPawn>();
        enemyData = GetComponent<EnemyData>();
        aiVision = GetComponentInChildren<AIVision>();
        aiHearing = GetComponentInChildren<AIHearing>();
        sensoryRange = GetComponentInChildren<SensoryRange>();

    }

    // Update is called once per frame
    void Update()
    {
        if (aiState == AIState.Idle)
        {
            DoIdle();
            ChangeState(AIState.Patrol); // TEST PURPOSES ONLY - SO TANKS AREN'T STUCK IN IDLE
        }
        if (aiState == AIState.Patrol)
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
        if (aiState == AIState.Attack)
        {
            DoAttack();

            // If the player is seen but not in attack range, transition to pursue
            TransitionPursue();

            // If the player is not seen, transition to search (go to last known player location)
            TransitionSearch();
        }
        if (aiState == AIState.Search)
        {
            DoSearch();
            // TODO: wrap the patrol transition in an if statement to run once the tank has reached the last known player location after a fixed amount of time
            // If the player is not seen or heard, transition to patrol
            TransitionPatrol();

            // If the player is seen while searching but out of firing range, transition to pursue
            TransitionPursue();

            // If the player is heard while searching, transition to investigate (go to sound origin)
            TransitionInvestigate();

            // If the player is seen and in firing range, transition to attack
            TransitionAttack();
        }
        if (aiState == AIState.Pursue)
        {
            DoPursue();

            // If the player is seen and in firing range, transition to attack
            TransitionAttack();

            // If the player is not seen, transition to search (go to last known player location)
            TransitionSearch();
        }
        if (aiState == AIState.Investigate)
        {
            DoInvestigate();
            // TODO: wrap the patrol transition in an if statement to run once the tank has reached the sound origin location after a fixed amount of time
            // If nothing is found return to "patrol"
            TransitionPatrol();

            // If the player is seen "pursue"
            TransitionPursue();

            // If the player is seen and in firing range, transition to attack
            TransitionAttack();
        }
        if (aiState == AIState.Flee)
        {
            DoFlee();
        }
    }
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
    public void TransitionPatrol()
    {
        if (aiVision.CanSee(GameManager.instance.playerTank) == false && aiHearing.canHear == false) //&& enemyPawn.isInvestigating == false && enemyPawn.isSearching == false)
        {
            ChangeState(AIState.Patrol);
        }
    }
    public void TransitionAttack()
    {
        if (aiVision.CanSee(GameManager.instance.playerTank) == true)
        {
            if (aiVision.AttackRange(aiVision.targetDistance) == true)
            {
                ChangeState(AIState.Attack);
            }
        }
    }
    public void TransitionFlee()
    {
        // Flee if low on health
        if (enemyData.tankHealth <= enemyData.criticalHealth)
        {
            ChangeState(AIState.Flee);
        }
    }
    public void TransitionSearch()
    {
        if (aiVision.CanSee(GameManager.instance.playerTank) == false) //&& enemyPawn.isSearching == true)
        {
            ChangeState(AIState.Search);
        }
    }
    public void TransitionInvestigate()
    {
        if (aiHearing.canHear == true && aiVision.CanSee(GameManager.instance.playerTank) == false) //&& enemyPawn.isInvestigating == true)
        {
            ChangeState(AIState.Investigate);
        }
    }
    public void TransitionPursue()
    {
        if (aiVision.CanSee(GameManager.instance.playerTank) == true)
        {
            if (aiVision.AttackRange(aiVision.targetDistance) == false)
            {
                ChangeState(AIState.Pursue);
            }
        }
    }
    public void TransitionIdle()
    {
        // Do nothing for now
    }
    public void ChangeState (AIState newState)
    {
        aiState = newState;
        stateEnterTime = Time.time;
    }
}
