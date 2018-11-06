using UnityEngine;
using System.Collections;

public class CollisionManager : MonoBehaviour {

	// Use this for initialization
	void Start () 
    {
        Physics.IgnoreLayerCollision(8, 10);
        Physics.IgnoreLayerCollision(9, 11);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
