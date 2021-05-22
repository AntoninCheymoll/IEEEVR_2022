using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateHandsPos : MonoBehaviour
{
    public GameObject leftHand;
    public GameObject rightHand;

    public GameObject leftAnchor;
    public GameObject rightAnchor;

    public Vector3 offset = new Vector3(0.2f, 0, 0f);
    [HideInInspector]
    bool offsetActivated = true;

    // Update is called once per frame
    void Update()
    {
        /*
        leftHand.transform.position = leftAnchor.transform.position;
        if (offsetActivated) leftHand.transform.position += offset;
        

        foreach (Transform hand in rightHands.transform)
        {
            hand.position = rightAnchor.transform.position;
            if(hand.tag == "index") hand.rotation = rightAnchor.transform.rotation;
            if (offsetActivated) hand.transform.position += offset;
        }*/
    }
}
