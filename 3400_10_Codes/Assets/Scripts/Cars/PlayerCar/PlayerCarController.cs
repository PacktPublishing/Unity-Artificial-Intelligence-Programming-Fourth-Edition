using UnityEngine;
using System.Collections;

public class PlayerCarController : Car 
{
    protected override void Initialise() 
    {
 
    }

    protected override void CarUpdate() 
    {
        GetInput();
    }

    void GetInput()
    {
        #if UNITY_IPHONE || UNITY_ANDROID
            // Read the steering from accelerometers
            throttle = 1.0f;
            steer = Mathf.Clamp(-Input.acceleration.y, -1, 1);
        #else
            throttle = Input.GetAxis("Vertical");
            steer = Input.GetAxis("Horizontal");
        #endif

        //if(throttle < 0.0f)
        //    brakeLights.SetFloat("_Intensity", Mathf.Abs(throttle));
        //else
        //    brakeLights.SetFloat("_Intensity", 0.0f);

        //Make a handbrake when pressing "Space"
        if (Input.GetKey("space"))
        {
            DoHandbrake();
        }
        else if (handbrake)
        {
            handbrake = false;
            StartCoroutine(StopHandbraking(Mathf.Min(5, Time.time - handbrakeTime)));
        }
    }
}
