using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Experiment
{

    public List<Participant> participants;

    public Experiment(List<Participant> participants)
    {
        this.participants = participants;
    }

    public Experiment(string parse)
    {
        participants = new List<Participant>();

        foreach (string participantString in parse.Split('\n'))
        {
            Participant participant = new Participant(participantString);
            participants.Add(participant);
        }
    }


    public string toString()
    {
        string result = "";

        foreach (Participant participant in participants)
        {
            result += participant.toString();
            result += "\n";
        }

        result = result.Substring(0, result.Length - 1);

        return result;
    }
}
