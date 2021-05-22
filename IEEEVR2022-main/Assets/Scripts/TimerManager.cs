using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    static Dictionary<TimerName, float> dic = new Dictionary<TimerName, float>();
    public enum TimerName { Task, Break }

    private void Update()
    {
        foreach (TimerName key in dic.Keys.ToList())
        {
            dic[key] += Time.deltaTime;
        }
    }

    static public void createTimer(TimerName s)
    {
        dic[s] = 0;
    }

    static public float getTimer(TimerName s)
    {
        float res;
        dic.TryGetValue(s, out res);

        return res;

    }
}
