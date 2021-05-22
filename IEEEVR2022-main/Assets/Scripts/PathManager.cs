using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [System.Serializable]
    public class Path
    {
        public string[] pointList;
    }

    public RetargettingManagement rm;

    public Path[] paths;

    private void Awake()
    {
    }

    List<int> type_list = new List<int>();
    int type = 0;

   bool offset_6th_finger = true;

    public Path generateNewPath(bool sixthFinger)
    {

        if (type_list.Count == 0)
        {
            type_list.Add(0, 1, 2);
        }

        int rand = Random.Range(0, type_list.Count);
        type = type_list[rand];
        type_list.RemoveAt(rand);

        offset_6th_finger = !offset_6th_finger;
        
        rm.update_finger_offset_matching(offset_6th_finger);

        List<string> points = new List<string>();

        string[] fingersA = (!sixthFinger) ? new string[] { "index", "middle", "ring", "pinky" } : new string[] { "index", "middle", "ring", "rinky", "pinky" };
        
        if (sixthFinger)
        {
            if (!offset_6th_finger) fingersA = new string[] { "middle", "ring", "rinky", "pinky" };
            else fingersA = new string[] { "index", "middle", "ring", "rinky" };
        }
        else
        {
            fingersA = new string[] { "index", "middle", "ring", "pinky" };
        }

        List<string> fingers = fingersA.ToList();

        if (type == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                string f = fingers[Random.Range(0, fingers.Count)];
                points.Add(f + "_e");
                fingers.Remove(f);
            }
        }
        else if (type == 1)
        {
            string f1 = fingers[Random.Range(0, fingers.Count)];
            fingers.Remove(f1);
            string f2 = fingers[Random.Range(0, fingers.Count)];
            bool goLeft = (Random.value > 0.5f);

            points.Add(f1 + "_e");
            points.Add(f1 + "_c");
            points.Add(f1 + "_b");
            points.Add(f1 + "_a");


            points.Add("hand" + "_m");
            points.Add("hand" + ((goLeft) ? "_l" : "_r"));
            points.Add("hand" + ((!goLeft) ? "_l" : "_r"));
            points.Add("hand" + "_m");


            points.Add(f2 + "_a");
            points.Add(f2 + "_b");
            points.Add(f2 + "_c");
            points.Add(f2 + "_e");
        }

        else if (type == 2)
        {
            int lastFingerIndex = -1;
            for (int i = 0; i < 3; i++)
            {
                string f = fingers[Random.Range(0, fingers.Count)];

                points.Add(f + "_a");
                points.Add(f + "_b");
                points.Add(f + "_c");
                points.Add(f + "_e");
                points.Add(f + "_c");
                points.Add(f + "_b");
                points.Add(f + "_a");

                fingers.Remove(f);

                int currentIndex = System.Array.IndexOf(fingersA, f);

                if (lastFingerIndex != -1 && Mathf.Abs(lastFingerIndex - currentIndex) > 1)
                {
                    bool b = lastFingerIndex - currentIndex > 0f;

                    while (lastFingerIndex != currentIndex)
                    {
                        lastFingerIndex += (b) ? -1 : +1;
                        points.Add(fingersA[lastFingerIndex] + "_a");

                    }
                }

                lastFingerIndex = currentIndex;
            }

            points.RemoveRange(0, 3);
            points.RemoveRange(points.Count - 3, 3);
        }


        Path p = new Path();
        p.pointList = points.ToArray();
        return p;
    }
}
