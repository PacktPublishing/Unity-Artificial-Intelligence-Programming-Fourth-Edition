using UnityEngine;
using System.Collections;

public class WeaponGun : MonoBehaviour 
{
    public GameObject Bullet;
    public GameObject[] GunGraphics;
    public float ratePerSecond;
    private bool bShoot;

	// Use this for initialization
	void Start () 
    {
        bShoot = false;
	}

    public void Shoot()
    {
        bShoot = true;

        foreach (GameObject obj in GunGraphics)
        {
            obj.animation.CrossFade("GunShooting", 0.5f);
        }

        StartCoroutine("ShootBullets");
    }
    public void StopShoot()
    {
        //Stop the shooting animation
        if (bShoot)
        {
            bShoot = false;

            foreach (GameObject obj in GunGraphics)
            {
                obj.animation.Stop("GunShooting");
            }
        }

        StopCoroutine("ShootBullets");
    }

    private IEnumerator ShootBullets()
    {
        SpawnBullet();
        yield return new WaitForSeconds(1.0f / ratePerSecond);
        StartCoroutine("ShootBullets");
    }

    private void SpawnBullet()
    {
        int rndSpawnPoint = Random.Range(0, GunGraphics.Length);
        Vector3 SpawnPos = GunGraphics[rndSpawnPoint].transform.position;
        Quaternion SpawnRot = GunGraphics[rndSpawnPoint].transform.rotation;

        //Create a new Bullet
        GameObject objBullet = (GameObject)Instantiate(Bullet, SpawnPos, SpawnRot);
    }

	// Update is called once per frame
	void Update () 
    {
	
	}
}
