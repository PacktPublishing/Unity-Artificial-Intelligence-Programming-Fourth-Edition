using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
    public GameObject particleHit;
    public float speed = 100.0f;

	// Use this for initialization
	void Start () 
    {
        Destroy(gameObject, 2.0f);
	}
	
	// Update is called once per frame
	void Update () {
	    transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
	}

    void OnCollisionEnter(Collision collision)
    {
        Vector3 contactPoint = collision.contacts[0].point;

        Instantiate(particleHit, contactPoint, Quaternion.identity);
        Destroy(gameObject);
    }
}
