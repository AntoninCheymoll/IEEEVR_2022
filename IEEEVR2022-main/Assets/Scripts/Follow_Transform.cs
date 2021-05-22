using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow_Transform : MonoBehaviour
{
    public Transform trans;
    public float offsetUp = 0;
    public float offsetForward = 0;

    Color col;

    private void Start()
    {
        col = gameObject.GetComponent<Renderer>().material.color;

    }

    void Update()
    {
        if (trans.gameObject.active)
        {
            gameObject.GetComponent<Renderer>().material.color = col;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
        }

        if (trans != null) transform.position = trans.position + trans.up * offsetUp + trans.forward * offsetForward;
    }
}
