using UnityEngine;
using System.Collections;

public class CarCamera : MonoBehaviour
{
	public Transform target = null;
	public float height = 1f;
    private float curDistance = 4.0f;
	public float positionDamping = 3f;
	public float velocityDamping = 3f;
	public float distance = 4f;
	public LayerMask ignoreLayers = -1;

	private RaycastHit hit = new RaycastHit();

	private Vector3 prevVelocity = Vector3.zero;
	private LayerMask raycastLayers = -1;
	
	private Vector3 currentVelocity = Vector3.zero;
	
	void Start()
	{
		raycastLayers = ~ignoreLayers;
        curDistance = distance;
	}

	void FixedUpdate()
	{
		currentVelocity = Vector3.Lerp(prevVelocity, target.root.rigidbody.velocity, velocityDamping * Time.deltaTime);
		currentVelocity.y = 0;
		prevVelocity = currentVelocity;
	}
	
	void LateUpdate()
	{
		float speedFactor = Mathf.Clamp01(target.root.rigidbody.velocity.magnitude / 70.0f);
		camera.fieldOfView = Mathf.Lerp(55, 72, 1.0f * Time.deltaTime);
        curDistance = Mathf.Lerp(curDistance, distance, 1.0f * Time.deltaTime);
		
		currentVelocity = currentVelocity.normalized;
		
		Vector3 newTargetPosition = target.position + Vector3.up * height;
        Vector3 newPosition = newTargetPosition - (currentVelocity * curDistance);
		newPosition.y = newTargetPosition.y;

        //Vector3 targetDirection = newPosition - newTargetPosition;
        //if (Physics.Raycast(newTargetPosition, targetDirection, out hit, curDistance, raycastLayers))
        //    newPosition = hit.point;

        transform.position = newPosition;
		transform.LookAt(newTargetPosition);
	}
}
