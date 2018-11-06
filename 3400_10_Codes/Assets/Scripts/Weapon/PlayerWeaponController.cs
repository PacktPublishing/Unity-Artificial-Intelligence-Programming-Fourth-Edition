using UnityEngine;
using System.Collections;

public class PlayerWeaponController : MonoBehaviour 
{
    public WeaponGun gun;
    public WeaponMissile[] missile; //Left and Right missile pod
    public Transform Turret;
    //private Transform Recticle; //The Recticle object, the mouse cursor graphic

	// Use this for initialization
	void Start () 
    {
        //if (!Recticle)
        //    Recticle = GameObject.Find("Recticle_Player").transform;
	}
	
	// Update is called once per frame
	void Update () 
    {
        //Shoot laser from the turret
        if (Input.GetMouseButtonDown(0))
        {
            gun.Shoot();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            gun.StopShoot();
        }

        //Shoot missile from the turret
        if (Input.GetMouseButtonDown(1))
        {
            missile[1].Shoot();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            missile[1].StopShoot();
        }

        //Rotate the turret
        //AIMING WITH THE MOUSE
        // Generate a plane that intersects the transform's position with an upwards normal.
        Plane playerPlane = new Plane(Vector3.up, transform.position);

        // Generate a ray from the cursor position
        Ray RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects the plane.
        float HitDist = 0;

        if (playerPlane.Raycast(RayCast, out HitDist))
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 TargetPoint = RayCast.GetPoint(HitDist);

            //Recticle.position = TargetPoint; //Set the position of the Recticle to be the same as the position of the mouse on the created plane
            Turret.LookAt(TargetPoint);

            //Limit the upper body's rotation so it doesn't look straight down or up
            //Vector3 eulerAngles = Turret.eulerAngles;
            //eulerAngles.x = 0.0f;
            //eulerAngles.z = 0.0f;
            //Turret.eulerAngles = eulerAngles;
        }
	}
}
