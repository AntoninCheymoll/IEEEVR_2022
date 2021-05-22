using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Events : MonoBehaviour
{
    float joystickPosThreshold = 0.85f;
    float joystickTimerThreshold = 0.1f;

    float timer = 0;

    private void Start()
    {

    }
    void Update()
    {
        timer += Time.deltaTime;
    }

    private bool pressControllerA() { return OVRInput.GetDown(OVRInput.RawButton.A); }
    private bool pressControllerB() { return OVRInput.GetDown(OVRInput.RawButton.B); }

    public bool selectLeft()
    {
        return (press("left") || OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x <= -joystickPosThreshold);
    }
    public bool selectRight()
    {
        return (press("right") || OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x >= joystickPosThreshold);
    }

    public bool press(string s) { return (Input.GetKeyDown(s)); }

    public bool selectParticipant()
    {
        return press("q") || pressControllerA();
    }

    public bool selectExperimenter()
    {
        return press("w");
    }

    public bool printResuts()
    {
        return press("p");
    }

    public bool selecDir(string dir)
    {
        if (dir.Equals("left") && selectLeft() || dir.Equals("right") && selectRight())
        {
            if (timer > joystickTimerThreshold)
            {
                timer = 0;
                return true;
            }
        }
        return false;
    }

}
