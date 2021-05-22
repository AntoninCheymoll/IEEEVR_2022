using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProprioceptiveDriftManager : MonoBehaviour
{

    public float leftX;
    public float rightX;
    public float speed;

    bool isRunning = false;
    bool isGoingRight = false;

    public GameObject panel;
    static GameObject tip;

    Events events = new Events();


    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
    }

    static public void initiate(ExperimentManager exp)
    {
        tip = exp.leftIndexInvisible;
    }


    void Update()
    {

        if (events.selectRight())
        {

            setPanelXPos(panel.transform.position.x - speed);
            Debug.Log(tip.transform.position.x - panel.transform.position.x);
        }

        if (events.selectLeft())
        {
            setPanelXPos(panel.transform.position.x + speed);
            Debug.Log(tip.transform.position.x - panel.transform.position.x);

        }
        /*
        if (isRunning)
        {
            if (isGoingRight)
            {
                setPanelZPos(panel.transform.position.x + speed);
                if (panel.transform.position.x > rightX) isGoingRight = false;
            }
            else
            {
                setPanelZPos(panel.transform.position.x - speed);
                if (panel.transform.position.x < leftX) isGoingRight = true;
            }
        }*/
    }

    // Update is called once per frame
    public void startTest()
    {
        panel.SetActive(true);
        setPanelXPos(leftX);
        isRunning = true;
    }

    public float endTest()
    {
        panel.SetActive(false);
        return tip.transform.position.x - panel.transform.position.x;
    }


    void setPanelXPos(float x)
    {
        panel.transform.position = new Vector3(x, panel.transform.position.y, panel.transform.position.z);
    }


}
