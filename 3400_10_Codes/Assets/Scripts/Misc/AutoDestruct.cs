using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour {

    public float DestructionTime = 1.0f;
	// Use this for initialization
	void Start () 
    {
        Destroy(gameObject, DestructionTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
