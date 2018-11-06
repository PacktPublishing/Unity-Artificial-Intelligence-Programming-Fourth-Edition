using UnityEngine;
using System.Collections;

public class PatrolState : FSMState
{
    private int curPathIndex = 0;

    public PatrolState(Transform[] wp) 
    { 
        waypoints = wp;
        stateID = FSMStateID.Patrolling;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
        elapsedTime = 0.0f;
        pathElapsedTime = 50.0f; //Interval for path finding
        path = new ArrayList();
    }

    public override void Reason(Transform player, Transform npc)
    {
        if (!bStateIntialised)
        {
            npcTransform = npc;
            bStateIntialised = true;

            //Find a new Destination position if not defined yet
            if (destPos == Vector3.zero)
            {
                FindNextPoint();
                Debug.Log(destPos);
            }

            //Find the path for the first time
            path = AStarManager.instance.FindPath(npc.position, destPos);

            Debug.Log("Finding Path");
        } 

        //return;

        //Check the distance with player Car
        //When the distance is near, transition to chase state
        if (Vector3.Distance(npc.position, player.position) <= 100.0f)
        {
            Debug.Log("Switch to Chase State");
            npc.GetComponent<AICarController>().SetTransition(Transition.SawPlayer);
            npc.GetComponent<AICarController>().throttle = 0.0f;
            npc.GetComponent<AICarController>().DoHandbrake();
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        //Find another random patrol point if the current point is reached		
        if (Vector3.Distance(npc.position, destPos) <= 5.0f)
        {
            //Debug.Log("Reached to the destination point\ncalculating the next point");
            FindNextPoint();
            curPathIndex = 0;

            //Brake it first before moving to the next point
            npc.GetComponent<AICarController>().DoHandbrake();
        }

        #region Using RayCasting
        //Avoid Obstacles
        //Directional vector to the target position
        Vector3 dir = (destPos - npc.position);
        dir.Normalize();

        //Apply obstacle avoidance
        if(!bShouldAvoid)
            AvoidObstacles(ref dir, ref bShouldAvoid);

        //Now you should avoid the obstacles
        if (bShouldAvoid)
        {
            elapsedTime += Time.deltaTime;

            //Backwards the vehicle for a while
            if (elapsedTime < 2.0f)
            {
                //Debug.Log("Backwarding the car");
                npc.GetComponent<AICarController>().throttle = -0.5f;
            }
            //Then follow the new direction for a while
            else if (elapsedTime > 2.0f && elapsedTime <= 4.0f)
            {
                npc.GetComponent<AICarController>().throttle = 1.0f;

                Debug.DrawLine(npc.position, 10.0f * dir, Color.green);
                //Debug.DrawRay(npc.position, dir, Color.green);
                //Debug.Log("Avoiding Obstacles");

                //var rot = Quaternion.LookRotation(dir);
                //npc.rotation = Quaternion.Slerp(npc.rotation, rot, 10.0f * Time.deltaTime);
                //Vector3 RelativeWaypointPosition = npc.InverseTransformPoint(new Vector3(destPos.x, npc.position.y, destPos.z));
                //npc.GetComponent<AICarController>().steer = -1.0f * (RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude);
            }
            else
            {
                //Debug.Log("Done Avoiding Obstacles");
                elapsedTime = 0.0f;
                bShouldAvoid = false;
            }
        }
        else
        {
            npc.GetComponent<AICarController>().throttle = 1.0f;

            // Find the relative position of the waypoint from the car transform so that way we can determine how far to the left and right the waypoint is.
            Vector3 RelativeWaypointPosition = npc.InverseTransformPoint(new Vector3(destPos.x, npc.position.y, destPos.z));

            // by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
            npc.GetComponent<AICarController>().steer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
        }

        Debug.DrawLine(npc.position, destPos, Color.red);

        #endregion

        //Check whether the car can go straight to the dest points or using AStar
        //if (Physics.Raycast(npc.position, (destPos - npc.position)))
        //    GoStraightToDest();
        //else
            FollowPath();

        OnDrawGizmos();
    }

    //Now go directly to the dest point
    private void GoStraightToDest()
    {
        npcTransform.GetComponent<AICarController>().throttle = 1.0f;
        Vector3 RelativeWaypointPosition = npcTransform.InverseTransformPoint(new Vector3(destPos.x, npcTransform.position.y, destPos.z));
        npcTransform.GetComponent<AICarController>().steer = 1.0f * (RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude);
        Debug.DrawLine(npcTransform.position, destPos, Color.magenta);
    }

    //Follow the path using AStar path finding method
    private void FollowPath()
    {
        #region Using AStar
        pathElapsedTime += Time.deltaTime;
        if (pathElapsedTime >= 50.0f)
        {
            pathElapsedTime = 0.0f;
            curPathIndex = 0;
            //Debug.Log("Nodes in path : " + path.Count);
        }

        //Move the vehicle
        if (path.Count > 0 && curPathIndex < path.Count)
        {
            //Go to the current node 
            Node nextNode = (Node)path[curPathIndex];
            Vector3 nextPos = nextNode.position;

            npcTransform.GetComponent<AICarController>().throttle = 1.0f;
            Vector3 RelativeWaypointPosition = npcTransform.InverseTransformPoint(new Vector3(nextPos.x, npcTransform.position.y, nextPos.z));
            npcTransform.GetComponent<AICarController>().steer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;

            //Go to the next node if the current node is reached
            if (Vector3.Distance(npcTransform.position, nextPos) <= 5.0f)
            {
                //Debug.Log("Node Reached");
                curPathIndex++;
            }

            //Debug Drawing
            Debug.DrawLine(npcTransform.position, nextPos, Color.red);
            Debug.DrawLine(npcTransform.position, destPos, Color.magenta);
        }
        #endregion
    }
}