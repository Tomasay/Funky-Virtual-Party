using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AiToolbox;

public class GeminiFakeAnswersGenerator : MonoBehaviour
{
    public GeminiParameters parameters;

    private void Start()
    {
        //Test
        //GenerateFakeAnswers("The worst costume you could wear to a halloween party?", "minion", 5, onRequestCompleteTest);
    }

    public void GenerateFakeAnswers(string question, string chosenAnswer, int numOfAnswersToGenerate, System.Action<string> onRequestCompleteCallback)
    {
        // Check if the API Key is set in the Inspector, just in case.
        if (parameters == null || string.IsNullOrEmpty(parameters.apiKey))
        {
            const string errorMessage = "Please set the <b>API Key</b> in the <b>Gemini Dialogue</b> Game Object.";
            Debug.Log(errorMessage);
        }

        string prompt = "You are playing a game of VRtistry, a minigame similar to Pictionary." +
        "In this game, players on mobile phones have been given the question: " + question + ". " +
        "Of the answers that the mobile users wrote, the selected answer was " + chosenAnswer + ". " +
        "This answer was shown to a player in VR who attempted to draw and paint on a mannequin to create this prompt. " + 
        "The drawing will now be shown to the mobile players along with all of the answers that the mobile users wrote. " +
        "In order to make the game more interesting, I need you to come up with " + numOfAnswersToGenerate + 
        " fake answers that are similar to " + chosenAnswer + ". The fake answers should be visually similar to the selected one." +
        "The fake answers should be one or two words max. Make sure your fake answers are not the same as the selected answer. " +
        "Please give me the answers only, each separated by a comma " +
        "You may include spaces in the answers, but make sure there are no spaces in between different answers, only commas." +
        "Feel free to use humor with some of these answers, as this game is usually played with friends and the " +
        "goal is for your answers to blend in with the selected one." +
        "Try to keep the fake answers somewhat different from each other, and that they are good answers for the original mobile question. " +
        "Also make sure the answers are natural and simple words/phrases that someone would normally type. " +
        "I'll give you an example scenario. Let's say the question was 'What did you always want to be when growing up?' " +
        "and the selected mobile answer is 'Basketball Player'. Some good decoy answers would be: " + 
        "Athlete, Michael Jordan, Baller, Dunk King, Allstar";

        // This request provides only `completeCallback` and `failureCallback` parameters. Since the `updateCallback`
        // is not provided, the request will be completed in one step, and the `completeCallback` will be called only
        // once, with the full text of the answer.
        Gemini.Request(prompt, parameters, onRequestCompleteCallback, failureCallback: (errorCode, errorMessage) => {
            // If the request fails, display the error message in the "You're chatting with" text.
            var errorType = (ErrorCodes)errorCode;
            Debug.Log($"Error {errorCode}: {errorType} - {errorMessage}");
        });
    }

    void onRequestCompleteTest(string s)
    {
        foreach (string ss in s.Split(','))
        {
            Debug.Log(ss);
        }
    }
}