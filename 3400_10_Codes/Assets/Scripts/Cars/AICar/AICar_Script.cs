using UnityEngine;
using System.Collections;

public class AICar_Script : MonoBehaviour 
{
    public Transform path;
    private Vector3 tarPos;
    private int curNode = 0;

    public Transform CenterOfMass;
    public WheelCollider FrontLeftWheel;
    public WheelCollider FrontRightWheel;
    public Transform[] WheelGraphics;

    public float[] GearRatio;
    public int CurrentGear = 4;

    public float EngineTorque = 600.0f;
    public float MaxEngineRPM = 3000.0f;
    public float MinEngineRPM = 1000.0f;

    private float EngineRPM = 0.0f;

    // Here's all the variables for the AI, the waypoints are determined in the "GetWaypoints" function.
    // the waypoint container is used to search for all the waypoints in the scene, and the current
    // waypoint is used to determine which waypoint in the array the car is aiming for.
    private ArrayList waypoints;
    private int currentWaypoint = 0;

    // input steer and input torque are the values substituted out for the player input. The 
    // "NavigateTowardsWaypoint" function determines values to use for these variables to move the car
    // in the desired direction.
    private float inputSteer = 0.0f;
    private float inputTorque = 0.0f;

    void Start ()
    {
	    // I usually alter the center of mass to make the car more stable. I'ts less likely to flip this way.
        //rigidbody.centerOfMass = CenterOfMass.position;
    	
	    // Call the function to determine the array of waypoints. This sets up the array of points by finding
	    // transform components inside of a source container.
        tarPos = path.GetChild(curNode).position;
    }

    void Update ()
    {
        //Rotate Wheels
        foreach (Transform wheel in WheelGraphics)
            wheel.Rotate(new Vector3(1000.0f * Time.deltaTime, 0.0f, 0.0f));

	    // This is to limith the maximum speed of the car, adjusting the drag probably isn't the best way of doing it,
	    // but it's easy, and it doesn't interfere with the physics processing.
	    GetComponent<Rigidbody>().drag = GetComponent<Rigidbody>().velocity.magnitude / 250;
    	
	    // Call the funtion to determine the desired input values for the car. This essentially steers and
	    // applies gas to the engine.
	    NavigateTowardsWaypoint();
    	
	    // Compute the engine RPM based on the average RPM of the two wheels, then call the shift gear function
        //EngineRPM = (FrontLeftWheel.rpm + FrontRightWheel.rpm)/2 * GearRatio[CurrentGear];
        EngineRPM = (FrontLeftWheel.rpm + FrontRightWheel.rpm)/2 * 10.0f;
        //ShiftGears();

	    // set the audio pitch to the percentage of RPM to the maximum RPM plus one, this makes the sound play
	    // up to twice it's pitch, where it will suddenly drop when it switches gears.
        //audio.pitch = Mathf.Abs(EngineRPM / MaxEngineRPM) + 1.0f ;

	    // this line is just to ensure that the pitch does not reach a value higher than is desired.
        //if ( audio.pitch > 2.0f ) 
        //{
        //    audio.pitch = 2.0f;
        //}
    	
	    // finally, apply the values to the wheels.	The torque applied is divided by the current gear, and
	    // multiplied by the calculated AI input variable.
	    FrontLeftWheel.motorTorque = EngineTorque / GearRatio[CurrentGear] * inputTorque;
	    FrontRightWheel.motorTorque = EngineTorque / GearRatio[CurrentGear] * inputTorque;
    		
	    // the steer angle is an arbitrary value multiplied by the calculated AI input.
	    FrontLeftWheel.steerAngle = 10.0f * inputSteer;
        FrontRightWheel.steerAngle = 10.0f * inputSteer;
    }

    void ShiftGears ()
    {
	    // this funciton shifts the gears of the vehcile, it loops through all the gears, checking which will make
	    // the engine RPM fall within the desired range. The gear is then set to this "appropriate" value.
        int AppropriateGear = 0;
	    if ( EngineRPM >= MaxEngineRPM ) 
        {
		    AppropriateGear = CurrentGear;
    		
		    for ( int i = 0; i < GearRatio.Length; i ++ ) 
            {
			    if ( FrontLeftWheel.rpm * GearRatio[i] < MaxEngineRPM ) 
                {
				    AppropriateGear = i;
				    break;
			    }
		    }
    		
		    CurrentGear = AppropriateGear;
	    }
    	
	    if ( EngineRPM <= MinEngineRPM ) 
        {
		    AppropriateGear = CurrentGear;
    		
		    for ( int j= GearRatio.Length-1; j >= 0; j -- ) 
            {
			    if ( FrontLeftWheel.rpm * GearRatio[j] > MinEngineRPM ) 
                {
				    AppropriateGear = j;
				    break;
			    }
		    }
    		
		    CurrentGear = AppropriateGear;
	    }
    }

    void NavigateTowardsWaypoint (){
	    // now we just find the relative position of the waypoint from the car transform,
	    // that way we can determine how far to the left and right the waypoint is.
	    Vector3 RelativeWaypointPosition = transform.InverseTransformPoint( new Vector3( 
												    path.GetChild(curNode).position.x, 
												    transform.position.y,
                                                    path.GetChild(curNode).position.z));
    																				
    																				
	    // by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
	    inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
    	
	    // now we do the same for torque, but make sure that it doesn't apply any engine torque when going around a sharp turn...
	    if ( Mathf.Abs( inputSteer ) < 0.5f ) {
		    inputTorque = RelativeWaypointPosition.z / RelativeWaypointPosition.magnitude - Mathf.Abs( inputSteer );
	    }else{
		    inputTorque = 0.0f;
	    }
    	
	    // this just checks if the car's position is near enough to a waypoint to count as passing it, if it is, then change the target waypoint to the
	    // next in the list.
	    if ( RelativeWaypointPosition.magnitude < 20 ) 
        {
            curNode++;
	    }
    	
    }
}