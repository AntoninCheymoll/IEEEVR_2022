using Oculus.Platform.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Questionnaire
{
    static int currentQuestion = 0;
    public static int currentRate = 4;

    static int basicTextSize = 45;
    static int smallTextSize = 35;
    static int selectedTextSize = 80;
    static int noneSelectedTextSize = 45;

    static ExperimentManager experimentManager;

    public static string[] questions_init = new string[] {

            "I felt as if the virtual left hand was my hand.",
            "I felt as if the virtual left hand was part of my body.",
            "I felt as if the virtual left hand began to resemble my real left hand.",
            "I felt as if the virtual hand was in the location where my real left hand was.",
            "I felt as if my real left hand turned identical to the virtual hand.",
            "I felt as if I was looking directly at my own hand, rather than at a virtual hand."
    };

    public static string[] questions;

    public static void initiate(ExperimentManager e)
    {
        experimentManager = e;

        string[] temp = new string[questions_init.Length];

        for (int i = 0; i < questions_init.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, questions_init.Length);
            while (temp[rnd] != null)
            {
                rnd = UnityEngine.Random.Range(0, questions_init.Length);
            }
            temp[rnd] = questions_init[i];
        }

        questions = temp;

    }

    public static int getQuestionNumber() { return Questionnaire.questions.Length; }

    static private string generateText()
    {

        return ss(questions[currentQuestion], basicTextSize) + "\n\n" + getCurrentRateText();
    }

    public static string getFirstQuestion()
    {
        currentQuestion = 0;
        currentRate = 0;
        return generateText();
    }

    static public string getNextQuestion()
    {
        currentQuestion++;
        currentRate = 0;
        return (currentQuestion > questions.Length - 1) ? null : generateText();
    }

    public static string current_res()
    {
        int x = -1;

        return currentRate + "_" + System.Array.IndexOf(questions_init, questions[currentQuestion]);
    }
    static private string getCurrentRateText()
    {
        string res = "";
        for (int i = -3; i < 4; i++)
        {
            res += ss(i + "  ", (currentRate == i) ? selectedTextSize : noneSelectedTextSize);
        }


        return res + ss("\n\nstrongly disagree                               strongly agree", smallTextSize);
    }

    static public string updateDisplayDecr()
    {

        currentRate = Math.Max(-3, currentRate - 1);
        return generateText();
    }

    static public string updateDisplayIncr()
    {
        currentRate = Math.Min(3, currentRate + 1);
        return generateText();
    }

    static private string ss(string s, int size)
    {

        return "<size=" + size + ">" + s + "</size>";
    }
}
