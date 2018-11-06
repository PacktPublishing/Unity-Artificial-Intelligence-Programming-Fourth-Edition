using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class is adapted and modified from the FSM implementation class available on UnifyCommunity website
/// The license for the code is Creative Commons Attribution Share Alike.
/// It's originally the port of C++ FSM implementation mentioned in Chapter01 of Game Programming Gems 1
/// You're free to use, modify and distribute the code in any projects including commercial ones.
/// Please read the link to know more about CCA license @http://creativecommons.org/licenses/by-sa/3.0/

/// This class represents the States in the Finite State System.
/// Each state has a Dictionary with pairs (transition-state) showing
/// which state the FSM should be if a transition is fired while this state
/// is the current state.
/// Reason method is used to determine which transition should be fired .
/// Act method has the code to perform the actions the NPC is supposed to do if it큦 on this state.
/// </summary>
public abstract class FSMState
{
    protected Dictionary<Transition, FSMStateID> map = new Dictionary<Transition, FSMStateID>();
    protected FSMStateID stateID;
    public FSMStateID ID { get { return stateID; } }
    protected Vector3 destPos;
    protected Transform[] waypoints;
    protected float curRotSpeed;
    protected float curSpeed;

    protected bool bStateIntialised;
    protected Transform npcTransform;
    protected bool bShouldAvoid = false;
    protected float elapsedTime, pathElapsedTime; 
    protected float force = 100.0f;
    protected float minimumDistToAvoid = 20.0f;
    protected ArrayList path;

    public void AddTransition(Transition transition, FSMStateID id)
    {
        // Check if anyone of the args is invallid
        if (transition == Transition.None || id == FSMStateID.None)
        {
            Debug.LogWarning("FSMState : Null transition not allowed");
            return;
        }

        //Since this is a Deterministc FSM,
        //Check if the current transition was already inside the map
        if (map.ContainsKey(transition))
        {
            Debug.LogWarning("FSMState ERROR: transition is already inside the map");
            return;
        }

        map.Add(transition, id);
        //Debug.Log("Added : " + transition + " with ID : " + id);
    }

    /// <summary>
    /// This method deletes a pair transition-state from this state큦 map.
    /// If the transition was not inside the state큦 map, an ERROR message is printed.
    /// </summary>
    public void DeleteTransition(Transition trans)
    {
        // Check for NullTransition
        if (trans == Transition.None)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return;
        }

        // Check if the pair is inside the map before deleting
        if (map.ContainsKey(trans))
        {
            map.Remove(trans);
            return;
        }
        Debug.LogError("FSMState ERROR: Transition passed was not on this State큦 List");
    }


    /// <summary>
    /// This method returns the new state the FSM should be if
    ///    this state receives a transition  
    /// </summary>
    public FSMStateID GetOutputState(Transition trans)
    {
        // Check for NullTransition
        if (trans == Transition.None)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return FSMStateID.None;
        }

        // Check if the map has this transition
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }

        Debug.LogError("FSMState ERROR: " + trans+ " Transition passed to the State was not on the list");
        return FSMStateID.None;
    }

    /// <summary>
    /// Decides if the state should transition to another on its list
    /// NPC is a reference to the npc tha is controlled by this class
    /// </summary>
    public abstract void Reason(Transform player, Transform npc);

    /// <summary>
    /// This method controls the behavior of the NPC in the game World.
    /// Every action, movement or communication the NPC does should be placed here
    /// NPC is a reference to the npc tha is controlled by this class
    /// </summary>
    public abstract void Act(Transform player, Transform npc);

    /// <summary>
    /// Find the next semi-random patrol point
    /// </summary>
    public void FindNextPoint()
    {
        //Debug.Log("Finding next point");
        int rndIndex = Random.Range(0, waypoints.Length);
        Vector3 rndPosition = Vector3.zero;
        destPos = waypoints[rndIndex].position + rndPosition;
    }

    //Calculate the new directional vector to avoid the obstacle
    //Deprecated in this example
    protected void AvoidObstacles(ref Vector3 dir, ref bool bShouldAvoid)
    {
        RaycastHit hit;

        //Only detect layer 12 (Obstacles)
        int layerMask = 1 << 12;
        bShouldAvoid = false;

        //Check that the vehicle hit with the obstacles within it's minimum distance to avoid
        if (Physics.Raycast(npcTransform.position, npcTransform.forward, out hit, minimumDistToAvoid, layerMask))
        {
            //Get the normal of the hit point to calculate the new direction
            Vector3 hitNormal = hit.normal;
            hitNormal.y = 0.0f; //Don't want to move in Y-Space

            //Get the new directional vector by adding force to vehicle's current forward vector
            dir = npcTransform.forward + hitNormal * force;

            bShouldAvoid = true;
        }
    }

    //Debug Draw Path
    protected void OnDrawGizmos()
    {
        if (path == null)
        {
            Debug.Log("No Path");
            return;
        }

        if (path.Count > 0)
        {
            int index = 1;
            foreach (Node node in path)
            {
                if (index < path.Count)
                {
                    Node nextNode = (Node)path[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.green);
                    index++;
                }
            };
        }
    }
}
