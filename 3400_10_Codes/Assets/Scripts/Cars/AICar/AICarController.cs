using UnityEngine;
using System.Collections;

public class AICarController : AdvancedFSM
{
    protected override void Initialise()
    {
        //Start Doing the Finite State Machine
        ConstructFSM();

        //Get the target enemy(Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        if (!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");
    }

    protected override void CarUpdate()
    {
        //If handbrake section is active then de-activate in some amount of time
        if (handbrake)
        {
            Invoke("StopHandbrake", 1.5f);
        }
    }

    void StopHandbrake()
    {
        handbrake = false;
        StartCoroutine(StopHandbraking(Mathf.Min(5, Time.time - handbrakeTime)));
    }

    protected override void CarFixedUpdate()
    {
        CurrentState.Reason(playerTransform, transform);
        CurrentState.Act(playerTransform, transform);
    }

    //Set the transition for FSM
    public void SetTransition(Transition t)
    {
        PerformTransition(t);
    }

    //Construct the Finite State Machine for the AI Car behavior
    private void ConstructFSM()
    {
        //Get the list of points
        pointList = GameObject.FindGameObjectsWithTag("WandarPoints");

        Transform[] waypoints = new Transform[pointList.Length];
        int i = 0;
        foreach (GameObject obj in pointList)
        {
            waypoints[i] = obj.transform;
            i++;
        }

        PatrolState patrol = new PatrolState(waypoints);
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        ChaseState chase = new ChaseState(waypoints);
        chase.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        chase.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        chase.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        AttackState attack = new AttackState(waypoints);
        attack.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        attack.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        DeadState dead = new DeadState();
        dead.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        AddFSMState(patrol);
        AddFSMState(chase);
        AddFSMState(attack);
        AddFSMState(dead);
    }

    //Hit with Missile or Bullet
    void OnCollisionEnter(Collision collision)
    {
        if (bDead)
            return;

        if (collision.gameObject.tag == "Bullet")
        {
            print("AICar Hit with Bullet");
            health -= 30;
        }
        else if (collision.gameObject.tag == "Missile")
        {
            print("AICar Hit with Missile");
            health -= 50;
        }

        if (health <= 0)
        {
            bDead = true;
            Explode();
            Destroy(gameObject, 4.0f);
        }
    }
}
