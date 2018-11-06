using UnityEngine;
using System.Collections;

public class DynamicObstacle : MonoBehaviour 
{
    private int health = 100;
    private float radius = 40.0f;
    private float power = 3000.0f;

	// Use this for initialization
	void Start () 
    {
        health = 100;
	}

    //Hit with Missile or Bullet
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            print("Obstacle Hit with Bullet");
            health -= 30;
        }
        else if (collision.gameObject.tag == "Missile")
        {
            print("Obstacle Hit with Missile");
            health -= 50;
        }

        if (health <= 0)
        {
            Explode();
            Destroy(gameObject, 2.0f);
        }
    }

    //Obstacle explosions
    private void Explode()
    {
        print("Obstacle Explosion");
        float rndX = Random.Range(10.0f, 20.0f);
        float rndZ = Random.Range(10.0f, 20.0f);
        rigidbody.AddExplosionForce(power, transform.position - new Vector3(rndX, 10.0f, rndZ), radius, 20.0F);
        rigidbody.velocity = transform.TransformDirection(new Vector3(0.0f, 10.0f, 0.0f));
    }
}
