using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;

public static class ExperimentFileGenerator
{
    static int replicationNum = 1;
    static int transformationNum = 4;
    static int participantNum = 20;

    static string path = "Assets/ExperimentFiles/RandomizedSamples.txt";
    static FileStream fs;
    static StreamWriter sw;

    static List<int> blockOrder = getBalancedIntList(participantNum / 2);
    static List<List<int>> transformOrders = getTransformOrderList(participantNum * 2);

    public static Experiment getExperiment()
    {

        if (File.Exists(path))
        {
            Debug.Log("please remove the current experiment file before creating a new one");
        }
        else
        {
            generateDocumentFile();
        }

        String file = File.ReadAllText(path);

        return new Experiment(file);
    }
    static void generateDocumentFile()
    {

        fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Write);
        sw = new StreamWriter(fs);

        sw.Write(createDocumentFile());

        sw.Close();
        fs.Close();

    }

    static string createDocumentFile()
    {

        List<Participant> participants = new List<Participant>();

        for (int i = 0; i < participantNum; i++) participants.Add(genererateParticipant());

        return (new Experiment(participants)).toString();
    }

    private static Participant genererateParticipant()
    {
        // 0 = visuomotor first, 1 = self touch first
        int stimilusOrder = blockOrder[0];
        blockOrder.RemoveAt(0);

        List<int> transformOrder = transformOrders[0];
        transformOrders.RemoveAt(0);

        List<List<Sample>> blocks = new List<List<Sample>>();

        for (int i = 0; i < replicationNum * 2; i++) blocks.Add(new List<Sample>());

        for (int i = 0; i < transformationNum; i++)
        {
            int transform = transformOrder[i];

            Sample.StimulusType st1 = (stimilusOrder == 0) ? Sample.StimulusType.vm : Sample.StimulusType.st;
            Sample.StimulusType st2 = (stimilusOrder == 0) ? Sample.StimulusType.st : Sample.StimulusType.vm;

            for (int y = 0; y < replicationNum; y++)
            {
                blocks[y * 2].Add((new Sample(st1, transform)));
                blocks[y * 2 + 1].Add(new Sample(st2, transform));
            }
        }

        return new Participant(blocks);
    }

    static List<int> getBalancedIntList(int replication)
    {
        List<int> res = new List<int>();

        for (int i = 0; i < replication; i++)
        {
            res.Add(0);
            res.Add(1);
        }

        return shuffle(res);
    }

    static List<List<int>> getTransformOrderList(int replication)
    {
        List<List<int>> res = new List<List<int>>();

        for (int i = 0; i < replication; i++)
        {
            List<int> order = new List<int>();
            for (int y = 0; y < transformationNum; y++) order.Add(y);
            res.Add(shuffle(order));
        }

        return res;
    }

    static private List<T> shuffle<T>(List<T> l)
    {
        for (int i = 0; i < l.Count; i++)
        {
            T temp = l[i];
            int randomIndex = UnityEngine.Random.Range(i, l.Count);
            l[i] = l[randomIndex];
            l[randomIndex] = temp;
        }
        return l;
    }

}
