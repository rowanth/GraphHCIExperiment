using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

using UXF;
using System.Linq;

public class GraphTask : MonoBehaviour
{
    public GraphTaskFeedback feedback;
    private int currentTrial = 0;
    private List<string> graphFiles = new List<string>();

    //Generate blocks and trials. We can call this from OnSessionBegin 
    public void GenerateExperiment(Session session)
    {
        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath);
        FileInfo[] info = dir.GetFiles("*.graph.json");
        foreach (FileInfo f in info)
        {
            graphFiles.Add(f.FullName);
        }

        // create block of five trials
        Block newBlock = session.CreateBlock(3);
        foreach (Trial trial in newBlock.trials)
        {

            // store them in settings
            trial.settings.SetValue("filename", graphFiles[trial.number - 1]);
        }
    }

    float RandomBetweenMinus1And1()
    {
        return (2f * Random.value) - 1f;
    }

    public void CheckStatusOrStartNext(Trial trial)
    {
        // end now if this is the last trial in the session
        bool endNow = trial == Session.instance.LastTrial;

        // if last trial in current block
        Block currentBlock = trial.block;
        if (trial == currentBlock.lastTrial && !endNow)
        {
            bool atLeastOneCorrect = false;
            endNow = !atLeastOneCorrect;
        }

        // if we didnt get at least one correct, or this is the last trial, end now
        if (endNow)
        {
            // show info, and end
            //int span = currentBlock.settings.GetInt("num_sequence");
            feedback.ShowFeedback(string.Format("The end!"), delay: float.PositiveInfinity);
            Session.instance.Invoke("End", 5); // 5 second delay
        }
        else
        {
            // show normal feedback for this trial, and start next trial
            string feedbackText = "Thank You!";
            feedback.ShowFeedback(feedbackText);
            Session.instance.Invoke("BeginNextTrialSafe", 1); // 1 second delay
        }
    }
}