using UnityEngine;
using System.Collections;

public class AIWeaponController : MonoBehaviour 
{
    public WeaponGun gun;
    public WeaponMissile[] missile; //Left and Right missile pod
    public Transform Turret;

	// Use this for initialization
	void Start () 
    {
	}

    public void ShootGun()
    {
        gun.Shoot();
    }

    public void ShootMissile()
    {
        missile[1].Shoot();
    }

    public void StopShootGun()
    {
        gun.StopShoot();
    }

    public void StopShootMissile()
    {
        missile[1].StopShoot();
    }

}
