using UnityEngine;
using System.Collections;

public class AttackState : FSMState
{
    private AIWeaponController weapon;
    private bool bAIWeaponInitialised;
    private bool bStartShooting;
    private Transform npcTransform;

    public AttackState(Transform[] wp) 
    { 
        waypoints = wp;
        stateID = FSMStateID.Attacking;
        curRotSpeed = 5.0f;
        curSpeed = 100.0f;
        bAIWeaponInitialised = false;
        bStartShooting = false;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //Check the distance with the player car
        float dist = Vector3.Distance(npc.position, player.position);
        if (dist >= 50.0f && dist < 100.0f)
        {
            Debug.Log("Switch to Chase State");
            npc.GetComponent<AICarController>().SetTransition(Transition.SawPlayer);
            StopShooting();
        }
        //Transition to patrol is the tank become too far
        else if (dist >= 100.0f)
        {
            Debug.Log("Switch to Patrol State");
            npc.GetComponent<AICarController>().SetTransition(Transition.LostPlayer);
            StopShooting();
        }  
    }

    public override void Act(Transform player, Transform npc)
    {
        //Set the target position as the player position
        destPos = player.position + new Vector3(0.0f, 1.0f, 0.0f);

        npc.GetComponent<AICarController>().throttle = 1.0f;

        //Calculate the steering value
        Vector3 RelativeWaypointPosition = npc.InverseTransformPoint(new Vector3(destPos.x, npc.position.y, destPos.z));
        npc.GetComponent<AICarController>().steer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;

        
        //Initialise the weapon controller for attack state
        if (!bAIWeaponInitialised)
        {
            weapon = npc.GetComponent<AIWeaponController>();
            npcTransform = npc;
            bAIWeaponInitialised = true;
        }

        Transform turret = weapon.Turret;
        Quaternion turretRotation = Quaternion.LookRotation(destPos - turret.position);
        turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);

        //Shoot shouldn't call every frame
        if (!bStartShooting)
        {
            //Shoot bullet/Missiles towards the player
            ShootShells();
            bStartShooting = true;
        }
    }

    //Probability : Shoot gun most of the time and only missile is shot 20% of the time
    private void ShootShells()
    {
        npcTransform.GetComponent<AIWeaponController>().ShootGun();
    }

    private void StopShooting()
    {
        npcTransform.GetComponent<AIWeaponController>().StopShootGun();
        npcTransform.GetComponent<AIWeaponController>().StopShootMissile();
    }

}
