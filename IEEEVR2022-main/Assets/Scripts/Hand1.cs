using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity
{

    public class Hand1 : MonoBehaviour
    {
        RiggedHand hand;


    void Start()
        {
            hand = gameObject.GetComponent<RiggedHand>();
        }

        // Update is called once per frame
        void Update()
        {
            foreach(RiggedFinger finger in hand.fingers)
            {
                Transform[] t = finger.bones;

                t[0].localScale = new Vector3(2, 1, 1);
                t[1].localScale = new Vector3 (1/t[1].lossyScale.x, 1/t[1].lossyScale.y, 1 / t[1].lossyScale.z);
                
            }
        }
    }

}