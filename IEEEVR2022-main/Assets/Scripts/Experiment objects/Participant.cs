using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Participant
{
    public List<List<Sample>> blocks;

    public Participant(List<List<Sample>> blocks)
    {
        this.blocks = blocks;
    }

    public Participant(string parse)
    {

        blocks = new List<List<Sample>>();

        foreach (string blocksString in parse.Split('|'))
        {
            List<Sample> block = new List<Sample>();

            foreach (string sampleString in blocksString.Split(','))
            {

                Sample sample = new Sample(sampleString);
                block.Add(sample);
            }
            blocks.Add(block);
        }
    }

    static public Participant getExampleParticipant()
    {
        return new Participant((Random.Range(0, 2) == 0) ? "m0|t0" : "t0|m0");
    }

    public int blockNumber()
    {
        return blocks.Count;
    }

    public int sampleNumber()
    {
        return blocks[0].Count;
    }


    public string toString()
    {

        string result = "";

        foreach (List<Sample> block in blocks)
        {

            foreach (Sample sample in block)
            {
                result += sample.toString() + ",";
            }
            result = result.Substring(0, result.Length - 1);

            result += "|";
        }

        result = result.Substring(0, result.Length - 1);

        return result;
    }
}
