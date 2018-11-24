using UnityEngine;
using System.Collections;

public class WeaponMissile: MonoBehaviour 
{
    public GameObject Missile;
    public Transform SpawnPoint;
    private bool bShoot, bHasTarget;
    private Transform target;

	// Use this for initialization
	void Start () 
    {
        bShoot = false;
        bHasTarget = false;
	}

    public void Shoot()
    {
        //Check Whether target exist or not
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        //RayCast only to AI Car which layer number is 9
        int layerMask = 1 << 9;
        if (Physics.Raycast(ray, out hitInfo, 1000.0f, layerMask))
        {
            bHasTarget = true;
            target = hitInfo.transform;
        }
        else
        {
            bHasTarget = false;
        }

        bShoot = true;
        StartCoroutine("ShootMissiles");
    }
    public void StopShoot()
    {
        //Stop the shooting animation
        if (bShoot)
        {
            bShoot = false;
        }

        StopCoroutine("ShootMissiles");
    }

    private IEnumerator ShootMissiles()
    {
        SpawnMissile();
        yield return new WaitForSeconds(Random.Range(0.3f, 0.6f));
        StartCoroutine("ShootMissiles");
    }

    private void SpawnMissile()
    {
        //Create a new Missile
        GameObject objMissile = (GameObject)Instantiate(Missile, SpawnPoint.position, SpawnPoint.rotation);
        objMissile.GetComponent<Missile>().Initialise(bHasTarget, target);
    }
}
