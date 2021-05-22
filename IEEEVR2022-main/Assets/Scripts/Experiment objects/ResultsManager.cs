using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class ResultsManager
{

    private class PD_result
    {
        public float proprioceptive_drift_value;
        public Sample sample;

    }

    private class Q_result
    {
        public string[] questionnaire_values = new string[Questionnaire.questions.Length];
        public Sample sample;

    }

    static List<PD_result> pd_results = new List<PD_result>();
    static List<Q_result> q_results = new List<Q_result>();
    static float first_PD = 0;

    static Q_result current_q_result;
    static PD_result current_pd_result;

    public static int participantNumber;

    static public ExperimentManager experimentManager;



    public static void initiate(ExperimentManager experimentManager)
    {
        Sample.experimentManager = experimentManager;
    }


    public static void add_Quest_Res(string val)
    {
        current_q_result.questionnaire_values[int.Parse(val.Split('_')[1])] = (val.Split('_')[0]);
    }



    public static void add_PD(float val)
    {
        current_pd_result.proprioceptive_drift_value = val;
        pd_results.Add(current_pd_result);
    }


    public static void add_Q()
    {
        q_results.Add(current_q_result);
    }

    public static void initiate_new_result(Sample sample)
    {
        current_pd_result = new PD_result();
        current_pd_result.sample = sample;

        current_q_result = new Q_result();
        current_q_result.sample = sample;
    }



    public static void writeResult()
    {
        string Q_fileName = "Assets/ExperimentFiles/Result_" + participantNumber + "_Q.csv";
        string PD_fileName = "Assets/ExperimentFiles/Result_" + participantNumber + "_PD.csv";

        try
        {
            // Check if file already exists. If yes, delete it.     
            if (File.Exists(Q_fileName)) File.Delete(Q_fileName);
            if (File.Exists(PD_fileName)) File.Delete(PD_fileName);


            // Create a new file     
            using (FileStream fs = File.Create(Q_fileName))
            {
                // Add some text to file    
                Byte[] content = new UTF8Encoding(true).GetBytes(Q_toCSV());
                fs.Write(content, 0, content.Length);
            }

            using (FileStream fs = File.Create(PD_fileName))
            {
                // Add some text to file    
                Byte[] content = new UTF8Encoding(true).GetBytes(PD_toCSV());
                fs.Write(content, 0, content.Length);
            }
        }
        catch (Exception Ex)
        {
            Console.WriteLine(Ex.ToString());
        }

    }

    public static string Q_toCSV()
    {
        string result = "Participant number, Stimulus type, Transformation number";



        for (int i = 0; i < Questionnaire.getQuestionNumber(); i++)
        {
            result += ", Question " + i;
        }

        result += "\n";

        foreach (Q_result sr in q_results)
        {
            string stimulus = (sr.sample.stimulus == Sample.StimulusType.st) ? "Self touch" : "Visuomotor";
            result += participantNumber + "," + stimulus + "," + sr.sample.transformation;

            foreach (string quest_val in sr.questionnaire_values) { result += "," + quest_val; }
            result.Substring(0, result.Length - 1);

            result += "\n";
        }
        result.Substring(0, result.Length - 1);

        return result;

    }

    public static string PD_toCSV()
    {
        string result = "Participant number, Stimulus type, Transformation number, Proprioceptive drift value";

        result += "\n";

        foreach (PD_result sr in pd_results)
        {
            string stimulus = (sr.sample.stimulus == Sample.StimulusType.st) ? "Self touch" : "Visuomotor";
            result += participantNumber + "," + stimulus + "," + sr.sample.transformation + "," + sr.proprioceptive_drift_value;

            result += "\n";
        }
        result.Substring(0, result.Length - 1);

        return result;
    }

}
