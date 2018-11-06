using UnityEngine;
using System.Collections;

public class CrashController : MonoBehaviour 
{
	public SoundController sound;	
	private Car car;
	
	void Start()
	{
		sound = transform.root.GetComponent<SoundController>();
		car = transform.GetComponent<Car>();
	}
	
	void OnCollisionEnter ( Collision collInfo  )
	{
		if(enabled && collInfo.contacts.Length > 0)
		{
			float volumeFactor = Mathf.Clamp01(collInfo.relativeVelocity.magnitude * 0.08f);
			volumeFactor *= Mathf.Clamp01(0.3f + Mathf.Abs(Vector3.Dot(collInfo.relativeVelocity.normalized, collInfo.contacts[0].normal)));
			volumeFactor = volumeFactor * 0.5f + 0.5f;
			sound.Crash(volumeFactor);
		}
	}
}