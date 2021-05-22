
using Leap;
using Leap.Unity;
using Leap.Unity.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class HandBehavior : MonoBehaviour
{
    public enum FingerName { index, middle, pinky, ring }
    public class Hand
    {

        public List<Transform> handPoints = new List<Transform>();
        public List<Transform> planePoints = new List<Transform>();
        public List<Transform> alternativeHandPoints = new List<Transform>();

        List<GameObject> spheres = new List<GameObject>();
        public PathManager.Path path;
        public int firstPoint = 0;

        public void displayPath(int transformation)
        {
            for (int i = firstPoint; i < path.pointList.Length; i++)
            {
                string point = path.pointList[i];
                Transform t = findPoint(point, transformation);
                displayPoint(t, i, transformation);
            }
        }

        public void emptyPath()
        {
            foreach (GameObject sphere in spheres) Destroy(sphere);
        }

        void displayPoint(Transform t, int pointNum, int transformation)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            if (transformation == 3)
            {
                sphere.AddComponent<Follow_Transform>();
                sphere.GetComponent<Follow_Transform>().trans = t;
                sphere.GetComponent<Follow_Transform>().offsetUp = 0.01f;
            }
            else
            {
                sphere.transform.position = t.position;
                sphere.transform.parent = t;
                if (t.gameObject.name.Split('_')[2].Equals("a")) sphere.transform.localPosition += new Vector3(0, 0.015f, 0);
                else sphere.transform.localPosition += new Vector3(0, 0.01f, 0);

                
                if(transformation == 2 && t.name == "L_hand_l")
                {
                    sphere.transform.localPosition += new Vector3(0, 0, 0.01f);
                }

            }


            float transVal;
            int distToFirst = pointNum - firstPoint;
            transVal = 1 - distToFirst * 0.33f;
            if (transVal < 0.1f) transVal = 0.1f;

            Material material = sphere.GetComponent<Renderer>().material;

            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            sphere.GetComponent<Renderer>().material = material;

            sphere.GetComponent<Renderer>().material.SetColor("_Color", new Color(0.75f, 0, 0, transVal));


            spheres.Add(sphere);

        }

        Transform findPoint(string pointName, int transformation)
        {
            if (pointName.Split('_')[0].Equals("rinky"))
            {
                foreach (Transform t in alternativeHandPoints)
                {

                    if (t.gameObject.name.Contains("ring_" + pointName.Split('_')[1]))
                    {
                        return t;
                    }
                }
            }

            else
                foreach (Transform t in (transformation == 3) ? planePoints : handPoints)
                {
                    if (t.gameObject.name.Contains(pointName))
                    {
                        return t;
                    }
                }
            return null;
        }

        public bool pathUpdate(GameObject rightIndex, int transformation)
        {
            string point = path.pointList[firstPoint];
            Transform t = findPoint(point, transformation);

            bool globalDistance = Vector3.Distance(rightIndex.transform.position, t.position) < 0.03f;


            Ray ray = new Ray(t.position, t.up);
            bool vectorDistance = Vector3.Cross(ray.direction, rightIndex.transform.position - ray.origin).magnitude < 0.01f;

            if (globalDistance && vectorDistance)
            {
                firstPoint++;
                emptyPath();
                if (path.pointList.Length == firstPoint) return true;
                displayPath(transformation);

            }

            return false;
        }
    }

    public GameObject main_hand;
    public GameObject alternative_hand;

    public Hand hand;

    void Start()
    {
        getHandPoints();
    }


    private void getHandPoints()
    {
        hand = new Hand();

        Queue<Transform> queue = new Queue<Transform>();

        queue.Enqueue(main_hand.transform);
        while (queue.Count > 0)
        {

            Transform c = queue.Dequeue();

            try
            {
                string fingerName = c.gameObject.name.Split('_')[1];
                string num = c.gameObject.name.Split('_')[2];

                if ((new string[] { "middle", "index", "ring", "pinky" }).Contains(fingerName)
                    && (new string[] { "a", "b", "c", "end" }).Contains(num))
                {
                    ((c.gameObject.name.Split('_')[0] == "L") ? hand.handPoints : hand.planePoints).Add(c);

                }
                else if ((new string[] { "hand" }).Contains(fingerName)
                    && (new string[] { "m", "l", "r" }).Contains(num))
                {
                    ((c.gameObject.name.Split('_')[0] == "L") ? hand.handPoints : hand.planePoints).Add(c);
                }
            }
            catch (FormatException e)
            {
            }
            catch (ArgumentException e)
            {
            }
            catch (IndexOutOfRangeException) { }

            foreach (Transform t in c)
                queue.Enqueue(t);
        }

        queue = new Queue<Transform>();

        queue.Enqueue(alternative_hand.transform);
        while (queue.Count > 0)
        {

            Transform c = queue.Dequeue();

            try
            {
                string fingerName = c.gameObject.name.Split('_')[1];
                string num = c.gameObject.name.Split('_')[2];

                if ("ring".Equals(fingerName)
                    && (new string[] { "a", "b", "c", "end" }).Contains(num))
                {
                    hand.alternativeHandPoints.Add(c);

                }

            }
            catch (FormatException e)
            {
            }
            catch (ArgumentException e)
            {
            }
            catch (IndexOutOfRangeException) { }

            foreach (Transform t in c)
                queue.Enqueue(t);
        }
    }
}


