using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour 
{
    public GameObject Particle_Hit;
    public float speed = 20.0f;
    private Transform target;

	// Use this for initialization
	void Start () 
    {
	}

    public void Initialise(bool bHasTarget, Transform target = null)
    {
        if (bHasTarget)
        {
            this.target = target;
            Destroy(gameObject, 4.0f);
        }
        else
        {
            Destroy(gameObject, 2.0f);
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (target != null)
        {
            //Make the target position upwards alittle bit
            Vector3 newTarPos = target.position + new Vector3(0.0f, 1.0f, 0.0f);

            //Rotate towards the target
            Vector3 tarDir = newTarPos - transform.position;
            Quaternion tarRot = Quaternion.LookRotation(tarDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, 3.0f * Time.deltaTime);
        }

        transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
	}

    void OnCollisionEnter(Collision collision)
    {
        Vector3 contactPoint = collision.contacts[0].point;

        Instantiate(Particle_Hit, contactPoint, Quaternion.identity);
        Destroy(gameObject);
    }
}
