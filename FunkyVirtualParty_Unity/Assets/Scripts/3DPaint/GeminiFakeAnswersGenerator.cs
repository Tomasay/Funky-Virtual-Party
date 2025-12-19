using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if !UNITY_WEBGL
using Glitch9.AIDevKit;
using Glitch9.AIDevKit.Google;
#endif

public class GeminiFakeAnswersGenerator : MonoBehaviour
{
    //Front, Back, Left, Right
    public Camera[] artViewCameras;
    public RenderTexture[] artViewRTs;
    private Texture2D[] artTextures; // Generated textures from cam views

    public bool disableFakeAnswers = false;

#if !UNITY_WEBGL
    private void Awake()
    {
        artTextures = new Texture2D[artViewCameras.Length];

        //Test
        //GenerateFakeAnswers("The worst costume you could wear to a halloween party?", "minion", 5, onRequestCompleteTest);
    }

    int errorMessageIndex = 0; //Count index to make sure consecutive error messages that are the same still trigger a callback
    public async void GenerateFakeAnswers(string question, string chosenAnswer, int numOfAnswersToGenerate, System.Action<string> onRequestCompleteCallback)
    {
        errorMessageIndex++;

        if (disableFakeAnswers)
        {
            onRequestCompleteCallback.Invoke("AI request failed: decoy answers disabled" + errorMessageIndex);
            return;
        }

        try
        {
            string testPrompt1 = "You are playing a game of VRtistry, a minigame similar to Pictionary. " +
            "In this game, players on mobile phones have been given the question: " + question + " and " +
            "one of their answers was chosen at random and given to a player in VR who attempted to draw " +
            "and paint on a poseable mannequin to create it. The answer that was chosen and given to the VR player is: \"" + chosenAnswer + "\". " +
            "You are looking at the front, back, left, and right views of the VR player's creation of that answer in that order. " +
            "In order to make the game more interesting, I need you to come up with " + numOfAnswersToGenerate + " " +
            "guesses of what you think it is based on the images. Your answers should be distinct from one another. " +
            "No two answers should contain the same word. " +
            "Your answers should be one or two words max. Make sure your answers are not too similar to the chosen answer. " +
            "Your response must be the answers only, each separated by a comma. Include no other text besides those answers. " +
            "You may include spaces in the answers, but make sure there are no spaces in between different answers, only commas." +
            "Use humor with some of these answers, as this game is usually played with friends and the " +
            "goal is to make people laugh. Just make sure the answers make sense for the original question and what you see in the images. " +
            "Make sure that your answers aren't too ridiculous, keep them relatively simple. " +
            "Also make sure the answers are natural and simple words/phrases that someone would normally think of and type." +
            "I'll give you an example scenario. Let's say the question was \"What did you always want to be when growing up?\" " +
            "and the selected mobile answer is 'Basketball Player'. Some good answers would be: " +
            "Athlete, Michael Jordan, Baller, Dunk King, Allstar";

            string testPrompt2 = "You are looking at a virtual mannequin that was posed and drawn on by a player to create " +
            "a prompt given to them, similar to pictionary. You are looking at the front, back, left, and right views " +
            "of the player's creation in that order. Give me " + numOfAnswersToGenerate + " distinct guesses of what you " +
            "think the player tried to create. Your answers should be one or two words max." +
            "Please give me the answers only, each separated by a comma. You may include spaces in the answers, " +
            "but make sure there are no spaces in between different answers, only commas. " +
            "Feel free to use humor with some of these answers, as this game is usually played with friends and the " +
            "goal is to have answers that make people laugh. " +
            "Also make sure the answers are natural and simple words/phrases that someone would normally type.";

            string testPrompt3 = "You are looking at the front, back, left, and right views in that order of a mannequin that has been " +
            "posed and drawn on by a player in a video game. Based on these images, can you give me " + numOfAnswersToGenerate + " " +
            "guesses of what you think the player tried to create? Try to use a little humor and give me some funny answers, " +
            "but also keep them realistic and not ridiculous. " +
            //"The correct answer is " + correctAnswer + " so make sure not give me answers that are identical or too similar to that. " +
            "Your answers should be one or two words max. Please give me the answers only, each separated by a comma. " +
            "You may include spaces in the answers, but make sure there are no spaces in between different answers, only commas.";

            string testPrompt4 = "You are looking at a mannequin that has been " +
            "posed and drawn on by a player in a video game. Based on these images, can you give me " + numOfAnswersToGenerate + " " +
            "guesses of what you think the player tried to create? Try to use a little humor and give me some funny answers";
            //"Your answers should be one or two words max. Please give me the answers only, each separated by a comma. " +
            //"You may include spaces in the answers, but make sure there are no spaces in between different answers, only commas.";

            //Grab views from camera
            for (int i = 0; i < artViewCameras.Length; i++)
            {
                //Render cam view
                artViewCameras[i].Render();

                //Convert RT to Texture2D
                artTextures[i] = new Texture2D(artViewRTs[i].width, artViewRTs[i].height);
                RenderTexture.active = artViewRTs[i];
                artTextures[i].ReadPixels(new Rect(0, 0, artViewRTs[i].width, artViewRTs[i].height), 0, 0);
                artTextures[i].Apply();
            }

            string reply = await testPrompt1
                .GENResponse()
                .SetModel(GoogleModel.Gemini2_0_Flash_Lite)
                .Attach(artTextures)
                .ExecuteAsync();

            onRequestCompleteCallback.Invoke(reply);
            //Debug.Log("Decoy Answers: " + reply);
        }
        catch (System.Exception ex)
        {
            string errorMessage = ($"❌ AI request failed: {ex.Message}" + errorMessageIndex);
            Debug.LogError(errorMessage);
            onRequestCompleteCallback.Invoke(errorMessage);
        }
    }

    void onRequestCompleteTest(string s)
    {
        foreach (string ss in s.Split(','))
        {
            Debug.Log(ss);
        }
    }
#endif
}