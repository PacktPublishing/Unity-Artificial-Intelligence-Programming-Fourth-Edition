using UnityEngine;
using System.Collections;

public class TestRelativeTransform : MonoBehaviour {
	
	public Transform cube1;
	
	// Use this for initialization
	void Start () {
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(cube1.position);
		Debug.Log(RelativeWaypointPosition);
		
		Vector3 diff = cube1.position - transform.position;
		Debug.Log(diff);
  
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
