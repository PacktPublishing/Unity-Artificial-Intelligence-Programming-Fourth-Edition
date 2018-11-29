using UnityEngine;
using System.Collections;

public class FSM : MonoBehaviour {
	
	public enum FSMState
    {
        Chase,
        Flee
    }
	
	public int chaseProbabiilty = 80;
	public int fleeProbabiilty = 20;
	
	public ArrayList statesPoll = new ArrayList();
	
	// Use this for initialization
	void Start () {
		//fill the array
		for (int i = 0; i < chaseProbabiilty; i++) {
			statesPoll.Add(FSMState.Chase);		
		}
		
		for (int i = 0; i < fleeProbabiilty; i++) {
			statesPoll.Add(FSMState.Flee);
		}
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int randomState = Random.Range(0, statesPoll.Count);
            Debug.Log(statesPoll[randomState].ToString());
        }
    }
}
