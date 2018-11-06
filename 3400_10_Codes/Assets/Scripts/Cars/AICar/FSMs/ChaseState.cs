using UnityEngine;
using System.Collections;

public class ChaseState : FSMState
{
    public ChaseState(Transform[] wp) 
    { 
        waypoints = wp;
        stateID = FSMStateID.Chasing;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;

        //find next Waypoint position
        FindNextPoint();
    }

    //Check the new reason to change state
    public override void Reason(Transform player, Transform npc)
    {
        //Set the target position as the player position
        destPos = player.position;

        //Check the distance with player tank
        //When the distance is near, transition to attack state
        float dist = Vector3.Distance(npc.position, destPos);
        //Debug.Log(dist);
        if (dist <= 60.0f)
        {
            Debug.Log("Switch to Attack state");
            npc.GetComponent<AICarController>().SetTransition(Transition.ReachPlayer);
        }

        //Go back to patrol is it become too far
        if (dist >= 110.0f)
        {
            Debug.Log("Switch to Patrol state");
            npc.GetComponent<AICarController>().SetTransition(Transition.LostPlayer);
        }
    }

    //Action taken in the current state
    public override void Act(Transform player, Transform npc)
    {
        ////Rotate to the target point
        destPos = player.position;

        npc.GetComponent<AICarController>().throttle = 1.0f;

        // now we just find the relative position of the waypoint from the car transform,
        // that way we can determine how far to the left and right the waypoint is.
        Vector3 RelativeWaypointPosition = npc.InverseTransformPoint(new Vector3(destPos.x, npc.position.y, destPos.z));

        // by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
        npc.GetComponent<AICarController>().steer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
    }
}
