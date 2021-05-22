using Leap;
using Leap.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RetargettingManagement : MonoBehaviour
{
    public class HandPoints
    {
        public List<List<AssociatePoint>> points;
    }

    public class AssociatePoint
    {

        public Transform invisible;
        public Transform display;

        public AssociatePoint(Transform i, Transform d)
        {
            invisible = i;
            display = d;
        }

        public Vector3 getVectDiff(int current_num)
        {
            Transform i = invisible;
            Transform d = display;


            foreach (Transform child in display)
            {
                if (child.name == "AltPoint")
                {
                    d = child;
                }
            }

            if(current_num != 0) foreach (Transform child in invisible)
            {
                if (child.name == "AltPoint")
                {
                    i = child;
                }
            }

            return d.position - i.position;
        }
    }

    public GameObject right_index_display;
    public GameObject right_index_invisible;

    ///////////////////

    public GameObject right_invisible_hand;
    public GameObject right_display_hand;

    public GameObject left_invisible_hand;
    public GameObject left_display_hand;
    public GameObject left_alternative_hand;

    ///////////////////

    GameObject right_invisible_hand_palm;
    GameObject right_display_hand_palm;

    GameObject left_invisible_hand_palm;
    GameObject left_display_hand_palm;

    ///////////////////

    GameObject plane;

    ///////////////////

    public int current_num = -1;
    int key;

    HandPoints handPoints = new HandPoints();

    public float withinFingerPow;
    public float betweenFingerPow;
    public float retargetBound;

    bool finger_offset_matching;
    bool first_mapping = false;

    private void Start()
    {
        key = UnityEngine.Random.Range(0, 1000);

        right_invisible_hand_palm = right_invisible_hand.transform.Find("R_Wrist").Find("R_Palm").gameObject;
        right_display_hand_palm = right_display_hand.transform.Find("R_Wrist").Find("R_Palm").gameObject;

        left_invisible_hand_palm = left_invisible_hand.transform.Find("L_Wrist").Find("L_Palm").gameObject;
        left_display_hand_palm = left_display_hand.transform.Find("L_Wrist").Find("L_Palm").gameObject;

        plane = left_display_hand_palm.transform.Find("Pave").gameObject;


    }
    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M))
        {
            UnityEditor.EditorWindow.focusedWindow.maximized = !UnityEditor.EditorWindow.focusedWindow.maximized;
        }
        #endif
        if (Input.GetKeyDown("c"))
        {
            update_finger_offset_matching(!finger_offset_matching);
        }

        // change hand type
        if (current_num != left_display_hand.GetComponent<RiggedHand>().handModel && left_invisible_hand.active && right_invisible_hand.active)
        {
            current_num = left_display_hand.GetComponent<RiggedHand>().handModel;
            searchHandPoints();
        }

        //check if we mapped at least once
        if (!first_mapping) return;


        //offset 
        (int, float)[] order = getFingerDistanceOrder();
        if (order.Length == 0) return;


        float totalDist = 0;
        foreach ((int, float) x in order) totalDist += x.Item2;

        float invertPower((int, float) x) { return Mathf.Pow(totalDist - x.Item2, betweenFingerPow); }

        float totalInvertedDist = 0;
        foreach ((int, float) x in order) totalInvertedDist += invertPower(x);

        Vector3 offset = Vector3.zero;

        foreach ((int, float) finger in order)
        {
            offset += getFingerOffset(handPoints.points[finger.Item1]) * (invertPower(finger)) / totalInvertedDist;
            // offset += getFingerOffset(handPoints.points[order[0].Item1]);
        }

        float offset_prop = Mathf.Max(0, (1 - order[0].Item2 / retargetBound));
        right_display_hand_palm.transform.position = right_invisible_hand_palm.transform.position + offset * offset_prop;

        //Vector3 offset = getFingerOffset(handPoints.points[order[0].Item1]);
        //right_display_hand_palm.transform.position = right_invisible_hand_palm.transform.position + offset;

        //AssociatePoint p = handPoints.points[order[0].Item1][3];
        //right_display_hand.transform.position = right_invisible_hand.transform.position + right_index.transform.position -  p.invisible.position;

        //right_display_hand_palm.transform.position = right_invisible_hand_palm.transform.position + (p.display.position - right_index_invisible.transform.position) * proportion;
        //Debug.Log(p.display.position + " " + right_index_display.transform.position);

    }

    public void update_finger_offset_matching(bool updated)
    {

        finger_offset_matching = updated;
        searchHandPoints();

    }

    private void searchHandPoints()
    {

        handPoints.points = new List<List<AssociatePoint>>();

        if (current_num == 1 || current_num == 0)
        {
            foreach (string finger in new string[] { "index", "middle", "ring", "pinky" })
            {
                List<AssociatePoint> list = new List<AssociatePoint>();

                foreach (string knuckle in new string[] { "a", "b", "c", "end" })
                {
                    string name = "L_" + finger + "_" + knuckle;
                    list.Add(new AssociatePoint(findTransform(left_invisible_hand.transform, name), findTransform(left_display_hand.transform, name)));
                }

                handPoints.points.Add(list);
            }
        }

        else if (current_num == 3)
        {
            foreach (string finger in new string[] { "index", "middle", "ring", "pinky" })
            {
                List<AssociatePoint> list = new List<AssociatePoint>();

                foreach (string knuckle in new string[] { "a", "b", "c", "end" })
                {
                    string name_invisible = "L_" + finger + "_" + knuckle;
                    string name_cube = "C_" + finger + "_" + knuckle;

                    list.Add(new AssociatePoint(findTransform(left_invisible_hand.transform, name_invisible), findTransform(left_display_hand.transform, name_cube)));
                }

                handPoints.points.Add(list);
            }
        }

        else if (current_num == 2 && finger_offset_matching)
        {
            /*foreach (string finger in new string[] { "index", "middle", "ring", "pinky" })
            {
                List<AssociatePoint> list = new List<AssociatePoint>();


                foreach (string knuckle in new string[] { "a", "b", "c", "end" })
                {
                    string name = "L_" + finger + "_" + knuckle;

                    list.Add(new AssociatePoint(findTransform(left_invisible_hand.transform, name), findTransform(left_display_hand.transform, name)));
                }

                handPoints.points.Add(list);
            }*/

            addKnucles(left_display_hand, "index", "index");
            addKnucles(left_display_hand, "middle", "middle");
            addKnucles(left_display_hand, "ring", "ring");
            addKnucles(left_alternative_hand, "ring", "pinky");

        }

        //else if (current_num == 2)
        else if (current_num == 2 && !finger_offset_matching)
        {
            /*Dictionary<string, string> correspondance = new Dictionary<string, string>();
            correspondance.Add("index", "a_middle");
            correspondance.Add("middle", "a_ring");
            correspondance.Add("ring", "a_pinky");
            correspondance.Add("pinky", "d_pinky");

            foreach (string finger in new string[] { "index", "middle", "ring", "pinky" })
            {
                List<AssociatePoint> list = new List<AssociatePoint>();

                foreach (string knuckle in new string[] { "a", "b", "c", "end" })
                {
                    string corr = correspondance[finger];
                    string invisible_name = "L_" + finger + "_" + knuckle;
                    string display_name = "L_" + corr.Split('_')[1] + "_" + knuckle;

                    AssociatePoint a = new AssociatePoint(findTransform(left_invisible_hand.transform, invisible_name), findTransform(((corr.Split('_')[0] == "a") ? left_alternative_hand : left_display_hand).transform, display_name));
                    list.Add(new AssociatePoint(findTransform(left_invisible_hand.transform, invisible_name), findTransform(((corr.Split('_')[0] =="a")?left_alternative_hand:left_display_hand).transform, display_name)));

                }

                handPoints.points.Add(list);
            }
            */

            addKnucles(left_display_hand, "middle", "index");
            addKnucles(left_display_hand, "ring", "middle");
            addKnucles(left_alternative_hand, "ring", "ring");
            addKnucles(left_display_hand, "pinky", "pinky");

            Debug.Log("b");
        }

        first_mapping = true;

    }

    //used for 6 fingers hands
    private void addKnucles(GameObject hand, string displayFingerName, string invisibleFingerName)
    {
        List<AssociatePoint> list = new List<AssociatePoint>();

        foreach (string knuckle in new string[] { "a", "b", "c", "end" })
        {
            string invisible_name = "L_" + invisibleFingerName + "_" + knuckle;
            string display_name = "L_" + displayFingerName + "_" + knuckle;

            list.Add(new AssociatePoint(findTransform(left_invisible_hand.transform, invisible_name), findTransform(hand.transform, display_name)));
        }

        handPoints.points.Add(list);

    }

    private Transform findTransform(Transform parent, string name)
    {

        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent.transform);

        while (queue.Count > 0)
        {
            Transform c = queue.Dequeue();
            if (c.gameObject.name.Equals(name)) return c;

            foreach (Transform t in c)
                queue.Enqueue(t);
        }

        return null;
    }

    (int, float)[] getFingerDistanceOrder()
    {

        (int, float)[] dists = new (int, float)[handPoints.points.Count];
        (int, float)[] ret = new (int, float)[handPoints.points.Count];


        for (int i = 0; i < handPoints.points.Count; i++)
        {
            dists[i] = (i, getFingerDistance(handPoints.points[i]));
        }

        for (int i = 0; i < dists.Length; i++)
        {
            int cpt = 0;
            float dist = dists[i].Item2;

            for (int j = 0; j < dists.Length; j++)
            {
                float dist2 = dists[j].Item2;
                if (i != j && dist > dist2) cpt++;
                if (i > j && dist == dist2) cpt++;
            }
            ret[cpt] = dists[i];
        }

        return ret;
    }

    float getFingerDistance(List<AssociatePoint> finger)
    {
        float dist = float.MaxValue;

        foreach (AssociatePoint ap in finger)
        {
            try
            {

                CapsuleCollider fingerCol = ap.invisible.gameObject.GetComponent<CapsuleCollider>();
                SphereCollider indexColl = right_index_invisible.GetComponent<SphereCollider>();

                dist = Mathf.Min(dist, fingerCol.bounds.SqrDistance(indexColl.transform.position) - 0.00003f);

            }
            catch (MissingComponentException) { }
        }

        return Math.Max(0, dist);
    }

    (int, float)[] getKnuckleDistanceOrder(List<AssociatePoint> knuckles)
    {
        (int, float)[] dists = new (int, float)[knuckles.Count];
        (int, float)[] ret = new (int, float)[knuckles.Count];


        for (int i = 0; i < knuckles.Count; i++)
        {
            SphereCollider fingerCol = knuckles[i].invisible.gameObject.GetComponent<SphereCollider>();
            dists[i] = (i, distToIndex(fingerCol));
        }

        for (int i = 0; i < dists.Length; i++)
        {
            int cpt = 0;
            float dist = dists[i].Item2;

            for (int j = 0; j < dists.Length; j++)
            {
                float dist2 = dists[j].Item2;
                if (i != j && dist > dist2) cpt++;
                if (i > j && dist == dist2) cpt++;
            }

            ret[cpt] = dists[i];
        }

        return ret;
    }

    Vector3 getFingerOffset(List<AssociatePoint> knuckles)
    {

        (int, float)[] dist_order = getKnuckleDistanceOrder(knuckles);
        float totalDist = 0;
        foreach ((int, float) x in dist_order) totalDist += x.Item2;

        float invertPower(AssociatePoint x) { return Mathf.Pow(totalDist - distToIndex(x.invisible.GetComponent<Collider>()), withinFingerPow); }

        float totalInvertedDist = 0;
        foreach (AssociatePoint x in knuckles) totalInvertedDist += invertPower(x);

        Vector3 offset = new Vector3(0, 0, 0);

        foreach (AssociatePoint ap in knuckles)
        {
            offset += ap.getVectDiff( current_num) * (invertPower(ap)) / totalInvertedDist;
        }
        //Debug.Log(offset);
        return offset;
    }

    float distToIndex(Collider c)
    {
        SphereCollider indexColl = right_index_invisible.GetComponent<SphereCollider>();
        return c.bounds.SqrDistance(indexColl.transform.position) - 0.00003f;
    }

}
